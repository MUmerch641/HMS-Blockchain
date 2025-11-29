-- =============================================
-- Fix Blockchain Hashes - Regenerate all bill hashes
-- Run this to fix any inconsistencies in the blockchain
-- =============================================

USE PatientDataDB;
GO

-- View current bills and their hashes
SELECT 
    BillID,
    PatientID,
    Description,
    Amount,
    Hash,
    CreatedAt
FROM Bills
ORDER BY BillID;
GO

-- OPTION 1: Delete all bills and start fresh (RECOMMENDED FOR TESTING)
-- WARNING: This will delete ALL bills and related data!
/*
DELETE FROM Ledger;
DELETE FROM InsuranceClaims;
DELETE FROM Bills;

DBCC CHECKIDENT ('Bills', RESEED, 0);
DBCC CHECKIDENT ('Ledger', RESEED, 0);
DBCC CHECKIDENT ('InsuranceClaims', RESEED, 0);

PRINT '✅ All bills deleted. Blockchain reset. You can create new bills now.';
*/

-- OPTION 2: Just view the data for debugging
SELECT 
    'Bill ID: ' + CAST(BillID AS VARCHAR) + 
    ' | Patient: ' + CAST(PatientID AS VARCHAR) +
    ' | Amount: ₹' + CAST(Amount AS VARCHAR) +
    ' | Hash: ' + LEFT(Hash, 20) + '...' AS BillInfo
FROM Bills
ORDER BY BillID;
