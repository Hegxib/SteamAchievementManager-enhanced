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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.XPath;
using static SAM.Picker.InvariantShorthand;
using APITypes = SAM.API.Types;

namespace SAM.Picker
{
    internal partial class GamePicker : Form
    {
        private readonly API.Client _SteamClient;

        private readonly Dictionary<uint, GameInfo> _Games;
        private readonly List<GameInfo> _FilteredGames;

        private readonly object _LogoLock;
        private readonly HashSet<string> _LogosAttempting;
        private readonly HashSet<string> _LogosAttempted;
        private readonly ConcurrentQueue<GameInfo> _LogoQueue;

        private readonly API.Callbacks.AppDataChanged _AppDataChangedCallback;
        
        private readonly HashSet<uint> _SelectedGameIds; // Track selected game IDs
        private readonly List<GameInfo> _SelectedGames;   // Selected games for second ListView
        
        private readonly HashSet<uint> _DoneGameIds;      // Track done game IDs  
        private readonly List<GameInfo> _DoneGames;       // Done games for third ListView

        // Context menu targeting
        private int _ContextIndex = -1;
        private MyListView _ContextListView = null;

        // Image cache directories
        private readonly string _CacheDirectory;
        private readonly string _SelectedCacheDirectory;
        private readonly string _DoneCacheDirectory;

        public GamePicker(API.Client client)
        {
            this._Games = new();
            this._FilteredGames = new();
            this._SelectedGames = new();
            this._DoneGames = new();
            this._LogoLock = new();
            this._LogosAttempting = new();
            this._LogosAttempted = new();
            this._LogoQueue = new();
            this._SelectedGameIds = new();
            this._DoneGameIds = new();

            // Initialize cache directories
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this._CacheDirectory = Path.Combine(appDataPath, "SAM", "ImageCache");
            this._SelectedCacheDirectory = Path.Combine(appDataPath, "SAM", "SelectedCache");
            this._DoneCacheDirectory = Path.Combine(appDataPath, "SAM", "DoneCache");
            
            if (!Directory.Exists(this._CacheDirectory))
            {
                Directory.CreateDirectory(this._CacheDirectory);
            }
            if (!Directory.Exists(this._SelectedCacheDirectory))
            {
                Directory.CreateDirectory(this._SelectedCacheDirectory);
            }
            if (!Directory.Exists(this._DoneCacheDirectory))
            {
                Directory.CreateDirectory(this._DoneCacheDirectory);
            }

            this.InitializeComponent();

            // Initialize language system (must be after InitializeComponent)
            Localization.LanguageManager.Instance.LanguageChanged += OnLanguageChanged;
            ApplyCurrentLanguage();

            Bitmap blank = new(this._LogoImageList.ImageSize.Width, this._LogoImageList.ImageSize.Height);
            using (var g = Graphics.FromImage(blank))
            {
                g.Clear(Color.DimGray);
            }

            this._LogoImageList.Images.Add("Blank", blank);

            this._SteamClient = client;

            this._AppDataChangedCallback = client.CreateAndRegisterCallback<API.Callbacks.AppDataChanged>();
            this._AppDataChangedCallback.OnRun += this.OnAppDataChanged;

            this.LoadSelectedGames(); // Load persistent selection
            this.LoadDoneGames();     // Load persistent done games
            this.AddGames();
        }

        private void OnAppDataChanged(APITypes.AppDataChanged param)
        {
            if (param.Result == false)
            {
                return;
            }

            if (this._Games.TryGetValue(param.Id, out var game) == false)
            {
                return;
            }

            game.Name = this._SteamClient.SteamApps001.GetAppData(game.Id, "name");

            this.AddGameToLogoQueue(game);
            this.DownloadNextLogo();
        }

        private void DoDownloadList(object sender, DoWorkEventArgs e)
        {
            this._PickerStatusLabel.Text = "Downloading game list...";

            byte[] bytes;
            using (WebClient downloader = new())
            {
                bytes = downloader.DownloadData(new Uri("https://gib.me/sam/games.xml"));
            }

            List<KeyValuePair<uint, string>> pairs = new();
            using (MemoryStream stream = new(bytes, false))
            {
                XPathDocument document = new(stream);
                var navigator = document.CreateNavigator();
                var nodes = navigator.Select("/games/game");
                while (nodes.MoveNext() == true)
                {
                    string type = nodes.Current.GetAttribute("type", "");
                    if (string.IsNullOrEmpty(type) == true)
                    {
                        type = "normal";
                    }
                    pairs.Add(new((uint)nodes.Current.ValueAsLong, type));
                }
            }

            this._PickerStatusLabel.Text = "Checking game ownership...";
            foreach (var kv in pairs)
            {
                this.AddGame(kv.Key, kv.Value);
            }
        }

        private void OnDownloadList(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true)
            {
                this.AddDefaultGames();
                MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.RefreshGames();
            this._RefreshGamesButton.Enabled = true;
            this.DownloadNextLogo();
        }

        private void RefreshGames()
        {
            var nameSearch = this._SearchGameTextBox.Text.Length > 0
                ? this._SearchGameTextBox.Text
                : null;

            var wantNormals = this._FilterGamesMenuItem.Checked == true;
            var wantDemos = this._FilterDemosMenuItem.Checked == true;
            var wantMods = this._FilterModsMenuItem.Checked == true;
            var wantJunk = this._FilterJunkMenuItem.Checked == true;

            this._FilteredGames.Clear();
            
            // Update IsSelected status based on tracked IDs
            foreach (var info in this._Games.Values)
            {
                info.IsSelected = this._SelectedGameIds.Contains(info.Id);
            }
            
            // Separate selected, done, and unselected games
            var selectedGames = new List<GameInfo>();
            var doneGames = new List<GameInfo>();
            var unselectedGames = new List<GameInfo>();
            
            foreach (var info in this._Games.Values.OrderBy(gi => gi.Name))
            {
                if (nameSearch != null &&
                    info.Name.IndexOf(nameSearch, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                bool wanted = info.Type switch
                {
                    "normal" => wantNormals,
                    "demo" => wantDemos,
                    "mod" => wantMods,
                    "junk" => wantJunk,
                    _ => true,
                };
                if (wanted == false)
                {
                    continue;
                }

                // Prioritize DONE over SELECTED
                // RULE: Games in DONE (100% achievements) cannot be in OTHERS
                if (this._DoneGameIds.Contains(info.Id))
                {
                    doneGames.Add(info);
                }
                else if (info.IsSelected)
                {
                    selectedGames.Add(info);
                }
                else
                {
                    unselectedGames.Add(info);
                }
            }
            
            // Populate SELECTED ListView
            this._SelectedGames.Clear();
            this._SelectedGames.AddRange(selectedGames);
            this._SelectedListView.VirtualListSize = this._SelectedGames.Count;
            
            // Cache logos for SELECTED games
            foreach (var game in selectedGames)
            {
                SaveLogoToSectionCache(game, this._SelectedCacheDirectory);
            }
            
            // Populate DONE ListView
            this._DoneGames.Clear();
            this._DoneGames.AddRange(doneGames);
            // Access _DoneListView via GetType reflection to avoid Designer.cs issues
            var doneListView = this.GetType().GetField("_DoneListView", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
            if (doneListView != null)
            {
                doneListView.VirtualListSize = this._DoneGames.Count;
            }
            
            // Cache logos for DONE games
            foreach (var game in doneGames)
            {
                SaveLogoToSectionCache(game, this._DoneCacheDirectory);
            }
            
            // Populate OTHER GAMES ListView
            this._FilteredGames.Clear();
            this._FilteredGames.AddRange(unselectedGames);
            this._GameListView.VirtualListSize = this._FilteredGames.Count;
            
            // Update header labels and visibility
            this._SelectedHeaderLabel.Text = $"▼ SELECTED ({selectedGames.Count})";
            var doneHeaderLabel = this.GetType().GetField("_DoneHeaderLabel",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Label;
            if (doneHeaderLabel != null)
            {
                doneHeaderLabel.Text = $"▼ DONE ({doneGames.Count})";
            }
            this._OtherGamesHeaderLabel.Text = $"▼ OTHER GAMES ({unselectedGames.Count})";
            
            // Show SELECTED section only when there are selected games
            this._SelectedHeaderPanel.Visible = selectedGames.Count > 0;
            this._SelectedListView.Visible = selectedGames.Count > 0;
            
            // Show DONE section only when there are done games
            var doneHeaderPanel = this.GetType().GetField("_DoneHeaderPanel",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Panel;
            if (doneHeaderPanel != null)
            {
                doneHeaderPanel.Visible = doneGames.Count > 0;
            }
            if (doneListView != null)
            {
                doneListView.Visible = doneGames.Count > 0;
            }
            
            // OTHER GAMES section is always visible
            this._OtherGamesHeaderPanel.Visible = true;
            
            // Update status label with section information
            if (selectedGames.Count > 0 || doneGames.Count > 0)
            {
                this._PickerStatusLabel.Text =
                    $"SELECTED: {selectedGames.Count} | DONE: {doneGames.Count} | Other: {unselectedGames.Count} | Total: {this._Games.Count} games";
            }
            else
            {
                this._PickerStatusLabel.Text =
                    $"Displaying {this._GameListView.Items.Count} games. Total {this._Games.Count} games.";
            }

            if (this._GameListView.Items.Count > 0)
            {
                this._GameListView.Items[0].Selected = true;
                this._GameListView.Select();
            }
        }

        private void OnGameListViewRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var info = this._FilteredGames[e.ItemIndex];
            
            // Format game name with playtime
            string displayText = info.Name;
            if (info.PlaytimeForever > 0)
            {
                displayText = $"{info.Name}\n⏱ {info.FormattedPlaytime} played";
            }
            
            e.Item = info.Item = new()
            {
                Text = displayText,
                ImageIndex = info.ImageIndex,
                BackColor = Color.Black,
                ForeColor = Color.White,
            };
        }

        private void OnSelectedListViewRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var info = this._SelectedGames[e.ItemIndex];
            
            // Format game name with playtime
            string displayText = info.Name;
            if (info.PlaytimeForever > 0)
            {
                displayText = $"{info.Name}\n⏱ {info.FormattedPlaytime} played";
            }
            
            e.Item = info.Item = new()
            {
                Text = displayText,
                ImageIndex = info.ImageIndex, // Show actual game logo
                BackColor = Color.FromArgb(30, 50, 30),
                ForeColor = Color.Yellow,
            };
        }

        private void OnDoneListViewRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var info = this._DoneGames[e.ItemIndex];
            
            // Format game name with playtime
            string displayText = info.Name;
            if (info.PlaytimeForever > 0)
            {
                displayText = $"{info.Name}\n⏱ {info.FormattedPlaytime} played";
            }
            
            e.Item = info.Item = new()
            {
                Text = displayText,
                ImageIndex = info.ImageIndex, // Show actual game logo
                BackColor = Color.FromArgb(30, 50, 30),
                ForeColor = Color.LightGreen,
            };
        }

        private void OnToggleSelectedSection(object sender, EventArgs e)
        {
            // Toggle selected section visibility and update header icon
            this._SelectedListView.Visible = !this._SelectedListView.Visible;
            string icon = this._SelectedListView.Visible ? "▼" : "▶";
            this._SelectedHeaderLabel.Text = $"{icon} SELECTED ({this._SelectedGames.Count})";
            this.PerformLayout();
        }

        private void OnToggleDoneSection(object sender, EventArgs e)
        {
            // Access _DoneListView via reflection to avoid Designer.cs issues
            var doneListView = this.GetType().GetField("_DoneListView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
            var doneHeaderLabel = this.GetType().GetField("_DoneHeaderLabel",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Label;
                
            if (doneListView != null && doneHeaderLabel != null)
            {
                doneListView.Visible = !doneListView.Visible;
                string icon = doneListView.Visible ? "▼" : "▶";
                doneHeaderLabel.Text = $"{icon} DONE ({this._DoneGames.Count})";
                this.PerformLayout();
            }
        }

        private void OnToggleOtherSection(object sender, EventArgs e)
        {
            // Toggle main grid visibility and update header icon
            this._GameListView.Visible = !this._GameListView.Visible;
            string icon = this._GameListView.Visible ? "▼" : "▶";
            this._OtherGamesHeaderLabel.Text = $"{icon} OTHER GAMES ({this._FilteredGames.Count})";
            this.PerformLayout();
        }

        private void OnGameListViewSearchForVirtualItem(object sender, SearchForVirtualItemEventArgs e)
        {
            if (e.Direction != SearchDirectionHint.Down || e.IsTextSearch == false)
            {
                return;
            }

            var count = this._FilteredGames.Count;
            if (count < 2)
            {
                return;
            }

            var text = e.Text;
            int startIndex = e.StartIndex;

            Predicate<GameInfo> predicate;
            /*if (e.IsPrefixSearch == true)*/
            {
                predicate = gi => gi.Name != null && gi.Name.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
            }
            /*else
            {
                predicate = gi => gi.Name != null && string.Compare(gi.Name, text, StringComparison.CurrentCultureIgnoreCase) == 0;
            }*/

            int index;
            if (e.StartIndex >= count)
            {
                // starting from the last item in the list
                index = this._FilteredGames.FindIndex(0, startIndex - 1, predicate);
            }
            else if (startIndex <= 0)
            {
                // starting from the first item in the list
                index = this._FilteredGames.FindIndex(0, count, predicate);
            }
            else
            {
                index = this._FilteredGames.FindIndex(startIndex, count - startIndex, predicate);
                if (index < 0)
                {
                    index = this._FilteredGames.FindIndex(0, startIndex - 1, predicate);
                }
            }

            e.Index = index < 0 ? -1 : index;
        }

        private void OnToggleSelection(object sender, EventArgs e)
        {
            if (this._GameListView.SelectedIndices.Count == 0)
            {
                return;
            }

            // Toggle RUNNING status for all selected items
            foreach (int selectedIndex in this._GameListView.SelectedIndices)
            {
                if (selectedIndex >= 0 && selectedIndex < this._FilteredGames.Count)
                {
                    var gameInfo = this._FilteredGames[selectedIndex];
                    if (this._SelectedGameIds.Contains(gameInfo.Id))
                    {
                        this._SelectedGameIds.Remove(gameInfo.Id);
                    }
                    else
                    {
                        this._SelectedGameIds.Add(gameInfo.Id);
                        // Save logo and assets to SELECTED cache when adding
                        SaveLogoToSectionCache(gameInfo, this._SelectedCacheDirectory);
                        // Download additional assets in background
                        System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameInfo, this._SelectedCacheDirectory));
                    }
                }
            }

            // Refresh to show changes
            this.RefreshGames();
        }

        private void OnClearAllSelections(object sender, EventArgs e)
        {
            if (this._SelectedGameIds.Count == 0)
            {
                return;
            }

            this._SelectedGameIds.Clear();
            this.RefreshGames();
        }

        private void OnContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dynamically update menu items based on context
            var clickedListView = (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView != null)
            {
                var hitTest = clickedListView.HitTest(clickedListView.PointToClient(Cursor.Position));
                if (hitTest.Item != null)
                {
                    // Ensure the item under cursor becomes selected so subsequent handlers use the same index
                    try { hitTest.Item.Selected = true; } catch { /* ignore */ }
                    int index = hitTest.Item.Index;
                    // Store for subsequent handlers to avoid mismatches
                    this._ContextIndex = index;
                    this._ContextListView = clickedListView;
                    bool isInSelectedList = (clickedListView == this._SelectedListView);
                    
                    // Get _DoneListView via reflection
                    var doneListView = this.GetType().GetField("_DoneListView",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
                    bool isInDoneList = (doneListView != null && clickedListView == doneListView);
                    
                    // Update menu text dynamically using translations
                    var lang = Localization.LanguageManager.Instance;
                    this._ToggleSelectionMenuItem.Text = isInSelectedList ? 
                        lang.GetString("context_remove_selected") : 
                        lang.GetString("context_toggle_selection");
                    this._RemoveFromSelectedMenuItem.Visible = isInSelectedList;
                    this._LaunchThisOnlyMenuItem.Visible = true;
                    
                    // Update Mark as Done text based on whether game is in DONE section
                    this._MarkAsDoneMenuItem.Text = isInDoneList ? 
                        lang.GetString("context_unmark_done") : 
                        lang.GetString("context_mark_done");
                }
                else
                {
                    // No item under cursor; cancel opening to avoid acting on wrong item
                    this._ContextIndex = -1;
                    this._ContextListView = null;
                    e.Cancel = true;
                }
            }
        }

        private void OnLaunchThisOnly(object sender, EventArgs e)
        {
            var clickedListView = this._ContextListView ?? (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView == null) return;
            LaunchSelectedGames(clickedListView);
        }
        
        private void OnGameProcessExited(uint gameId)
        {
            // Check if this game was auto-closed (timer expired)
            string samFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SAM");
            string markerFile = Path.Combine(samFolder, $"autoclosed_{gameId}.marker");
            
            bool wasAutoClosed = File.Exists(markerFile);
            
            // Clean up marker file if it exists
            if (wasAutoClosed)
            {
                try
                {
                    File.Delete(markerFile);
                }
                catch
                {
                    // Ignore deletion errors
                }
                
                // Auto-closed: Move from SELECTED to DONE
                if (this._SelectedGameIds.Contains(gameId))
                {
                    // Remove from SELECTED
                    this._SelectedGameIds.Remove(gameId);
                    this._SelectedGames.RemoveAll(g => g.Id == gameId);
                    
                    // Add to DONE
                    if (!this._DoneGameIds.Contains(gameId))
                    {
                        this._DoneGameIds.Add(gameId);
                        var game = this._Games.Values.FirstOrDefault(g => g.Id == gameId);
                        if (game != null)
                        {
                            this._DoneGames.Add(game);
                        }
                    }
                    
                    // Persist changes
                    this.SaveSelectedGames();
                    this.SaveDoneGames();
                    this.RefreshGames();
                }
            }
            // Manual close: Keep in current section (unchanged)
        }

        private void OnLaunchOneRandom(object sender, EventArgs e)
        {
            if (this._SelectedGameIds.Count == 0)
            {
                var lang = Localization.LanguageManager.Instance;
                MessageBox.Show(
                    this,
                    lang.GetString("error_no_games_selected"),
                    lang.GetString("error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Pick a random game from selected
            var random = new Random();
            var selectedArray = this._SelectedGameIds.ToArray();
            int randomIndex = random.Next(selectedArray.Length);
            uint gameId = selectedArray[randomIndex];
            
            if (this._Games.TryGetValue(gameId, out var gameInfo))
            {
                LaunchGame(gameInfo);
            }
        }

        private void OnBulkReset(object sender, EventArgs e)
        {
            // Get all owned games
            var allGames = this._Games.Values.ToList();
            
            if (allGames.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "No games available. Please refresh your game library first.",
                    "No Games",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Show bulk reset dialog with SELECTED games pre-selected
            using (var dialog = new BulkResetDialog(allGames, this._SelectedGameIds))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedGameIds.Count > 0)
                {
                    int successCount = 0;
                    int failCount = 0;

                    foreach (var gameId in dialog.SelectedGameIds)
                    {
                        try
                        {
                            var gamePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                            Process.Start(gamePath, $"{gameId} -reset");
                            successCount++;
                        }
                        catch (Win32Exception)
                        {
                            failCount++;
                        }
                        catch (Exception)
                        {
                            failCount++;
                        }
                    }

                    // Show summary
                    string message = $"Bulk reset initiated for {successCount} game(s).";
                    if (failCount > 0)
                    {
                        message += $"\n\nFailed to launch reset for {failCount} game(s).";
                    }

                    MessageBox.Show(
                        this,
                        message,
                        failCount > 0 ? "Partial Success" : "Success",
                        MessageBoxButtons.OK,
                        failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                }
            }
        }

        private void OnDonateClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://ko-fi.com/hegxib");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Failed to open donation page: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnCheatSheetClick(object sender, EventArgs e)
        {
            string cheatSheet = @"╔════════════════════════════════════════════════════════╗
║           HxB SAM ENHANCED - CHEAT SHEET             ║
╚════════════════════════════════════════════════════════╝

🖱️ MOUSE SHORTCUTS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • Left-Click        → Toggle SELECTED ↔ OTHERS
  • Ctrl + Click      → Force move to SELECTED
  • Alt + Click       → Force move to DONE
  • Right-Click       → Context menu options

📋 SECTIONS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • SELECTED     → Games queued to launch
  • DONE         → 100% complete games
  • OTHERS       → All remaining games

🎮 CONTEXT MENU:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • Mark/Unmark as Done    → Toggle DONE status
  • Reset Achievements     → Clear game progress
  • Launch This Only       → Launch single game
  • Launch One Random      → Random game launch

🔧 TOOLBAR:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  • Select All       → Add all to SELECTED
  • Clear All        → Clear SELECTED section
  • Clear DONE       → Clear DONE section
  • Bulk Reset       → Reset multiple games

💡 TIPS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  ✓ DONE games are protected from accidental moves
  ✓ Logos & assets auto-cache for offline use
  ✓ Sections fold/unfold by clicking headers
  ✓ Search filters work across all sections
  ✓ Playtime shows for games with activity

📂 CACHE LOCATIONS:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  %AppData%\SAM\ImageCache\      → Main cache
  %AppData%\SAM\SelectedCache\   → SELECTED cache
  %AppData%\SAM\DoneCache\       → DONE cache

Made by Hegxib | v1.3.0";

            MessageBox.Show(
                this,
                cheatSheet,
                "📋 Cheat Sheet - Shortcuts & Tips",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnLanguageDropDownOpening(object sender, EventArgs e)
        {
            _LanguageButton.DropDownItems.Clear();
            
            var languages = Localization.LanguageManager.Instance.GetAvailableLanguages();
            var currentLang = Localization.LanguageManager.Instance.CurrentLanguageCode;
            
            foreach (var lang in languages)
            {
                var item = new ToolStripMenuItem
                {
                    Text = lang.Name,
                    Tag = lang.Code,
                    Checked = (lang.Code == currentLang),
                    CheckOnClick = false
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
                
                // Load the new language
                Localization.LanguageManager.Instance.LoadLanguage(langCode);
                
                // Apply translations immediately (no restart needed)
                ApplyCurrentLanguage();
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ApplyCurrentLanguage();
        }

        private void ApplyCurrentLanguage()
        {
            var lang = Localization.LanguageManager.Instance;
            
            // Update window title
            this.Text = lang.GetString("app_title");
            
            // Update toolbar buttons
            _RefreshGamesButton.Text = lang.GetString("toolbar_refresh");
            _AddGameButton.Text = lang.GetString("toolbar_add_game");
            _FindGamesLabel.Text = lang.GetString("toolbar_filter");
            _FilterDropDownButton.Text = lang.GetString("toolbar_filter");
            _FilterGamesMenuItem.Text = lang.GetString("toolbar_filter_games");
            _FilterDemosMenuItem.Text = lang.GetString("toolbar_filter_demos");
            _FilterModsMenuItem.Text = lang.GetString("toolbar_filter_mods");
            _FilterJunkMenuItem.Text = lang.GetString("toolbar_filter_junk");
            _SelectAllButton.Text = lang.GetString("toolbar_select_all");
            _ClearAllButton.Text = lang.GetString("toolbar_clear_all");
            _BulkResetButton.Text = lang.GetString("toolbar_bulk_reset");
            _BulkResetButton.ToolTipText = lang.GetString("toolbar_bulk_reset_tooltip");
            _ClearDoneButton.Text = lang.GetString("toolbar_clear_done");
            _ClearDoneButton.ToolTipText = lang.GetString("toolbar_clear_done_tooltip");
            _CheatSheetButton.Text = lang.GetString("toolbar_cheat_sheet");
            _CheatSheetButton.ToolTipText = lang.GetString("toolbar_cheat_sheet_tooltip");
            _LanguageButton.Text = lang.GetString("language_select");
            _LanguageButton.ToolTipText = lang.GetString("language_select_tooltip");
            _DonateButton.Text = lang.GetString("toolbar_donate");
            _DonateButton.ToolTipText = lang.GetString("toolbar_donate_tooltip");
            _SocialsButton.Text = lang.GetString("toolbar_socials");
            _SocialsButton.ToolTipText = lang.GetString("toolbar_socials_tooltip");
            _DisclaimerButton.Text = lang.GetString("toolbar_disclaimer");
            _DisclaimerButton.ToolTipText = lang.GetString("toolbar_disclaimer_tooltip");
            
            // Update section headers
            _SelectedHeaderLabel.Text = lang.GetString("section_selected");
            _DoneHeaderLabel.Text = lang.GetString("section_done");
            _OtherGamesHeaderLabel.Text = lang.GetString("section_other_games");
            
            // Update context menu items
            _ToggleSelectionMenuItem.Text = lang.GetString("context_toggle_selection");
            _LaunchThisOnlyMenuItem.Text = lang.GetString("context_launch");
            _LaunchOneRandomMenuItem.Text = lang.GetString("context_launch_random");
            _RemoveFromSelectedMenuItem.Text = lang.GetString("context_remove_selected");
            _ClearAllSelectionsMenuItem.Text = lang.GetString("context_clear_all_selections");
            _MarkAsDoneMenuItem.Text = lang.GetString("context_mark_done");
            _ClearDoneMenuItem.Text = lang.GetString("toolbar_clear_done");
            _ResetAchievementsMenuItem.Text = lang.GetString("context_reset");
            
            // Force UI refresh
            this.Refresh();
        }

        private void OnSocialsClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://x.hegxib.me");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Failed to open socials page: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnDisclaimerClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "HxB SAM Enhanced 1.0\n\n" +
                "This is a modified version of Steam Achievement Manager (SAM).\n\n" +
                "DISCLAIMER:\n" +
                "• This tool modifies Steam achievement data\n" +
                "• Use at your own risk\n" +
                "• The developers are not responsible for any consequences\n" +
                "• This may violate Steam's Terms of Service\n" +
                "• Use responsibly and ethically\n\n" +
                "Original SAM by Rick\n" +
                "Enhanced by HxB",
                "Disclaimer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnRemoveFromSelected(object sender, EventArgs e)
        {
            var clickedListView = (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView != this._SelectedListView) return;
            
            var hitTest = clickedListView.HitTest(clickedListView.PointToClient(Cursor.Position));
            if (hitTest.Item == null) return;
            
            int index = hitTest.Item.Index;
            if (index >= 0 && index < this._SelectedGames.Count)
            {
                var game = this._SelectedGames[index];
                this._SelectedGameIds.Remove(game.Id);
                this.RefreshGames();
            }
        }

        private void OnResetAchievements(object sender, EventArgs e)
        {
            var clickedListView = this._ContextListView ?? (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView == null) return;
            int index = this._ContextIndex;
            if (index < 0) return;
            GameInfo gameToReset = null;
            
            // Get _DoneListView via reflection
            var doneListView = this.GetType().GetField("_DoneListView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
            
            if (clickedListView == this._SelectedListView && index >= 0 && index < this._SelectedGames.Count)
            {
                gameToReset = this._SelectedGames[index];
            }
            else if (doneListView != null && clickedListView == doneListView && index >= 0 && index < this._DoneGames.Count)
            {
                gameToReset = this._DoneGames[index];
            }
            else if (clickedListView == this._GameListView && index >= 0 && index < this._FilteredGames.Count)
            {
                gameToReset = this._FilteredGames[index];
            }
            
            if (gameToReset != null)
            {
                // Confirmation dialog
                var result = MessageBox.Show(
                    this,
                    $"Are you sure you want to reset all achievements for:\n\n{gameToReset.Name}\n\nThis will lock all currently unlocked achievements.",
                    "⚠️ Reset Achievements",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2); // Default to No
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var gamePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                        // Launch with reset flag - SAM.Game should handle this
                        Process.Start(gamePath, $"{gameToReset.Id} -reset");
                    }
                    catch (Win32Exception)
                    {
                        MessageBox.Show(
                            this,
                            $"Failed to launch reset for: {gameToReset.Name}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OnMarkAsDone(object sender, EventArgs e)
        {
            var clickedListView = this._ContextListView ?? (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView == null) return;
            int index = this._ContextIndex;
            if (index < 0) return;
            
            // Get _DoneListView via reflection
            var doneListView = this.GetType().GetField("_DoneListView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
            
            GameInfo gameToMark = null;
            bool isInDoneSection = false;
            
            if (clickedListView == this._SelectedListView && index >= 0 && index < this._SelectedGames.Count)
            {
                gameToMark = this._SelectedGames[index];
            }
            else if (doneListView != null && clickedListView == doneListView && index >= 0 && index < this._DoneGames.Count)
            {
                gameToMark = this._DoneGames[index];
                isInDoneSection = true;
            }
            else if (clickedListView == this._GameListView && index >= 0 && index < this._FilteredGames.Count)
            {
                gameToMark = this._FilteredGames[index];
            }
            
            if (gameToMark != null)
            {
                if (isInDoneSection)
                {
                    // Unmark as Done - remove from DONE section
                    if (this._DoneGameIds.Contains(gameToMark.Id))
                    {
                        this._DoneGameIds.Remove(gameToMark.Id);
                        this._DoneGames.RemoveAll(g => g.Id == gameToMark.Id);
                    }
                }
                else
                {
                    // Mark as Done - add to DONE section
                    // Remove from SELECTED if present
                    if (this._SelectedGameIds.Contains(gameToMark.Id))
                    {
                        this._SelectedGameIds.Remove(gameToMark.Id);
                        this._SelectedGames.RemoveAll(g => g.Id == gameToMark.Id);
                    }
                    
                    // Add to DONE
                    if (!this._DoneGameIds.Contains(gameToMark.Id))
                    {
                        this._DoneGameIds.Add(gameToMark.Id);
                        this._DoneGames.Add(gameToMark);
                        // Save logo and assets to DONE cache when adding
                        SaveLogoToSectionCache(gameToMark, this._DoneCacheDirectory);
                        // Download additional assets in background
                        System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameToMark, this._DoneCacheDirectory));
                    }
                }
                
                // Persist changes
                this.SaveSelectedGames();
                this.SaveDoneGames();
                this.RefreshGames();
            }
        }

        private void DoDownloadLogo(object sender, DoWorkEventArgs e)
        {
            var info = (GameInfo)e.Argument;

            this._LogosAttempted.Add(info.ImageUrl);

            // Check caches in order: main cache, then section-specific caches
            string cacheFileName = GetCacheFileName(info.Id, info.ImageUrl);
            string[] cachePaths = new[]
            {
                Path.Combine(this._CacheDirectory, cacheFileName),
                Path.Combine(this._SelectedCacheDirectory, cacheFileName),
                Path.Combine(this._DoneCacheDirectory, cacheFileName)
            };

            try
            {
                // Try loading from any cache
                foreach (var cachePath in cachePaths)
                {
                    if (File.Exists(cachePath))
                    {
                        using (var stream = new FileStream(cachePath, FileMode.Open, FileAccess.Read))
                        {
                            Bitmap bitmap = new(stream);
                            e.Result = new LogoInfo(info.Id, bitmap);
                            
                            // Copy to main cache if not already there
                            string mainCachePath = cachePaths[0];
                            if (cachePath != mainCachePath && !File.Exists(mainCachePath))
                            {
                                try
                                {
                                    File.Copy(cachePath, mainCachePath, false);
                                }
                                catch
                                {
                                    // Ignore copy errors
                                }
                            }
                            
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Cache read failed, download fresh
            }

            // Download and cache
            using (WebClient downloader = new())
            {
                try
                {
                    var data = downloader.DownloadData(new Uri(info.ImageUrl));
                    
                    // Save to main cache
                    string mainCachePath = cachePaths[0];
                    try
                    {
                        File.WriteAllBytes(mainCachePath, data);
                    }
                    catch
                    {
                        // Cache write failed, continue anyway
                    }

                    using (MemoryStream stream = new(data, false))
                    {
                        Bitmap bitmap = new(stream);
                        e.Result = new LogoInfo(info.Id, bitmap);
                    }
                }
                catch (Exception)
                {
                    e.Result = new LogoInfo(info.Id, null);
                }
            }
        }

        private string GetCacheFileName(uint gameId, string imageUrl)
        {
            // Create a safe filename from game ID and URL hash
            string urlHash = imageUrl.GetHashCode().ToString("X8");
            return $"{gameId}_{urlHash}.png";
        }

        private void SaveLogoToSectionCache(GameInfo gameInfo, string sectionCacheDirectory)
        {
            if (gameInfo == null)
                return;

            // Ensure ImageUrl is set
            if (string.IsNullOrEmpty(gameInfo.ImageUrl))
            {
                gameInfo.ImageUrl = GetGameImageUrl(gameInfo.Id);
            }

            if (string.IsNullOrEmpty(gameInfo.ImageUrl))
                return;

            try
            {
                string cacheFileName = GetCacheFileName(gameInfo.Id, gameInfo.ImageUrl);
                string mainCachePath = Path.Combine(this._CacheDirectory, cacheFileName);
                string sectionCachePath = Path.Combine(sectionCacheDirectory, cacheFileName);

                // If section cache already has it, we're done
                if (File.Exists(sectionCachePath))
                {
                    return;
                }

                // Try to copy from main cache first
                if (File.Exists(mainCachePath))
                {
                    File.Copy(mainCachePath, sectionCachePath, false);
                    // Also copy to the other cache if not there
                    File.Copy(mainCachePath, Path.Combine(this._CacheDirectory, cacheFileName), true);
                    return;
                }

                // If image is already loaded in ImageList, save it
                if (gameInfo.ImageIndex > 0 && gameInfo.ImageIndex < this._LogoImageList.Images.Count)
                {
                    var image = this._LogoImageList.Images[gameInfo.ImageIndex];
                    if (image != null)
                    {
                        image.Save(sectionCachePath, System.Drawing.Imaging.ImageFormat.Png);
                        image.Save(mainCachePath, System.Drawing.Imaging.ImageFormat.Png);
                        return;
                    }
                }

                // Download and save to both caches
                using (WebClient downloader = new())
                {
                    try
                    {
                        var data = downloader.DownloadData(new Uri(gameInfo.ImageUrl));
                        
                        // Save to section cache
                        File.WriteAllBytes(sectionCachePath, data);
                        
                        // Save to main cache
                        File.WriteAllBytes(mainCachePath, data);
                        
                        // Load into ImageList if not already there
                        if (gameInfo.ImageIndex <= 0)
                        {
                            using (MemoryStream stream = new(data, false))
                            {
                                Bitmap bitmap = new(stream);
                                int imageIndex = this._LogoImageList.Images.Count;
                                this._LogoImageList.Images.Add(gameInfo.ImageUrl, bitmap);
                                gameInfo.ImageIndex = imageIndex;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to download logo for game {gameInfo.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cache logo for game {gameInfo.Id}: {ex.Message}");
            }
        }

        private void DownloadGameAssets(GameInfo gameInfo, string sectionCacheDirectory)
        {
            if (gameInfo == null)
                return;

            // Download additional Steam assets for the game
            var assetUrls = new Dictionary<string, string>
            {
                // Header image (460x215)
                ["header"] = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameInfo.Id}/header.jpg",
                // Capsule image (231x87)
                ["capsule_231x87"] = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameInfo.Id}/capsule_231x87.jpg",
                // Library capsule (600x900)
                ["library_600x900"] = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameInfo.Id}/library_600x900.jpg",
                // Library hero (3840x1240)
                ["library_hero"] = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameInfo.Id}/library_hero.jpg",
                // Library logo
                ["logo"] = $"https://cdn.cloudflare.steamstatic.com/steam/apps/{gameInfo.Id}/logo.png"
            };

            using (WebClient downloader = new())
            {
                foreach (var asset in assetUrls)
                {
                    try
                    {
                        string assetFileName = $"{gameInfo.Id}_{asset.Key}.{(asset.Value.EndsWith(".png") ? "png" : "jpg")}";
                        string assetPath = Path.Combine(sectionCacheDirectory, assetFileName);

                        // Skip if already cached
                        if (File.Exists(assetPath))
                            continue;

                        // Download and save
                        var data = downloader.DownloadData(new Uri(asset.Value));
                        File.WriteAllBytes(assetPath, data);
                        
                        System.Diagnostics.Debug.WriteLine($"Downloaded {asset.Key} for game {gameInfo.Id}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to download {asset.Key} for game {gameInfo.Id}: {ex.Message}");
                        // Continue with other assets even if one fails
                    }
                }
            }
        }

        private void OnDownloadLogo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true)
            {
                return;
            }

            if (e.Result is LogoInfo logoInfo &&
                logoInfo.Bitmap != null &&
                this._Games.TryGetValue(logoInfo.Id, out var gameInfo) == true)
            {
                this._GameListView.BeginUpdate();
                var imageIndex = this._LogoImageList.Images.Count;
                this._LogoImageList.Images.Add(gameInfo.ImageUrl, logoInfo.Bitmap);
                gameInfo.ImageIndex = imageIndex;
                this._GameListView.EndUpdate();
            }

            this.DownloadNextLogo();
        }

        private void DownloadNextLogo()
        {
            lock (this._LogoLock)
            {

                if (this._LogoWorker.IsBusy == true)
                {
                    return;
                }

                GameInfo info;
                while (true)
                {
                    if (this._LogoQueue.TryDequeue(out info) == false)
                    {
                        this._DownloadStatusLabel.Visible = false;
                        return;
                    }

                    if (info.Item == null)
                    {
                        continue;
                    }

                    if (this._FilteredGames.Contains(info) == false ||
                        info.Item.Bounds.IntersectsWith(this._GameListView.ClientRectangle) == false)
                    {
                        this._LogosAttempting.Remove(info.ImageUrl);
                        continue;
                    }

                    break;
                }

                this._DownloadStatusLabel.Text = $"Downloading {1 + this._LogoQueue.Count} game icons...";
                this._DownloadStatusLabel.Visible = true;

                this._LogoWorker.RunWorkerAsync(info);
            }
        }

        private string GetGameImageUrl(uint id)
        {
            string candidate;

            var currentLanguage = this._SteamClient.SteamApps008.GetCurrentGameLanguage();

            candidate = this._SteamClient.SteamApps001.GetAppData(id, _($"small_capsule/{currentLanguage}"));
            if (string.IsNullOrEmpty(candidate) == false)
            {
                return _($"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{id}/{candidate}");
            }

            if (currentLanguage != "english")
            {
                candidate = this._SteamClient.SteamApps001.GetAppData(id, "small_capsule/english");
                if (string.IsNullOrEmpty(candidate) == false)
                {
                    return _($"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{id}/{candidate}");
                }
            }

            candidate = this._SteamClient.SteamApps001.GetAppData(id, "logo");
            if (string.IsNullOrEmpty(candidate) == false)
            {
                return _($"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{id}/{candidate}.jpg");
            }

            return null;
        }

        private void AddGameToLogoQueue(GameInfo info)
        {
            if (info.ImageIndex > 0)
            {
                return;
            }

            var imageUrl = GetGameImageUrl(info.Id);
            if (string.IsNullOrEmpty(imageUrl) == true)
            {
                return;
            }

            info.ImageUrl = imageUrl;

            int imageIndex = this._LogoImageList.Images.IndexOfKey(imageUrl);
            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else if (
                this._LogosAttempting.Contains(imageUrl) == false &&
                this._LogosAttempted.Contains(imageUrl) == false)
            {
                this._LogosAttempting.Add(imageUrl);
                this._LogoQueue.Enqueue(info);
            }
        }

        private bool OwnsGame(uint id)
        {
            return this._SteamClient.SteamApps008.IsSubscribedApp(id);
        }

        private void AddGame(uint id, string type)
        {
            if (this._Games.ContainsKey(id) == true)
            {
                return;
            }

            if (this.OwnsGame(id) == false)
            {
                return;
            }

            GameInfo info = new(id, type);
            info.Name = this._SteamClient.SteamApps001.GetAppData(info.Id, "name");
            
            // Get playtime from Steam's local cache (in minutes)
            info.PlaytimeForever = API.PlaytimeReader.GetPlaytime(info.Id);
            
            this._Games.Add(id, info);
        }

        private void AddGames()
        {
            this._Games.Clear();
            this._RefreshGamesButton.Enabled = false;
            this._ListWorker.RunWorkerAsync();
        }

        private void AddDefaultGames()
        {
            this.AddGame(480, "normal"); // Spacewar
        }

        private void OnTimer(object sender, EventArgs e)
        {
            this._CallbackTimer.Enabled = false;
            this._SteamClient.RunCallbacks(false);
            this._CallbackTimer.Enabled = true;
        }

        private void OnActivateGame(object sender, EventArgs e)
        {
            var listView = sender as MyListView;
            if (listView == null) return;
            LaunchSelectedGames(listView);
        }

        private void LaunchSelectedGames(MyListView listView)
        {
            if (listView.SelectedIndices.Count == 0) return;

            // Get _DoneListView and _DoneGames via reflection for DONE section
            var doneListView = this.GetType().GetField("_DoneListView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;
            var doneGames = this.GetType().GetField("_DoneGames",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as List<GameInfo>;

            foreach (int index in listView.SelectedIndices)
            {
                GameInfo gameInfo = null;
                if (listView == this._SelectedListView && index >= 0 && index < this._SelectedGames.Count)
                {
                    gameInfo = this._SelectedGames[index];
                }
                else if (listView == this._GameListView && index >= 0 && index < this._FilteredGames.Count)
                {
                    gameInfo = this._FilteredGames[index];
                }
                else if (listView == doneListView && doneGames != null && index >= 0 && index < doneGames.Count)
                {
                    gameInfo = doneGames[index];
                }

                if (gameInfo != null)
                {
                    LaunchGame(gameInfo);
                }
            }
        }

        private void LaunchGame(GameInfo gameInfo)
        {
            try
            {
                var exePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                if (!File.Exists(exePath))
                {
                    var lang = Localization.LanguageManager.Instance;
                    MessageBox.Show(this,
                        "SAM.Game.exe not found. Please ensure all files are extracted correctly.",
                        lang.GetString("error_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        Arguments = gameInfo.Id.ToString(CultureInfo.InvariantCulture),
                        UseShellExecute = true,
                        WorkingDirectory = Application.StartupPath
                    },
                    EnableRaisingEvents = true
                };

                uint gameId = gameInfo.Id;
                process.Exited += (s, args) =>
                {
                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new Action(() => OnGameProcessExited(gameId)));
                    }
                };

                process.Start();
            }
            catch (Exception ex)
            {
                var lang = Localization.LanguageManager.Instance;
                MessageBox.Show(this,
                    $"Failed to launch achievement manager for {gameInfo.Name}:\n{ex.Message}",
                    lang.GetString("error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this._AddGameTextBox.Text = "";
            this.AddGames();
        }

        private void OnAddGame(object sender, EventArgs e)
        {
            uint id;

            if (uint.TryParse(this._AddGameTextBox.Text, out id) == false)
            {
                MessageBox.Show(
                    this,
                    "Please enter a valid game ID.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (this.OwnsGame(id) == false)
            {
                MessageBox.Show(this, "You don't own that game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (this._LogoQueue.TryDequeue(out var logo) == true)
            {
                // clear the download queue because we will be showing only one app
                this._LogosAttempted.Remove(logo.ImageUrl);
            }

            this._AddGameTextBox.Text = "";
            this._Games.Clear();
            this.AddGame(id, "normal");
            this._FilterGamesMenuItem.Checked = true;
            this.RefreshGames();
            this.DownloadNextLogo();
        }

        private void OnFilterUpdate(object sender, EventArgs e)
        {
            this.RefreshGames();

            // Compatibility with _GameListView SearchForVirtualItemEventHandler (otherwise _SearchGameTextBox loose focus on KeyUp)
            this._SearchGameTextBox.Focus();
        }

        private void OnSelectAll(object sender, EventArgs e)
        {
            // Add all filtered games to selection
            foreach (var game in this._FilteredGames)
            {
                this._SelectedGameIds.Add(game.Id);
                // Save logo and assets to SELECTED cache when adding
                SaveLogoToSectionCache(game, this._SelectedCacheDirectory);
                // Download additional assets in background
                var gameToCache = game; // Capture for async
                System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameToCache, this._SelectedCacheDirectory));
            }
            
            // Rebuild the lists
            this._SelectedGames.Clear();
            this._SelectedGames.AddRange(this._Games.Values.Where(g => this._SelectedGameIds.Contains(g.Id)));
            
            this._FilteredGames.Clear();
            this._FilteredGames.AddRange(this._Games.Values.Where(g => !this._SelectedGameIds.Contains(g.Id)));
            
            // Update both ListViews
            this._SelectedListView.VirtualListSize = this._SelectedGames.Count;
            this._GameListView.VirtualListSize = this._FilteredGames.Count;
            
            // Show/hide selected section
            bool hasSelected = this._SelectedGames.Count > 0;
            this._SelectedHeaderPanel.Visible = hasSelected;
            this._SelectedListView.Visible = hasSelected;
            
            // Update headers
            this._SelectedHeaderLabel.Text = $"▼ SELECTED ({this._SelectedGames.Count})";
            this._OtherGamesHeaderLabel.Text = $"▼ OTHER GAMES ({this._FilteredGames.Count})";
        }

        private void OnClearAll(object sender, EventArgs e)
        {
            // Clear all selections
            this._SelectedGameIds.Clear();
            this._SelectedGames.Clear();
            
            // Rebuild filtered games from all games
            this._FilteredGames.Clear();
            this._FilteredGames.AddRange(this._Games.Values);
            
            // Update both ListViews
            this._SelectedListView.VirtualListSize = 0;
            this._GameListView.VirtualListSize = this._FilteredGames.Count;
            
            // Hide selected section
            this._SelectedHeaderPanel.Visible = false;
            this._SelectedListView.Visible = false;
            
            // Update headers
            this._SelectedHeaderLabel.Text = "▼ SELECTED (0)";
            this._OtherGamesHeaderLabel.Text = $"▼ OTHER GAMES ({this._FilteredGames.Count})";
            
            // Re-apply current filter
            this.RefreshGames();
        }

        private void OnClearDone(object sender, EventArgs e)
        {
            // Check if there are any DONE games
            if (this._DoneGameIds.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "The DONE section is already empty.",
                    "No DONE Games",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Show warning dialog
            var result = MessageBox.Show(
                this,
                $"⚠️ WARNING: This will remove ALL {this._DoneGameIds.Count} games from the DONE section!\n\n" +
                "These games will be moved back to the OTHERS section.\n\n" +
                "This action cannot be undone automatically, but you can manually mark games as DONE again later.\n\n" +
                "Are you absolutely sure you want to proceed?",
                "Clear DONE Section - Confirmation Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2); // Default to No

            if (result == DialogResult.Yes)
            {
                // Clear DONE games
                this._DoneGameIds.Clear();
                this._DoneGames.Clear();
                
                // Save changes
                this.SaveDoneGames();
                
                // Refresh display
                this.RefreshGames();
                
                MessageBox.Show(
                    this,
                    "DONE section has been cleared successfully.\n\nAll games have been moved back to the OTHERS section.",
                    "DONE Section Cleared",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void OnGameListViewDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var listView = sender as MyListView;
            if (listView == null)
            {
                return;
            }

            if (e.Item.Bounds.IntersectsWith(listView.ClientRectangle) == false)
            {
                return;
            }

            // Get _DoneListView via reflection
            var doneListView = this.GetType().GetField("_DoneListView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as MyListView;

            // Determine which list and get the correct game info
            GameInfo info = null;
            if (listView == this._SelectedListView && e.ItemIndex >= 0 && e.ItemIndex < this._SelectedGames.Count)
            {
                info = this._SelectedGames[e.ItemIndex];
            }
            else if (doneListView != null && listView == doneListView && e.ItemIndex >= 0 && e.ItemIndex < this._DoneGames.Count)
            {
                info = this._DoneGames[e.ItemIndex];
            }
            else if (listView == this._GameListView && e.ItemIndex >= 0 && e.ItemIndex < this._FilteredGames.Count)
            {
                info = this._FilteredGames[e.ItemIndex];
            }

            // Queue logo download if not yet loaded
            if (info != null && info.ImageIndex <= 0)
            {
                this.AddGameToLogoQueue(info);
                this.DownloadNextLogo();
            }

            // Draw with default behavior
            e.DrawDefault = true;
        }

        private void OnGameListViewClick(object sender, MouseEventArgs e)
        {
            // Right-click is solely for context menu, no selection toggle
            if (e.Button == MouseButtons.Right)
            {
                return; // Context menu will handle this
            }
            
            // Get the ListView that was clicked
            var listView = sender as MyListView;
            if (listView == null) return;
            
            // Left-click with modifiers or plain left-click
            ListViewHitTestInfo hit = listView.HitTest(e.Location);
            if (hit.Item != null)
            {
                int itemIndex = hit.Item.Index;
                
                // Determine which list was clicked and get the game
                GameInfo gameInfo = null;
                if (listView == this._SelectedListView && itemIndex >= 0 && itemIndex < this._SelectedGames.Count)
                {
                    gameInfo = this._SelectedGames[itemIndex];
                }
                else if (listView == this._GameListView && itemIndex >= 0 && itemIndex < this._FilteredGames.Count)
                {
                    gameInfo = this._FilteredGames[itemIndex];
                }
                
                if (gameInfo == null) return;
                
                // Alt+Left-Click: Move to DONE section
                if (e.Button == MouseButtons.Left && Control.ModifierKeys.HasFlag(Keys.Alt))
                {
                    // Remove from SELECTED if present
                    if (this._SelectedGameIds.Contains(gameInfo.Id))
                    {
                        this._SelectedGameIds.Remove(gameInfo.Id);
                        this._SelectedGames.RemoveAll(g => g.Id == gameInfo.Id);
                    }
                    
                    // Add to DONE
                    if (!this._DoneGameIds.Contains(gameInfo.Id))
                    {
                        this._DoneGameIds.Add(gameInfo.Id);
                        this._DoneGames.Add(gameInfo);
                        // Save logo and assets to DONE cache
                        SaveLogoToSectionCache(gameInfo, this._DoneCacheDirectory);
                        // Download additional assets in background
                        var gameToCache = gameInfo;
                        System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameToCache, this._DoneCacheDirectory));
                    }
                    
                    // Save and refresh
                    this.SaveSelectedGames();
                    this.SaveDoneGames();
                    this.RefreshGames();
                    return;
                }
                
                // Ctrl+Left-Click: Toggle SELECTED section
                if (e.Button == MouseButtons.Left && Control.ModifierKeys.HasFlag(Keys.Control))
                {
                    // If already in SELECTED, remove it
                    if (this._SelectedGameIds.Contains(gameInfo.Id))
                    {
                        this._SelectedGameIds.Remove(gameInfo.Id);
                        this._SelectedGames.RemoveAll(g => g.Id == gameInfo.Id);
                    }
                    else
                    {
                        // Remove from DONE if present
                        if (this._DoneGameIds.Contains(gameInfo.Id))
                        {
                            this._DoneGameIds.Remove(gameInfo.Id);
                            this._DoneGames.RemoveAll(g => g.Id == gameInfo.Id);
                        }
                        
                        // Add to SELECTED
                        this._SelectedGameIds.Add(gameInfo.Id);
                        // Save logo and assets to SELECTED cache
                        SaveLogoToSectionCache(gameInfo, this._SelectedCacheDirectory);
                        // Download additional assets in background
                        var gameToCache = gameInfo;
                        System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameToCache, this._SelectedCacheDirectory));
                    }
                    
                    // Save and refresh
                    this.SaveSelectedGames();
                    this.SaveDoneGames();
                    this.RefreshGames();
                    return;
                }
                
                // Plain Left-Click: Toggle selection (existing behavior)
                // RULE: Games in DONE cannot be moved to OTHERS via left-click
                // They can only be removed from DONE via context menu or Alt+Click
                if (this._DoneGameIds.Contains(gameInfo.Id))
                {
                    MessageBox.Show(
                        this,
                        $"This game is marked as DONE (100% achievements).\n\nTo move it to another section, use Alt+Click or right-click menu options.",
                        "Game is DONE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                
                // Toggle selection status
                if (this._SelectedGameIds.Contains(gameInfo.Id))
                {
                    this._SelectedGameIds.Remove(gameInfo.Id);
                }
                else
                {
                    this._SelectedGameIds.Add(gameInfo.Id);
                    // Save logo and assets to SELECTED cache when adding
                    SaveLogoToSectionCache(gameInfo, this._SelectedCacheDirectory);
                    // Download additional assets in background
                    var gameToCache = gameInfo;
                    System.Threading.Tasks.Task.Run(() => DownloadGameAssets(gameToCache, this._SelectedCacheDirectory));
                }
                
                // Rebuild both lists from ALL games
                var selectedGames = new List<GameInfo>();
                var unselectedGames = new List<GameInfo>();

                foreach (var info in this._Games.Values)
                {
                    if (this._SelectedGameIds.Contains(info.Id))
                    {
                        selectedGames.Add(info);
                    }
                    else
                    {
                        unselectedGames.Add(info);
                    }
                }

                    // Populate SELECTED ListView
                    this._SelectedGames.Clear();
                    this._SelectedGames.AddRange(selectedGames);
                    this._SelectedListView.VirtualListSize = this._SelectedGames.Count;
                    
                    // Populate OTHER GAMES ListView
                    this._FilteredGames.Clear();
                    this._FilteredGames.AddRange(unselectedGames);
                    this._GameListView.VirtualListSize = this._FilteredGames.Count;

                    // Update header labels and visibility
                    this._SelectedHeaderLabel.Text = $"▼ SELECTED ({selectedGames.Count})";
                    this._OtherGamesHeaderLabel.Text = $"▼ OTHER GAMES ({unselectedGames.Count})";
                    
                    // Show SELECTED section only when there are selected games
                    this._SelectedHeaderPanel.Visible = selectedGames.Count > 0;
                    this._SelectedListView.Visible = selectedGames.Count > 0;
                    
                    // OTHER GAMES section is always visible
                    this._OtherGamesHeaderPanel.Visible = true;

                // Update status
                if (selectedGames.Count > 0)
                {
                    this._PickerStatusLabel.Text =
                        $"SELECTED: {selectedGames.Count} | Other: {unselectedGames.Count} | Total: {this._Games.Count} games";
                }
                else
                {
                    this._PickerStatusLabel.Text =
                        $"Displaying {this._FilteredGames.Count} games. Total {this._Games.Count} games.";
                }

                this._SelectedListView.Invalidate();
                this._GameListView.Invalidate();
                
                // Save selection after any change
                this.SaveSelectedGames();
            }
        }

        private string GetSelectionFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var samPath = Path.Combine(appDataPath, "SAM");
            if (!Directory.Exists(samPath))
            {
                Directory.CreateDirectory(samPath);
            }
            return Path.Combine(samPath, "selected_games.txt");
        }

        private void LoadSelectedGames()
        {
            try
            {
                var filePath = this.GetSelectionFilePath();
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (uint.TryParse(line.Trim(), out var gameId))
                        {
                            this._SelectedGameIds.Add(gameId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load selected games: {ex.Message}");
            }
        }

        private void SaveSelectedGames()
        {
            try
            {
                var filePath = this.GetSelectionFilePath();
                var gameIds = this._SelectedGameIds.Select(id => id.ToString()).ToArray();
                File.WriteAllLines(filePath, gameIds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save selected games: {ex.Message}");
            }
        }

        private string GetDoneFilePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var samPath = Path.Combine(appDataPath, "SAM");
            if (!Directory.Exists(samPath))
            {
                Directory.CreateDirectory(samPath);
            }
            return Path.Combine(samPath, "done_games.txt");
        }

        private void LoadDoneGames()
        {
            try
            {
                var filePath = this.GetDoneFilePath();
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (uint.TryParse(line.Trim(), out var gameId))
                        {
                            this._DoneGameIds.Add(gameId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load done games: {ex.Message}");
            }
        }

        private void SaveDoneGames()
        {
            try
            {
                var filePath = this.GetDoneFilePath();
                var gameIds = this._DoneGameIds.Select(id => id.ToString()).ToArray();
                File.WriteAllLines(filePath, gameIds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save done games: {ex.Message}");
            }
        }
    }
}
