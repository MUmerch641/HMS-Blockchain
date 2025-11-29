-- Check Bill #3 exact data to debug the tampering issue
USE PatientDataDB;

-- Get the exact data for Bill #3
SELECT 
    BillID,
    PatientID,
    Description,
    Amount,
    Hash,
    CreatedAt,
    DATEDIFF(MILLISECOND, '1970-01-01', CreatedAt) AS TimestampMs
FROM Bills
WHERE BillID = 3;

-- Get all bills to compare
SELECT 
    BillID,
    PatientID,
    Description,
    CAST(Amount AS VARCHAR) AS Amount,
    LEFT(Hash, 40) AS HashPreview,
    CONVERT(VARCHAR(30), CreatedAt, 121) AS CreatedAtFormatted
FROM Bills
ORDER BY BillID;
