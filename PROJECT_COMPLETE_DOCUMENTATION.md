# 🏥 Blockchain-Based Healthcare Management System (HMS)

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Technology Stack](#technology-stack)
4. [System Architecture](#system-architecture)
5. [Database Schema](#database-schema)
6. [How to Run](#how-to-run)
7. [User Guide](#user-guide)
8. [API Endpoints](#api-endpoints)
9. [Blockchain Implementation](#blockchain-implementation)
10. [Future Enhancements](#future-enhancements)

---

## 🎯 Project Overview

A **blockchain-based Healthcare Management System** that provides:
- **Secure patient data storage** using blockchain technology
- **NFT-based unique Health IDs** for each patient
- **Automated insurance claim processing** via smart contracts
- **AI-powered fraud detection** for suspicious claims
- **Immutable medical records** with SHA256 hashing
- **Three portals**: Hospital, Insurance, and Patient

### Purpose
To create a **transparent, secure, and tamper-proof** healthcare system where:
- Patient data integrity is guaranteed by blockchain
- Insurance claims are automatically verified
- Fraudulent activities are detected using AI
- Patients have full access to their medical records

---

## ✨ Features

### 🏥 Hospital Portal
- ✅ Register new patients with NFT Health ID generation
- ✅ Create and manage medical bills
- ✅ View all patients and their records
- ✅ View patient details with complete billing history
- ✅ Dashboard with statistics (total patients, recent bills)
- ✅ Blockchain hash generation for each bill

### 🏢 Insurance Portal
- ✅ View all insurance claims with status tracking
- ✅ Verify patient eligibility for claims
- ✅ AI-powered fraud detection analysis
- ✅ Approve/Reject claims (manual + smart contract)
- ✅ View claim details with patient and bill information
- ✅ Blockchain hash verification for bills
- ✅ Dashboard with claim statistics

### 👤 Patient Portal
- ✅ Login using Patient ID
- ✅ View complete medical records
- ✅ Access NFT Health ID
- ✅ View all medical bills with blockchain verification
- ✅ Track insurance claims and their status
- ✅ Verify data integrity using blockchain
- ✅ Personal dashboard with statistics

### 🔐 Security Features
- ✅ **Blockchain immutability** - All bills are SHA256 hashed
- ✅ **NFT Health IDs** - Unique digital identity for each patient
- ✅ **Smart Contracts** - Automated insurance payout calculation
- ✅ **Fraud Detection** - AI analyzes suspicious patterns
- ✅ **Data Integrity Verification** - Hash recalculation to detect tampering

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 6.0 MVC
- **Language**: C# 10
- **Database**: SQL Server Express (UMER\SQLEXPRESS)
- **Data Access**: ADO.NET with SqlClient

### Frontend
- **View Engine**: Razor (.cshtml)
- **Styling**: 100% Inline CSS (No Bootstrap)
- **Layout**: CSS Grid + Flexbox
- **JavaScript**: Vanilla JS (hover effects, form validation)

### Blockchain
- **Hashing Algorithm**: SHA256
- **Hash Generation**: Cryptographic service for bill immutability
- **Smart Contracts**: Simulated contract logic for insurance claims
- **NFT Generation**: GUID-based unique health identifiers

### AI/ML
- **Fraud Detection**: Rule-based AI system
- **Risk Scoring**: 0-100 scale with multiple detection rules
- **Pattern Analysis**: Checks for duplicate bills, high amounts, frequency

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    USER INTERFACE LAYER                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Hospital   │  │  Insurance   │  │   Patient    │      │
│  │    Portal    │  │    Portal    │  │    Portal    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   CONTROLLER LAYER (MVC)                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Hospital   │  │  Insurance   │  │   Patient    │      │
│  │  Controller  │  │  Controller  │  │  Controller  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    SERVICE LAYER                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Blockchain  │  │   Smart      │  │    Fraud     │      │
│  │   Service    │  │  Contract    │  │   Detector   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐                                           │
│  │   Database   │                                           │
│  │   Service    │                                           │
│  └──────────────┘                                           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                     DATA LAYER                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Patient    │  │     Bill     │  │  Insurance   │      │
│  │    Model     │  │    Model     │  │    Claim     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐                                           │
│  │   Ledger     │                                           │
│  │    Entry     │                                           │
│  └──────────────┘                                           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    DATABASE LAYER                            │
│         SQL Server Express - PatientDataDB                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Patients   │  │    Bills     │  │   Claims     │      │
│  │    Table     │  │    Table     │  │    Table     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐                                           │
│  │   Ledger     │                                           │
│  │    Table     │                                           │
│  └──────────────┘                                           │
└─────────────────────────────────────────────────────────────┘
```

---

## 🗄️ Database Schema

### Patients Table
```sql
CREATE TABLE Patients (
    PatientID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    NFT_ID NVARCHAR(50) NOT NULL UNIQUE,
    Address NVARCHAR(200),
    PhoneNumber NVARCHAR(15),
    Email NVARCHAR(100),
    BloodGroup NVARCHAR(5),
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

### Bills Table
```sql
CREATE TABLE Bills (
    BillID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT FOREIGN KEY REFERENCES Patients(PatientID),
    Description NVARCHAR(500) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Hash NVARCHAR(100) NOT NULL,
    HospitalName NVARCHAR(100),
    DoctorName NVARCHAR(100),
    BillType NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE()
);
```

### InsuranceClaims Table
```sql
CREATE TABLE InsuranceClaims (
    ClaimID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT FOREIGN KEY REFERENCES Patients(PatientID),
    BillID INT FOREIGN KEY REFERENCES Bills(BillID),
    Amount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending',
    IsAutoTriggered BIT DEFAULT 0,
    IsFraudSuspected BIT DEFAULT 0,
    ClaimDate DATETIME DEFAULT GETDATE(),
    ApprovedDate DATETIME NULL,
    Remarks NVARCHAR(500)
);
```

### Ledger Table (Blockchain)
```sql
CREATE TABLE Ledger (
    LedgerID INT PRIMARY KEY IDENTITY(1,1),
    PatientID INT FOREIGN KEY REFERENCES Patients(PatientID),
    BillID INT FOREIGN KEY REFERENCES Bills(BillID),
    Hash NVARCHAR(100) NOT NULL,
    PreviousHash NVARCHAR(100),
    Timestamp DATETIME DEFAULT GETDATE()
);
```

---

## 🚀 How to Run

### Prerequisites
1. **Visual Studio 2022** or **VS Code** with C# extension
2. **SQL Server Express** installed (Instance: UMER\SQLEXPRESS)
3. **.NET 6.0 SDK** installed

### Step 1: Database Setup
```sql
-- Open SQL Server Management Studio (SSMS)
-- Connect to: UMER\SQLEXPRESS
-- Run the setup.sql file from Database folder
-- This creates PatientDataDB database with all tables
```

### Step 2: Update Connection String (if needed)
```json
// File: appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=UMER\\SQLEXPRESS;Database=PatientDataDB;Trusted_Connection=True;"
  }
}
```

### Step 3: Build the Project
```powershell
cd C:\Users\SHARJAH LAPTOPS\OneDrive\Desktop\HMS_Blockchain_Project\BlockchainPatientData
dotnet build
```

### Step 4: Run the Application
```powershell
dotnet run
```

### Step 5: Access the Application
- **Homepage**: http://localhost:5000
- **Hospital Portal**: http://localhost:5000/Hospital/Dashboard
- **Insurance Portal**: http://localhost:5000/Insurance/Dashboard
- **Patient Portal**: http://localhost:5000/Patient/Login

---

## 📖 User Guide

### 🏥 Hospital Workflow

#### 1. Register a New Patient
1. Go to **Hospital Dashboard**
2. Click **"Register Patient"**
3. Fill in patient details:
   - Name (required)
   - Age (required)
   - Gender (required)
   - Phone, Email, Address (optional)
   - Blood Group (optional)
4. Click **"Register Patient"**
5. System generates **NFT Health ID** automatically
6. Note down the **Patient ID** for login

#### 2. Create a Bill
1. Go to **Hospital Dashboard**
2. Click **"Create Bill"**
3. Enter:
   - Patient ID
   - Description (e.g., "X-Ray Scan", "Surgery")
   - Amount
   - Hospital Name (optional)
   - Doctor Name (optional)
4. Click **"Create Bill"**
5. System automatically:
   - Generates **SHA256 blockchain hash**
   - Adds entry to **Ledger**
   - Triggers **insurance claim** (if enabled)

#### 3. View Patient Details
1. Go to **Hospital Dashboard**
2. Click on any **Patient Name** in the table
3. View complete patient information
4. See all bills associated with the patient

### 🏢 Insurance Workflow

#### 1. View All Claims
1. Go to **Insurance Dashboard**
2. Click **"View All Claims"**
3. See all claims with:
   - Status (Pending/Approved/Rejected)
   - Amount
   - Fraud detection status
4. Click **"View Details"** on any claim

#### 2. Verify Claim Eligibility
1. Click **"Verify Claim"** in dashboard
2. Enter **Patient ID**
3. System shows:
   - Patient information
   - Total bills vs total claims
   - Billing history
   - Verification result

#### 3. Fraud Detection Analysis
1. Click **"Fraud Analysis"**
2. View all suspicious claims detected by AI
3. See risk scores and reasons
4. Review high-risk cases manually

### 👤 Patient Workflow

#### 1. Login
1. Go to **Patient Portal**: http://localhost:5000/Patient/Login
2. Enter your **Patient ID** (get from hospital)
3. Click **"Access My Records"**

#### 2. View Medical Records
1. From dashboard, click **"View Records"**
2. See complete patient information
3. View NFT Health ID

#### 3. View Bills
1. Click **"View Bills"**
2. See all medical bills with:
   - Description
   - Amount
   - Date
   - Blockchain hash

#### 4. View Insurance Claims
1. Click **"View Claims"**
2. Track claim status (Pending/Approved/Rejected)
3. See claim amounts and dates

#### 5. Verify Data Integrity
1. Click on any bill
2. System shows:
   - Stored blockchain hash
   - Recalculated hash
   - Verification result (✅ or ❌)

---

## 🔗 API Endpoints

### Hospital Controller
```
GET  /Hospital/Dashboard              - Hospital dashboard
GET  /Hospital/AddPatient             - Patient registration form
POST /Hospital/AddPatient             - Register new patient
GET  /Hospital/ViewPatients           - View all patients
GET  /Hospital/PatientDetails/{id}    - View patient details
GET  /Hospital/AddBill                - Bill creation form
POST /Hospital/AddBill                - Create new bill
GET  /Hospital/ViewBills              - View all bills
```

### Insurance Controller
```
GET  /Insurance/Dashboard             - Insurance dashboard
GET  /Insurance/ViewClaims            - View all claims
GET  /Insurance/ClaimDetails/{id}     - View claim details
GET  /Insurance/VerifyClaim           - Verify claim form
POST /Insurance/VerifyClaim           - Verify patient eligibility
GET  /Insurance/FraudAnalysis         - Fraud detection report
GET  /Insurance/VerifyBlockchainHash/{billId} - Verify bill hash
```

### Patient Controller
```
GET  /Patient/Login                   - Patient login page
POST /Patient/Login                   - Authenticate patient
GET  /Patient/Dashboard/{id}          - Patient dashboard
GET  /Patient/ViewRecords/{id}        - View medical records
GET  /Patient/ViewBills/{id}          - View all bills
GET  /Patient/ViewClaims/{id}         - View insurance claims
GET  /Patient/ViewNFT/{id}            - View NFT Health ID
GET  /Patient/VerifyData/{patientId}/{billId} - Verify data integrity
GET  /Patient/Logout                  - Logout patient
```

---

## ⛓️ Blockchain Implementation

### 1. NFT Health ID Generation
```csharp
public static string GenerateNFT_ID()
{
    return "NFT-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
}
// Example output: NFT-A1B2C3D4
```

### 2. SHA256 Hash Generation
```csharp
public string GenerateBillHash(int patientId, string description, decimal amount, DateTime timestamp)
{
    string input = $"{patientId}-{description}-{amount}-{timestamp:yyyy-MM-dd HH:mm:ss}";
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
// Example: "a3f5c1e9d2b4a6c8e1f3d5a7b9c2e4f6..."
```

### 3. Blockchain Ledger Entry
```csharp
public void AddToLedger(int patientId, int billId, string hash)
{
    string previousHash = GetLastHash() ?? "0";
    
    // Insert into Ledger table
    INSERT INTO Ledger (PatientID, BillID, Hash, PreviousHash, Timestamp)
    VALUES (@patientId, @billId, @hash, @previousHash, GETDATE());
}
```

### 4. Smart Contract Payout Calculation
```csharp
public decimal CalculatePayout(decimal billAmount)
{
    if (billAmount < 1000) return billAmount * 0.80m;      // 80%
    if (billAmount < 10000) return billAmount * 0.70m;     // 70%
    if (billAmount < 50000) return billAmount * 0.60m;     // 60%
    return billAmount * 0.50m;                             // 50%
}
```

### 5. Fraud Detection Rules
```csharp
// Rule 1: High amount (40 risk points)
if (amount > 50000) riskScore += 40;

// Rule 2: High frequency (30 risk points)
if (recentBills.Count > 5) riskScore += 30;

// Rule 3: Round numbers (15 risk points)
if (amount % 1000 == 0) riskScore += 15;

// Rule 4: Sudden spike (25 risk points)
if (amount > avgAmount * 3) riskScore += 25;

// Final verdict
if (riskScore >= 50) → HIGH RISK (Manual review)
if (riskScore >= 30) → MEDIUM RISK (Verify)
if (riskScore < 30)  → LOW RISK (Auto-approve)
```

---

## 📊 Project Statistics

- **Total Files**: 35+
- **Total Lines of Code**: 5000+
- **Controllers**: 4 (Home, Hospital, Insurance, Patient)
- **Services**: 4 (Blockchain, SmartContract, FraudDetector, Database)
- **Models**: 4 (Patient, Bill, InsuranceClaim, LedgerEntry)
- **Views**: 20+ (All portals with inline CSS)
- **Database Tables**: 4 (Patients, Bills, InsuranceClaims, Ledger)

---

## 🎨 Design Features

### Color Schemes
- **Primary**: Purple gradient (#667eea → #764ba2)
- **Success**: Green gradient (#10b981 → #059669)
- **Warning**: Orange gradient (#f59e0b → #d97706)
- **Danger**: Red gradient (#ef4444 → #dc2626)
- **Info**: Blue gradient (#4facfe → #00f2fe)

### UI Elements
- **Cards**: Rounded corners (15-20px), shadow effects
- **Buttons**: Gradient backgrounds, hover animations
- **Tables**: Alternating row colors, hover highlights
- **Forms**: Focus effects, validation messages
- **Responsive**: CSS Grid + auto-fit columns

---

## 🔮 Future Enhancements

### Phase 1: Security
- [ ] Add user authentication (JWT tokens)
- [ ] Implement role-based access control
- [ ] Two-factor authentication (2FA)
- [ ] Encrypted data storage

### Phase 2: Blockchain
- [ ] Integrate with real blockchain (Ethereum/Hyperledger)
- [ ] Deploy actual NFTs on blockchain
- [ ] Smart contract on Solidity
- [ ] Decentralized storage (IPFS)

### Phase 3: AI/ML
- [ ] Train ML model for fraud detection
- [ ] Predictive analytics for patient health
- [ ] Automated diagnosis suggestions
- [ ] Anomaly detection in billing patterns

### Phase 4: Features
- [ ] Doctor portal for prescriptions
- [ ] Lab portal for test results
- [ ] Pharmacy integration
- [ ] Appointment scheduling
- [ ] Telemedicine support
- [ ] Mobile app (React Native/Flutter)

### Phase 5: Integration
- [ ] Payment gateway integration
- [ ] SMS/Email notifications
- [ ] PDF report generation
- [ ] QR code for quick access
- [ ] API for third-party integration

---

## 🐛 Known Issues & Solutions

### Issue 1: Patient Registration Not Working
**Cause**: NFT_ID field was marked as [Required] in model  
**Solution**: Removed [Required] attribute since it's auto-generated

### Issue 2: Bootstrap CSS Conflicts
**Cause**: Bootstrap overriding custom styles  
**Solution**: Removed all Bootstrap, used 100% inline CSS

### Issue 3: 500 Internal Server Errors
**Cause**: Missing view files for controllers  
**Solution**: Created all 20+ view files with inline styling

### Issue 4: Wrong Property Names
**Cause**: Using `BlockchainHash` instead of `Hash`, `CreatedAt` instead of `ClaimDate`  
**Solution**: Fixed property names to match model definitions

---

## 👨‍💻 Developer Notes

### Code Organization
```
BlockchainPatientData/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── HospitalController.cs
│   ├── InsuranceController.cs
│   └── PatientController.cs
├── Models/              # Data Models
│   ├── Patient.cs
│   ├── Bill.cs
│   ├── InsuranceClaim.cs
│   └── LedgerEntry.cs
├── Services/            # Business Logic
│   ├── BlockchainService.cs
│   ├── SmartContractService.cs
│   ├── FraudDetector.cs
│   └── DatabaseService.cs
├── Views/               # Razor Views
│   ├── Home/
│   ├── Hospital/
│   ├── Insurance/
│   ├── Patient/
│   └── Shared/
└── wwwroot/             # Static files
    ├── css/
    └── js/
```

### Coding Standards
- **C# Naming**: PascalCase for public members
- **Variables**: camelCase for private fields
- **Comments**: XML documentation for public methods
- **Formatting**: 4-space indentation
- **Error Handling**: Try-catch blocks with ViewBag.Error

### Database Best Practices
- Foreign keys for referential integrity
- Indexes on frequently queried columns
- Parameterized queries to prevent SQL injection
- Transaction support for critical operations

---

## 📞 Support & Contact

For questions or issues:
- **Project Location**: `C:\Users\SHARJAH LAPTOPS\OneDrive\Desktop\HMS_Blockchain_Project`
- **Database**: SQL Server Express (UMER\SQLEXPRESS)
- **Framework**: ASP.NET Core 6.0
- **Port**: http://localhost:5000

---

## 📄 License

This project is created for **educational purposes** and **final year project** demonstration.

---

## 🎓 Academic Information

**Project Type**: Final Year Project / Blockchain Healthcare System  
**Technology**: ASP.NET Core 6.0 MVC + SQL Server + Blockchain  
**Key Concepts**: SHA256 Hashing, NFT, Smart Contracts, Fraud Detection  
**Completion Date**: November 2025  

---

## 🎉 Acknowledgments

This project demonstrates:
- ✅ Full-stack web development with ASP.NET Core MVC
- ✅ Blockchain concepts (hashing, immutability, NFTs)
- ✅ Smart contract simulation for automation
- ✅ AI-based fraud detection algorithms
- ✅ Secure healthcare data management
- ✅ Multi-portal architecture (Hospital, Insurance, Patient)
- ✅ Database design and SQL Server integration
- ✅ Responsive UI with pure CSS (no frameworks)

**Status**: ✅ **FULLY FUNCTIONAL & COMPLETE**

---

**END OF DOCUMENTATION**
