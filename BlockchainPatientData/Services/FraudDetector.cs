using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainPatientData.Services
{
    /// <summary>
    /// AI Fraud Detector - Rule-based fraud detection system
    /// Simulates AI/ML fraud detection for insurance claims
    /// </summary>
    public class FraudDetector
    {
        private readonly decimal _suspiciousAmountThreshold;
        private readonly int _frequencyCheckDays;

        public FraudDetector()
        {
            _suspiciousAmountThreshold = 50000; // Default threshold
            _frequencyCheckDays = 30; // Check frequency within 30 days
        }

        public FraudDetector(decimal threshold, int frequencyDays)
        {
            _suspiciousAmountThreshold = threshold;
            _frequencyCheckDays = frequencyDays;
        }

        /// <summary>
        /// Checks if the bill amount is suspicious
        /// Rule 1: Amount exceeds high-value threshold
        /// </summary>
        public bool IsSuspicious(decimal amount)
        {
            return amount > _suspiciousAmountThreshold;
        }

        /// <summary>
        /// Advanced fraud detection with multiple rules
        /// </summary>
        public FraudDetectionResult AnalyzeBill(decimal amount, int patientId, List<BillHistory>? recentBills = null)
        {
            var result = new FraudDetectionResult
            {
                PatientID = patientId,
                Amount = amount,
                IsFraudulent = false,
                RiskScore = 0,
                Reasons = new List<string>()
            };

            // Rule 1: Extremely high amount
            if (amount > _suspiciousAmountThreshold)
            {
                result.RiskScore += 40;
                result.Reasons.Add($"⚠️ Unusually high amount: ₹{amount:N2}");
            }

            // Rule 2: Check for frequency (multiple bills in short time)
            if (recentBills != null && recentBills.Count > 5)
            {
                result.RiskScore += 30;
                result.Reasons.Add($"⚠️ Multiple bills detected ({recentBills.Count} in last {_frequencyCheckDays} days)");
            }

            // Rule 3: Round number detection (common in fraudulent claims)
            if (amount % 1000 == 0 && amount > 10000)
            {
                result.RiskScore += 15;
                result.Reasons.Add("⚠️ Suspiciously round amount");
            }

            // Rule 4: Rapid escalation (if recent bills show sudden spike)
            if (recentBills != null && recentBills.Any())
            {
                var avgRecentAmount = recentBills.Average(b => b.Amount);
                if (amount > avgRecentAmount * 3)
                {
                    result.RiskScore += 25;
                    result.Reasons.Add($"⚠️ Amount is 3x higher than recent average (₹{avgRecentAmount:N2})");
                }
            }

            // Final fraud determination
            if (result.RiskScore >= 50)
            {
                result.IsFraudulent = true;
                result.Recommendation = "🚨 HIGH RISK - Manual review required";
            }
            else if (result.RiskScore >= 30)
            {
                result.IsFraudulent = false;
                result.Recommendation = "⚠️ MEDIUM RISK - Additional verification recommended";
            }
            else
            {
                result.IsFraudulent = false;
                result.Recommendation = "✅ LOW RISK - Normal processing";
            }

            return result;
        }

        /// <summary>
        /// Checks for duplicate bills (possible fraud)
        /// </summary>
        public bool IsDuplicateBill(string description, decimal amount, List<BillHistory> recentBills)
        {
            if (recentBills == null || !recentBills.Any())
                return false;

            // Check if similar bill exists in recent history
            return recentBills.Any(b => 
                b.Description.Equals(description, StringComparison.OrdinalIgnoreCase) 
                && Math.Abs(b.Amount - amount) < 100
                && (DateTime.Now - b.Date).Days < 7
            );
        }

        /// <summary>
        /// Generates risk score visualization
        /// </summary>
        public string GetRiskLevel(int riskScore)
        {
            return riskScore switch
            {
                >= 70 => "🔴 CRITICAL",
                >= 50 => "🟠 HIGH",
                >= 30 => "🟡 MEDIUM",
                >= 10 => "🟢 LOW",
                _ => "✅ MINIMAL"
            };
        }
    }

    /// <summary>
    /// Fraud detection result object
    /// </summary>
    public class FraudDetectionResult
    {
        public int PatientID { get; set; }
        public decimal Amount { get; set; }
        public bool IsFraudulent { get; set; }
        public int RiskScore { get; set; } // 0-100
        public List<string> Reasons { get; set; } = new List<string>();
        public string Recommendation { get; set; } = string.Empty;

        public string GetRiskBadgeClass()
        {
            return RiskScore switch
            {
                >= 70 => "badge-danger",
                >= 50 => "badge-warning",
                >= 30 => "badge-info",
                _ => "badge-success"
            };
        }
    }

    /// <summary>
    /// Helper class for bill history tracking
    /// </summary>
    public class BillHistory
    {
        public int BillID { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
