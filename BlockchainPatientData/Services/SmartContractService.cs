using System;
using BlockchainPatientData.Models;

namespace BlockchainPatientData.Services
{
    /// <summary>
    /// Smart Contract Service - Simulates blockchain smart contract logic
    /// Automatically triggers insurance claims based on predefined rules
    /// </summary>
    public class SmartContractService
    {
        private readonly decimal _insuranceThreshold;
        private readonly FraudDetector _fraudDetector;

        public SmartContractService()
        {
            _insuranceThreshold = 10000; // Default threshold
            _fraudDetector = new FraudDetector();
        }

        public SmartContractService(decimal threshold)
        {
            _insuranceThreshold = threshold;
            _fraudDetector = new FraudDetector();
        }

        /// <summary>
        /// Evaluates if insurance claim should be auto-triggered
        /// Smart Contract Rule: If bill amount > threshold, auto-trigger claim
        /// </summary>
        /// <param name="amount">Bill amount</param>
        /// <returns>Insurance status message</returns>
        public string TriggerInsurance(decimal amount)
        {
            if (amount > _insuranceThreshold)
            {
                return "✅ Micro-Insurance Auto-Triggered (Smart Contract)";
            }
            else
            {
                return "ℹ️ No insurance claim required";
            }
        }

        /// <summary>
        /// Processes insurance claim with smart contract logic
        /// </summary>
        public InsuranceClaimResult ProcessClaim(int patientId, int billId, decimal amount)
        {
            var result = new InsuranceClaimResult
            {
                PatientID = patientId,
                BillID = billId,
                Amount = amount,
                IsAutoTriggered = false,
                Status = InsuranceClaim.ClaimStatus.Pending
            };

            // Smart Contract Rule 1: Auto-trigger for high-value bills
            if (amount > _insuranceThreshold)
            {
                result.IsAutoTriggered = true;
                result.Status = InsuranceClaim.ClaimStatus.Pending;
                result.Message = "Smart contract auto-triggered insurance claim";
            }

            // Smart Contract Rule 2: Fraud detection check
            if (_fraudDetector.IsSuspicious(amount))
            {
                result.IsFraudSuspected = true;
                result.Status = InsuranceClaim.ClaimStatus.UnderReview;
                result.Message = "⚠️ Claim flagged for fraud review";
            }

            // Smart Contract Rule 3: Immediate approval for small claims
            if (amount <= 5000 && !result.IsFraudSuspected)
            {
                result.Status = InsuranceClaim.ClaimStatus.Approved;
                result.Message = "✅ Small claim auto-approved";
            }

            return result;
        }

        /// <summary>
        /// Validates claim eligibility based on smart contract rules
        /// </summary>
        public bool IsClaimEligible(decimal amount, DateTime billDate)
        {
            // Rule: Bill must be recent (within 90 days)
            var daysSinceBill = (DateTime.Now - billDate).Days;
            
            if (daysSinceBill > 90)
            {
                return false; // Too old
            }

            // Rule: Amount must be within valid range
            if (amount <= 0 || amount > 1000000)
            {
                return false; // Invalid amount
            }

            return true;
        }

        /// <summary>
        /// Calculates claim payout based on smart contract terms
        /// </summary>
        public decimal CalculatePayout(decimal billAmount)
        {
            // Smart contract payout logic:
            // - Bills up to 10,000: 80% coverage
            // - Bills 10,001 to 50,000: 70% coverage
            // - Bills above 50,000: 60% coverage

            if (billAmount <= 10000)
            {
                return billAmount * 0.80m;
            }
            else if (billAmount <= 50000)
            {
                return billAmount * 0.70m;
            }
            else
            {
                return billAmount * 0.60m;
            }
        }
    }

    /// <summary>
    /// Result object for insurance claim processing
    /// </summary>
    public class InsuranceClaimResult
    {
        public int PatientID { get; set; }
        public int BillID { get; set; }
        public decimal Amount { get; set; }
        public bool IsAutoTriggered { get; set; }
        public bool IsFraudSuspected { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal EstimatedPayout { get; set; }
    }
}
