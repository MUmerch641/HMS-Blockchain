# 🏗️ System Architecture
## Blockchain-Based Patient Data Management System

---

## 1. SYSTEM OVERVIEW

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                       │
│  ┌──────────┐  ┌────────────┐  ┌──────────┐                │
│  │ Hospital │  │ Insurance  │  │  Patient │                │
│  │  Portal  │  │   Portal   │  │  Portal  │                │
│  └──────────┘  └────────────┘  └──────────┘                │
│                  (ASP.NET MVC Views)                         │
└─────────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────────┐
│                     BUSINESS LOGIC LAYER                     │
│  ┌────────────┐  ┌──────────┐  ┌───────────┐               │
│  │ Blockchain │  │  Smart   │  │    AI     │               │
│  │  Service   │  │ Contract │  │   Fraud   │               │
│  │  (SHA256)  │  │ Service  │  │ Detector  │               │
│  └────────────┘  └──────────┘  └───────────┘               │
│         Controllers + Services (C#)                          │
└─────────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────────┐
│                      DATA ACCESS LAYER                       │
│               Database Service (ADO.NET)                     │
└─────────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────────┐
│                      DATABASE LAYER                          │
│     SQL Server (Patients, Bills, Ledger, Claims)           │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. TECHNOLOGY STACK

### Backend
- **Framework**: ASP.NET Core 6.0 MVC
- **Language**: C# 10
- **Database**: SQL Server 2019+
- **Data Access**: ADO.NET with SqlClient

### Frontend
- **Template Engine**: Razor Views (.cshtml)
- **CSS Framework**: Bootstrap 5.3
- **JavaScript**: Vanilla JS (ES6+)
- **Icons**: Unicode Emojis

### Cryptography
- **Hashing**: SHA256 (System.Security.Cryptography)
- **Encoding**: UTF-8

### Development Tools
- **IDE**: Visual Studio 2022
- **Database Tool**: SQL Server Management Studio (SSMS)
- **Version Control**: Git

---

## 3. PROJECT STRUCTURE

```
HMS_Blockchain_Project/
│
├── BlockchainPatientData/           # Main ASP.NET Project
│   ├── Controllers/                 # MVC Controllers
│   │   ├── HomeController.cs
│   │   ├── HospitalController.cs
│   │   ├── InsuranceController.cs
│   │   └── PatientController.cs
│   │
│   ├── Models/                      # Data Models
│   │   ├── Patient.cs
│   │   ├── Bill.cs
│   │   ├── LedgerEntry.cs
│   │   └── InsuranceClaim.cs
│   │
│   ├── Services/                    # Business Logic
│   │   ├── BlockchainService.cs     # SHA256 hashing
│   │   ├── SmartContractService.cs  # Insurance automation
│   │   ├── FraudDetector.cs         # AI fraud detection
│   │   └── DatabaseService.cs       # Database operations
│   │
│   ├── Views/                       # Razor Views
│   │   ├── Home/
│   │   ├── Hospital/
│   │   ├── Insurance/
│   │   ├── Patient/
│   │   └── Shared/
│   │
│   ├── wwwroot/                     # Static Files
│   │   ├── css/site.css
│   │   └── js/site.js
│   │
│   ├── Program.cs                   # Application entry point
│   ├── appsettings.json            # Configuration
│   └── BlockchainPatientData.csproj # Project file
│
├── Database/                        # SQL Scripts
│   └── setup.sql                    # Database schema
│
├── Documentation/                   # Project Docs
│   ├── VivaQA.md
│   └── Architecture.md
│
├── README.md                        # Project overview
└── QUICKSTART.md                   # Setup guide
```

---

## 4. DATABASE SCHEMA

### Entity Relationship Diagram (ERD)

```
┌─────────────────┐
│    Patients     │
├─────────────────┤
│ PatientID (PK)  │───┐
│ Name            │   │
│ Age             │   │
│ Gender          │   │
│ NFT_ID (UNIQUE) │   │
│ Address         │   │
│ PhoneNumber     │   │
│ Email           │   │
│ BloodGroup      │   │
│ CreatedAt       │   │
└─────────────────┘   │
                      │
        ┌─────────────┴─────────────┐
        │                           │
        ▼                           ▼
┌─────────────────┐         ┌──────────────────┐
│     Bills       │         │ InsuranceClaims  │
├─────────────────┤         ├──────────────────┤
│ BillID (PK)     │───┐     │ ClaimID (PK)     │
│ PatientID (FK)  │   │     │ PatientID (FK)   │
│ Description     │   │     │ BillID (FK)      │
│ Amount          │   │     │ Amount           │
│ Hash            │   │     │ Status           │
│ CreatedAt       │   │     │ IsAutoTriggered  │
│ HospitalName    │   │     │ IsFraudSuspected │
│ DoctorName      │   │     │ ClaimDate        │
│ BillType        │   │     │ ApprovedDate     │
└─────────────────┘   │     │ Remarks          │
                      │     └──────────────────┘
                      │
                      ▼
              ┌─────────────────┐
              │     Ledger      │
              ├─────────────────┤
              │ LedgerID (PK)   │
              │ BillID (FK)     │
              │ PatientID (FK)  │
              │ BillHash        │
              │ PreviousHash    │
              │ BlockNumber     │
              │ Timestamp       │
              │ IsVerified      │
              └─────────────────┘
```

### Table Details

#### 1. Patients Table
```sql
PatientID       INT PRIMARY KEY IDENTITY
Name            NVARCHAR(100) NOT NULL
Age             INT CHECK (Age > 0 AND Age < 150)
Gender          NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other'))
NFT_ID          NVARCHAR(50) UNIQUE NOT NULL
Address         NVARCHAR(200)
PhoneNumber     NVARCHAR(15)
Email           NVARCHAR(100)
BloodGroup      NVARCHAR(5)
CreatedAt       DATETIME DEFAULT GETDATE()
```

#### 2. Bills Table
```sql
BillID          INT PRIMARY KEY IDENTITY
PatientID       INT FOREIGN KEY REFERENCES Patients(PatientID)
Description     NVARCHAR(500) NOT NULL
Amount          DECIMAL(10,2) CHECK (Amount > 0)
Hash            NVARCHAR(100) NOT NULL
CreatedAt       DATETIME DEFAULT GETDATE()
HospitalName    NVARCHAR(100)
DoctorName      NVARCHAR(100)
BillType        NVARCHAR(50)
```

#### 3. Ledger Table (Blockchain Simulation)
```sql
LedgerID        INT PRIMARY KEY IDENTITY
BillID          INT FOREIGN KEY REFERENCES Bills(BillID)
PatientID       INT FOREIGN KEY REFERENCES Patients(PatientID)
BillHash        NVARCHAR(100) NOT NULL
PreviousHash    NVARCHAR(100)
BlockNumber     INT NOT NULL
Timestamp       DATETIME DEFAULT GETDATE()
IsVerified      BIT DEFAULT 1
```

#### 4. InsuranceClaims Table
```sql
ClaimID         INT PRIMARY KEY IDENTITY
PatientID       INT FOREIGN KEY REFERENCES Patients(PatientID)
BillID          INT FOREIGN KEY REFERENCES Bills(BillID)
Amount          DECIMAL(10,2) NOT NULL
Status          NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'Under Review'))
IsAutoTriggered BIT DEFAULT 0
IsFraudSuspected BIT DEFAULT 0
ClaimDate       DATETIME DEFAULT GETDATE()
ApprovedDate    DATETIME NULL
Remarks         NVARCHAR(500)
```

---

## 5. WORKFLOW DIAGRAMS

### A. Patient Registration Flow

```
┌──────────┐
│ Hospital │
│  Staff   │
└────┬─────┘
     │
     ▼
┌─────────────────────┐
│ Enter Patient Info  │
│ (Name, Age, Gender) │
└─────────┬───────────┘
          │
          ▼
┌──────────────────────┐
│ Generate NFT Health  │
│ ID (GUID-based)      │
└─────────┬────────────┘
          │
          ▼
┌──────────────────────┐
│ Save to Patients DB  │
└─────────┬────────────┘
          │
          ▼
┌──────────────────────┐
│ Display Success +    │
│ NFT ID to Staff      │
└──────────────────────┘
```

### B. Bill Creation & Blockchain Flow

```
┌──────────┐
│ Hospital │
│  Staff   │
└────┬─────┘
     │
     ▼
┌──────────────────────────┐
│ Select Patient & Enter   │
│ Bill Details             │
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────┐
│ Generate SHA256 Hash     │
│ Hash = SHA256(PatientID  │
│ + Description + Amount)  │
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────┐
│ Save Bill to Database    │
│ (with Hash)              │
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────┐
│ Get Previous Ledger Hash │
│ (for blockchain chaining)│
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────┐
│ Create Ledger Entry      │
│ (Block in Blockchain)    │
└────────┬─────────────────┘
         │
         ▼
┌──────────────────────────┐
│ Smart Contract Evaluation│
│ Is Amount > ₹10,000?     │
└────────┬─────────────────┘
         │
    ┌────┴────┐
    │         │
   YES       NO
    │         │
    ▼         ▼
┌─────────┐  ┌───────────┐
│ Auto-   │  │ No Claim  │
│ Trigger │  │ Required  │
│ Claim   │  └───────────┘
└────┬────┘
     │
     ▼
┌──────────────────────────┐
│ AI Fraud Detection       │
│ Calculate Risk Score     │
└────────┬─────────────────┘
         │
    ┌────┴────┐
    │         │
   Safe   Suspicious
    │         │
    ▼         ▼
┌─────────┐  ┌───────────┐
│ Approve │  │ Flag for  │
│ Claim   │  │ Review    │
└─────────┘  └───────────┘
         │
         ▼
┌──────────────────────────┐
│ Display Success Screen   │
│ with Blockchain Hash     │
└──────────────────────────┘
```

### C. Insurance Claim Verification

```
┌──────────────┐
│  Insurance   │
│   Company    │
└──────┬───────┘
       │
       ▼
┌────────────────────┐
│ Enter Patient ID   │
└─────────┬──────────┘
          │
          ▼
┌────────────────────┐
│ Fetch Patient Data │
│ + Bills + Claims   │
└─────────┬──────────┘
          │
          ▼
┌────────────────────────┐
│ For Each Bill:         │
│ Recalculate Hash       │
└─────────┬──────────────┘
          │
          ▼
┌────────────────────────┐
│ Compare:               │
│ Stored Hash ==         │
│ Calculated Hash?       │
└─────────┬──────────────┘
          │
     ┌────┴────┐
     │         │
    YES       NO
     │         │
     ▼         ▼
┌─────────┐  ┌──────────┐
│ ✅ Valid│  │ ❌ Data  │
│  Data   │  │ Tampered │
└─────────┘  └──────────┘
     │
     ▼
┌────────────────────────┐
│ Display Verification   │
│ Result Dashboard       │
└────────────────────────┘
```

---

## 6. KEY COMPONENTS

### A. BlockchainService.cs

**Purpose**: Handles all cryptographic operations

**Key Methods**:
```csharp
string GenerateHash(string input)
// Generates SHA256 hash from input string

string GenerateBillHash(int patientId, string desc, decimal amount, DateTime timestamp)
// Creates hash specifically for bill data

string GenerateBlockHash(string currentData, string previousHash)
// Links blocks together in blockchain

bool ValidateHash(string data, string hash)
// Verifies data integrity
```

**Algorithm**:
```
1. Convert input string to UTF-8 bytes
2. Apply SHA256 hash function
3. Convert resulting bytes to hexadecimal string
4. Return 64-character hash
```

---

### B. SmartContractService.cs

**Purpose**: Simulates blockchain smart contracts

**Business Rules**:
1. **Auto-Trigger Rule**: Amount > ₹10,000 → Create insurance claim
2. **Small Claim Rule**: Amount ≤ ₹5,000 → Auto-approve
3. **Fraud Check Rule**: If flagged → Status = "Under Review"

**Payout Calculation**:
```
Bill Amount          Coverage
≤ ₹10,000      →     80%
₹10,001-50,000 →     70%
> ₹50,000      →     60%
```

---

### C. FraudDetector.cs

**Purpose**: AI-based fraud detection system

**Risk Scoring Logic**:
```
Base Score: 0

+40 points → Amount > ₹50,000
+30 points → Frequency > 5 bills/month
+15 points → Round numbers (₹10,000, ₹20,000)
+25 points → 3x higher than patient's average

Final Score:
  0-29   → Low Risk (✅)
  30-49  → Medium Risk (⚠️)
  50-69  → High Risk (🟠)
  70-100 → Critical Risk (🔴)
```

---

### D. DatabaseService.cs

**Purpose**: Database operations (CRUD)

**Key Operations**:
- `AddPatient()` - Insert new patient with NFT ID
- `AddBillWithLedger()` - Transactional bill + ledger insertion
- `GetBillsByPatient()` - Retrieve patient's billing history
- `AddInsuranceClaim()` - Create claim record
- `GetAllClaims()` - Fetch all insurance claims

---

## 7. SECURITY ARCHITECTURE

### Data Integrity
```
┌──────────────┐
│ Original     │
│ Bill Data    │
└──────┬───────┘
       │
       ▼
┌──────────────┐     ┌─────────────┐
│ SHA256 Hash  │────▶│ Store in DB │
└──────────────┘     └─────────────┘

Later:
┌──────────────┐
│ Retrieve     │
│ Bill Data    │
└──────┬───────┘
       │
       ▼
┌──────────────┐     ┌─────────────┐
│ Recalculate  │────▶│ Compare     │
│ Hash         │     │ with Stored │
└──────────────┘     └──────┬──────┘
                            │
                       ┌────┴────┐
                       │         │
                      Same   Different
                       │         │
                       ▼         ▼
                   ✅ Valid  ❌ Tampered
```

### Access Control (Proposed for Production)
```
User Role        Permissions
─────────────────────────────
Hospital Admin   - Register patients
                 - Create bills
                 - View own hospital data

Insurance Co.    - View claims
                 - Verify bills
                 - Approve/Reject claims

Patient          - View own records
                 - Verify bill authenticity
                 - View claim status

System Admin     - Full access
                 - User management
                 - System configuration
```

---

## 8. DEPLOYMENT ARCHITECTURE

### Development Environment
```
┌──────────────────────────────┐
│     Developer Machine        │
│  ┌────────────────────────┐  │
│  │ Visual Studio 2022     │  │
│  │ SQL Server LocalDB     │  │
│  │ IIS Express           │  │
│  └────────────────────────┘  │
└──────────────────────────────┘
```

### Production Environment (Proposed)
```
┌──────────────────────────────────────────────┐
│              Cloud Platform                  │
│  ┌────────────────────────────────────────┐  │
│  │        Application Tier                │  │
│  │  ┌──────────┐  ┌──────────┐           │  │
│  │  │ IIS/     │  │ IIS/     │ (Load     │  │
│  │  │ Kestrel  │  │ Kestrel  │ Balanced) │  │
│  │  └────┬─────┘  └────┬─────┘           │  │
│  └───────┼─────────────┼──────────────────┘  │
│          │             │                      │
│  ┌───────▼─────────────▼──────────────────┐  │
│  │        Database Tier                   │  │
│  │  ┌──────────────┐  ┌──────────────┐   │  │
│  │  │ SQL Server   │  │ SQL Server   │   │  │
│  │  │ (Primary)    │──│ (Replica)    │   │  │
│  │  └──────────────┘  └──────────────┘   │  │
│  └────────────────────────────────────────┘  │
└──────────────────────────────────────────────┘
```

---

## 9. PERFORMANCE CONSIDERATIONS

### Database Indexing
```sql
-- Primary Keys (Auto-indexed)
-- Foreign Keys (Indexed)

-- Additional Indexes
CREATE INDEX IDX_Bills_PatientID ON Bills(PatientID);
CREATE INDEX IDX_Ledger_BillID ON Ledger(BillID);
CREATE INDEX IDX_Claims_Status ON InsuranceClaims(Status);
CREATE INDEX IDX_Patients_NFTID ON Patients(NFT_ID);
```

### Caching Strategy (Future Enhancement)
```
Level 1: In-Memory Cache (Patient data)
Level 2: Redis Cache (Frequently accessed bills)
Level 3: Database
```

### Optimization Techniques
- **Parameterized Queries**: Prevent SQL injection + improve performance
- **Connection Pooling**: Reuse database connections
- **Lazy Loading**: Load related data only when needed
- **Asynchronous Operations**: Non-blocking I/O for database calls

---

## 10. SCALABILITY

### Horizontal Scaling
```
Current: Single server
Future:  Multiple app servers behind load balancer
```

### Database Sharding (Future)
```
Shard 1: Patients with ID 1-10000
Shard 2: Patients with ID 10001-20000
Shard 3: Patients with ID 20001-30000
```

### Blockchain Network Expansion
```
Current: Centralized simulation
Phase 2: Private blockchain (Hyperledger Fabric)
Phase 3: Consortium blockchain (Multi-hospital network)
Phase 4: Public blockchain (Ethereum/Polygon)
```

---

## 11. TESTING STRATEGY

### Unit Tests
- BlockchainService hash generation
- SmartContractService logic
- FraudDetector scoring algorithm

### Integration Tests
- Controller → Service → Database flow
- Transaction rollback scenarios

### System Tests
- End-to-end patient registration
- Bill creation with blockchain entry
- Insurance claim verification

### Performance Tests
- Load testing (100 concurrent users)
- Stress testing (Database under heavy load)
- Hash generation benchmarks

---

## 12. MONITORING & LOGGING

### Application Logs
```csharp
// Example logging points
Logger.Info("Patient registered: {PatientID}");
Logger.Info("Bill created: {BillID}, Hash: {Hash}");
Logger.Warning("Fraud detected: {BillID}, Risk: {Score}");
Logger.Error("Database connection failed: {Exception}");
```

### Metrics to Track
- Average bill creation time
- Hash generation performance
- Database query execution time
- Fraud detection accuracy rate
- System uptime

---

**This architecture is designed for academic demonstration while being production-ready with minimal modifications.**
