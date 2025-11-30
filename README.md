# HxB SAM Enhanced 1.0

<img width="453" height="210" alt="Hegxib transp" src="https://github.com/user-attachments/assets/07fe9bfe-f99b-4447-8f10-616c9e996fb0" />

**Enhanced Steam Achievement Manager** - A powerful, feature-rich fork of the original Steam Achievement Manager with modern improvements, realistic achievement unlocking, and quality-of-life enhancements.

## 🎮 About

Steam Achievement Manager (SAM) is a lightweight, portable application used to manage achievements and statistics in Steam games. This enhanced version builds upon the original SAM with significant new features, including realistic achievement timing based on Steam global statistics, while maintaining its simplicity and ease of use.

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
- **Launch Random from Selected** - Randomly pick and launch one game from your SELECTED list

### 🏆 Achievement Intelligence
- **Steam Global Statistics** - Achievements sorted by actual Steam completion rates (most common → rarest)
- **% of Players Column** - See what percentage of players have unlocked each achievement
- **Realistic Display Order** - Achievements appear in the same order as Steam's native interface
- **Live Data Sync** - Global percentages fetched directly from Steam API

### ⏱️ Smart Auto-Close Timer
- **Countdown Timer** - Set a custom countdown (hours, minutes, seconds) to automatically close the game window
- **Silent Auto-Close** - Windows close automatically without confirmation when timer reaches zero
- **Timer Persistence** - Timers save their state when you close a game and resume when you reopen it
- **Visual Countdown** - See remaining time in the window title (e.g., "⏱ 1h 30m 15s remaining")
- **Auto-Integration** - Smart distribution automatically uses remaining countdown time

### 🎲 Realistic Achievement Unlocking
- **Smart Distribution (Default)** - Achievements unlock automatically based on realistic timing:
  - **85% common achievements**: ~15 minutes (easy unlocks)
  - **50% medium achievements**: ~45 minutes (moderate difficulty)
  - **10% uncommon achievements**: ~3 hours (requires dedication)
  - **1% rare achievements**: ~8 hours (hard to get)
  - **0.1% ultra-rare achievements**: ~10+ hours (nearly impossible)
  
- **Exponential Scaling** - Time increases exponentially as rarity decreases (matches real gameplay patterns)
- **Automatic Mode** - When auto-close timer is active, achievements distribute perfectly across remaining time
- **Auto-Unlock System** - Set times and achievements unlock automatically in the background
- **Manual Override** - Option to set fixed intervals or manual times per achievement if needed

### 🔧 Bulk Operations
- **Bulk Set Times** - Configure unlock times for multiple achievements at once
- **Apply to All / Selected** - Choose to apply timing to all achievements or just your selection
- **Smart Defaults** - System automatically suggests realistic timing based on achievement rarity

---

## 🚀 Quick Start Guide

### Basic Usage
1. Launch `SAM.Picker.exe`
2. Select a game from your library
3. Double-click or click "Manager" to open achievement manager
4. Check/uncheck achievements to lock/unlock them
5. Click "Commit Changes" to save to Steam

### Smart Achievement Unlocking
1. Open a game in the achievement manager
2. **Optional:** Set an auto-close timer for your play session
3. Click **"Bulk Set Times..."** button
4. **AUTO MODE activates if timer is set** - shows remaining time automatically
5. Click OK - achievements will unlock automatically across your play session
6. Common achievements unlock first, rare ones later (realistic timing!)
7. Keep the window open - achievements unlock in the background
8. Click "Commit Changes" when done to save to Steam

### Manual Timing (Alternative)
- Uncheck "Smart distribute" for fixed intervals
- Or adjust the total duration manually if no timer is set

---

## ⚠️ Disclaimer

**USE AT YOUR OWN RISK**

This tool modifies Steam achievement data. Please be aware:
- Using this tool may violate Steam's Terms of Service
- The developers are not responsible for any consequences of use
- Use responsibly and ethically
- Steam accounts could potentially be affected by improper use
- **Realistic timing helps avoid detection** - achievements unlock naturally over time

This is a community-enhanced version of the original open-source Steam Achievement Manager.

---

## 💖 Support & Links

### Support the Development
If you find HxB SAM Enhanced useful, consider supporting its development:

**☕ Donate:** [ko-fi.com/hegxib](https://ko-fi.com/hegxib)

**🌐 Website & Socials:** [x.hegxib.me](https://x.hegxib.me)

### Stay Connected
- Follow for updates and new features
- Report bugs and request features
- Share your feedback and suggestions

---

## 🛠️ Building from Source

### Prerequisites
- Visual Studio 2019 or later (with .NET Framework 4.8 SDK)
- .NET SDK 10.0 or later
- Windows OS

### Build Instructions

```powershell
# Clone the repository
git clone https://github.com/Hegxib/SteamAchievementManager-enhanced.git
cd SteamAchievementManager-enhanced

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

Dual license - Original SAM (zlib) + Enhanced features by HEGXIB. See [LICENSE.txt](LICENSE.txt) for details.

---

## 🔗 Links

- **Original SAM:** [github.com/gibbed/SteamAchievementManager](https://github.com/gibbed/SteamAchievementManager)
- **Donate:** [ko-fi.com/hegxib](https://ko-fi.com/hegxib)
- **Website & Socials:** [x.hegxib.me](https://x.hegxib.me)

---

## 🎯 Key Improvements Over Original SAM

| Feature | Original SAM | HxB Enhanced |
|---------|-------------|--------------|
| Achievement Display | Alphabetical | **Steam Global % Order** |
| Unlock Timing | Manual only | **Automatic + Realistic** |
| Rarity Intelligence | None | **% of Players Column** |
| Bulk Operations | Individual only | **Smart Bulk Distribution** |
| Auto-Close Timer | None | **Persistent Countdown** |
| Playtime Display | None | **Integrated Display** |
| Scheduled Unlocks | None | **Background Auto-Unlock** |

---

<div align="center">

**HxB SAM Enhanced 1.0** - Making achievement management realistic, intelligent, and effortless

*Remember: Use responsibly and ethically*

---

### 💝 Support This Project

**Donate:** [ko-fi.com/hegxib](https://ko-fi.com/hegxib)  
**Website:** [x.hegxib.me](https://x.hegxib.me)

---

Made by [**HEGXIB**](https://x.hegxib.me) | Enhanced 2025  
Visit [hegxib.me](https://hegxib.me) | All Socials: [x.hegxib.me](https://x.hegxib.me)

</div>
