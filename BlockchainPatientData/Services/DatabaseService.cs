using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BlockchainPatientData.Models;

namespace BlockchainPatientData.Services
{
    /// <summary>
    /// Database Service - Handles all database operations
    /// Provides CRUD operations for Patients, Bills, Ledger, and Insurance Claims
    /// </summary>
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly BlockchainService _blockchainService;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string not found");
            _blockchainService = new BlockchainService();
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ==================== PATIENT OPERATIONS ====================

        public int AddPatient(Patient patient)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Patients (Name, Age, Gender, NFT_ID, Address, PhoneNumber, Email, BloodGroup)
                            VALUES (@Name, @Age, @Gender, @NFT_ID, @Address, @PhoneNumber, @Email, @BloodGroup);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", patient.Name);
            cmd.Parameters.AddWithValue("@Age", patient.Age);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@NFT_ID", patient.NFT_ID);
            cmd.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)patient.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", (object?)patient.BloodGroup ?? DBNull.Value);

            return (int)cmd.ExecuteScalar();
        }

        public Patient? GetPatient(int patientId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PatientID", patientId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Patient
                {
                    PatientID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Age = reader.GetInt32(2),
                    Gender = reader.GetString(3),
                    NFT_ID = reader.GetString(4),
                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PhoneNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Email = reader.IsDBNull(7) ? null : reader.GetString(7),
                    BloodGroup = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9)
                };
            }
            return null;
        }

        public List<Patient> GetAllPatients()
        {
            var patients = new List<Patient>();
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Patients ORDER BY CreatedAt DESC";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Age = reader.GetInt32(2),
                    Gender = reader.GetString(3),
                    NFT_ID = reader.GetString(4),
                    Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PhoneNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Email = reader.IsDBNull(7) ? null : reader.GetString(7),
                    BloodGroup = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9)
                });
            }
            return patients;
        }

        public void UpdatePatient(Patient patient)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Patients 
                            SET Name = @Name, 
                                Age = @Age, 
                                Gender = @Gender,
                                Address = @Address,
                                PhoneNumber = @PhoneNumber,
                                Email = @Email,
                                BloodGroup = @BloodGroup
                            WHERE PatientID = @PatientID";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PatientID", patient.PatientID);
            cmd.Parameters.AddWithValue("@Name", patient.Name);
            cmd.Parameters.AddWithValue("@Age", patient.Age);
            cmd.Parameters.AddWithValue("@Gender", patient.Gender);
            cmd.Parameters.AddWithValue("@Address", (object?)patient.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)patient.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)patient.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", (object?)patient.BloodGroup ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        // ==================== BILL OPERATIONS ====================

        public int AddBillWithLedger(Bill bill)
        {
            using var conn = GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                // Set CreatedAt to current time if not set, then truncate milliseconds for consistency
                if (bill.CreatedAt == default(DateTime))
                {
                    bill.CreatedAt = DateTime.Now;
                }
                
                // IMPORTANT: Truncate milliseconds to ensure hash consistency
                // SQL Server datetime has different precision than .NET DateTime
                bill.CreatedAt = new DateTime(
                    bill.CreatedAt.Year,
                    bill.CreatedAt.Month,
                    bill.CreatedAt.Day,
                    bill.CreatedAt.Hour,
                    bill.CreatedAt.Minute,
                    bill.CreatedAt.Second,
                    0  // Set milliseconds to 0
                );

                // Get previous block's hash for blockchain chaining
                string previousHash = GetLatestLedgerHash(conn, transaction);
                
                // Generate BLOCKCHAIN hash with previous block's hash (TRUE BLOCKCHAIN)
                // This links this block to the previous block, creating an immutable chain
                bill.Hash = _blockchainService.GenerateBlockchainHash(
                    bill.PatientID, 
                    bill.Description, 
                    bill.Amount, 
                    bill.CreatedAt,
                    previousHash  // Links to previous block!
                );

                Console.WriteLine($"🔗 BLOCKCHAIN: Creating block with PreviousHash: {previousHash.Substring(0, 16)}...");
                Console.WriteLine($"🔗 BLOCKCHAIN: New block hash: {bill.Hash.Substring(0, 16)}...");

                // Insert Bill
                string billQuery = @"INSERT INTO Bills (PatientID, Description, Amount, Hash, HospitalName, DoctorName, BillType)
                                    VALUES (@PatientID, @Description, @Amount, @Hash, @HospitalName, @DoctorName, @BillType);
                                    SELECT CAST(SCOPE_IDENTITY() as int);";

                int billId;
                using (var cmd = new SqlCommand(billQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@PatientID", bill.PatientID);
                    cmd.Parameters.AddWithValue("@Description", bill.Description);
                    cmd.Parameters.AddWithValue("@Amount", bill.Amount);
                    cmd.Parameters.AddWithValue("@Hash", bill.Hash);
                    cmd.Parameters.AddWithValue("@HospitalName", (object?)bill.HospitalName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DoctorName", (object?)bill.DoctorName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BillType", (object?)bill.BillType ?? DBNull.Value);

                    billId = (int)cmd.ExecuteScalar();
                }

                // Get block number (previousHash already retrieved above)
                int blockNumber = GetLatestBlockNumber(conn, transaction) + 1;

                // Insert Ledger Entry
                string ledgerQuery = @"INSERT INTO Ledger (BillID, PatientID, BillHash, PreviousHash, BlockNumber)
                                      VALUES (@BillID, @PatientID, @BillHash, @PreviousHash, @BlockNumber)";

                using (var cmd = new SqlCommand(ledgerQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BillID", billId);
                    cmd.Parameters.AddWithValue("@PatientID", bill.PatientID);
                    cmd.Parameters.AddWithValue("@BillHash", bill.Hash);
                    cmd.Parameters.AddWithValue("@PreviousHash", previousHash);
                    cmd.Parameters.AddWithValue("@BlockNumber", blockNumber);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return billId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Bill> GetBillsByPatient(int patientId)
        {
            var bills = new List<Bill>();
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Bills WHERE PatientID = @PatientID ORDER BY CreatedAt DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PatientID", patientId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                bills.Add(ReadBillFromReader(reader));
            }
            return bills;
        }

        /// <summary>
        /// Get all ledger entries for blockchain validation
        /// </summary>
        public List<LedgerEntry> GetAllLedgerEntries()
        {
            var entries = new List<LedgerEntry>();
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Ledger ORDER BY BlockNumber ASC";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                entries.Add(new LedgerEntry
                {
                    LedgerID = reader.GetInt32(reader.GetOrdinal("LedgerID")),
                    BillID = reader.GetInt32(reader.GetOrdinal("BillID")),
                    PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                    BillHash = reader.GetString(reader.GetOrdinal("BillHash")),
                    PreviousHash = reader.IsDBNull(reader.GetOrdinal("PreviousHash")) 
                        ? LedgerEntry.GENESIS_HASH 
                        : reader.GetString(reader.GetOrdinal("PreviousHash")),
                    BlockNumber = reader.GetInt32(reader.GetOrdinal("BlockNumber")),
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                });
            }
            return entries;
        }

        // ==================== LEDGER OPERATIONS ====================

        private string GetLatestLedgerHash(SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT TOP 1 BillHash FROM Ledger ORDER BY LedgerID DESC";
            using var cmd = new SqlCommand(query, conn, transaction);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? LedgerEntry.GENESIS_HASH;
        }

        private int GetLatestBlockNumber(SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT ISNULL(MAX(BlockNumber), 0) FROM Ledger";
            using var cmd = new SqlCommand(query, conn, transaction);
            return (int)cmd.ExecuteScalar();
        }

        // ==================== INSURANCE OPERATIONS ====================

        public int AddInsuranceClaim(InsuranceClaim claim)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO InsuranceClaims (PatientID, BillID, Amount, Status, IsAutoTriggered, IsFraudSuspected, Remarks)
                            VALUES (@PatientID, @BillID, @Amount, @Status, @IsAutoTriggered, @IsFraudSuspected, @Remarks);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PatientID", claim.PatientID);
            cmd.Parameters.AddWithValue("@BillID", claim.BillID);
            cmd.Parameters.AddWithValue("@Amount", claim.Amount);
            cmd.Parameters.AddWithValue("@Status", claim.Status);
            cmd.Parameters.AddWithValue("@IsAutoTriggered", claim.IsAutoTriggered);
            cmd.Parameters.AddWithValue("@IsFraudSuspected", claim.IsFraudSuspected);
            cmd.Parameters.AddWithValue("@Remarks", (object?)claim.Remarks ?? DBNull.Value);

            return (int)cmd.ExecuteScalar();
        }

        public List<InsuranceClaim> GetAllClaims()
        {
            var claims = new List<InsuranceClaim>();
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM InsuranceClaims ORDER BY ClaimDate DESC";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                claims.Add(ReadClaimFromReader(reader));
            }
            return claims;
        }

        /// <summary>
        /// Update claim status (Pending → Approved or Rejected)
        /// Used by insurance company to approve/reject claims
        /// </summary>
        public void UpdateClaimStatus(int claimId, string status)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE InsuranceClaims 
                            SET Status = @Status, 
                                ApprovedDate = @ApprovedDate 
                            WHERE ClaimID = @ClaimID";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@ApprovedDate", 
                status == "Approved" || status == "Rejected" ? DateTime.Now : DBNull.Value);
            cmd.Parameters.AddWithValue("@ClaimID", claimId);

            cmd.ExecuteNonQuery();
        }

        // ==================== HELPER METHODS ====================

        private Bill ReadBillFromReader(SqlDataReader reader)
        {
            return new Bill
            {
                BillID = reader.GetInt32(0),
                PatientID = reader.GetInt32(1),
                Description = reader.GetString(2),
                Amount = reader.GetDecimal(3),
                Hash = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                HospitalName = reader.IsDBNull(6) ? null : reader.GetString(6),
                DoctorName = reader.IsDBNull(7) ? null : reader.GetString(7),
                BillType = reader.IsDBNull(8) ? null : reader.GetString(8)
            };
        }

        private InsuranceClaim ReadClaimFromReader(SqlDataReader reader)
        {
            return new InsuranceClaim
            {
                ClaimID = reader.GetInt32(0),
                PatientID = reader.GetInt32(1),
                BillID = reader.GetInt32(2),
                Amount = reader.GetDecimal(3),
                Status = reader.GetString(4),
                IsAutoTriggered = reader.GetBoolean(5),
                IsFraudSuspected = reader.GetBoolean(6),
                ClaimDate = reader.GetDateTime(7),
                ApprovedDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                Remarks = reader.IsDBNull(9) ? null : reader.GetString(9)
            };
        }
    }
}
