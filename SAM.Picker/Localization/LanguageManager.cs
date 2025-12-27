using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SAM.Picker.Localization
{
    public class LanguageManager
    {
        private static LanguageManager _instance;
        private Dictionary<string, string> _currentLanguage;
        private string _currentLanguageCode;
        
        public static LanguageManager Instance => _instance ?? (_instance = new LanguageManager());
        
        public event EventHandler LanguageChanged;
        
        private LanguageManager()
        {
            string savedLang = LoadLanguagePreference();
            LoadLanguage(savedLang);
        }
        
        public void LoadLanguage(string languageCode)
        {
            try
            {
                // Try to load from Languages folder in app directory
                string langPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", $"{languageCode}.json");
                
                // Fallback to AppData if not found
                if (!File.Exists(langPath))
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    langPath = Path.Combine(appDataPath, "SAM", "Languages", $"{languageCode}.json");
                }
                
                if (File.Exists(langPath))
                {
                    string json = File.ReadAllText(langPath, System.Text.Encoding.UTF8);
                    _currentLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    _currentLanguageCode = languageCode;
                    SaveLanguagePreference(languageCode);
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Fallback to English if file doesn't exist
                    if (languageCode != "en")
                    {
                        LoadLanguage("en");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load language {languageCode}: {ex.Message}");
                // Fallback to English if loading fails
                if (languageCode != "en")
                {
                    LoadLanguage("en");
                }
            }
        }
        
        public string GetString(string key, params object[] args)
        {
            if (_currentLanguage != null && _currentLanguage.ContainsKey(key))
            {
                string value = _currentLanguage[key];
                if (args != null && args.Length > 0)
                {
                    try
                    {
                        return string.Format(value, args);
                    }
                    catch
                    {
                        return value;
                    }
                }
                return value;
            }
            return key; // Return key if translation not found
        }
        
        public string CurrentLanguageCode => _currentLanguageCode ?? "en";
        
        public List<LanguageInfo> GetAvailableLanguages()
        {
            var languages = new List<LanguageInfo>
            {
                new LanguageInfo { Code = "en", Name = "English", NativeName = "English" },
                new LanguageInfo { Code = "ar", Name = "Arabic", NativeName = "العربية" },
                new LanguageInfo { Code = "zh-CN", Name = "Chinese Simplified", NativeName = "简体中文" },
                new LanguageInfo { Code = "zh-TW", Name = "Chinese Traditional", NativeName = "繁體中文" },
                new LanguageInfo { Code = "ja", Name = "Japanese", NativeName = "日本語" },
                new LanguageInfo { Code = "ko", Name = "Korean", NativeName = "한국어" },
                new LanguageInfo { Code = "es", Name = "Spanish", NativeName = "Español" },
                new LanguageInfo { Code = "fr", Name = "French", NativeName = "Français" },
                new LanguageInfo { Code = "de", Name = "German", NativeName = "Deutsch" }
            };
            
            return languages;
        }
        
        private void SaveLanguagePreference(string languageCode)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string prefPath = Path.Combine(appDataPath, "SAM", "language.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(prefPath));
                File.WriteAllText(prefPath, languageCode, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save language preference: {ex.Message}");
            }
        }
        
        public string LoadLanguagePreference()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string prefPath = Path.Combine(appDataPath, "SAM", "language.txt");
                if (File.Exists(prefPath))
                {
                    return File.ReadAllText(prefPath, System.Text.Encoding.UTF8).Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load language preference: {ex.Message}");
            }
            
            // Auto-detect system language
            string systemLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string systemRegion = CultureInfo.CurrentCulture.Name;
            
            // Map common system languages to our language codes
            var availableLanguages = GetAvailableLanguages();
            var match = availableLanguages.FirstOrDefault(l => l.Code == systemRegion) 
                     ?? availableLanguages.FirstOrDefault(l => l.Code.StartsWith(systemLang));
            
            return match?.Code ?? "en"; // Default to English
        }
    }
    
    public class LanguageInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
    }
}
