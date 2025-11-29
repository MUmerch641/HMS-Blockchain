# 🚀 INSTALLATION GUIDE - DO THIS FIRST!
## HMS Blockchain Project Setup

**Current Status**: ✅ All code files created  
**Next Step**: Install required software

---

## 📥 DOWNLOAD LINKS (Open these in your browser)

### 1. .NET 6.0 SDK (REQUIRED - 5 minutes)
**Download**: https://dotnet.microsoft.com/download/dotnet/6.0
- Click "Download .NET SDK x64" for Windows
- Size: ~200 MB
- Installation: Run the .exe file, click Install

**Verify installation**:
- Open new PowerShell window
- Type: `dotnet --version`
- Should show: 6.0.xxx

---
   
### 2. SQL Server Express (REQUIRED - 15 minutes)
**Download**: https://www.microsoft.com/sql-server/sql-server-downloads
- Scroll to "Express" edition
- Click "Download now"
- Size: ~250 MB
- Installation: Choose "Basic" installation type

**Server Name will be**: `localhost\SQLEXPRESS` or `(localdb)\MSSQLLocalDB`

---

### 3. SQL Server Management Studio - SSMS (REQUIRED - 10 minutes)
**Download**: https://aka.ms/ssmsfullsetup
- Direct download link
- Size: ~600 MB
- Installation: Run installer, follow prompts

**Purpose**: Used to run the database setup script

---

### 4. Visual Studio 2022 Community (RECOMMENDED - 30 minutes)
**Download**: https://visualstudio.microsoft.com/downloads/
- Choose "Community" edition (FREE)
- Size: ~3-4 GB (with workloads)

**During Installation - Select These Workloads**:
- ✅ ASP.NET and web development
- ✅ Data storage and processing
- ✅ .NET desktop development (optional)

**Alternative**: You can use VS Code, but Visual Studio is easier for beginners

---

## 📋 INSTALLATION CHECKLIST

**Phase 1: Download Everything First** (30 minutes)
- [ ] .NET 6.0 SDK installer downloaded
- [ ] SQL Server Express installer downloaded
- [ ] SSMS installer downloaded
- [ ] Visual Studio 2022 installer downloaded

**Phase 2: Install in This Order** (60 minutes)
- [ ] Install .NET 6.0 SDK → Restart PowerShell
- [ ] Install SQL Server Express → Note the server name
- [ ] Install SSMS
- [ ] Install Visual Studio 2022 → Select ASP.NET workload

**Phase 3: Restart** (Required!)
- [ ] Restart your computer after all installations

---

## ✅ VERIFICATION STEPS

After installation and restart:

### Test 1: Check .NET SDK
```powershell
dotnet --version
# Should show: 6.0.xxx
```

### Test 2: Check SQL Server
```powershell
Get-Service | Where-Object {$_.Name -like "*SQL*"}
# Should show running SQL Server services
```

### Test 3: Open Visual Studio
- Start menu → Visual Studio 2022
- Should open without errors

### Test 4: Open SSMS
- Start menu → SQL Server Management Studio
- Connect to: localhost\SQLEXPRESS
- Should connect successfully

---

## 🎯 AFTER INSTALLATION IS COMPLETE

**THEN you can proceed to**:

### Step A: Setup Database (5 minutes)
1. Open SQL Server Management Studio
2. Connect to: `localhost\SQLEXPRESS`
3. File → Open → File
4. Select: `HMS_Blockchain_Project\Database\setup.sql`
5. Press F5 to execute
6. Verify: Database "PatientDataDB" appears in Object Explorer

### Step B: Open Project in Visual Studio (2 minutes)
1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Navigate to: `HMS_Blockchain_Project\BlockchainPatientData\`
4. Open: `BlockchainPatientData.csproj`
5. Wait for NuGet packages to restore (status bar shows progress)

### Step C: Update Connection String (1 minute)
1. In Visual Studio, open `appsettings.json`
2. Find the connection string
3. If using SQL Express, it should be:
   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=PatientDataDB;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
4. Save the file (Ctrl+S)

### Step D: Build and Run (2 minutes)
1. Press Ctrl+Shift+B to build
2. Check Output window for "Build succeeded"
3. Press F5 to run
4. Browser opens with your application!

---

## 🐛 COMMON ISSUES & SOLUTIONS

### Issue: ".NET SDK not found" after installation
**Solution**: 
- Restart your computer
- Open new PowerShell window
- Try `dotnet --version` again

### Issue: "Cannot connect to SQL Server"
**Solution**:
- Open "SQL Server Configuration Manager"
- Enable TCP/IP protocol
- Restart SQL Server service
- Or use connection string: `Server=(localdb)\\MSSQLLocalDB;...`

### Issue: "Build failed - Package not found"
**Solution**:
- Right-click project in Visual Studio
- Click "Restore NuGet Packages"
- Wait for restoration to complete
- Build again

### Issue: Visual Studio shows errors in code
**Solution**:
- Tools → Options → Text Editor → C# → IntelliSense
- Restart Visual Studio
- Let it index all files

---

## ⏱️ ESTIMATED TIME

| Task | Time |
|------|------|
| Download all installers | 30 min |
| Install .NET SDK | 5 min |
| Install SQL Server | 15 min |
| Install SSMS | 10 min |
| Install Visual Studio | 30 min |
| Restart computer | 5 min |
| Setup database | 5 min |
| Configure & test project | 10 min |
| **TOTAL** | **~2 hours** |

---

## 💡 TIPS

1. **Download everything first** before starting installations
2. **Use stable internet** - Visual Studio is large (3-4 GB)
3. **Don't skip the restart** - Required for PATH updates
4. **Keep server names handy** - Note down SQL Server name during installation
5. **One thing at a time** - Don't rush, follow steps carefully

---

## 📞 NEED HELP?

**If stuck during installation**:
1. Google the specific error message
2. Check if antivirus is blocking installation
3. Make sure you have admin rights on your PC
4. Try running installers as Administrator (right-click → Run as administrator)

**If stuck during project setup**:
1. Check `QUICKSTART.md` in project folder
2. Read `Documentation\VivaQA.md` for concept explanations
3. Verify all services are running

---

## ✨ YOU'RE ALMOST THERE!

**Current Progress**: 
- ✅ All code files created (29 files)
- ✅ Complete documentation ready
- ⏳ **Next**: Install required software (this guide)
- ⏳ **Then**: Run the project and demo it!

---

## 🎯 YOUR TIMELINE

**Today**: Install all software (2 hours)  
**Tonight/Tomorrow**: Setup database and run project (30 minutes)  
**Before Viva**: Practice demo and read documentation (2-3 hours)  

You can complete everything and have a working demo **within 24 hours**!

---

**Start with .NET SDK installation first!**

Download link again: https://dotnet.microsoft.com/download/dotnet/6.0

---

_This guide is in your project folder: INSTALLATION_GUIDE.md_
