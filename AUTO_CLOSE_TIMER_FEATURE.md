# Auto-Close Countdown Timer Feature

## Overview
Automatically closes SAM.Game windows after a specified countdown time.

## How It Works

### User Flow
1. Open a game in SAM.Game
2. Click "Auto-Close Timer" button in toolbar
3. Set countdown duration (hours, minutes, seconds)
4. Click "Enable Timer" to start countdown
5. Window title updates every second with remaining time
6. Window automatically closes when countdown reaches zero

### Example
- **Set countdown**: 1 hour 30 minutes (1h 30m 0s)
- **Timer starts**: Countdown begins immediately
- **Title displays**: "SAM | GameName | ⏱ 1h 29m 45s remaining"
- **At zero**: Window closes automatically with notification

## Technical Implementation

### Files Modified/Created

#### 1. **SAM.Game/AutoCloseTimerDialog.cs** (NEW)
Dialog for setting countdown duration:
- NumericUpDown controls for hours/minutes/seconds
- Shows preview of countdown duration
- Color-coded display (green=valid, red=invalid)
- Validation: countdown must be greater than zero
- Simple and straightforward interface

#### 2. **SAM.Game/Manager.cs**
Added countdown timer monitoring:
- `_AutoCloseEnabled`: Flag to enable/disable countdown
- `_CountdownSeconds`: Total seconds to count down
- `_CountdownStartTime`: When countdown was started
- `CheckAutoCloseTimer()`: Called every timer tick
  - Calculates elapsed time since start
  - Updates window title with remaining time
  - Closes window when countdown reaches zero
- `OnAutoCloseTimer()`: Button click handler opens dialog
- `FormatSeconds()`: Formats seconds as "Xh Ym Zs"

#### 3. **SAM.Game/Manager.Designer.cs**
Added toolbar button:
- `_AutoCloseTimerButton`: ToolStripButton
- Text: "Auto-Close Timer"
- ToolTip: "Set a target playtime to automatically close this game"
- Click event wired to `OnAutoCloseTimer`

#### 4. **SAM.API/PlaytimeReader.cs**
Shared playtime reader (used for playtime display feature, NOT for timer):
- `GetPlaytime(uint appId)`: Returns minutes played from Steam
- Parses Steam's `localconfig.vdf` file
- Caches data for performance
- Used by SAM.Picker for display purposes

## How Countdown Works

### Formula
```
Elapsed Seconds = (DateTime.Now - StartTime).TotalSeconds
Remaining Seconds = Countdown Duration - Elapsed Seconds
```

### Example Calculation
1. User sets countdown at 12:00 PM
   - Countdown set: 5400 seconds (1h 30m 0s)
   - Start time: 12:00:00 PM

2. At 12:30:00 PM (30 minutes elapsed)
   - Elapsed: 1800 seconds
   - Remaining: 5400 - 1800 = 3600 seconds
   - Display: "⏱ 1h 0m 0s remaining"

3. At 1:30:00 PM (90 minutes elapsed)
   - Elapsed: 5400 seconds
   - Remaining: 5400 - 5400 = 0 seconds
   - Action: Window closes automatically

## Key Features

### Live Countdown
- Window title updates every second
- Shows remaining time in "Xh Ym Zs" format
- Pure countdown timer - no external dependencies
- Accurate time tracking using DateTime

### Simple & Reliable
- No Steam sync issues
- Works independently of game state
- No network or file dependencies
- Countdown starts immediately when enabled

### User Control
- Easy to enable via toolbar button
- Clear visual feedback (countdown in title)
- Can start new countdown by clicking button again
- Each window has independent countdown timer

## Use Cases

### 1. Timed Sessions
**Scenario**: Want to manage achievements for exactly 1 hour
- Set countdown: 1 hour
- Work on achievements
- Window closes automatically after 1 hour

### 2. Batch Processing
**Scenario**: Processing multiple games with specific time limits
- Open multiple SAM.Game windows
- Set different countdown for each (e.g., 30min, 1h, 2h)
- Each window closes when its countdown finishes

### 3. Reminder System
**Scenario**: Don't want to spend too long on one game
- Set countdown: 30 minutes
- Get notification when time is up
- Prevents spending excessive time

## Future Enhancements (Not Implemented)

Possible improvements:
- Sound notification before closing
- Configurable warning time (e.g., "Closing in 5 minutes")
- Option to minimize instead of close
- Global timer settings (apply same target to all windows)
- Log file of auto-close events

## Testing Checklist

- [ ] Button appears in toolbar
- [ ] Dialog opens when clicked
- [ ] Current playtime displays correctly
- [ ] Can set target hours/minutes
- [ ] Validation prevents invalid targets
- [ ] Window title updates with countdown
- [ ] Window closes at target time
- [ ] Multiple windows can have different targets
- [ ] Timer persists across Steam restarts
- [ ] Works with games at 0 playtime

## Related Features

- **Playtime Display** (SAM.Picker): Shows total playtime for each game in library
- **Launch Queue** (SAM.Picker): Opens multiple games automatically
- **Combined Use**: Launch queue + auto-close timer = automated playtime farming

## Build Status
✅ Compiles successfully with no errors (1 unused variable warning)
