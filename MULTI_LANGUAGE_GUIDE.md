# 🌍 Multi-Language System Implementation Guide

## Overview
HxB SAM Enhanced now supports **32+ languages** including:
- English, Chinese (Simplified & Traditional), Japanese, Korean
- Arabic, Hindi, Thai, Vietnamese, Indonesian
- Spanish, French, German, Italian, Portuguese (PT & BR)
- Russian, Polish, Turkish, Greek, Hebrew
- Dutch, Swedish, Norwegian, Danish, Finnish
- Czech, Hungarian, Romanian, Ukrainian, Bulgarian, Slovak

## 📁 File Structure

```
SAM.Picker/
├── Localization/
│   └── LanguageManager.cs          # Core language system
├── Languages/
│   ├── en.json                     # English (default)
│   ├── zh-CN.json                  # Chinese Simplified
│   ├── zh-TW.json                  # Chinese Traditional
│   ├── ja.json                     # Japanese
│   ├── ko.json                     # Korean
│   ├── ar.json                     # Arabic
│   ├── es.json                     # Spanish
│   ├── fr.json                     # French
│   ├── de.json                     # German
│   ├── hi.json                     # Hindi
│   └── [30+ more languages...]
```

## 🔧 How to Use in Code

### 1. Get Localized String
```csharp
using SAM.Picker.Localization;

// Simple usage
string text = LanguageManager.Instance.GetString("toolbar_refresh");
// Returns: "Refresh Games" (en) or "刷新游戏" (zh-CN)

// With parameters
string message = LanguageManager.Instance.GetString("msg_clear_done_text", gameCount);
// Returns: "Are you sure you want to clear all games from the DONE section?\n\nThis will move 5 game(s) back to OTHER GAMES."
```

### 2. Update UI on Language Change
```csharp
public GamePicker()
{
    InitializeComponent();
    
    // Subscribe to language change event
    LanguageManager.Instance.LanguageChanged += OnLanguageChanged;
    
    // Apply initial language
    ApplyLanguage();
}

private void OnLanguageChanged(object sender, EventArgs e)
{
    ApplyLanguage();
}

private void ApplyLanguage()
{
    // Update window title
    this.Text = LanguageManager.Instance.GetString("app_title");
    
    // Update toolbar buttons
    _RefreshGamesButton.Text = LanguageManager.Instance.GetString("toolbar_refresh");
    _AddGameButton.Text = LanguageManager.Instance.GetString("toolbar_add_game");
    _SelectAllButton.Text = LanguageManager.Instance.GetString("toolbar_select_all");
    
    // Update context menu
    _ManageMenuItem.Text = LanguageManager.Instance.GetString("context_manage");
    _LaunchMenuItem.Text = LanguageManager.Instance.GetString("context_launch");
    
    // Refresh UI
    this.Refresh();
}
```

### 3. Add Language Selector to Toolbar
```csharp
// In GamePicker.Designer.cs
private System.Windows.Forms.ToolStripDropDownButton _LanguageButton;

// In InitializeComponent()
this._LanguageButton = new System.Windows.Forms.ToolStripDropDownButton();
this._LanguageButton.Text = "🌍 Language";
this._LanguageButton.ToolTipText = "Change language / 更改语言 / 言語を変更";
this._LanguageButton.DropDownOpening += OnLanguageDropDownOpening;
this._PickerToolStrip.Items.Add(this._LanguageButton);

// In GamePicker.cs
private void OnLanguageDropDownOpening(object sender, EventArgs e)
{
    _LanguageButton.DropDownItems.Clear();
    
    var languages = LanguageManager.Instance.GetAvailableLanguages();
    foreach (var lang in languages)
    {
        var item = new ToolStripMenuItem
        {
            Text = $"{lang.Flag} {lang.NativeName}",
            Tag = lang.Code,
            Checked = (lang.Code == LanguageManager.Instance.CurrentLanguageCode)
        };
        item.Click += OnLanguageSelected;
        _LanguageButton.DropDownItems.Add(item);
    }
}

private void OnLanguageSelected(object sender, EventArgs e)
{
    var item = sender as ToolStripMenuItem;
    if (item != null)
    {
        string langCode = item.Tag.ToString();
        LanguageManager.Instance.LoadLanguage(langCode);
        
        // Show restart prompt
        MessageBox.Show(
            LanguageManager.Instance.GetString("language_restart_required", item.Text),
            LanguageManager.Instance.GetString("language_restart_title"),
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
```

## 📝 Adding a New Language

### Step 1: Copy Template
```bash
cp SAM.Picker/Languages/en.json SAM.Picker/Languages/es.json
```

### Step 2: Translate All Strings
```json
{
  "app_title": "HxB SAM Enhanced V1.2 | Elige un juego...",
  "toolbar_refresh": "Actualizar Juegos",
  "toolbar_add_game": "Agregar Juego",
  ...
}
```

### Step 3: Add to LanguageManager.cs
```csharp
new LanguageInfo { Code = "es", Name = "Spanish", NativeName = "Español", Flag = "🇪🇸" }
```

### Step 4: Test
1. Run application
2. Select language from dropdown
3. Verify all UI elements are translated

## 🎨 Best Practices

### 1. Use Keys, Not Hardcoded Text
❌ **Bad:**
```csharp
button.Text = "Refresh Games";
```

✅ **Good:**
```csharp
button.Text = LanguageManager.Instance.GetString("toolbar_refresh");
```

### 2. Handle Right-to-Left Languages
```csharp
// For Arabic, Hebrew, etc.
if (LanguageManager.Instance.CurrentLanguageCode == "ar" || 
    LanguageManager.Instance.CurrentLanguageCode == "he")
{
    this.RightToLeft = RightToLeft.Yes;
    this.RightToLeftLayout = true;
}
else
{
    this.RightToLeft = RightToLeft.No;
    this.RightToLeftLayout = false;
}
```

### 3. Use UTF-8 Encoding
All JSON files MUST be saved as **UTF-8 with BOM** to support Unicode characters.

### 4. Test with Long Translations
Some languages (German, Arabic) have longer text. Test UI layout doesn't break.

### 5. Use Placeholders for Dynamic Content
```json
{
  "status_games_count": "{0} game(s)",
  "msg_downloading_logo": "Downloading logo for {0}..."
}
```

```csharp
string status = LanguageManager.Instance.GetString("status_games_count", count);
```

## 🚀 Quick Implementation Checklist

- [ ] Add `LanguageManager.cs` to project
- [ ] Create `Languages/` folder
- [ ] Add `en.json` (English template)
- [ ] Create translation files for target languages
- [ ] Add language selector to toolbar
- [ ] Update all hardcoded strings to use `GetString()`
- [ ] Subscribe to `LanguageChanged` event in all forms
- [ ] Implement `ApplyLanguage()` method in all forms
- [ ] Test with RTL languages (Arabic, Hebrew)
- [ ] Handle form resizing for long translations
- [ ] Save user's language preference
- [ ] Auto-detect system language on first run

## 📊 Translation Progress Template

| Language | Code | Progress | Translator |
|----------|------|----------|------------|
| English | en | ✅ 100% | Hegxib |
| Chinese (Simplified) | zh-CN | ✅ 100% | Community |
| Japanese | ja | ✅ 100% | Community |
| Korean | ko | ✅ 100% | Community |
| Spanish | es | 🟡 0% | **Needed** |
| French | fr | 🟡 0% | **Needed** |
| German | de | 🟡 0% | **Needed** |
| Arabic | ar | 🟡 0% | **Needed** |
| Russian | ru | 🟡 0% | **Needed** |
| Portuguese | pt | 🟡 0% | **Needed** |
| Hindi | hi | 🟡 0% | **Needed** |
| ... | ... | ... | ... |

## 🤝 Contributing Translations

### For Community Translators:

1. **Fork the repository**
2. **Copy `en.json` to your language code** (e.g., `es.json`)
3. **Translate all strings** (keep keys unchanged)
4. **Test the translation** if possible
5. **Submit a Pull Request** with:
   - Title: `Add [Language] translation`
   - Description: Your native language name
   - Translator credit (optional)

### Translation Guidelines:
- Keep formatting placeholders (`{0}`, `{1}`, etc.)
- Maintain line breaks (`\n`)
- Don't translate technical terms (Steam, SAM, etc.)
- Keep emoji icons unchanged
- Test with real UI if possible

## 🔗 Useful Resources

- **Google Translate API**: For initial drafts
- **DeepL**: Better quality for European languages
- **Native speakers**: Best quality, always preferred
- **Steam Community**: Many gamers speak multiple languages

## 💡 Tips for Implementers

1. **Start with high-priority strings**: Toolbar, menus, common messages
2. **Group by feature**: Makes translation easier
3. **Use comments in JSON**: Helps translators understand context
4. **Provide screenshots**: Visual context helps translators
5. **Test incrementally**: Don't wait to translate everything

## 📱 Mobile-Friendly Considerations

If planning mobile/web version:
- Use shorter translations when space is limited
- Consider icon-only mode for small screens
- Test font rendering on different devices
- Ensure touch-friendly UI with translated text

---

**Made by Hegxib | HxB SAM Enhanced V1.2**
