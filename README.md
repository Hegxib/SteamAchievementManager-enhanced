# HxB SAM Enhanced V1.3

<img width="453" height="210" alt="Hegxib transp" src="https://github.com/user-attachments/assets/07fe9bfe-f99b-4447-8f10-616c9e996fb0" />

**The Most Advanced Steam Achievement Manager** - A comprehensive enhancement of the original SAM with intelligent features, automation, multi-language support, and professional-grade game management tools.

---

## 🎯 What is HxB SAM Enhanced?

A powerful fork of Gibbed's Steam Achievement Manager with **25+ exclusive features** not available in the original. Built for serious gamers who want complete control over their Steam achievements with intelligent automation, visual organization, multi-language support, and realistic timing systems.

**Requirements:**
- Windows 7 or higher
- .NET Framework 4.8
- Steam client installed and running

---

## ⚡ Quick Comparison: Original vs Enhanced

| Feature | Gibbed's Original SAM | HxB SAM Enhanced V1.3 |
|---------|----------------------|------------------------|
| **Multi-Language** | English only | 9 languages (EN, AR, DE, ES, FR, JA, KO, ZH-CN, ZH-TW) |
| **Achievement Display** | Alphabetical list | Steam global % order + rarity indicators |
| **Unlock Timing** | Manual only | Smart auto-distribution + auto-unlock system |
| **Game Organization** | Single flat list | 3 sections: SELECTED / DONE / OTHER |
| **Image Caching** | Basic cache | Multi-tier cache + auto-download assets |
| **Keyboard Shortcuts** | None | Alt+Click, Ctrl+Click quick actions |
| **Bulk Operations** | None | Bulk reset, bulk timing, select all |
| **Section Management** | N/A | Persistent sections with dedicated caches |
| **Context Menus** | Basic | Dynamic context-aware menus |
| **Auto-Close Timer** | None | Persistent countdown with auto-unlock sync |
| **Playtime Display** | None | Integrated playtime reader |
| **Asset Management** | None | Auto-downloads headers, capsules, library images |
| **Help System** | None | Built-in cheat sheet with all shortcuts |
| **Launch Features** | Launch game only | Launch random from selected + launch queue |
| **Visual Feedback** | Basic | Color-coded sections, emojis, progress indicators |

---

## 🚀 Exclusive V1.3 Features

### 🌍 **Multi-Language Support** *(New in V1.3)*
- **9 Languages**: Full localization for global accessibility
  - 🇺🇸 English (Default)
  - 🇸🇦 Arabic (العربية)
  - 🇩🇪 German (Deutsch)
  - 🇪🇸 Spanish (Español)
  - 🇫🇷 French (Français)
  - 🇯🇵 Japanese (日本語)
  - 🇰🇷 Korean (한국어)
  - 🇨🇳 Chinese Simplified (简体中文)
  - 🇹🇼 Chinese Traditional (繁體中文)
- **One-Click Switching**: Language dropdown in toolbar
- **Persistent Preference**: Language choice saved between sessions
- **Complete Coverage**: All UI elements, menus, dialogs, and tooltips translated
- **Real-Time Updates**: Interface changes instantly without restart

### 📋 **Cheat Sheet System** *(V1.2)*
- **Built-in Help**: Toolbar button with comprehensive shortcuts reference
- **Quick Access**: All keyboard shortcuts, tips, and cache locations in one place
- **Professional Layout**: ASCII-formatted sections for easy reading
- Never forget a shortcut again!

### ⌨️ **Keyboard Shortcuts** *(V1.2)*
- **Alt + Left Click**: Instantly move game to DONE section
- **Ctrl + Left Click**: Toggle game in/out of SELECTED section
- **Lightning Fast**: Manage hundreds of games without using menus
- **Intuitive**: Natural modifier key combinations

### 🗑️ **Clear DONE Section** *(V1.2)*
- **Toolbar Button**: One-click to clear all DONE games
- **Context Menu**: Right-click option for quick access
- **Safety Warning**: Confirmation dialog prevents accidents
- **Bulk Management**: Clear completed games in seconds

### 💾 **Enhanced Multi-Tier Caching** *(V1.2)*
- **Main Cache**: `%AppData%\SAM\ImageCache\` - Global game logo cache
- **SELECTED Cache**: `%AppData%\SAM\SelectedCache\` - Section-specific cache
- **DONE Cache**: `%AppData%\SAM\DoneCache\` - Completion section cache
- **Auto-Download**: Images download immediately when adding to sections
- **Asset Collection**: Auto-downloads headers, capsules, library images, logos
- **Background Processing**: Downloads don't block UI operations

### 🔄 **Bulk Reset** *(V1.2)*
- **Multi-Game Reset**: Reset achievements for multiple games at once
- **Toolbar Access**: Quick access from main interface
- **Time Saver**: Process dozens of games in one operation

### 🎮 **Three-Section Organization** *(V1.0)*
- **SELECTED**: Your active game collection
  - Persistent between sessions
  - Quick access toolbar position
  - Dedicated cache for fast loading
  
- **DONE**: Completed games (100% achievements)
  - Visual separation with light green theme
  - Auto-mark on game close
  - Manual mark/unmark via context menu
  - Never lose track of finished games
  
- **OTHER GAMES**: Your full Steam library
  - Searchable and filterable
  - Shows playtime for each game
  - Easy selection to SELECTED section

### 🎯 **Dynamic Context Menus** *(V1.2)*
- **Context-Aware**: Menu text changes based on section
  - "Mark as Done" when not in DONE
  - "Unmark as Done" when in DONE
- **Intelligent Actions**: Right-click adapts to game location
- **Professional UX**: Menus make sense in context

### 🏆 **Steam Global Statistics Integration**
- **Real Data**: Achievements sorted by actual Steam completion rates
- **% of Players Column**: See exact percentage who unlocked each achievement
- **Rarity Indicators**: Visual feedback for ultra-rare achievements
- **Realistic Display**: Order matches Steam's native interface
- **Live Sync**: Data fetched directly from Steam API

### ⏱️ **Smart Auto-Close Timer with Auto-Unlock**
- **Countdown Timer**: Set custom hours, minutes, seconds
- **Auto-Close**: Window closes silently when timer reaches zero
- **Persistent State**: Timers save and resume between sessions
- **Title Display**: See remaining time live (e.g., "⏱ 1h 30m 15s remaining")
- **Smart Integration**: Auto-unlock uses remaining timer for distribution
- **Background Processing**: Achievements unlock while you work

### 🎲 **Intelligent Achievement Distribution**
- **Realistic Timing**: Based on global achievement rarity
  - **85%+ common**: ~15 minutes (easy unlocks)
  - **50%+ medium**: ~45 minutes (moderate difficulty)  
  - **10%+ uncommon**: ~3 hours (dedication required)
  - **1%+ rare**: ~8 hours (hard to achieve)
  - **<1% ultra-rare**: ~10+ hours (nearly impossible)
  
- **Exponential Scaling**: Time increases realistically as rarity decreases
- **Auto Mode**: Perfect distribution across remaining timer countdown
- **Manual Override**: Fixed intervals or custom times per achievement
- **Bulk Configuration**: Set timing for multiple achievements at once

### 📊 **Playtime Integration**
- **Display Format**: "⏱ Xh Ym played" directly in game list
- **Steam Data**: Reads actual playtime from Steam
- **Quick Reference**: See game investment at a glance
- **Sorting Support**: Organize by playtime

### 🎰 **Launch Features**
- **Launch Random**: Randomly select and launch from SELECTED games
- **Launch Queue**: Manage launch order for multiple games
- **Launch Options**: Configure custom launch parameters per game
- **Direct Steam**: Launches through Steam for proper tracking

### 🔧 **Advanced Bulk Operations**
- **Bulk Set Times**: Configure unlock times for multiple achievements
- **Apply to All/Selected**: Choose scope of operations
- **Smart Defaults**: System suggests realistic timing based on rarity
- **Mass Operations**: Save hours on large achievement lists

### 🎨 **Visual & UX Improvements**
- **Color-Coded Sections**: Easy visual distinction
- **Emoji Icons**: Modern toolbar with clear emoji labels
- **Progress Indicators**: Visual feedback for operations
- **Status Bar Updates**: Real-time operation status
- **Responsive UI**: Smooth performance with large libraries

---

## 🚀 Quick Start Guide

### Installation
1. Download `HxB_SAM_Enhanced_v1.3.0.zip` from [Releases](https://github.com/Hegxib/SteamAchievementManager-enhanced/releases)
2. Extract to any folder
3. Run `SAM.Picker.exe` (no installation needed)
4. Steam must be running

### Basic Usage
1. **Select Games**: 
   - Browse your library in OTHER GAMES
   - Click to add to SELECTED
   - Or use **Ctrl+Click** for quick toggle
   
2. **Manage Achievements**:
   - Double-click game or click "Manager" button
   - Check/uncheck achievements to lock/unlock
   - Click "Commit Changes" to save to Steam
   
3. **Mark as Complete**:
   - Right-click → "Mark as Done"
   - Or use **Alt+Click** for instant move
   - Game appears in DONE section

### Smart Achievement Unlocking
1. Open game in achievement manager
2. **(Optional)** Set auto-close timer for play session
3. Click **"Bulk Set Times..."** button
4. **AUTO MODE** activates if timer is set
5. Click OK - achievements unlock automatically
6. Common achievements unlock first, rare ones later
7. Keep window open for background unlocking
8. Click "Commit Changes" to save

### Keyboard Shortcuts (V1.2+)
- **Alt + Left Click**: Move game to DONE
- **Ctrl + Left Click**: Toggle SELECTED section
- **Click 📋 Cheat Sheet**: View all shortcuts and tips
- **Click 🌍 Language**: Change UI language

---

## 📂 Technical Details

### Cache Locations
- **Main Cache**: `%AppData%\SAM\ImageCache\`
- **SELECTED Cache**: `%AppData%\SAM\SelectedCache\`
- **DONE Cache**: `%AppData%\SAM\DoneCache\`
- **Settings**: `%AppData%\SAM\selected_games.json`
- **Done List**: `%AppData%\SAM\done_games.json`

### Downloaded Assets
- Game logos (logo.png)
- Header images (header.jpg)
- Capsule graphics (capsule_231x87.jpg)
- Library images (library_600x900.jpg, library_hero.jpg)

### Performance
- **Async Operations**: Downloads never block UI
- **Smart Caching**: Images cached locally for instant loading
- **Background Tasks**: Auto-unlock runs in background thread
- **Memory Efficient**: Optimized for large libraries (10,000+ games)

---

## 🛠️ Building from Source

### Prerequisites
- Visual Studio 2019+ or .NET SDK 10.0+
- .NET Framework 4.8 SDK
- Windows OS

### Build Instructions

```powershell
# Clone repository
git clone https://github.com/Hegxib/SteamAchievementManager-enhanced.git
cd SteamAchievementManager-enhanced

# Build with PowerShell
.\rebuild.ps1

# Or use dotnet CLI
dotnet build --configuration Release
```

**Output**: `upload\` directory contains release executables

---

## ⚠️ Important Disclaimer

**USE AT YOUR OWN RISK**

- This tool modifies Steam achievement data
- May violate Steam's Terms of Service
- Use responsibly and ethically
- The developers assume no liability for consequences
- **Realistic timing reduces detection risk** - achievements unlock naturally over time
- Not affiliated with or endorsed by Valve Corporation or Steam

---

## 📝 Version History

### V1.3.0 (December 2025) - **Current Release**
- ✅ **Multi-Language Support**: 9 languages (EN, AR, DE, ES, FR, JA, KO, ZH-CN, ZH-TW)
- ✅ Language selector dropdown in toolbar
- ✅ Persistent language preference
- ✅ Complete UI translation coverage
- ✅ Bug fixes for game launching
- ✅ Improved error handling

### V1.2.0 (December 2025)
- ✅ Cheat Sheet button with comprehensive shortcuts
- ✅ Keyboard shortcuts (Alt+Click, Ctrl+Click)
- ✅ Clear DONE section feature
- ✅ Enhanced multi-tier caching with auto-download
- ✅ Bulk reset functionality
- ✅ Dynamic context menus
- ✅ Fixed Ctrl+Click toggle behavior
- ✅ Window title updated to V1.2

### V1.0.0 (Initial Enhanced Release)
- Three-section organization (SELECTED/DONE/OTHER)
- Steam global statistics integration
- Smart auto-close timer
- Intelligent achievement distribution
- Playtime display
- Launch random feature
- Bulk operations
- Persistent selections

---

## 💖 Support & Connect

### Support Development
If you find HxB SAM Enhanced valuable:

**☕ Donate**: [ko-fi.com/hegxib](https://ko-fi.com/hegxib)

### Stay Connected
**🌐 All Socials**: [x.hegxib.me](https://x.hegxib.me)  
**🏠 Website**: [hegxib.me](https://hegxib.me)

Follow for updates, report bugs, request features!

---

## 🏆 Credits

**Original SAM** - Rick (gibbed)
- Created: 2008
- Open-sourced: 2013
- [Original Repository](https://github.com/gibbed/SteamAchievementManager)

**HxB SAM Enhanced** - HEGXIB
- Version 1.3.0
- Enhanced features and modern improvements
- [Enhanced Repository](https://github.com/Hegxib/SteamAchievementManager-enhanced)

**Icons** - [Fugue Icons](https://p.yusukekamiyamane.com/) by Yusuke Kamiyamane

---

## 📄 License

Dual License:
- Original SAM components: zlib License
- Enhanced features: Copyright HEGXIB 2025

See [LICENSE.txt](LICENSE.txt) for complete details.

---

## 🔗 Links

- **Enhanced Version**: [github.com/Hegxib/SteamAchievementManager-enhanced](https://github.com/Hegxib/SteamAchievementManager-enhanced)
- **Original SAM**: [github.com/gibbed/SteamAchievementManager](https://github.com/gibbed/SteamAchievementManager)
- **Support**: [ko-fi.com/hegxib](https://ko-fi.com/hegxib)
- **All Socials**: [x.hegxib.me](https://x.hegxib.me)

---

<div align="center">

## 🎯 Why Choose HxB SAM Enhanced?

| Reason | Benefit |
|--------|---------|
| **25+ Exclusive Features** | Features not available anywhere else |
| **Intelligent Automation** | Save hours with smart bulk operations |
| **Professional Organization** | Three-section system keeps games organized |
| **Realistic Timing** | Achievements unlock naturally over time |
| **Active Development** | Regular updates with new features |
| **Free & Open Source** | No cost, fully transparent code |
| **Multi-Language** | Use in your native language |
| **Steam Integration** | Uses real Steam global statistics |

---

**HxB SAM Enhanced V1.3** - Professional Achievement Management

*Use Responsibly & Ethically*

---

Made by **HEGXIB** | 2025  
[hegxib.me](https://hegxib.me) | [x.hegxib.me](https://x.hegxib.me)

</div>
