/* Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static SAM.Game.InvariantShorthand;
using APITypes = SAM.API.Types;

namespace SAM.Game
{
    internal partial class Manager : Form
    {
        private readonly long _GameId;
        private readonly API.Client _SteamClient;

        private readonly WebClient _IconDownloader = new();

        private readonly List<Stats.AchievementInfo> _IconQueue = new();
        private readonly List<Stats.StatDefinition> _StatDefinitions = new();

        private readonly List<Stats.AchievementDefinition> _AchievementDefinitions = new();

        private readonly BindingList<Stats.StatInfo> _Statistics = new();

        private readonly API.Callbacks.UserStatsReceived _UserStatsReceivedCallback;

        //private API.Callback<APITypes.UserStatsStored> UserStatsStoredCallback;

        // Auto-close countdown timer fields
        private bool _AutoCloseEnabled = false;
        private int _CountdownSeconds = 0;
        private DateTime _CountdownStartTime = DateTime.Now;

        public Manager(long gameId, API.Client client)
        {
            this.InitializeComponent();

            this._MainTabControl.SelectedTab = this._AchievementsTabPage;
            //this.statisticsList.Enabled = this.checkBox1.Checked;

            this._AchievementImageList.Images.Add("Blank", new Bitmap(64, 64));

            this._StatisticsDataGridView.AutoGenerateColumns = false;

            this._StatisticsDataGridView.Columns.Add("name", "Name");
            this._StatisticsDataGridView.Columns[0].ReadOnly = true;
            this._StatisticsDataGridView.Columns[0].Width = 200;
            this._StatisticsDataGridView.Columns[0].DataPropertyName = "DisplayName";

            this._StatisticsDataGridView.Columns.Add("value", "Value");
            this._StatisticsDataGridView.Columns[1].ReadOnly = this._EnableStatsEditingCheckBox.Checked == false;
            this._StatisticsDataGridView.Columns[1].Width = 90;
            this._StatisticsDataGridView.Columns[1].DataPropertyName = "Value";

            this._StatisticsDataGridView.Columns.Add("extra", "Extra");
            this._StatisticsDataGridView.Columns[2].ReadOnly = true;
            this._StatisticsDataGridView.Columns[2].Width = 200;
            this._StatisticsDataGridView.Columns[2].DataPropertyName = "Extra";

            this._StatisticsDataGridView.DataSource = new BindingSource()
            {
                DataSource = this._Statistics,
            };

            this._GameId = gameId;
            this._SteamClient = client;

            this._IconDownloader.DownloadDataCompleted += this.OnIconDownload;

            string name = this._SteamClient.SteamApps001.GetAppData((uint)this._GameId, "name");
            if (name != null)
            {
                base.Text += " | " + name;
            }
            else
            {
                base.Text += " | " + this._GameId.ToString(CultureInfo.InvariantCulture);
            }

            this._UserStatsReceivedCallback = client.CreateAndRegisterCallback<API.Callbacks.UserStatsReceived>();
            this._UserStatsReceivedCallback.OnRun += this.OnUserStatsReceived;

            //this.UserStatsStoredCallback = new API.Callback(1102, new API.Callback.CallbackFunction(this.OnUserStatsStored));

            this.RefreshStats();
            this.LoadTimerState();
        }

        private void LoadTimerState()
        {
            try
            {
                string timerFile = GetTimerFilePath();
                if (File.Exists(timerFile))
                {
                    string[] lines = File.ReadAllLines(timerFile);
                    if (lines.Length >= 2)
                    {
                        if (int.TryParse(lines[0], out int countdownSeconds) && 
                            DateTime.TryParse(lines[1], out DateTime startTime))
                        {
                            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
                            int remainingSeconds = countdownSeconds - elapsedSeconds;
                            
                            if (remainingSeconds > 0)
                            {
                                // Timer still has time left, restore it
                                this._CountdownSeconds = countdownSeconds;
                                this._CountdownStartTime = startTime;
                                this._AutoCloseEnabled = true;
                            }
                            else
                            {
                                // Timer expired while closed, delete the file
                                File.Delete(timerFile);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timer state: {ex.Message}");
            }
        }

        private void SaveTimerState()
        {
            try
            {
                string timerFile = GetTimerFilePath();
                if (this._AutoCloseEnabled)
                {
                    File.WriteAllLines(timerFile, new[]
                    {
                        this._CountdownSeconds.ToString(),
                        this._CountdownStartTime.ToString("o")
                    });
                }
                else if (File.Exists(timerFile))
                {
                    File.Delete(timerFile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving timer state: {ex.Message}");
            }
        }

        private string GetTimerFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string samFolder = Path.Combine(appData, "SAM");
            if (!Directory.Exists(samFolder))
            {
                Directory.CreateDirectory(samFolder);
            }
            return Path.Combine(samFolder, $"timer_{this._GameId}.txt");
        }

        private void AddAchievementIcon(Stats.AchievementInfo info, Image icon)
        {
            if (icon == null)
            {
                info.ImageIndex = 0;
            }
            else
            {
                info.ImageIndex = this._AchievementImageList.Images.Count;
                this._AchievementImageList.Images.Add(info.IsAchieved == true ? info.IconNormal : info.IconLocked, icon);
            }
        }

        private void OnIconDownload(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                var info = (Stats.AchievementInfo)e.UserState;

                Bitmap bitmap;
                try
                {
                    using (MemoryStream stream = new())
                    {
                        stream.Write(e.Result, 0, e.Result.Length);
                        bitmap = new(stream);
                    }
                }
                catch (Exception)
                {
                    bitmap = null;
                }

                this.AddAchievementIcon(info, bitmap);
                this._AchievementListView.Update();
            }

            this.DownloadNextIcon();
        }

        private void DownloadNextIcon()
        {
            if (this._IconQueue.Count == 0)
            {
                this._DownloadStatusLabel.Visible = false;
                return;
            }

            if (this._IconDownloader.IsBusy == true)
            {
                return;
            }

            this._DownloadStatusLabel.Text = $"Downloading {this._IconQueue.Count} icons...";
            this._DownloadStatusLabel.Visible = true;

            var info = this._IconQueue[0];
            this._IconQueue.RemoveAt(0);


            this._IconDownloader.DownloadDataAsync(
                new Uri(_($"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{this._GameId}/{(info.IsAchieved == true ? info.IconNormal : info.IconLocked)}")),
                info);
        }

        private static string TranslateError(int id) => id switch
        {
            2 => "generic error -- this usually means you don't own the game",
            _ => _($"{id}"),
        };

        private static string GetLocalizedString(KeyValue kv, string language, string defaultValue)
        {
            var name = kv[language].AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            if (language != "english")
            {
                name = kv["english"].AsString("");
                if (string.IsNullOrEmpty(name) == false)
                {
                    return name;
                }
            }

            name = kv.AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            return defaultValue;
        }

        private bool LoadUserGameStatsSchema()
        {
            string path;
            try
            {
                string fileName = _($"UserGameStatsSchema_{this._GameId}.bin");
                path = API.Steam.GetInstallPath();
                path = Path.Combine(path, "appcache", "stats", fileName);
                if (File.Exists(path) == false)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            var kv = KeyValue.LoadAsBinary(path);
            if (kv == null)
            {
                return false;
            }

            var currentLanguage = this._SteamClient.SteamApps008.GetCurrentGameLanguage();

            this._AchievementDefinitions.Clear();
            this._StatDefinitions.Clear();

            var stats = kv[this._GameId.ToString(CultureInfo.InvariantCulture)]["stats"];
            if (stats.Valid == false || stats.Children == null)
            {
                return false;
            }

            foreach (var stat in stats.Children)
            {
                if (stat.Valid == false)
                {
                    continue;
                }

                var rawType = stat["type_int"].Valid
                                  ? stat["type_int"].AsInteger(0)
                                  : stat["type"].AsInteger(0);
                var type = (APITypes.UserStatType)rawType;
                switch (type)
                {
                    case APITypes.UserStatType.Invalid:
                    {
                        break;
                    }

                    case APITypes.UserStatType.Integer:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                        this._StatDefinitions.Add(new Stats.IntegerStatDefinition()
                        {
                            Id = stat["name"].AsString(""),
                            DisplayName = name,
                            MinValue = stat["min"].AsInteger(int.MinValue),
                            MaxValue = stat["max"].AsInteger(int.MaxValue),
                            MaxChange = stat["maxchange"].AsInteger(0),
                            IncrementOnly = stat["incrementonly"].AsBoolean(false),
                            SetByTrustedGameServer = stat["bSetByTrustedGS"].AsBoolean(false),
                            DefaultValue = stat["default"].AsInteger(0),
                            Permission = stat["permission"].AsInteger(0),
                        });
                        break;
                    }

                    case APITypes.UserStatType.Float:
                    case APITypes.UserStatType.AverageRate:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                        this._StatDefinitions.Add(new Stats.FloatStatDefinition()
                        {
                            Id = stat["name"].AsString(""),
                            DisplayName = name,
                            MinValue = stat["min"].AsFloat(float.MinValue),
                            MaxValue = stat["max"].AsFloat(float.MaxValue),
                            MaxChange = stat["maxchange"].AsFloat(0.0f),
                            IncrementOnly = stat["incrementonly"].AsBoolean(false),
                            DefaultValue = stat["default"].AsFloat(0.0f),
                            Permission = stat["permission"].AsInteger(0),
                        });
                        break;
                    }

                    case APITypes.UserStatType.Achievements:
                    case APITypes.UserStatType.GroupAchievements:
                    {
                        if (stat.Children != null)
                        {
                            foreach (var bits in stat.Children.Where(
                                b => string.Compare(b.Name, "bits", StringComparison.InvariantCultureIgnoreCase) == 0))
                            {
                                if (bits.Valid == false ||
                                    bits.Children == null)
                                {
                                    continue;
                                }

                                foreach (var bit in bits.Children)
                                {
                                    string id = bit["name"].AsString("");
                                    string name = GetLocalizedString(bit["display"]["name"], currentLanguage, id);
                                    string desc = GetLocalizedString(bit["display"]["desc"], currentLanguage, "");

                                    this._AchievementDefinitions.Add(new()
                                    {
                                        Id = id,
                                        Name = name,
                                        Description = desc,
                                        IconNormal = bit["display"]["icon"].AsString(""),
                                        IconLocked = bit["display"]["icon_gray"].AsString(""),
                                        IsHidden = bit["display"]["hidden"].AsBoolean(false),
                                        Permission = bit["permission"].AsInteger(0),
                                    });
                                }
                            }
                        }

                        break;
                    }

                    default:
                    {
                        throw new InvalidOperationException("invalid stat type");
                    }
                }
            }

            return true;
        }

        private void OnUserStatsReceived(APITypes.UserStatsReceived param)
        {
            if (param.Result != 1)
            {
                this._GameStatusLabel.Text = $"Error while retrieving stats: {TranslateError(param.Result)}";
                this.EnableInput();
                return;
            }

            if (this.LoadUserGameStatsSchema() == false)
            {
                this._GameStatusLabel.Text = "Failed to load schema.";
                this.EnableInput();
                return;
            }

            try
            {
                this.GetAchievements();
            }
            catch (Exception e)
            {
                this._GameStatusLabel.Text = "Error when handling achievements retrieval.";
                this.EnableInput();
                MessageBox.Show(
                    "Error when handling achievements retrieval:\n" + e,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                this.GetStatistics();
            }
            catch (Exception e)
            {
                this._GameStatusLabel.Text = "Error when handling stats retrieval.";
                this.EnableInput();
                MessageBox.Show(
                    "Error when handling stats retrieval:\n" + e,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this._GameStatusLabel.Text = $"Retrieved {this._AchievementListView.Items.Count} achievements and {this._StatisticsDataGridView.Rows.Count} statistics.";
            this.EnableInput();
        }

        private void RefreshStats()
        {
            this._AchievementListView.Items.Clear();
            this._StatisticsDataGridView.Rows.Clear();

            var steamId = this._SteamClient.SteamUser.GetSteamId();

            // This still triggers the UserStatsReceived callback, in addition to the callresult.
            // No need to implement callresults for the time being.
            var callHandle = this._SteamClient.SteamUserStats.RequestUserStats(steamId);
            if (callHandle == API.CallHandle.Invalid)
            {
                MessageBox.Show(this, "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this._GameStatusLabel.Text = "Retrieving stat information...";
            this.DisableInput();
        }

        private bool _IsUpdatingAchievementList;

        private void GetAchievements()
        {
            var textSearch = this._MatchingStringTextBox.Text.Length > 0
                ? this._MatchingStringTextBox.Text
                : null;

            this._IsUpdatingAchievementList = true;

            this._AchievementListView.Items.Clear();
            this._AchievementListView.BeginUpdate();
            //this.Achievements.Clear();

            bool wantLocked = this._DisplayLockedOnlyButton.Checked == true;
            bool wantUnlocked = this._DisplayUnlockedOnlyButton.Checked == true;

            // Request global achievement percentages (async - may take a moment)
            this._SteamClient.SteamUserStats.RequestGlobalAchievementPercentages();
            
            // Give Steam a moment to populate the global stats cache
            System.Threading.Thread.Sleep(100);

            // Collect all achievements first for sorting
            var achievementsList = new List<Stats.AchievementInfo>();

            foreach (var def in this._AchievementDefinitions)
            {
                if (string.IsNullOrEmpty(def.Id) == true)
                {
                    continue;
                }

                if (this._SteamClient.SteamUserStats.GetAchievementAndUnlockTime(
                    def.Id,
                    out bool isAchieved,
                    out var unlockTime) == false)
                {
                    continue;
                }

                bool wanted = (wantLocked == false && wantUnlocked == false) || isAchieved switch
                {
                    true => wantUnlocked,
                    false => wantLocked,
                };
                if (wanted == false)
                {
                    continue;
                }

                if (textSearch != null)
                {
                    if (def.Name.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase) < 0 &&
                        def.Description.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                }

                // Get global achievement percentage
                if (!this._SteamClient.SteamUserStats.GetAchievementAchievedPercent(def.Id, out float globalPercent))
                {
                    globalPercent = -1; // Unknown
                }

                Stats.AchievementInfo info = new()
                {
                    Id = def.Id,
                    IsAchieved = isAchieved,
                    UnlockTime = isAchieved == true && unlockTime > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(unlockTime).LocalDateTime
                        : null,
                    IconNormal = string.IsNullOrEmpty(def.IconNormal) ? null : def.IconNormal,
                    IconLocked = string.IsNullOrEmpty(def.IconLocked) ? def.IconNormal : def.IconLocked,
                    Permission = def.Permission,
                    Name = def.Name,
                    Description = def.Description,
                    GlobalPercentUnlocked = globalPercent,
                };

                achievementsList.Add(info);
            }

            // Sort by global percentage (highest to lowest) - most common achievements first
            // If global percentages are available, sort by them; otherwise keep original order
            int validPercentCount = achievementsList.Count(a => a.GlobalPercentUnlocked >= 0);
            
            if (validPercentCount > 0)
            {
                // Sort by global percentage (highest to lowest)
                achievementsList.Sort((a, b) => 
                {
                    // Put unknown (-1) at the end
                    if (a.GlobalPercentUnlocked < 0 && b.GlobalPercentUnlocked < 0) return 0;
                    if (a.GlobalPercentUnlocked < 0) return 1;
                    if (b.GlobalPercentUnlocked < 0) return -1;
                    return b.GlobalPercentUnlocked.CompareTo(a.GlobalPercentUnlocked);
                });
                
                System.Diagnostics.Debug.WriteLine($"Sorted {achievementsList.Count} achievements by global %, {validPercentCount} have valid percentages");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No global percentages available, keeping original order for {achievementsList.Count} achievements");
            }

            // Now add them to the list view in sorted order
            foreach (var info in achievementsList)
            {
                ListViewItem item = new()
                {
                    Checked = info.IsAchieved,
                    Tag = info,
                    Text = info.Name,
                    BackColor = (info.Permission & 3) == 0 ? Color.Black : Color.FromArgb(64, 0, 0),
                };

                info.Item = item;

                if (item.Text.StartsWith("#", StringComparison.InvariantCulture) == true)
                {
                    item.Text = info.Id;
                    item.SubItems.Add("");
                }
                else
                {
                    item.SubItems.Add(info.Description);
                }

                // Display scheduled time if set, otherwise actual unlock time with global %
                if (info.ScheduledUnlockTime.HasValue)
                {
                    item.SubItems.Add($"⏰ {info.ScheduledUnlockTime.Value:yyyy-MM-dd HH:mm:ss}");
                    item.BackColor = Color.FromArgb(32, 32, 64); // Blue tint for scheduled
                }
                else if (info.UnlockTime.HasValue)
                {
                    item.SubItems.Add(info.UnlockTime.Value.ToString());
                }
                else
                {
                    item.SubItems.Add("");
                }

                // Add global percentage as a subitem
                if (info.GlobalPercentUnlocked >= 0)
                {
                    item.SubItems.Add($"{info.GlobalPercentUnlocked:F1}%");
                }
                else
                {
                    item.SubItems.Add("");
                }

                info.ImageIndex = 0;

                this.AddAchievementToIconQueue(info, false);
                this._AchievementListView.Items.Add(item);
            }

            this._AchievementListView.EndUpdate();
            this._IsUpdatingAchievementList = false;

            this.DownloadNextIcon();
        }

        private void GetStatistics()
        {
            this._Statistics.Clear();
            foreach (var stat in this._StatDefinitions)
            {
                if (string.IsNullOrEmpty(stat.Id) == true)
                {
                    continue;
                }

                if (stat is Stats.IntegerStatDefinition intStat)
                {
                    if (this._SteamClient.SteamUserStats.GetStatValue(intStat.Id, out int value) == false)
                    {
                        continue;
                    }
                    this._Statistics.Add(new Stats.IntStatInfo()
                    {
                        Id = intStat.Id,
                        DisplayName = intStat.DisplayName,
                        IntValue = value,
                        OriginalValue = value,
                        IsIncrementOnly = intStat.IncrementOnly,
                        Permission = intStat.Permission,
                    });
                }
                else if (stat is Stats.FloatStatDefinition floatStat)
                {
                    if (this._SteamClient.SteamUserStats.GetStatValue(floatStat.Id, out float value) == false)
                    {
                        continue;
                    }
                    this._Statistics.Add(new Stats.FloatStatInfo()
                    {
                        Id = floatStat.Id,
                        DisplayName = floatStat.DisplayName,
                        FloatValue = value,
                        OriginalValue = value,
                        IsIncrementOnly = floatStat.IncrementOnly,
                        Permission = floatStat.Permission,
                    });
                }
            }
        }

        private void AddAchievementToIconQueue(Stats.AchievementInfo info, bool startDownload)
        {
            int imageIndex = this._AchievementImageList.Images.IndexOfKey(
                info.IsAchieved == true ? info.IconNormal : info.IconLocked);

            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else
            {
                this._IconQueue.Add(info);

                if (startDownload == true)
                {
                    this.DownloadNextIcon();
                }
            }
        }

        private int StoreAchievements()
        {
            if (this._AchievementListView.Items.Count == 0)
            {
                return 0;
            }

            List<Stats.AchievementInfo> achievements = new();
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                if (item.Tag is not Stats.AchievementInfo achievementInfo ||
                    achievementInfo.IsAchieved == item.Checked)
                {
                    continue;
                }

                achievementInfo.IsAchieved = item.Checked;
                achievements.Add(achievementInfo);
            }

            if (achievements.Count == 0)
            {
                return 0;
            }

            // Sort achievements with scheduled times by their unlock time
            var scheduledAchievements = achievements
                .Where(a => a.IsAchieved && a.ScheduledUnlockTime.HasValue)
                .OrderBy(a => a.ScheduledUnlockTime.Value)
                .ToList();

            var immediateAchievements = achievements
                .Where(a => !a.ScheduledUnlockTime.HasValue)
                .ToList();

            // Handle scheduled achievements with a warning
            if (scheduledAchievements.Count > 0)
            {
                var message = $"You have {scheduledAchievements.Count} achievement(s) with scheduled unlock times.\n\n" +
                             "NOTE: Steam's API does not support setting custom unlock times directly. " +
                             "The achievements will be unlocked now, but the unlock time displayed in Steam " +
                             "will be the current time, not your scheduled time.\n\n" +
                             "Scheduled achievements:\n" +
                             string.Join("\n", scheduledAchievements.Take(5).Select(a => 
                                 $"  • {a.Name} - {a.ScheduledUnlockTime:yyyy-MM-dd HH:mm:ss}")) +
                             (scheduledAchievements.Count > 5 ? $"\n  ... and {scheduledAchievements.Count - 5} more" : "") +
                             "\n\nDo you want to continue?";

                var result = MessageBox.Show(
                    this,
                    message,
                    "Scheduled Unlock Times",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return -1;
                }
            }

            // Process all achievements (immediate + scheduled)
            foreach (var info in immediateAchievements.Concat(scheduledAchievements))
            {
                if (this._SteamClient.SteamUserStats.SetAchievement(info.Id, info.IsAchieved) == false)
                {
                    MessageBox.Show(
                        this,
                        $"An error occurred while setting the state for {info.Id}, aborting store.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return -1;
                }
            }

            return achievements.Count;
        }

        private int StoreStatistics()
        {
            if (this._Statistics.Count == 0)
            {
                return 0;
            }

            var statistics = this._Statistics.Where(stat => stat.IsModified == true).ToList();
            if (statistics.Count == 0)
            {
                return 0;
            }

            foreach (var stat in statistics)
            {
                if (stat is Stats.IntStatInfo intStat)
                {
                    if (this._SteamClient.SteamUserStats.SetStatValue(
                        intStat.Id,
                        intStat.IntValue) == false)
                    {
                        MessageBox.Show(
                            this,
                            $"An error occurred while setting the value for {stat.Id}, aborting store.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return -1;
                    }
                }
                else if (stat is Stats.FloatStatInfo floatStat)
                {
                    if (this._SteamClient.SteamUserStats.SetStatValue(
                        floatStat.Id,
                        floatStat.FloatValue) == false)
                    {
                        MessageBox.Show(
                            this,
                            $"An error occurred while setting the value for {stat.Id}, aborting store.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return -1;
                    }
                }
                else
                {
                    throw new InvalidOperationException("unsupported stat type");
                }
            }

            return statistics.Count;
        }

        private void DisableInput()
        {
            this._ReloadButton.Enabled = false;
            this._StoreButton.Enabled = false;
        }

        private void EnableInput()
        {
            this._ReloadButton.Enabled = true;
            this._StoreButton.Enabled = true;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            this._CallbackTimer.Enabled = false;
            this._SteamClient.RunCallbacks(false);
            
            // Check auto-close timer
            if (this._AutoCloseEnabled)
            {
                CheckAutoCloseTimer();
            }
            
            // Check for scheduled achievement unlocks
            CheckScheduledUnlocks();
            
            this._CallbackTimer.Enabled = true;
        }

        private void CheckAutoCloseTimer()
        {
            try
            {
                // Calculate elapsed time since countdown started
                int elapsedSeconds = (int)(DateTime.Now - this._CountdownStartTime).TotalSeconds;
                int remainingSeconds = this._CountdownSeconds - elapsedSeconds;
                
                // Update title with remaining time
                string baseName = this._SteamClient.SteamApps001.GetAppData((uint)this._GameId, "name");
                
                if (remainingSeconds > 0)
                {
                    this.Text = $"HxB | {baseName} | ⏱ {FormatSeconds(remainingSeconds)} remaining";
                }
                else
                {
                    // Countdown reached! Close the window silently
                    this._AutoCloseEnabled = false;
                    SaveTimerState(); // This will delete the timer file
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auto-close timer error: {ex.Message}");
            }
        }

        private string FormatMinutes(int minutes)
        {
            if (minutes <= 0)
                return "0m";

            int hours = minutes / 60;
            int mins = minutes % 60;

            if (hours > 0 && mins > 0)
                return $"{hours}h {mins}m";
            else if (hours > 0)
                return $"{hours}h";
            else
                return $"{mins}m";
        }

        private string FormatSeconds(int totalSeconds)
        {
            if (totalSeconds <= 0)
                return "0s";

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            string result = "";
            if (hours > 0)
                result += $"{hours}h ";
            if (minutes > 0)
                result += $"{minutes}m ";
            if (seconds > 0 || result == "")
                result += $"{seconds}s";

            return result.Trim();
        }

        private void CheckScheduledUnlocks()
        {
            try
            {
                var now = DateTime.Now;
                bool anyUnlocked = false;

                // Check all items in the list view for scheduled unlocks
                foreach (ListViewItem item in this._AchievementListView.Items)
                {
                    if (item.Tag is Stats.AchievementInfo info)
                    {
                        // If achievement has a scheduled time and it's time to unlock
                        if (info.ScheduledUnlockTime.HasValue && 
                            !info.IsAchieved && 
                            now >= info.ScheduledUnlockTime.Value)
                        {
                            // Unlock the achievement
                            if (this._SteamClient.SteamUserStats.SetAchievement(info.Id, true))
                            {
                                info.IsAchieved = true;
                                info.UnlockTime = now;
                                info.ScheduledUnlockTime = null; // Clear schedule after unlock
                                
                                // Update the item
                                item.Checked = true;
                                item.SubItems[2].Text = now.ToString();
                                item.BackColor = (info.Permission & 3) == 0 ? Color.Black : Color.FromArgb(64, 0, 0);
                                
                                anyUnlocked = true;
                                
                                System.Diagnostics.Debug.WriteLine($"Auto-unlocked achievement: {info.Name} at {now:HH:mm:ss}");
                            }
                        }
                    }
                }

                // If any achievements were unlocked, enable the Store button
                if (anyUnlocked)
                {
                    this._StoreButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scheduled unlock check error: {ex.Message}");
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this.RefreshStats();
        }

        private void OnLockAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = false;
            }
        }

        private void OnInvertAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        private void OnUnlockAll(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this._AchievementListView.Items)
            {
                item.Checked = true;
            }
        }

        private bool Store()
        {
            if (this._SteamClient.SteamUserStats.StoreStats() == false)
            {
                MessageBox.Show(
                    this,
                    "An error occurred while storing, aborting.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void OnStore(object sender, EventArgs e)
        {
            int achievements = this.StoreAchievements();
            if (achievements < 0)
            {
                this.RefreshStats();
                return;
            }

            int stats = this.StoreStatistics();
            if (stats < 0)
            {
                this.RefreshStats();
                return;
            }

            if (this.Store() == false)
            {
                this.RefreshStats();
                return;
            }

            MessageBox.Show(
                this,
                $"Stored {achievements} achievements and {stats} statistics.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            this.RefreshStats();
        }

        private void OnStatDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context != DataGridViewDataErrorContexts.Commit)
            {
                return;
            }

            var view = (DataGridView)sender;
            if (e.Exception is Stats.StatIsProtectedException)
            {
                e.ThrowException = false;
                e.Cancel = true;
                view.Rows[e.RowIndex].ErrorText = "Stat is protected! -- you can't modify it";
            }
            else
            {
                e.ThrowException = false;
                e.Cancel = true;
                view.Rows[e.RowIndex].ErrorText = "Invalid value";
            }
        }

        private void OnStatAgreementChecked(object sender, EventArgs e)
        {
            this._StatisticsDataGridView.Columns[1].ReadOnly = this._EnableStatsEditingCheckBox.Checked == false;
        }

        private void OnStatCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var view = (DataGridView)sender;
            view.Rows[e.RowIndex].ErrorText = "";
        }

        private void OnAutoCloseTimer(object sender, EventArgs e)
        {
            using (var dialog = new AutoCloseTimerDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.EnableAutoClose)
                {
                    _AutoCloseEnabled = true;
                    _CountdownSeconds = dialog.CountdownSeconds;
                    _CountdownStartTime = DateTime.Now;
                    SaveTimerState();
                }
                else if (dialog.DialogResult == DialogResult.Cancel)
                {
                    // User cancelled, disable timer
                    _AutoCloseEnabled = false;
                    SaveTimerState();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SaveTimerState();
        }

        private void OnResetAllStats(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you absolutely sure you want to reset stats?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            bool achievementsToo = DialogResult.Yes == MessageBox.Show(
                "Do you want to reset achievements too?",
                "Question",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (MessageBox.Show(
                "Really really sure?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error) == DialogResult.No)
            {
                return;
            }

            if (this._SteamClient.SteamUserStats.ResetAllStats(achievementsToo) == false)
            {
                MessageBox.Show(this, "Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.RefreshStats();
        }

        private void OnCheckAchievement(object sender, ItemCheckEventArgs e)
        {
            if (sender != this._AchievementListView)
            {
                return;
            }

            if (this._IsUpdatingAchievementList == true)
            {
                return;
            }

            if (this._AchievementListView.Items[e.Index].Tag is not Stats.AchievementInfo info)
            {
                return;
            }

            if ((info.Permission & 3) != 0)
            {
                MessageBox.Show(
                    this,
                    "Sorry, but this is a protected achievement and cannot be managed with Steam Achievement Manager.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                e.NewValue = e.CurrentValue;
            }
        }

        private void OnDisplayUncheckedOnly(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Checked == true)
            {
                this._DisplayLockedOnlyButton.Checked = false;
            }

            this.GetAchievements();
        }

        private void OnDisplayCheckedOnly(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Checked == true)
            {
                this._DisplayUnlockedOnlyButton.Checked = false;
            }

            this.GetAchievements();
        }

        private void OnFilterUpdate(object sender, KeyEventArgs e)
        {
            this.GetAchievements();
        }

        private void OnSetUnlockTime(object sender, EventArgs e)
        {
            if (this._AchievementListView.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Please select an achievement first.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var selectedItem = this._AchievementListView.SelectedItems[0];
            if (selectedItem.Tag is not Stats.AchievementInfo info)
            {
                return;
            }

            if ((info.Permission & 3) != 0)
            {
                MessageBox.Show(
                    this,
                    "Sorry, but this is a protected achievement and cannot be managed with Steam Achievement Manager.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            using (var dialog = new AchievementTimeDialog(info.Name, info.ScheduledUnlockTime))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    info.ScheduledUnlockTime = dialog.ScheduledTime;
                    
                    // Update the display
                    if (info.ScheduledUnlockTime.HasValue)
                    {
                        selectedItem.SubItems[2].Text = $"⏰ {info.ScheduledUnlockTime.Value:yyyy-MM-dd HH:mm:ss}";
                        selectedItem.BackColor = Color.FromArgb(32, 32, 64); // Blue tint for scheduled
                    }
                    else
                    {
                        selectedItem.SubItems[2].Text = info.UnlockTime.HasValue 
                            ? info.UnlockTime.Value.ToString() 
                            : "";
                        selectedItem.BackColor = (info.Permission & 3) == 0 ? Color.Black : Color.FromArgb(64, 0, 0);
                    }
                }
            }
        }

        private void OnBulkSetUnlockTime(object sender, EventArgs e)
        {
            // Calculate remaining time from auto-close timer if enabled
            int? remainingMinutes = null;
            if (this._AutoCloseEnabled && this._CountdownSeconds > 0)
            {
                int elapsed = (int)(DateTime.Now - this._CountdownStartTime).TotalSeconds;
                int remaining = this._CountdownSeconds - elapsed;
                if (remaining > 0)
                {
                    remainingMinutes = remaining / 60; // Convert to minutes
                }
            }
            
            using (var dialog = new BulkUnlockTimeDialog(remainingMinutes))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var achievements = new List<Stats.AchievementInfo>();

                if (dialog.ApplyToAll)
                {
                    // Apply to all displayed achievements
                    foreach (ListViewItem item in this._AchievementListView.Items)
                    {
                        if (item.Tag is Stats.AchievementInfo info && (info.Permission & 3) == 0)
                        {
                            achievements.Add(info);
                        }
                    }
                }
                else
                {
                    // Apply to selected achievements only
                    foreach (ListViewItem item in this._AchievementListView.SelectedItems)
                    {
                        if (item.Tag is Stats.AchievementInfo info && (info.Permission & 3) == 0)
                        {
                            achievements.Add(info);
                        }
                    }

                    if (achievements.Count == 0)
                    {
                        MessageBox.Show(
                            this,
                            "Please select at least one achievement first.",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }
                }

                if (achievements.Count == 0)
                {
                    MessageBox.Show(
                        this,
                        "No achievements available to set unlock times.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                this._AchievementListView.BeginUpdate();

                if (dialog.UseSmartDistribution)
                {
                    // Realistic Smart Distribution: Scale unlock times based on achievement rarity
                    // Example: 85% achievement = 15 min, 0.1% achievement = 100+ hours
                    // Formula uses exponential scale where rarer achievements take exponentially longer
                    
                    var totalDuration = TimeSpan.FromMinutes(dialog.TotalDurationMinutes);
                    
                    // Calculate "difficulty time" for each achievement based on rarity
                    // Using logarithmic scale: time increases exponentially as rarity decreases
                    var difficultyTimes = new List<double>();
                    
                    foreach (var ach in achievements)
                    {
                        double percent = ach.GlobalPercentUnlocked >= 0 ? ach.GlobalPercentUnlocked : 50.0; // Default to 50% if unknown
                        
                        // Clamp percent to reasonable bounds
                        percent = Math.Max(0.01, Math.Min(100.0, percent));
                        
                        // Calculate difficulty time using exponential scaling
                        // Formula: time = baseTime * e^(k * (1 - percent/100))
                        // Where k controls the exponential curve steepness
                        
                        // Map percentage to difficulty score (0 to 1, where 1 = rarest)
                        double rarityScore = 1.0 - (percent / 100.0);
                        
                        // Exponential scaling factor (adjust for desired curve steepness)
                        // k=5 gives good spread: 85% ≈ 15min, 0.1% ≈ 100h
                        double k = 5.0;
                        double difficultyMultiplier = Math.Exp(k * rarityScore);
                        
                        difficultyTimes.Add(difficultyMultiplier);
                    }
                    
                    // Normalize difficulty times to fit within total duration
                    double totalDifficultyTime = difficultyTimes.Sum();
                    double scale = totalDuration.TotalMinutes / totalDifficultyTime;
                    
                    // Assign unlock times cumulatively
                    double cumulativeMinutes = 0;
                    for (int i = 0; i < achievements.Count; i++)
                    {
                        var info = achievements[i];
                        
                        // Add small random variation (±5%) for realism
                        Random rng = new Random(info.Id.GetHashCode());
                        double variation = 0.95 + (rng.NextDouble() * 0.1); // 0.95 to 1.05
                        
                        double allocatedMinutes = difficultyTimes[i] * scale * variation;
                        cumulativeMinutes += allocatedMinutes;
                        
                        var scheduled = dialog.StartTime.AddMinutes(cumulativeMinutes);
                        info.ScheduledUnlockTime = scheduled;

                        // Update the display
                        if (info.Item != null)
                        {
                            info.Item.SubItems[2].Text = $"⏰ {scheduled:yyyy-MM-dd HH:mm:ss}";
                            info.Item.BackColor = Color.FromArgb(32, 32, 64); // Blue tint for scheduled
                        }
                    }
                }
                else
                {
                    // Apply sequential unlock times
                    var currentTime = dialog.StartTime;
                    var interval = TimeSpan.FromMinutes(dialog.IntervalMinutes);

                    foreach (var info in achievements)
                    {
                        info.ScheduledUnlockTime = currentTime;

                        // Update the display
                        if (info.Item != null)
                        {
                            info.Item.SubItems[2].Text = $"⏰ {currentTime:yyyy-MM-dd HH:mm:ss}";
                            info.Item.BackColor = Color.FromArgb(32, 32, 64); // Blue tint for scheduled
                        }

                        currentTime = currentTime.Add(interval);
                    }
                }
                // Determine last scheduled time for reporting
                DateTime lastScheduled = DateTime.MinValue;
                if (dialog.UseSmartDistribution)
                {
                    lastScheduled = achievements.Max(a => a.ScheduledUnlockTime ?? DateTime.MinValue);
                }
                else
                {
                    // previous logic left currentTime at last + interval
                    // compute last scheduled by subtracting one interval
                    var interval = TimeSpan.FromMinutes(dialog.IntervalMinutes);
                    lastScheduled = achievements.Count > 0 ? achievements.Last().ScheduledUnlockTime ?? DateTime.MinValue : DateTime.MinValue;
                }

                this._AchievementListView.EndUpdate();

                string intervalInfo = dialog.UseSmartDistribution
                    ? $"Total duration: {dialog.TotalDurationMinutes} minutes"
                    : $"Interval: {dialog.IntervalMinutes} minutes";

                MessageBox.Show(
                    this,
                    $"Set scheduled unlock times for {achievements.Count} achievement(s).\n\n" +
                    $"Starting at: {dialog.StartTime:yyyy-MM-dd HH:mm:ss}\n" +
                    intervalInfo + "\n" +
                    $"Last: { (lastScheduled == DateTime.MinValue ? "N/A" : lastScheduled.ToString("yyyy-MM-dd HH:mm:ss")) }",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
