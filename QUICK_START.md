# Quick Start Guide - Steam Achievement Manager with New Features

## 🚀 You have TWO options:

---

## Option A: Install Visual Studio & Build (15 minutes)

### Step 1: Download Visual Studio
- Go to: https://visualstudio.microsoft.com/downloads/
- Download **Visual Studio 2022 Community** (FREE)

### Step 2: Install with Correct Workload
When the installer runs:
1. Check "**.NET desktop development**"
2. Make sure these are selected in the right panel:
   - ✅ .NET Framework 4.8 SDK
   - ✅ .NET Framework 4.8 targeting pack

### Step 3: Build the Project
Open PowerShell in this folder and run:
```powershell
.\build.ps1
```

### Step 4: Run It!
```powershell
.\bin\SAM.Picker.exe
```

**NEW FEATURES AVAILABLE:**
- ✅ Select multiple games (Ctrl+Click) and launch them all at once
- ✅ Right-click achievements to set custom unlock times
- ✅ Use "Bulk Set Times..." button for batch scheduling

---

## Option B: Use Pre-Built Version (No new features)

If you just want to use SAM without the new features:

1. Download latest release: https://github.com/gibbed/SteamAchievementManager/releases/latest
2. Extract the ZIP file
3. Run `SAM.Picker.exe`

**Note:** This won't have the new bulk-run and scheduled-time features we just added.

---

## 📝 What's Different?

The version you downloaded is SOURCE CODE, not compiled executables.

**Source code** = Needs compilation (like raw ingredients)
**Compiled .exe** = Ready to run (like a finished meal)

To get the new features working, you need to "cook" (compile) the source code.

---

## ❓ Questions?

- **"Can I build without Visual Studio?"** - Technically yes with standalone MSBuild, but VS is easier
- **"How big is Visual Studio?"** - About 4-8 GB depending on components
- **"Is it worth it?"** - If you want the new features and to modify SAM, yes!

---

Happy achievement hunting! 🎮
