using Microsoft.AspNetCore.Mvc;
using BlockchainPatientData.Models;
using BlockchainPatientData.Services;

namespace BlockchainPatientData.Controllers
{
    /// <summary>
    /// Patient Controller - View personal records and bills
    /// Portal for patient access to their own medical data
    /// </summary>
    public class PatientController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly BlockchainService _blockchainService;

        public PatientController(DatabaseService databaseService, BlockchainService blockchainService)
        {
            _databaseService = databaseService;
            _blockchainService = blockchainService;
        }

        // ==================== PATIENT LOGIN ====================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(int patientId)
        {
            try
            {
                var patient = _databaseService.GetPatient(patientId);
                if (patient == null)
                {
                    ViewBag.Error = "Patient not found. Please check your Patient ID.";
                    return View();
                }

                // Store patient ID in session (simple authentication simulation)
                HttpContext.Session.SetInt32("PatientID", patientId);
                
                return RedirectToAction("Dashboard", new { id = patientId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Login failed: " + ex.Message;
                return View();
            }
        }

        // ==================== DASHBOARD ====================

        public IActionResult Dashboard(int id)
        {
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                var bills = _databaseService.GetBillsByPatient(id);
                var claims = _databaseService.GetAllClaims()
                    .Where(c => c.PatientID == id)
                    .ToList();

                ViewBag.Bills = bills;
                ViewBag.Claims = claims;
                ViewBag.TotalBills = bills.Sum(b => b.Amount);
                ViewBag.TotalClaims = claims.Sum(c => c.Amount);
                ViewBag.RecentBills = bills.Take(5).ToList();

                return View(patient);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load dashboard: " + ex.Message;
                return View();
            }
        }

        // ==================== VIEW RECORDS ====================

        public IActionResult ViewRecords(int id)
        {
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                ViewBag.Patient = patient;
                return View(patient);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load records: " + ex.Message;
                return View();
            }
        }

        // ==================== VIEW BILLS ====================

        public IActionResult ViewBills(int id)
        {
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                var bills = _databaseService.GetBillsByPatient(id);
                
                ViewBag.Patient = patient;
                ViewBag.TotalAmount = bills.Sum(b => b.Amount);

                return View(bills);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load bills: " + ex.Message;
                return View(new List<Bill>());
            }
        }

        // ==================== BILL DETAILS ====================

        public IActionResult BillDetails(int patientId, int billId)
        {
            try
            {
                var patient = _databaseService.GetPatient(patientId);
                var bills = _databaseService.GetBillsByPatient(patientId);
                var bill = bills.FirstOrDefault(b => b.BillID == billId);

                if (bill == null)
                {
                    ViewBag.Error = "Bill not found";
                    return View("Error");
                }

                ViewBag.Patient = patient;
                ViewBag.HashVerified = true;

                return View(bill);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load bill details: " + ex.Message;
                return View("Error");
            }
        }

        // ==================== VIEW INSURANCE CLAIMS ====================

        public IActionResult ViewClaims(int id)
        {
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                var claims = _databaseService.GetAllClaims()
                    .Where(c => c.PatientID == id)
                    .ToList();

                ViewBag.Patient = patient;
                ViewBag.TotalClaimAmount = claims.Sum(c => c.Amount);
                ViewBag.ApprovedAmount = claims
                    .Where(c => c.Status == InsuranceClaim.ClaimStatus.Approved)
                    .Sum(c => c.Amount);

                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load claims: " + ex.Message;
                return View(new List<InsuranceClaim>());
            }
        }

        // ==================== VERIFY DATA INTEGRITY ====================

        public IActionResult VerifyData(int patientId, int billId)
        {
            try
            {
                var bills = _databaseService.GetBillsByPatient(patientId);
                var bill = bills.FirstOrDefault(b => b.BillID == billId);

                if (bill == null)
                {
                    ViewBag.Error = "Bill not found";
                    return View("Error");
                }

                // Recalculate hash to verify integrity
                string recalculatedHash = _blockchainService.GenerateBillHash(
                    bill.PatientID,
                    bill.Description,
                    bill.Amount,
                    bill.CreatedAt
                );

                bool isValid = recalculatedHash.Equals(bill.Hash, StringComparison.OrdinalIgnoreCase);

                ViewBag.Bill = bill;
                ViewBag.RecalculatedHash = recalculatedHash;
                ViewBag.StoredHash = bill.Hash;
                ViewBag.IsValid = isValid;
                ViewBag.Message = isValid 
                    ? "✅ Data integrity verified - No tampering detected" 
                    : "❌ Warning: Data may have been tampered with";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Verification failed: " + ex.Message;
                return View("Error");
            }
        }

        // ==================== NFT HEALTH ID ====================

        public IActionResult ViewNFT(int id)
        {
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                ViewBag.Message = "Your unique NFT-based Health ID ensures secure blockchain identity";
                return View(patient);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load NFT details: " + ex.Message;
                return View();
            }
        }

        // ==================== BLOCKCHAIN VERIFICATION ====================

        /// <summary>
        /// Verify blockchain integrity - Test if any bill data has been tampered
        /// AND validate the entire blockchain chain structure
        /// </summary>
        public IActionResult VerifyBlockchain()
        {
            try
            {
                // Get all patients and their bills
                var allPatients = _databaseService.GetAllPatients();
                var allBills = new List<Bill>();
                
                foreach (var patient in allPatients)
                {
                    var bills = _databaseService.GetBillsByPatient(patient.PatientID);
                    allBills.AddRange(bills);
                }

                // Sort bills by BillID to process in blockchain order
                allBills = allBills.OrderBy(b => b.BillID).ToList();

                // Get all ledger entries for blockchain chain validation
                var ledgerEntries = _databaseService.GetAllLedgerEntries();

                // Validate each bill's hash AND blockchain chain
                var validationResults = new Dictionary<int, bool>();
                var computedHashes = new Dictionary<int, string>();
                var chainValidation = new Dictionary<int, bool>();
                
                // Track previous hash for blockchain chain validation
                string expectedPreviousHash = "0000000000000000000000000000000000000000000000000000000000000000"; // Genesis
                
                foreach (var bill in allBills)
                {
                    // Get the ledger entry for this bill
                    var ledgerEntry = ledgerEntries.FirstOrDefault(l => l.BillID == bill.BillID);
                    
                    // Truncate milliseconds from CreatedAt to match hash generation format
                    var truncatedCreatedAt = new DateTime(
                        bill.CreatedAt.Year,
                        bill.CreatedAt.Month,
                        bill.CreatedAt.Day,
                        bill.CreatedAt.Hour,
                        bill.CreatedAt.Minute,
                        bill.CreatedAt.Second,
                        0  // Set milliseconds to 0
                    );
                    
                    // Get the previous hash from ledger (blockchain chain)
                    string previousHashFromLedger = ledgerEntry?.PreviousHash ?? expectedPreviousHash;
                    
                    // Recompute BLOCKCHAIN hash (including previous hash)
                    string computedHash = _blockchainService.GenerateBlockchainHash(
                        bill.PatientID, 
                        bill.Description, 
                        bill.Amount, 
                        truncatedCreatedAt,
                        previousHashFromLedger  // Use the previous block's hash
                    );
                    
                    computedHashes[bill.BillID] = computedHash;
                    
                    // Validate 1: Check if current block's hash matches stored hash
                    bool hashMatches = bill.Hash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
                    
                    // Validate 2: Check if previous hash in chain is correct
                    bool chainLinked = true;
                    if (ledgerEntry != null)
                    {
                        string actualPreviousHash = ledgerEntry.PreviousHash ?? LedgerEntry.GENESIS_HASH;
                        chainLinked = actualPreviousHash.Equals(expectedPreviousHash, StringComparison.OrdinalIgnoreCase);
                        
                        if (!chainLinked)
                        {
                            Console.WriteLine($"⛓️ CHAIN BROKEN at Block #{bill.BillID}!");
                            Console.WriteLine($"   Expected PreviousHash: {expectedPreviousHash.Substring(0, 20)}...");
                            Console.WriteLine($"   Actual PreviousHash: {actualPreviousHash.Substring(0, 20)}...");
                        }
                    }
                    
                    chainValidation[bill.BillID] = chainLinked;
                    
                    // Overall validation: Both hash AND chain must be valid
                    bool isValid = hashMatches && chainLinked;
                    validationResults[bill.BillID] = isValid;
                    
                    // Debug logging for failures
                    if (!isValid)
                    {
                        Console.WriteLine($"❌ Block #{bill.BillID} VALIDATION FAILED:");
                        Console.WriteLine($"   Hash Valid: {hashMatches}");
                        Console.WriteLine($"   Chain Valid: {chainLinked}");
                        Console.WriteLine($"   PatientID: {bill.PatientID}");
                        Console.WriteLine($"   Description: {bill.Description}");
                        Console.WriteLine($"   Amount: {bill.Amount}");
                        Console.WriteLine($"   CreatedAt (truncated): {truncatedCreatedAt:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine($"   Previous Hash: {previousHashFromLedger.Substring(0, 20)}...");
                        Console.WriteLine($"   Stored Hash: {bill.Hash.Substring(0, 20)}...");
                        Console.WriteLine($"   Computed Hash: {computedHash.Substring(0, 20)}...");
                    }
                    
                    // Update expected previous hash for next block
                    expectedPreviousHash = bill.Hash;
                }

                ViewBag.Bills = allBills;
                ViewBag.ValidationResults = validationResults;
                ViewBag.ComputedHashes = computedHashes;
                ViewBag.ChainValidation = chainValidation;
                ViewBag.LedgerEntries = ledgerEntries;
                ViewBag.IsFullChainValid = validationResults.All(v => v.Value);

                if (ViewBag.IsFullChainValid)
                {
                    Console.WriteLine("✅ BLOCKCHAIN INTEGRITY: Entire chain is valid!");
                }
                else
                {
                    Console.WriteLine("❌ BLOCKCHAIN INTEGRITY: Chain has been compromised!");
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Blockchain verification failed: " + ex.Message;
                ViewBag.Bills = new List<Bill>();
                ViewBag.ValidationResults = new Dictionary<int, bool>();
                ViewBag.ComputedHashes = new Dictionary<int, string>();
                ViewBag.ChainValidation = new Dictionary<int, bool>();
                ViewBag.IsFullChainValid = false;
                return View();
            }
        }

        // ==================== LOGOUT ====================

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
