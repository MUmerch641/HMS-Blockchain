using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainPatientData.Models
{
    /// <summary>
    /// Insurance Claim Model
    /// Handles auto-triggered insurance claims via smart contract simulation
    /// </summary>
    public class InsuranceClaim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        [Required]
        [ForeignKey("Bill")]
        public int BillID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public bool IsAutoTriggered { get; set; } = false;

        public bool IsFraudSuspected { get; set; } = false;

        public DateTime ClaimDate { get; set; } = DateTime.Now;

        public DateTime? ApprovedDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        // Navigation properties
        public virtual Patient? Patient { get; set; }
        public virtual Bill? Bill { get; set; }

        /// <summary>
        /// Claim status options
        /// </summary>
        public static class ClaimStatus
        {
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
            public const string UnderReview = "Under Review";
        }

        /// <summary>
        /// Gets status badge CSS class for UI
        /// </summary>
        public string GetStatusBadgeClass()
        {
            return Status switch
            {
                "Approved" => "badge-success",
                "Rejected" => "badge-danger",
                "Under Review" => "badge-warning",
                _ => "badge-secondary"
            };
        }
    }
}
