# Steam Achievement Manager - New Features

## 🌟 MAJOR UPDATE: December 2025 - "The Stability & Integration Update" (V1.4.0)

### What's New in V1.4.0

This update focuses on stability, system integration, and user experience improvements.

**✅ NEW FEATURES:**
1. 🔄 **Live Language Sync** - Changing language in the Picker now instantly updates all open Game windows.
2. 🏁 **Windows Auto-Start** - New toolbar button to toggle SAM starting automatically with Windows.
3. ⌨️ **Enter Key Launch** - Press `Enter` in the game list to launch all selected games.
4. 🛠️ **Encoding Fix** - Switched to UTF-8 (No BOM) to fix "???" character issues in release builds.
5. 🔤 **ASCII Compatibility** - Replaced emojis with ASCII symbols for better cross-locale support.

---

## 🌟 MAJOR UPDATE: November 2025 - "The Ultimate Edition" (V1.3.0)

### What's New in V1.3.0

This massive update adds **10 game-changing features** to transform how you manage your Steam achievements! From persistent selections to launch queues, every feature is designed to save you time and give you complete control.

**✅ FULLY IMPLEMENTED:**
1. ✨ **Select All / Clear All Buttons** - Quick selection management
2. 💾 **Persistent Selection** - Selections saved between sessions  
3. 📋 **Enhanced Right-Click Menu** - Context-aware actions
4. 🚀 **Launch Queue with Progress** - Visual launch tracking
5. ⚙️ **Launch Options Dialog** - Control delays and modes
6. 🎯 **Dynamic Context Menu** - Smart menu items based on location
7. 🌍 **Multi-Language Support** - 9 languages supported (EN, AR, DE, ES, FR, JA, KO, ZH-CN, ZH-TW)
8. ⏰ **Auto-Close Timer** - Automatically close games after a set time
9. 📊 **Playtime Reader** - View your current playtime for each game
10. 🖼️ **Asset Downloader** - Automatically downloads game headers and capsules

---

## Recent Enhancements (November 2025)

### 1. **Bulk Run Games** 🎮

You can now launch multiple games simultaneously from the game picker!

**How to use:**
1. Open `SAM.Picker.exe`
2. Hold `Ctrl` or `Shift` to select multiple games
3. Double-click or press `Enter` to launch all selected games at once
4. Each game will open in its own SAM.Game window

**Benefits:**
- Save time by managing multiple games at once
- No need to go back and forth between picker and game manager
- Perfect for batch achievement management

---

### 2. **Scheduled Achievement Unlock Times** ⏰

Set specific unlock times for your achievements with precision!

**Features:**
- **Individual time setting**: Right-click any achievement and select "Set Unlock Time..."
- **Bulk time setting**: Use the "Bulk Set Times..." button to schedule multiple achievements
  - Set a start time
  - Define interval between unlocks (in minutes)
  - Apply to all or only selected achievements

**How to use individual scheduling:**
1. Open a game in `SAM.Game.exe`
2. Right-click on an achievement
3. Select "Set Unlock Time..."
4. Enable scheduling and pick your desired date/time
5. The achievement will show with a ⏰ icon and blue background
6. Click "Commit Changes" to apply

**How to use bulk scheduling:**
1. Open a game in `SAM.Game.exe`
2. (Optional) Select specific achievements with `Ctrl+Click`
3. Click the "Bulk Set Times..." button in the toolbar
4. Configure:
   - **Start Time**: When the first achievement unlocks
   - **Interval**: Minutes between each subsequent unlock
   - **Scope**: All achievements or selected only
5. Example: Start at 5:00 PM with 5-minute intervals
   - Achievement #1: 5:00 PM
   - Achievement #2: 5:05 PM  
   - Achievement #3: 5:10 PM
   - And so on...

**Visual indicators:**
- ⏰ icon in the "Unlock Time" column for scheduled achievements
- Blue background color (#202040) for scheduled items
- Clear display of scheduled time in "yyyy-MM-dd HH:mm:ss" format

**Important Notes:**
⚠️ **Steam API Limitation**: Steam's official API does not support setting custom unlock timestamps directly. When you commit changes, achievements will be unlocked immediately with Steam's current timestamp, not your scheduled time. The scheduling feature is primarily for planning and organization purposes.

---

## Updated Files

### SAM.Picker (Game Selector)
- `GamePicker.Designer.cs`: Enabled `MultiSelect` on game list
- `GamePicker.cs`: Enhanced `OnActivateGame` to handle multiple selections

### SAM.Game (Achievement Manager)
- `Manager.cs`: 
  - Added `OnSetUnlockTime()` for individual scheduling
  - Added `OnBulkSetUnlockTime()` for batch scheduling
  - Updated `StoreAchievements()` to handle scheduled times
  - Updated `GetAchievements()` to display scheduled times
- `Manager.Designer.cs`: Added context menu and toolbar button
- `AchievementInfo.cs`: Added `ScheduledUnlockTime` property
- `AchievementTimeDialog.cs/Designer.cs`: New dialog for single achievement
- `BulkUnlockTimeDialog.cs/Designer.cs`: New dialog for bulk operations

---

## Tips & Tricks

**Bulk Game Management:**
- Use `Ctrl+A` in the picker to select all filtered games
- Filter by game type (normal/demo/mod/junk) before bulk launching
- Use the search box to find specific games quickly

**Achievement Timing:**
- Plan your achievement progression realistically
- Use 1-2 minute intervals for quick unlocks
- Use 5-10+ minute intervals for realistic gameplay simulation
- Schedule around typical play session lengths

**Workflow Example:**
```
1. Select 5 games in picker → Launch all
2. In Game #1: Bulk set 10 achievements, 3-min intervals, start 6:00 PM
3. In Game #2: Bulk set 15 achievements, 5-min intervals, start 7:00 PM  
4. In Game #3: Right-click specific achievement, set exact time
5. Return to each window and commit when ready
```

---

## Technical Details

**Architecture:**
- Context menu integration with `ContextMenuStrip`
- Custom `DateTimePicker` with format "yyyy-MM-dd HH:mm:ss"
- LINQ ordering for scheduled achievement processing
- Visual feedback through `BackColor` and special characters
- Process launching via `Process.Start()` for multi-instance support

**UI Enhancements:**
- Blue color tint (RGB: 32, 32, 64) for scheduled items
- Clock emoji (⏰) prefix for scheduled times
- Warning dialogs for Steam API limitations
- Success confirmation with summary details

---

## Future Considerations

Potential enhancements for future versions:
- Export/import scheduled achievement plans
- Templates for common unlock patterns
- Integration with Steam profile analysis
- Visual timeline view of scheduled unlocks
- Undo/redo for bulk operations

---

**Enjoy the enhanced Steam Achievement Manager!** 🎉
