# HxB SAM Enhanced 1.0

<img width="453" height="210" alt="Hegxib transp" src="https://github.com/user-attachments/assets/07fe9bfe-f99b-4447-8f10-616c9e996fb0" />

**Enhanced Steam Achievement Manager** - A powerful, feature-rich fork of the original Steam Achievement Manager with modern improvements and quality-of-life enhancements.

## 🎮 About

Steam Achievement Manager (SAM) is a lightweight, portable application used to manage achievements and statistics in Steam games. This enhanced version builds upon the original SAM with significant new features while maintaining its simplicity and ease of use.

**Requirements:**
- [Steam client](https://store.steampowered.com/about/) must be installed and running
- Active Steam account with network access
- Windows OS with .NET Framework 4.8

---

## ✨ Enhanced Features

This enhanced version includes all original SAM functionality plus:

### 🎯 Game Management
- **Select All / Clear All** - Quickly manage your game selections with toolbar buttons
- **Persistent Selection** - Your selected games are automatically saved and restored between sessions
- **Playtime Display** - View total playtime for each game directly in the picker (shown as "⏱ Xh Ym played")
- **Enhanced Context Menu** - Right-click games for quick actions:
  - Add/Remove from SELECTED list
  - Launch this game only
  - Launch one random game from SELECTED
  - Clear all selections

### ⏱️ Auto-Close Timer
- **Countdown Timer** - Set a custom countdown (hours, minutes, seconds) to automatically close the game window
- **Silent Auto-Close** - Windows close automatically without confirmation when timer reaches zero
- **Timer Persistence** - Timers save their state when you close a game and resume when you reopen it
- **Visual Countdown** - See remaining time in the window title (e.g., "⏱ 1h 30m 15s remaining")

### 🚀 Launch Features
- **Launch Queue** - Launch multiple selected games with a progress dialog
- **Launch Random** - Randomly pick and launch one game from your SELECTED list
- **Launch This Only** - Quick-launch individual games from the context menu

---

## ⚠️ Disclaimer

**USE AT YOUR OWN RISK**

This tool modifies Steam achievement data. Please be aware:
- Using this tool may violate Steam's Terms of Service
- The developers are not responsible for any consequences of use
- Use responsibly and ethically
- Steam accounts could potentially be affected by improper use

This is a community-enhanced version of the original open-source Steam Achievement Manager.

---

## 💖 Support the Project

If you find this tool useful, consider supporting its development:

[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20on%20Ko--fi-FF5E5B?logo=ko-fi&logoColor=white)](https://ko-fi.com/hegxib)

**Socials:** [x.hegxib.me](https://x.hegxib.me)

---

## 🛠️ Building from Source

### Prerequisites
- Visual Studio 2019 or later (with .NET Framework 4.8 SDK)
- .NET SDK 10.0 or later
- Windows OS

### Build Instructions

```powershell
# Clone the repository
git clone https://github.com/yourusername/SteamAchievementManager.git
cd SteamAchievementManager

# Build using PowerShell script
.\rebuild.ps1

# Or build using dotnet CLI
dotnet build SAM.sln --configuration Debug
```

The compiled executables will be in the `bin\` directory:
- `SAM.Picker.exe` - Main game picker interface
- `SAM.Game.exe` - Achievement manager for individual games

---

## 📝 Credits

**Original SAM** - Created by Rick (gibbed)
- Original release: 2008
- Open-sourced: 2013
- [Original Repository](https://github.com/gibbed/SteamAchievementManager)

**Enhanced Version** - HxB (HEGXIB)
- Version 1.0
- Enhanced features and modern improvements
- 2025

**Icons** - Most icons from the [Fugue Icons](https://p.yusukekamiyamane.com/) set

---

## 📄 License

This project inherits its license from the original Steam Achievement Manager. See [LICENSE.txt](LICENSE.txt) for details.

---

## 🔗 Links

- **Original SAM:** [github.com/gibbed/SteamAchievementManager](https://github.com/gibbed/SteamAchievementManager)
- **Support Development:** [ko-fi.com/hegxib](https://ko-fi.com/hegxib)
- **Socials:** [x.hegxib.me](https://x.hegxib.me)

---

<div align="center">

**HxB SAM Enhanced 1.0** - Making achievement management easier and more powerful

*Remember: Use responsibly and ethically*

</div>
