-- =============================================
-- HMS Blockchain Project - Database Setup Script
-- Final Year Project: Blockchain-Based Patient Data Management System
-- =============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PatientDataDB')
BEGIN
    CREATE DATABASE PatientDataDB;
END
GO

USE PatientDataDB;
GO

-- =============================================
-- Table 1: Patients
-- Stores patient information with NFT-based Health ID
-- =============================================
IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL
    DROP TABLE dbo.Patients;
GO
        
CREATE TABLE Patients (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    NFT_ID NVARCHAR(50) UNIQUE NOT NULL,
    Address NVARCHAR(200),
    PhoneNumber NVARCHAR(15),
    Email NVARCHAR(100),
    BloodGroup NVARCHAR(5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT CK_Age CHECK (Age > 0 AND Age < 150),
    CONSTRAINT CK_Gender CHECK (Gender IN ('Male', 'Female', 'Other'))
);
GO

-- =============================================
-- Table 2: Bills
-- Stores hospital bills with blockchain hash
-- =============================================
IF OBJECT_ID('dbo.Bills', 'U') IS NOT NULL
    DROP TABLE dbo.Bills;
GO

CREATE TABLE Bills (
    BillID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Hash NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    HospitalName NVARCHAR(100),
    DoctorName NVARCHAR(100),
    BillType NVARCHAR(50),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT CK_Amount CHECK (Amount > 0)
);
GO

-- =============================================
-- Table 3: Ledger (Blockchain Simulation)
-- Immutable ledger for blockchain entries
-- =============================================
IF OBJECT_ID('dbo.Ledger', 'U') IS NOT NULL
    DROP TABLE dbo.Ledger;
GO

CREATE TABLE Ledger (
    LedgerID INT IDENTITY(1,1) PRIMARY KEY,
    BillID INT NOT NULL,
    PatientID INT NOT NULL,
    BillHash NVARCHAR(100) NOT NULL,
    PreviousHash NVARCHAR(100),
    BlockNumber INT NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    IsVerified BIT DEFAULT 1,
    FOREIGN KEY (BillID) REFERENCES Bills(BillID),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID)
);
GO

-- =============================================
-- Table 4: InsuranceClaims
-- Stores insurance claims with smart contract status
-- =============================================
IF OBJECT_ID('dbo.InsuranceClaims', 'U') IS NOT NULL
    DROP TABLE dbo.InsuranceClaims;
GO

CREATE TABLE InsuranceClaims (
    ClaimID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    BillID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    IsAutoTriggered BIT DEFAULT 0,
    IsFraudSuspected BIT DEFAULT 0,
    ClaimDate DATETIME DEFAULT GETDATE(),
    ApprovedDate DATETIME NULL,
    Remarks NVARCHAR(500),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (BillID) REFERENCES Bills(BillID),
    CONSTRAINT CK_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Under Review'))
);
GO

-- =============================================
-- Sample Data (Optional for Testing)
-- =============================================

-- Insert Sample Patients
INSERT INTO Patients (Name, Age, Gender, NFT_ID, Address, PhoneNumber, Email, BloodGroup)
VALUES 
    ('Ali Ahmed', 28, 'Male', 'NFT-A1B2C3D4', 'Karachi, Pakistan', '03001234567', 'ali@example.com', 'O+'),
    ('Fatima Khan', 35, 'Female', 'NFT-E5F6G7H8', 'Lahore, Pakistan', '03007654321', 'fatima@example.com', 'A+'),
    ('Hassan Raza', 42, 'Male', 'NFT-I9J0K1L2', 'Islamabad, Pakistan', '03009876543', 'hassan@example.com', 'B+');
GO

-- Insert Sample Bills
DECLARE @Patient1 INT = (SELECT PatientID FROM Patients WHERE NFT_ID = 'NFT-A1B2C3D4');
DECLARE @Patient2 INT = (SELECT PatientID FROM Patients WHERE NFT_ID = 'NFT-E5F6G7H8');

INSERT INTO Bills (PatientID, Description, Amount, Hash, HospitalName, DoctorName, BillType)
VALUES 
    (@Patient1, 'Blood Test + X-Ray', 5000.00, 'HASH1234567890ABCDEF', 'City Hospital', 'Dr. Ahmed', 'Diagnostic'),
    (@Patient2, 'MRI Scan', 15000.00, 'HASH2345678901BCDEFG', 'Medical Center', 'Dr. Fatima', 'Imaging');
GO

-- =============================================
-- Indexes for Performance
-- =============================================
CREATE INDEX IDX_Bills_PatientID ON Bills(PatientID);
CREATE INDEX IDX_Ledger_BillID ON Ledger(BillID);
CREATE INDEX IDX_Claims_PatientID ON InsuranceClaims(PatientID);
CREATE INDEX IDX_Claims_Status ON InsuranceClaims(Status);
GO

-- =============================================
-- Views for Easy Data Access
-- =============================================

-- View: Patient Bill Summary
CREATE VIEW vw_PatientBillSummary AS
SELECT 
    p.PatientID,
    p.Name,
    p.NFT_ID,
    b.BillID,
    b.Description,
    b.Amount,
    b.Hash AS BlockchainHash,
    b.CreatedAt AS BillDate
FROM Patients p
INNER JOIN Bills b ON p.PatientID = b.PatientID;
GO

-- View: Insurance Claims Dashboard
CREATE VIEW vw_InsuranceClaimsDashboard AS
SELECT 
    ic.ClaimID,
    p.Name AS PatientName,
    p.NFT_ID,
    ic.Amount,
    ic.Status,
    ic.IsAutoTriggered,
    ic.IsFraudSuspected,
    ic.ClaimDate
FROM InsuranceClaims ic
INNER JOIN Patients p ON ic.PatientID = p.PatientID;
GO

-- =============================================
-- Stored Procedures
-- =============================================

-- Procedure: Add Bill with Ledger Entry
CREATE PROCEDURE sp_AddBillToLedger
    @PatientID INT,
    @Description NVARCHAR(500),
    @Amount DECIMAL(10,2),
    @Hash NVARCHAR(100),
    @HospitalName NVARCHAR(100),
    @DoctorName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @BillID INT;
    DECLARE @BlockNumber INT;
    DECLARE @PreviousHash NVARCHAR(100);
    
    -- Get previous hash from ledger
    SELECT TOP 1 @PreviousHash = BillHash, @BlockNumber = BlockNumber
    FROM Ledger
    ORDER BY LedgerID DESC;
    
    IF @PreviousHash IS NULL
    BEGIN
        SET @PreviousHash = '0000000000000000000000000000000000000000000000000000000000000000';
        SET @BlockNumber = 0;
    END
    
    -- Insert Bill
    INSERT INTO Bills (PatientID, Description, Amount, Hash, HospitalName, DoctorName, BillType)
    VALUES (@PatientID, @Description, @Amount, @Hash, @HospitalName, @DoctorName, 'General');
    
    SET @BillID = SCOPE_IDENTITY();
    
    -- Insert into Ledger (Blockchain)
    INSERT INTO Ledger (BillID, PatientID, BillHash, PreviousHash, BlockNumber)
    VALUES (@BillID, @PatientID, @Hash, @PreviousHash, @BlockNumber + 1);
    
    -- Auto-trigger insurance if amount > 10000
    IF @Amount > 10000
    BEGIN
        INSERT INTO InsuranceClaims (PatientID, BillID, Amount, Status, IsAutoTriggered)
        VALUES (@PatientID, @BillID, @Amount, 'Pending', 1);
    END
    
    SELECT @BillID AS BillID, @Hash AS BlockchainHash, 'Success' AS Status;
END
GO

PRINT '✅ Database setup completed successfully!';
PRINT '✅ 4 Tables Created: Patients, Bills, Ledger, InsuranceClaims';
PRINT '✅ 2 Views Created: vw_PatientBillSummary, vw_InsuranceClaimsDashboard';
PRINT '✅ 1 Stored Procedure Created: sp_AddBillToLedger';
GO
