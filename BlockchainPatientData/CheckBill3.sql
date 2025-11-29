-- =============================================
-- Check Bill #3 Details to Find Tampering
-- =============================================

USE PatientDataDB;
GO

-- See Bill #3 details
SELECT 
    BillID,
    PatientID,
    Description,
    Amount,
    Hash AS StoredHash,
    CreatedAt,
    HospitalName,
    DoctorName,
    BillType
FROM Bills
WHERE BillID = 3;
GO

-- Check what the hash SHOULD be
-- The system generates hash from: PatientID + Description + Amount + CreatedAt
PRINT '-------------------------------------------';
PRINT 'Bill #3 Analysis:';
PRINT '-------------------------------------------';
PRINT 'If any of these values were changed after creation,';
PRINT 'the stored hash will NOT match the computed hash.';
PRINT '';
PRINT 'Possible tampering scenarios:';
PRINT '1. Amount was changed (e.g., 10000 → 15000)';
PRINT '2. Description was modified';
PRINT '3. PatientID was altered';
PRINT '4. Data was manually inserted with wrong hash';
GO

-- Compare with other bills
SELECT 
    BillID,
    PatientID,
    Description,
    Amount,
    LEFT(Hash, 30) + '...' AS HashPreview,
    CreatedAt
FROM Bills
ORDER BY BillID;
GO

-- Check Ledger entry for Bill #3
SELECT 
    LedgerID,
    BillID,
    PatientID,
    LEFT(BillHash, 30) + '...' AS BillHashPreview,
    LEFT(PreviousHash, 30) + '...' AS PrevHashPreview,
    BlockNumber,
    Timestamp
FROM Ledger
WHERE BillID = 3;
GO
