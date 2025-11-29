# 🚀 HMS Blockchain Project - Deployment Guide

## For New Laptop Setup (Windows)

### 📥 STEP 1: Install Prerequisites

#### A. Install .NET 6.0 SDK
1. Visit: https://dotnet.microsoft.com/download/dotnet/6.0
2. Click "Download .NET 6.0 SDK (v6.0.x)" for Windows
3. Run the installer
4. Verify installation:
   ```powershell
   dotnet --version
   ```
   Should show: `6.0.x`

#### B. Install SQL Server Express (Free)
1. Visit: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Download "SQL Server 2022 Express"
3. Run installer, choose "Basic" installation
4. Note the connection string shown at the end (usually `.\SQLEXPRESS`)

#### C. Install SQL Server Management Studio (SSMS) - Optional
1. Visit: https://aka.ms/ssmsfullsetup
2. Download and install SSMS
3. Use this to manage your database visually

---

### 📂 STEP 2: Copy Project Files

**Copy these folders/files to new laptop:**
- ✅ `BlockchainPatientData/` (ENTIRE folder including all subfolders)
- ✅ `Database/setup.sql`
- ✅ `README.md`
- ✅ All documentation files

**DO NOT copy (will be auto-generated):**
- ❌ `BlockchainPatientData/bin/`
- ❌ `BlockchainPatientData/obj/`
- ❌ `.vs/` folder

**Recommended:** Copy the entire `HMS_Blockchain_Project` folder to:
```
C:\Users\YourName\Desktop\HMS_Blockchain_Project
```

---

### 🗄️ STEP 3: Setup Database

#### Option A: Using SQL Server Management Studio (SSMS)
1. Open SSMS
2. Connect to your SQL Server:
   - Server name: `.\SQLEXPRESS` or `localhost`
   - Authentication: Windows Authentication
3. Click "New Query"
4. Open file: `Database/setup.sql`
5. Copy all content and paste into query window
6. Click "Execute" or press F5
7. Verify database created: You should see `HospitalDB` in Object Explorer

#### Option B: Using Command Line
```powershell
# Connect to SQL Server and run setup script
sqlcmd -S .\SQLEXPRESS -i "C:\Path\To\HMS_Blockchain_Project\Database\setup.sql"
```

---

### ⚙️ STEP 4: Configure Connection String

1. Navigate to: `BlockchainPatientData/appsettings.json`
2. Open the file in any text editor
3. Update the connection string:

**For SQL Server Express (most common):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=HospitalDB;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**For SQL Server (localhost):**
```json
"DefaultConnection": "Server=localhost;Database=HospitalDB;Integrated Security=True;TrustServerCertificate=True;"
```

**For SQL Server with username/password:**
```json
"DefaultConnection": "Server=localhost;Database=HospitalDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

4. Save the file

---

### 🔧 STEP 5: Build & Run Project

1. **Open PowerShell or Command Prompt**
2. **Navigate to project folder:**
   ```powershell
   cd "C:\Users\YourName\Desktop\HMS_Blockchain_Project\BlockchainPatientData"
   ```

3. **Restore NuGet packages:**
   ```powershell
   dotnet restore
   ```
   Wait for all packages to download...

4. **Build the project:**
   ```powershell
   dotnet build
   ```
   Should show: "Build succeeded"

5. **Run the application:**
   ```powershell
   dotnet run
   ```

6. **Look for output:**
   ```
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: https://localhost:7001
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: http://localhost:5001
   ```

7. **Open browser and navigate to:**
   - `https://localhost:7001` (or whatever port shown)
   - Or: `http://localhost:5001`

---

### ✅ STEP 6: Test the Application

1. **Test Hospital Dashboard:**
   - Go to: `https://localhost:7001/Hospital/Dashboard`
   - You should see professional blue/white interface

2. **Add Test Patient:**
   - Click "Add New Patient"
   - Fill in test data:
     - Name: John Doe
     - Age: 30
     - Gender: Male
     - Phone: 1234567890
     - Email: john@test.com
     - Blood Group: O+
   - Submit

3. **Add Test Bill:**
   - Click "Add New Bill"
   - Select the patient you just created
   - Description: "General Checkup"
   - Amount: 1000
   - Submit
   - Note the blockchain hash generated!

4. **Verify Blockchain:**
   - Click "Verify Blockchain" button on Hospital Dashboard
   - Should show:
     - Block #1 (Genesis block with 0000...0000)
     - Block #2 (Your test bill with proper chain linkage)
     - ✅ Full chain valid status

5. **Test Patient Portal:**
   - Navigate to: `/Patient/Dashboard/{patientId}` (use the ID from your test patient)
   - All views should have clean white backgrounds with blue headers
   - No colorful gradients anywhere!

---

### 🐛 Troubleshooting

#### Problem: "dotnet command not found"
**Solution:**
- Restart PowerShell/Command Prompt after installing .NET SDK
- Or install .NET 6.0 SDK from: https://dotnet.microsoft.com/download/dotnet/6.0

#### Problem: "Unable to connect to database"
**Solution:**
1. Check SQL Server is running:
   - Open "Services" (Windows + R, type `services.msc`)
   - Look for "SQL Server (SQLEXPRESS)"
   - Make sure it's "Running"
2. Verify connection string in `appsettings.json`
3. Try connecting with SSMS first to verify server name

#### Problem: "Database 'HospitalDB' does not exist"
**Solution:**
- Run the `Database/setup.sql` script again
- Or manually create database:
  ```sql
  CREATE DATABASE HospitalDB;
  GO
  USE HospitalDB;
  -- Then run rest of setup.sql
  ```

#### Problem: "Port 5001 or 7001 already in use"
**Solution:**
1. Stop other applications using those ports
2. Or change ports in `Properties/launchSettings.json`

#### Problem: "Build failed with errors"
**Solution:**
- Run `dotnet clean` first
- Then `dotnet restore`
- Then `dotnet build` again

#### Problem: "Trust the HTTPS development certificate"
**Solution:**
```powershell
dotnet dev-certs https --trust
```
Click "Yes" when prompted

---

### 📦 Project Features Confirmed Working

✅ **TRUE Blockchain Implementation:**
- Each block contains previous block's hash
- Genesis block starts with 0000...0000
- Tampering detection works (break chain by modifying data)
- Full chain validation with cascade effect

✅ **Professional UI Design:**
- All Patient Portal views: White backgrounds (#F5F6FA)
- Consistent blue headers (#0078D7 gradient)
- No colorful gradients anywhere
- Excellent contrast and readability
- Responsive design (mobile-friendly)

✅ **Currency Display:**
- All amounts use $ (Dollar) symbol
- Consistent formatting: $X,XXX.XX

✅ **Features:**
- Hospital Dashboard: Add patients, add bills, view all data
- Patient Dashboard: View bills, claims, NFT health ID, medical records
- Insurance Dashboard: View claims, approve/deny, fraud detection
- Blockchain Verification: Visual chain display with hash links

---

### 🎯 Quick Start Commands

```powershell
# One-time setup
cd "C:\Path\To\HMS_Blockchain_Project\BlockchainPatientData"
dotnet restore
dotnet build

# Run application (every time)
dotnet run

# Stop application
Ctrl + C
```

---

### 📞 Need Help?

If you encounter issues:
1. Check the error message in terminal/PowerShell
2. Verify SQL Server is running
3. Verify connection string is correct
4. Make sure .NET 6.0 SDK is installed (`dotnet --version`)
5. Try running `dotnet clean` and rebuilding

---

### 🎉 Success Indicators

You'll know everything is working when:
- ✅ Application starts without errors
- ✅ Browser opens to professional white/blue interface
- ✅ You can add patients and bills
- ✅ Blockchain hashes are generated
- ✅ Chain verification shows proper linkage
- ✅ All Patient Portal views have white backgrounds

**Project is ready for demonstration or Viva presentation!** 🚀
