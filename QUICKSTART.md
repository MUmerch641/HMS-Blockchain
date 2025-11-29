# ⚡ Quick Start Guide
## Get Your HMS Blockchain Project Running in 5 Minutes!

---

## 🎯 Prerequisites Checklist

- [ ] Windows 10 or 11
- [ ] .NET 6.0 SDK ([Download](https://dotnet.microsoft.com/download))
- [ ] SQL Server 2019+ or SQL Express ([Download](https://www.microsoft.com/sql-server/sql-server-downloads))
- [ ] Visual Studio 2022 ([Download](https://visualstudio.microsoft.com/))
- [ ] SQL Server Management Studio ([Download](https://aka.ms/ssmsfullsetup))

---

## 🗄️ Step 1: Setup Database (2 minutes)

1. **Open SQL Server Management Studio (SSMS)**
2. **Connect** to `localhost` or `.\SQLEXPRESS`
3. **Open** `Database\setup.sql`
4. **Execute** (Press F5)
5. **Verify**: Database `PatientDataDB` created with 4 tables

---

## 🔧 Step 2: Configure Connection String

Edit `BlockchainPatientData\appsettings.json`:

For SQL Server:
```json
"DefaultConnection": "Server=localhost;Database=PatientDataDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

For SQL Express:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PatientDataDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

## 🖥️ Step 3: Open in Visual Studio

1. Open `BlockchainPatientData\BlockchainPatientData.csproj`
2. Wait for NuGet packages to restore
3. Press **F5** to run

---

## ✅ Step 4: Test the System

### Register a Patient
1. Go to **Hospital Portal**
2. Click **Add Patient**
3. Fill details and submit
4. ✅ You'll get an NFT Health ID

### Create a Bill
1. Go to **Hospital Portal** → **Create Bill**
2. Enter amount (try ₹15,000)
3. ✅ See blockchain hash + insurance auto-trigger

### View Claims
1. Go to **Insurance Portal**
2. ✅ See auto-triggered claims

---

## 🐛 Troubleshooting

### Database Connection Failed
```powershell
# Check SQL Server is running
services.msc
# Find "SQL Server" service, ensure it's Running
```

### Build Errors
```powershell
cd BlockchainPatientData
dotnet restore
dotnet build
```

---

## 🎯 Quick Demo Flow (5 minutes)

1. **Homepage** → Show 3 portals
2. **Hospital** → Register patient → Create bill
3. **Insurance** → View auto-triggered claims
4. **Patient** → Login → View records

---

## 📞 Need Help?

Check:
- [README.md](README.md) - Project overview
- [Documentation/VivaQA.md](Documentation/VivaQA.md) - 27 Q&A
- [Documentation/Architecture.md](Documentation/Architecture.md) - Technical details

---

**Project Location**: `C:\Users\SHARJAH LAPTOPS\OneDrive\Desktop\HMS_Blockchain_Project`

**Good luck! 🎉**
