using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainPatientData.Models
{
    /// <summary>
    /// Bill Model with Blockchain Hash
    /// Each bill is hashed using SHA256 for blockchain immutability
    /// </summary>
    public class Bill
    {
        [Key]
        public int BillID { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Bill description is required")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Hash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? HospitalName { get; set; }

        [StringLength(100)]
        public string? DoctorName { get; set; }

        [StringLength(50)]
        public string? BillType { get; set; }

        // Navigation property
        public virtual Patient? Patient { get; set; }

        /// <summary>
        /// Creates a hashable string representation of the bill
        /// </summary>
        public string GetHashableString()
        {
            return $"{PatientID}-{Description}-{Amount}-{CreatedAt:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
