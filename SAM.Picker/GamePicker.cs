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

        public GamePicker(API.Client client)
        {
            this._Games = new();
            this._FilteredGames = new();
            this._SelectedGames = new();
            this._LogoLock = new();
            this._LogosAttempting = new();
            this._LogosAttempted = new();
            this._LogoQueue = new();
            this._SelectedGameIds = new();

            this.InitializeComponent();

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
            
            // Separate selected and unselected games, then combine with selected first
            var selectedGames = new List<GameInfo>();
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

                if (info.IsSelected)
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
            
            // Update status label with section information
            if (selectedGames.Count > 0)
            {
                this._PickerStatusLabel.Text =
                    $"SELECTED: {selectedGames.Count} | Other: {unselectedGames.Count} | Total: {this._Games.Count} games";
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
                ImageIndex = info.ImageIndex,
                BackColor = Color.FromArgb(30, 50, 30),
                ForeColor = Color.Yellow,
            };
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
                    int index = hitTest.Item.Index;
                    bool isInSelectedList = (clickedListView == this._SelectedListView);
                    
                    // Update menu text dynamically
                    this._ToggleSelectionMenuItem.Text = isInSelectedList ? "Remove from SELECTED" : "Add to SELECTED";
                    this._RemoveFromSelectedMenuItem.Visible = isInSelectedList;
                    this._LaunchThisOnlyMenuItem.Visible = true;
                }
            }
        }

        private void OnLaunchThisOnly(object sender, EventArgs e)
        {
            var clickedListView = (this._GameContextMenu.SourceControl as MyListView);
            if (clickedListView == null) return;
            
            var hitTest = clickedListView.HitTest(clickedListView.PointToClient(Cursor.Position));
            if (hitTest.Item == null) return;
            
            int index = hitTest.Item.Index;
            GameInfo gameToLaunch = null;
            
            if (clickedListView == this._SelectedListView && index >= 0 && index < this._SelectedGames.Count)
            {
                gameToLaunch = this._SelectedGames[index];
            }
            else if (clickedListView == this._GameListView && index >= 0 && index < this._FilteredGames.Count)
            {
                gameToLaunch = this._FilteredGames[index];
            }
            
            if (gameToLaunch != null)
            {
                try
                {
                    var gamePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                    Process.Start(gamePath, gameToLaunch.Id.ToString(CultureInfo.InvariantCulture));
                }
                catch (Win32Exception)
                {
                    MessageBox.Show(
                        this,
                        $"Failed to launch game: {gameToLaunch.Name}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void OnLaunchOneRandom(object sender, EventArgs e)
        {
            if (this._SelectedGameIds.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "No games selected. Please add games to the SELECTED list first.",
                    "No Games Selected",
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
                try
                {
                    var gamePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                    Process.Start(gamePath, gameInfo.Id.ToString(CultureInfo.InvariantCulture));
                }
                catch (Win32Exception)
                {
                    MessageBox.Show(
                        this,
                        $"Failed to launch game: {gameInfo.Name}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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

        private void DoDownloadLogo(object sender, DoWorkEventArgs e)
        {
            var info = (GameInfo)e.Argument;

            this._LogosAttempted.Add(info.ImageUrl);

            using (WebClient downloader = new())
            {
                try
                {
                    var data = downloader.DownloadData(new Uri(info.ImageUrl));
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
            // Launch ALL games that are in the SELECTED section (_SelectedGameIds)
            if (this._SelectedGameIds.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "No games selected. Click games to add them to the SELECTED section first.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Show launch options dialog
            using (var optionsDialog = new LaunchOptionsDialog())
            {
                if (optionsDialog.ShowDialog(this) != DialogResult.OK)
                {
                    return; // User cancelled
                }

                // Get selected games
                var gamesToLaunch = new List<GameInfo>();
                foreach (var gameId in this._SelectedGameIds)
                {
                    if (this._Games.TryGetValue(gameId, out var gameInfo))
                    {
                        gamesToLaunch.Add(gameInfo);
                    }
                }

                if (optionsDialog.UseQueue)
                {
                    // Use launch queue with progress dialog
                    using (var queueDialog = new LaunchQueueDialog(gamesToLaunch, optionsDialog.DelaySeconds))
                    {
                        queueDialog.ShowDialog(this);
                    }
                }
                else
                {
                    // Launch all immediately (original behavior)
                    int successCount = 0;
                    int failCount = 0;
                    
                    foreach (var gameInfo in gamesToLaunch)
                    {
                        try
                        {
                            var exePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                            Process.Start(exePath, gameInfo.Id.ToString(CultureInfo.InvariantCulture));
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

                    if (failCount > 0)
                    {
                        MessageBox.Show(
                            this,
                            $"Successfully launched {successCount} game(s).\nFailed to launch {failCount} game(s).",
                            failCount == this._SelectedGameIds.Count ? "Error" : "Warning",
                            MessageBoxButtons.OK,
                            failCount == this._SelectedGameIds.Count ? MessageBoxIcon.Error : MessageBoxIcon.Warning);
                    }
                    else if (successCount > 1)
                    {
                        MessageBox.Show(
                            this,
                            $"Successfully launched {successCount} games.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
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

        private void OnGameListViewDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if (e.Item.Bounds.IntersectsWith(this._GameListView.ClientRectangle) == false)
            {
                return;
            }

            var info = this._FilteredGames[e.ItemIndex];
            if (info.ImageIndex <= 0)
            {
                this.AddGameToLogoQueue(info);
                this.DownloadNextLogo();
            }

            // Draw with default behavior
            e.DrawDefault = true;
        }

        private void OnGameListViewClick(object sender, MouseEventArgs e)
        {
            // Get the ListView that was clicked
            var listView = sender as MyListView;
            if (listView == null) return;
            
            // Single click toggles selection status
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
                
                // Toggle selection status
                if (this._SelectedGameIds.Contains(gameInfo.Id))
                {
                    this._SelectedGameIds.Remove(gameInfo.Id);
                }
                else
                {
                    this._SelectedGameIds.Add(gameInfo.Id);
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
    }
}
