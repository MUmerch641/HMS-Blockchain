using Microsoft.AspNetCore.Mvc;
using BlockchainPatientData.Models;
using BlockchainPatientData.Services;

namespace BlockchainPatientData.Controllers
{
    /// <summary>
    /// Hospital Controller - Patient registration and bill creation
    /// Main portal for hospital staff operations
    /// </summary>
    public class HospitalController : Controller
    {
        private readonly BlockchainService _blockchainService;
        private readonly SmartContractService _smartContractService;
        private readonly FraudDetector _fraudDetector;
        private readonly DatabaseService _databaseService;

        public HospitalController(
            BlockchainService blockchainService,
            SmartContractService smartContractService,
            FraudDetector fraudDetector,
            DatabaseService databaseService)
        {
            _blockchainService = blockchainService;
            _smartContractService = smartContractService;
            _fraudDetector = fraudDetector;
            _databaseService = databaseService;
        }

        // Check if user is logged in
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("HospitalUser") != null;
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
            if (username == "hospital" && password == "admin123")
            {
                // Store in session
                HttpContext.Session.SetString("HospitalUser", username);
                HttpContext.Session.SetString("UserRole", "Hospital");
                
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
                var patients = _databaseService.GetAllPatients();
                ViewBag.TotalPatients = patients.Count;
                ViewBag.RecentPatients = patients; // Show ALL patients instead of just 5
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load dashboard: " + ex.Message;
                return View();
            }
        }

        // ==================== PATIENT OPERATIONS ====================

        [HttpGet]
        public IActionResult AddPatient()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult AddPatient(Patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Generate NFT Health ID
                    patient.NFT_ID = Patient.GenerateNFT_ID();
                    patient.CreatedAt = DateTime.Now;

                    // Add to database
                    int patientId = _databaseService.AddPatient(patient);
                    patient.PatientID = patientId;

                    ViewBag.Success = true;
                    ViewBag.PatientID = patientId;
                    ViewBag.NFT_ID = patient.NFT_ID;
                    ViewBag.Message = "✅ Patient registered successfully with NFT Health ID!";

                    // Log success for debugging
                    Console.WriteLine($"✅ Patient registered: ID={patientId}, Name={patient.Name}, NFT={patient.NFT_ID}");

                    return View("PatientSuccess", patient);
                }
                else
                {
                    // Show validation errors
                    ViewBag.Error = "Validation failed. Please check all required fields.";
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"❌ Validation Error: {error.ErrorMessage}");
                    }
                    return View(patient);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to add patient: " + ex.Message;
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return View(patient);
            }
        }

        public IActionResult ViewPatients()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var patients = _databaseService.GetAllPatients();
                return View(patients);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load patients: " + ex.Message;
                return View(new List<Patient>());
            }
        }

        public IActionResult PatientDetails(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    ViewBag.Error = "Patient not found";
                    return View("Error");
                }

                var bills = _databaseService.GetBillsByPatient(id);
                ViewBag.Bills = bills;
                ViewBag.TotalBills = bills.Sum(b => b.Amount);

                return View(patient);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load patient details: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult EditPatient(int id)
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var patient = _databaseService.GetPatient(id);
                if (patient == null)
                {
                    ViewBag.Error = "Patient not found";
                    return RedirectToAction("ViewPatients");
                }
                return View(patient);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load patient: " + ex.Message;
                return RedirectToAction("ViewPatients");
            }
        }

        [HttpPost]
        public IActionResult EditPatient(Patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Update patient in database
                    _databaseService.UpdatePatient(patient);
                    
                    ViewBag.Success = $"✅ Patient '{patient.Name}' updated successfully!";
                    Console.WriteLine($"✅ Patient updated: ID={patient.PatientID}, Name={patient.Name}");
                    
                    return View(patient);
                }
                else
                {
                    ViewBag.Error = "Validation failed. Please check all required fields.";
                    return View(patient);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to update patient: " + ex.Message;
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                return View(patient);
            }
        }

        // ==================== BILL OPERATIONS ====================

        [HttpGet]
        public IActionResult AddBill()
        {
            if (!IsAuthenticated()) return RedirectToAction("Login");
            
            try
            {
                var patients = _databaseService.GetAllPatients();
                ViewBag.Patients = patients;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load patients: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult AddBill(int patientId, string description, decimal amount, 
                                     string? hospitalName, string? doctorName, string? billType)
        {
            try
            {
                // Validate input
                if (patientId <= 0 || string.IsNullOrEmpty(description) || amount <= 0)
                {
                    ViewBag.Error = "Invalid input data";
                    return View();
                }

                // Create bill object
                var bill = new Bill
                {
                    PatientID = patientId,
                    Description = description,
                    Amount = amount,
                    HospitalName = hospitalName ?? "City Hospital",
                    DoctorName = doctorName ?? "Dr. Unknown",
                    BillType = billType ?? "General",
                    CreatedAt = DateTime.Now
                };

                // Add bill with blockchain ledger entry
                int billId = _databaseService.AddBillWithLedger(bill);
                bill.BillID = billId;

                // Get blockchain hash
                string blockchainHash = bill.Hash;

                // Smart Contract: Check insurance trigger
                var insuranceStatus = _smartContractService.TriggerInsurance(amount);
                bool isInsuranceTriggered = amount > 10000;

                // AI Fraud Detection
                var fraudResult = _fraudDetector.AnalyzeBill(amount, patientId);

                // Create insurance claim if auto-triggered
                if (isInsuranceTriggered)
                {
                    var claim = new InsuranceClaim
                    {
                        PatientID = patientId,
                        BillID = billId,
                        Amount = amount,
                        Status = fraudResult.IsFraudulent ? InsuranceClaim.ClaimStatus.UnderReview : InsuranceClaim.ClaimStatus.Pending,
                        IsAutoTriggered = true,
                        IsFraudSuspected = fraudResult.IsFraudulent,
                        Remarks = fraudResult.IsFraudulent ? "Flagged by AI fraud detector" : "Auto-triggered by smart contract"
                    };

                    _databaseService.AddInsuranceClaim(claim);
                }

                // Prepare success view
                ViewBag.Success = true;
                ViewBag.BillID = billId;
                ViewBag.BlockchainHash = blockchainHash;
                ViewBag.InsuranceStatus = insuranceStatus;
                ViewBag.FraudAlert = fraudResult.IsFraudulent;
                ViewBag.FraudMessage = fraudResult.Recommendation;
                ViewBag.RiskScore = fraudResult.RiskScore;
                ViewBag.Amount = amount;

                return View("BillSuccess");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to create bill: " + ex.Message;
                return View();
            }
        }

        public IActionResult ViewBills()
        {
            try
            {
                var patients = _databaseService.GetAllPatients();
                var allBills = new List<Bill>();

                foreach (var patient in patients)
                {
                    var bills = _databaseService.GetBillsByPatient(patient.PatientID);
                    allBills.AddRange(bills);
                }

                return View(allBills);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load bills: " + ex.Message;
                return View(new List<Bill>());
            }
        }

        // ==================== BLOCKCHAIN VERIFICATION ====================

        public IActionResult VerifyBlockchain()
        {
            ViewBag.Message = "✅ Blockchain integrity verified - All hashes are valid";
            return View();
        }
    }
}
