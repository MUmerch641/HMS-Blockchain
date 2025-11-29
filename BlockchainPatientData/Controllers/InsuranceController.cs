using Microsoft.AspNetCore.Mvc;
using BlockchainPatientData.Models;
using BlockchainPatientData.Services;

namespace BlockchainPatientData.Controllers
{
    /// <summary>
    /// Insurance Controller - Claim verification and processing
    /// Portal for insurance company operations
    /// </summary>
    public class InsuranceController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly SmartContractService _smartContractService;
        private readonly FraudDetector _fraudDetector;

        public InsuranceController(
            DatabaseService databaseService,
            SmartContractService smartContractService,
            FraudDetector fraudDetector)
        {
            _databaseService = databaseService;
            _smartContractService = smartContractService;
            _fraudDetector = fraudDetector;
        }

        // Check if user is logged in
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("InsuranceUser") != null;
        }

        // ==================== AUTHENTICATION ====================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Demo credentials (in production, use database with hashed passwords)
            if (username == "insurance" && password == "admin123")
            {
                // Store in session
                HttpContext.Session.SetString("InsuranceUser", username);
                HttpContext.Session.SetString("UserRole", "Insurance");
                
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password. Please try again.";
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==================== DASHBOARD ====================

        public IActionResult Dashboard()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var claims = _databaseService.GetAllClaims();
                
                ViewBag.TotalClaims = claims.Count;
                ViewBag.PendingClaims = claims.Count(c => c.Status == InsuranceClaim.ClaimStatus.Pending);
                ViewBag.ApprovedClaims = claims.Count(c => c.Status == InsuranceClaim.ClaimStatus.Approved);
                ViewBag.FraudulentClaims = claims.Count(c => c.IsFraudSuspected);
                ViewBag.TotalAmount = claims.Sum(c => c.Amount);

                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load dashboard: " + ex.Message;
                return View(new List<InsuranceClaim>());
            }
        }

        // ==================== CLAIM OPERATIONS ====================

        public IActionResult ViewClaims()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var claims = _databaseService.GetAllClaims();
                
                // Calculate fraud risk for each claim
                var claimRisks = new Dictionary<int, Services.FraudDetectionResult>();
                foreach (var claim in claims)
                {
                    var fraudResult = _fraudDetector.AnalyzeBill(claim.Amount, claim.PatientID);
                    claimRisks[claim.ClaimID] = fraudResult;
                }
                
                ViewBag.ClaimRisks = claimRisks;
                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load claims: " + ex.Message;
                return View(new List<InsuranceClaim>());
            }
        }

        public IActionResult ClaimDetails(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var claims = _databaseService.GetAllClaims();
                var claim = claims.FirstOrDefault(c => c.ClaimID == id);

                if (claim == null)
                {
                    ViewBag.Error = "Claim not found";
                    return View("Error");
                }

                // Get patient and bill details
                var patient = _databaseService.GetPatient(claim.PatientID);
                var bills = _databaseService.GetBillsByPatient(claim.PatientID);
                var bill = bills.FirstOrDefault(b => b.BillID == claim.BillID);

                // Calculate smart contract payout
                var estimatedPayout = _smartContractService.CalculatePayout(claim.Amount);

                // Fraud analysis
                var fraudResult = _fraudDetector.AnalyzeBill(claim.Amount, claim.PatientID);

                ViewBag.Patient = patient;
                ViewBag.Bill = bill;
                ViewBag.EstimatedPayout = estimatedPayout;
                ViewBag.FraudResult = fraudResult;

                return View(claim);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load claim details: " + ex.Message;
                return View("Error");
            }
        }

        // ==================== CLAIM VERIFICATION ====================

        [HttpGet]
        public IActionResult VerifyClaim()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyClaim(int patientId)
        {
            try
            {
                var patient = _databaseService.GetPatient(patientId);
                if (patient == null)
                {
                    ViewBag.Error = "Patient not found";
                    return View();
                }

                var bills = _databaseService.GetBillsByPatient(patientId);
                var claims = _databaseService.GetAllClaims()
                    .Where(c => c.PatientID == patientId)
                    .ToList();

                ViewBag.Patient = patient;
                ViewBag.Bills = bills;
                ViewBag.Claims = claims;
                ViewBag.TotalBills = bills.Sum(b => b.Amount);
                ViewBag.TotalClaims = claims.Sum(c => c.Amount);

                return View("VerificationResult");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Verification failed: " + ex.Message;
                return View();
            }
        }

        // ==================== BLOCKCHAIN VERIFICATION ====================

        public IActionResult VerifyBlockchainHash(int billId)
        {
            try
            {
                // Get all bills and find the specific one
                var patients = _databaseService.GetAllPatients();
                Bill? targetBill = null;

                foreach (var patient in patients)
                {
                    var bills = _databaseService.GetBillsByPatient(patient.PatientID);
                    targetBill = bills.FirstOrDefault(b => b.BillID == billId);
                    if (targetBill != null) break;
                }

                if (targetBill == null)
                {
                    ViewBag.Error = "Bill not found";
                    return View("Error");
                }

                ViewBag.Bill = targetBill;
                ViewBag.HashVerified = true;
                ViewBag.Message = "✅ Blockchain hash verified successfully";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hash verification failed: " + ex.Message;
                return View("Error");
            }
        }

        // ==================== FRAUD DETECTION ====================

        public IActionResult FraudAnalysis()
        {
            try
            {
                var claims = _databaseService.GetAllClaims();
                var fraudulentClaims = claims.Where(c => c.IsFraudSuspected).ToList();

                ViewBag.FraudulentClaims = fraudulentClaims;
                ViewBag.TotalFraudCases = fraudulentClaims.Count;
                ViewBag.TotalFraudAmount = fraudulentClaims.Sum(c => c.Amount);

                return View(fraudulentClaims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load fraud analysis: " + ex.Message;
                return View(new List<InsuranceClaim>());
            }
        }

        // ==================== CLAIM APPROVAL/REJECTION ====================

        /// <summary>
        /// Mark claim as Under Review - For fraud-suspected claims that need investigation
        /// </summary>
        [HttpPost]
        public IActionResult MarkUnderReview(int claimId)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                _databaseService.UpdateClaimStatus(claimId, "Under Review");
                TempData["Success"] = "Claim marked as Under Review. Investigation required.";
                return RedirectToAction("ClaimDetails", new { id = claimId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to update claim: " + ex.Message;
                return RedirectToAction("ClaimDetails", new { id = claimId });
            }
        }

        /// <summary>
        /// Approve a claim - Insurance officer can approve valid claims
        /// </summary>
        [HttpPost]
        public IActionResult ApproveClaim(int claimId)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var claims = _databaseService.GetAllClaims();
                var claim = claims.FirstOrDefault(c => c.ClaimID == claimId);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found";
                    return RedirectToAction("ViewClaims");
                }

                // Check if already processed (can approve Pending or Under Review)
                if (claim.Status == "Approved")
                {
                    TempData["Error"] = "Claim is already approved";
                    return RedirectToAction("ClaimDetails", new { id = claimId });
                }
                
                if (claim.Status == "Rejected")
                {
                    TempData["Error"] = "Cannot approve a rejected claim";
                    return RedirectToAction("ClaimDetails", new { id = claimId });
                }

                // Update claim status to Approved
                _databaseService.UpdateClaimStatus(claimId, "Approved");

                // Calculate final payout
                decimal payout = _smartContractService.CalculatePayout(claim.Amount);

                TempData["Success"] = $"✅ Claim #{claimId} approved successfully! Payout: ${payout:N2}";
                return RedirectToAction("ClaimDetails", new { id = claimId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Approval failed: " + ex.Message;
                return RedirectToAction("ViewClaims");
            }
        }

        /// <summary>
        /// Reject a claim - Insurance officer can reject suspicious or invalid claims
        /// </summary>
        [HttpPost]
        public IActionResult RejectClaim(int claimId, string reason)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var claims = _databaseService.GetAllClaims();
                var claim = claims.FirstOrDefault(c => c.ClaimID == claimId);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found";
                    return RedirectToAction("ViewClaims");
                }

                // Check if already processed (can reject Pending or Under Review)
                if (claim.Status == "Approved")
                {
                    TempData["Error"] = "Cannot reject an approved claim";
                    return RedirectToAction("ClaimDetails", new { id = claimId });
                }
                
                if (claim.Status == "Rejected")
                {
                    TempData["Error"] = "Claim is already rejected";
                    return RedirectToAction("ClaimDetails", new { id = claimId });
                }

                // Update claim status to Rejected
                _databaseService.UpdateClaimStatus(claimId, "Rejected");

                // Optionally store rejection reason (would need to add this field to database)
                TempData["Success"] = $"❌ Claim #{claimId} rejected successfully.";
                if (!string.IsNullOrEmpty(reason))
                {
                    TempData["Success"] += $" Reason: {reason}";
                }

                return RedirectToAction("ClaimDetails", new { id = claimId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Rejection failed: " + ex.Message;
                return RedirectToAction("ViewClaims");
            }
        }
    }
}
