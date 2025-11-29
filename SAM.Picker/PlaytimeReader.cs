/* Playtime Reader - Reads playtime from Steam's local cache */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SAM.Picker
{
    internal static class PlaytimeReader
    {
        private static Dictionary<uint, int> _PlaytimeCache = null;

        public static int GetPlaytime(uint appId)
        {
            if (_PlaytimeCache == null)
            {
                LoadPlaytimeData();
            }

            return _PlaytimeCache.TryGetValue(appId, out int playtime) ? playtime : 0;
        }

        private static void LoadPlaytimeData()
        {
            _PlaytimeCache = new Dictionary<uint, int>();

            try
            {
                // Get Steam install path
                string steamPath = (string)Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\Software\Valve\Steam", 
                    "InstallPath", 
                    null);

                if (string.IsNullOrEmpty(steamPath))
                {
                    // Try 64-bit registry
                    steamPath = (string)Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Valve\Steam", 
                        "InstallPath", 
                        null);
                }

                if (string.IsNullOrEmpty(steamPath))
                {
                    return;
                }

                // Find user data folder
                string userDataPath = Path.Combine(steamPath, "userdata");
                if (!Directory.Exists(userDataPath))
                {
                    return;
                }

                // Get all user folders
                string[] userFolders = Directory.GetDirectories(userDataPath);
                foreach (string userFolder in userFolders)
                {
                    // Try to read localconfig.vdf
                    string configPath = Path.Combine(userFolder, "config", "localconfig.vdf");
                    if (File.Exists(configPath))
                    {
                        ParseLocalConfig(configPath);
                        break; // Use first valid user config found
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load playtime data: {ex.Message}");
            }
        }

        private static void ParseLocalConfig(string configPath)
        {
            try
            {
                string content = File.ReadAllText(configPath);

                // Parse the VDF format to find playtime data
                // Pattern: "appid" { ... "playtime" "minutes" ... }
                Regex appPattern = new Regex(@"""(\d+)""\s*\{[^\}]*?""Playtime(?:Forever)?""\s+""(\d+)""", 
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                MatchCollection matches = appPattern.Matches(content);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 3)
                    {
                        if (uint.TryParse(match.Groups[1].Value, out uint appId) &&
                            int.TryParse(match.Groups[2].Value, out int playtime))
                        {
                            if (!_PlaytimeCache.ContainsKey(appId) || _PlaytimeCache[appId] < playtime)
                            {
                                _PlaytimeCache[appId] = playtime;
                            }
                        }
                    }
                }

                // Also try alternate format in apps section
                Regex appsPattern = new Regex(@"""apps""[^\{]*\{(.*?)^\s*\}", 
                    RegexOptions.Singleline | RegexOptions.Multiline);
                Match appsMatch = appsPattern.Match(content);
                
                if (appsMatch.Success)
                {
                    string appsSection = appsMatch.Groups[1].Value;
                    Regex gamePattern = new Regex(@"""(\d+)""\s*\{[^\}]*?""playtime""\s+""(\d+)""", 
                        RegexOptions.IgnoreCase);
                    
                    MatchCollection gameMatches = gamePattern.Matches(appsSection);
                    foreach (Match gameMatch in gameMatches)
                    {
                        if (gameMatch.Groups.Count >= 3)
                        {
                            if (uint.TryParse(gameMatch.Groups[1].Value, out uint appId) &&
                                int.TryParse(gameMatch.Groups[2].Value, out int playtime))
                            {
                                if (!_PlaytimeCache.ContainsKey(appId) || _PlaytimeCache[appId] < playtime)
                                {
                                    _PlaytimeCache[appId] = playtime;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse localconfig: {ex.Message}");
            }
        }

        public static void ClearCache()
        {
            _PlaytimeCache = null;
        }
    }
}
