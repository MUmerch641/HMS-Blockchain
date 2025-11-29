using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainPatientData.Models
{
    /// <summary>
    /// Ledger Entry Model - Simulated Blockchain
    /// Represents an immutable block in the blockchain ledger
    /// </summary>
    public class LedgerEntry
    {
        [Key]
        public int LedgerID { get; set; }

        [Required]
        [ForeignKey("Bill")]
        public int BillID { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        [Required]
        [StringLength(100)]
        public string BillHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PreviousHash { get; set; }

        [Required]
        public int BlockNumber { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsVerified { get; set; } = true;

        // Navigation properties
        public virtual Bill? Bill { get; set; }
        public virtual Patient? Patient { get; set; }

        /// <summary>
        /// Genesis block hash (first block in blockchain)
        /// </summary>
        public const string GENESIS_HASH = "0000000000000000000000000000000000000000000000000000000000000000";

        /// <summary>
        /// Validates the blockchain integrity by checking hash chain
        /// </summary>
        public bool ValidateBlock(string expectedPreviousHash)
        {
            return PreviousHash == expectedPreviousHash;
        }
    }
}
