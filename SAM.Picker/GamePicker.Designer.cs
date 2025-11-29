namespace SAM.Picker
{
    partial class GamePicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GamePicker));
            this._LogoImageList = new System.Windows.Forms.ImageList(this.components);
            this._CallbackTimer = new System.Windows.Forms.Timer(this.components);
            this._PickerToolStrip = new System.Windows.Forms.ToolStrip();
            this._RefreshGamesButton = new System.Windows.Forms.ToolStripButton();
            this._AddGameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this._AddGameButton = new System.Windows.Forms.ToolStripButton();
            this._FindGamesLabel = new System.Windows.Forms.ToolStripLabel();
            this._SearchGameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this._FilterDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this._FilterGamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._FilterDemosMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._FilterModsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._FilterJunkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._DonateButton = new System.Windows.Forms.ToolStripButton();
            this._SocialsButton = new System.Windows.Forms.ToolStripButton();
            this._DisclaimerButton = new System.Windows.Forms.ToolStripButton();
            this._GameListView = new SAM.Picker.MyListView();
            this._SelectedListView = new SAM.Picker.MyListView();
            this._PickerStatusStrip = new System.Windows.Forms.StatusStrip();
            this._PickerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._DownloadStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._LogoWorker = new System.ComponentModel.BackgroundWorker();
            this._ListWorker = new System.ComponentModel.BackgroundWorker();
            this._GameContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._ToggleSelectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._LaunchThisOnlyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._LaunchOneRandomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._RemoveFromSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ClearAllSelectionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._SelectAllButton = new System.Windows.Forms.ToolStripButton();
            this._ClearAllButton = new System.Windows.Forms.ToolStripButton();
            this._SelectedHeaderPanel = new System.Windows.Forms.Panel();
            this._SelectedHeaderLabel = new System.Windows.Forms.Label();
            this._OtherGamesHeaderPanel = new System.Windows.Forms.Panel();
            this._OtherGamesHeaderLabel = new System.Windows.Forms.Label();
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._PickerToolStrip.SuspendLayout();
            this._PickerStatusStrip.SuspendLayout();
            this._SelectedHeaderPanel.SuspendLayout();
            this._OtherGamesHeaderPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _ToolStripSeparator1
            //
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            // _ToolStripSeparator2
            //
            _ToolStripSeparator2.Name = "_ToolStripSeparator2";
            _ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            //
            // _LogoImageList
            //
            this._LogoImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this._LogoImageList.ImageSize = new System.Drawing.Size(184, 69);
            this._LogoImageList.TransparentColor = System.Drawing.Color.Transparent;
            //
            // _CallbackTimer
            //
            this._CallbackTimer.Enabled = true;
            this._CallbackTimer.Tick += new System.EventHandler(this.OnTimer);
            //
            // _PickerToolStrip
            //
            this._PickerToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._RefreshGamesButton,
            _ToolStripSeparator1,
            this._AddGameTextBox,
            this._AddGameButton,
            _ToolStripSeparator2,
            this._FindGamesLabel,
            this._SearchGameTextBox,
            this._FilterDropDownButton,
            this._SelectAllButton,
            this._ClearAllButton,
            new System.Windows.Forms.ToolStripSeparator(),
            this._DonateButton,
            this._SocialsButton,
            this._DisclaimerButton});
            this._PickerToolStrip.Location = new System.Drawing.Point(0, 0);
            this._PickerToolStrip.Name = "_PickerToolStrip";
            this._PickerToolStrip.Size = new System.Drawing.Size(742, 25);
            this._PickerToolStrip.TabIndex = 1;
            this._PickerToolStrip.Text = "toolStrip1";
            //
            // _RefreshGamesButton
            //
            this._RefreshGamesButton.Image = global::SAM.Picker.Resources.Refresh;
            this._RefreshGamesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._RefreshGamesButton.Name = "_RefreshGamesButton";
            this._RefreshGamesButton.Size = new System.Drawing.Size(105, 22);
            this._RefreshGamesButton.Text = "Refresh Games";
            this._RefreshGamesButton.Click += new System.EventHandler(this.OnRefresh);
            //
            // _AddGameTextBox
            //
            this._AddGameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._AddGameTextBox.Name = "_AddGameTextBox";
            this._AddGameTextBox.Size = new System.Drawing.Size(100, 25);
            //
            // _AddGameButton
            //
            this._AddGameButton.Image = global::SAM.Picker.Resources.Search;
            this._AddGameButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._AddGameButton.Name = "_AddGameButton";
            this._AddGameButton.Size = new System.Drawing.Size(83, 22);
            this._AddGameButton.Text = "Add Game";
            this._AddGameButton.Click += new System.EventHandler(this.OnAddGame);
            //
            // _FindGamesLabel
            //
            this._FindGamesLabel.Name = "_FindGamesLabel";
            this._FindGamesLabel.Size = new System.Drawing.Size(33, 22);
            this._FindGamesLabel.Text = "Filter";
            //
            // _SearchGameTextBox
            //
            this._SearchGameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._SearchGameTextBox.Name = "_SearchGameTextBox";
            this._SearchGameTextBox.Size = new System.Drawing.Size(100, 25);
            this._SearchGameTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnFilterUpdate);
            //
            // _FilterDropDownButton
            //
            this._FilterDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._FilterDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._FilterGamesMenuItem,
            this._FilterDemosMenuItem,
            this._FilterModsMenuItem,
            this._FilterJunkMenuItem});
            this._FilterDropDownButton.Image = global::SAM.Picker.Resources.Filter;
            this._FilterDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._FilterDropDownButton.Name = "_FilterDropDownButton";
            this._FilterDropDownButton.Size = new System.Drawing.Size(29, 22);
            this._FilterDropDownButton.Text = "Game filtering";
            //
            // _SelectAllButton
            //
            this._SelectAllButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._SelectAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._SelectAllButton.Name = "_SelectAllButton";
            this._SelectAllButton.Size = new System.Drawing.Size(60, 22);
            this._SelectAllButton.Text = "Select All";
            this._SelectAllButton.Click += new System.EventHandler(this.OnSelectAll);
            //
            // _ClearAllButton
            //
            this._ClearAllButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._ClearAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._ClearAllButton.Name = "_ClearAllButton";
            this._ClearAllButton.Size = new System.Drawing.Size(60, 22);
            this._ClearAllButton.Text = "Clear All";
            this._ClearAllButton.Click += new System.EventHandler(this.OnClearAll);
            //
            // _DonateButton
            //
            this._DonateButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._DonateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._DonateButton.Name = "_DonateButton";
            this._DonateButton.Size = new System.Drawing.Size(60, 22);
            this._DonateButton.Text = "☕ Donate";
            this._DonateButton.ToolTipText = "Support on Ko-fi";
            this._DonateButton.Click += new System.EventHandler(this.OnDonateClick);
            //
            // _SocialsButton
            //
            this._SocialsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._SocialsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._SocialsButton.Name = "_SocialsButton";
            this._SocialsButton.Size = new System.Drawing.Size(60, 22);
            this._SocialsButton.Text = "🌐 Socials";
            this._SocialsButton.ToolTipText = "Visit x.hegxib.me";
            this._SocialsButton.Click += new System.EventHandler(this.OnSocialsClick);
            //
            // _DisclaimerButton
            //
            this._DisclaimerButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._DisclaimerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._DisclaimerButton.Name = "_DisclaimerButton";
            this._DisclaimerButton.Size = new System.Drawing.Size(80, 22);
            this._DisclaimerButton.Text = "ℹ Disclaimer";
            this._DisclaimerButton.ToolTipText = "View disclaimer";
            this._DisclaimerButton.Click += new System.EventHandler(this.OnDisclaimerClick);
            //
            // _FilterGamesMenuItem
            //
            this._FilterGamesMenuItem.Checked = true;
            this._FilterGamesMenuItem.CheckOnClick = true;
            this._FilterGamesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this._FilterGamesMenuItem.Name = "_FilterGamesMenuItem";
            this._FilterGamesMenuItem.Size = new System.Drawing.Size(180, 22);
            this._FilterGamesMenuItem.Text = "Show &games";
            this._FilterGamesMenuItem.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            //
            // _FilterDemosMenuItem
            //
            this._FilterDemosMenuItem.CheckOnClick = true;
            this._FilterDemosMenuItem.Name = "_FilterDemosMenuItem";
            this._FilterDemosMenuItem.Size = new System.Drawing.Size(180, 22);
            this._FilterDemosMenuItem.Text = "Show &demos";
            this._FilterDemosMenuItem.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            //
            // _FilterModsMenuItem
            //
            this._FilterModsMenuItem.CheckOnClick = true;
            this._FilterModsMenuItem.Name = "_FilterModsMenuItem";
            this._FilterModsMenuItem.Size = new System.Drawing.Size(180, 22);
            this._FilterModsMenuItem.Text = "Show &mods";
            this._FilterModsMenuItem.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            //
            // _FilterJunkMenuItem
            //
            this._FilterJunkMenuItem.CheckOnClick = true;
            this._FilterJunkMenuItem.Name = "_FilterJunkMenuItem";
            this._FilterJunkMenuItem.Size = new System.Drawing.Size(180, 22);
            this._FilterJunkMenuItem.Text = "Show &junk";
            this._FilterJunkMenuItem.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            //
            // _GameContextMenu
            //
            this._GameContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToggleSelectionMenuItem,
            this._LaunchThisOnlyMenuItem,
            this._LaunchOneRandomMenuItem,
            this._RemoveFromSelectedMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            this._ClearAllSelectionsMenuItem});
            this._GameContextMenu.Name = "_GameContextMenu";
            this._GameContextMenu.Size = new System.Drawing.Size(200, 120);
            this._GameContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnContextMenuOpening);
            //
            // _ToggleSelectionMenuItem
            //
            this._ToggleSelectionMenuItem.Name = "_ToggleSelectionMenuItem";
            this._ToggleSelectionMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this._ToggleSelectionMenuItem.Size = new System.Drawing.Size(220, 22);
            this._ToggleSelectionMenuItem.Text = "Add to SELECTED";
            this._ToggleSelectionMenuItem.Click += new System.EventHandler(this.OnToggleSelection);
            //
            // _LaunchThisOnlyMenuItem
            //
            this._LaunchThisOnlyMenuItem.Name = "_LaunchThisOnlyMenuItem";
            this._LaunchThisOnlyMenuItem.Size = new System.Drawing.Size(220, 22);
            this._LaunchThisOnlyMenuItem.Text = "Launch This Game Only";
            this._LaunchThisOnlyMenuItem.Click += new System.EventHandler(this.OnLaunchThisOnly);
            //
            // _LaunchOneRandomMenuItem
            //
            this._LaunchOneRandomMenuItem.Name = "_LaunchOneRandomMenuItem";
            this._LaunchOneRandomMenuItem.Size = new System.Drawing.Size(270, 22);
            this._LaunchOneRandomMenuItem.Text = "Launch One Random from SELECTED";
            this._LaunchOneRandomMenuItem.Click += new System.EventHandler(this.OnLaunchOneRandom);
            //
            // _RemoveFromSelectedMenuItem
            //
            this._RemoveFromSelectedMenuItem.Name = "_RemoveFromSelectedMenuItem";
            this._RemoveFromSelectedMenuItem.Size = new System.Drawing.Size(220, 22);
            this._RemoveFromSelectedMenuItem.Text = "Remove from SELECTED";
            this._RemoveFromSelectedMenuItem.Click += new System.EventHandler(this.OnRemoveFromSelected);
            //
            // _ClearAllSelectionsMenuItem
            //
            this._ClearAllSelectionsMenuItem.Name = "_ClearAllSelectionsMenuItem";
            this._ClearAllSelectionsMenuItem.Size = new System.Drawing.Size(220, 22);
            this._ClearAllSelectionsMenuItem.Text = "Clear SELECTED Section";
            this._ClearAllSelectionsMenuItem.Click += new System.EventHandler(this.OnClearAllSelections);
            //
            // _SelectedHeaderPanel
            //
            this._SelectedHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this._SelectedHeaderPanel.Controls.Add(this._SelectedHeaderLabel);
            this._SelectedHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._SelectedHeaderPanel.Location = new System.Drawing.Point(0, 25);
            this._SelectedHeaderPanel.Name = "_SelectedHeaderPanel";
            this._SelectedHeaderPanel.Size = new System.Drawing.Size(742, 24);
            this._SelectedHeaderPanel.TabIndex = 5;
            this._SelectedHeaderPanel.Visible = false;
            //
            // _SelectedHeaderLabel
            //
            this._SelectedHeaderLabel.AutoSize = true;
            this._SelectedHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._SelectedHeaderLabel.ForeColor = System.Drawing.Color.Yellow;
            this._SelectedHeaderLabel.Location = new System.Drawing.Point(4, 4);
            this._SelectedHeaderLabel.Name = "_SelectedHeaderLabel";
            this._SelectedHeaderLabel.Size = new System.Drawing.Size(95, 15);
            this._SelectedHeaderLabel.TabIndex = 0;
            this._SelectedHeaderLabel.Text = "▼ SELECTED (0)";
            //
            // _SelectedListView
            //
            this._SelectedListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(30)))));
            this._SelectedListView.Dock = System.Windows.Forms.DockStyle.Top;
            this._SelectedListView.ForeColor = System.Drawing.Color.Yellow;
            this._SelectedListView.HideSelection = false;
            this._SelectedListView.LargeImageList = this._LogoImageList;
            this._SelectedListView.Location = new System.Drawing.Point(0, 49);
            this._SelectedListView.MultiSelect = true;
            this._SelectedListView.Name = "_SelectedListView";
            this._SelectedListView.OwnerDraw = true;
            this._SelectedListView.Size = new System.Drawing.Size(742, 100);
            this._SelectedListView.SmallImageList = this._LogoImageList;
            this._SelectedListView.TabIndex = 7;
            this._SelectedListView.TileSize = new System.Drawing.Size(184, 69);
            this._SelectedListView.UseCompatibleStateImageBehavior = false;
            this._SelectedListView.VirtualMode = true;
            this._SelectedListView.Visible = false;
            this._SelectedListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OnGameListViewDrawItem);
            this._SelectedListView.ItemActivate += new System.EventHandler(this.OnActivateGame);
            this._SelectedListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnGameListViewClick);
            this._SelectedListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.OnSelectedListViewRetrieveVirtualItem);
            //
            // _OtherGamesHeaderPanel
            //
            this._OtherGamesHeaderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this._OtherGamesHeaderPanel.Controls.Add(this._OtherGamesHeaderLabel);
            this._OtherGamesHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._OtherGamesHeaderPanel.Location = new System.Drawing.Point(0, 49);
            this._OtherGamesHeaderPanel.Name = "_OtherGamesHeaderPanel";
            this._OtherGamesHeaderPanel.Size = new System.Drawing.Size(742, 24);
            this._OtherGamesHeaderPanel.TabIndex = 6;
            this._OtherGamesHeaderPanel.Visible = true;
            //
            // _OtherGamesHeaderLabel
            //
            this._OtherGamesHeaderLabel.AutoSize = true;
            this._OtherGamesHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._OtherGamesHeaderLabel.ForeColor = System.Drawing.Color.LightBlue;
            this._OtherGamesHeaderLabel.Location = new System.Drawing.Point(4, 4);
            this._OtherGamesHeaderLabel.Name = "_OtherGamesHeaderLabel";
            this._OtherGamesHeaderLabel.Size = new System.Drawing.Size(120, 15);
            this._OtherGamesHeaderLabel.TabIndex = 0;
            this._OtherGamesHeaderLabel.Text = "▼ OTHER GAMES (0)";
            //
            // _GameListView
            //
            this._GameListView.BackColor = System.Drawing.Color.Black;
            this._GameListView.ContextMenuStrip = this._GameContextMenu;
            this._GameListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._GameListView.ForeColor = System.Drawing.Color.White;
            this._GameListView.HideSelection = false;
            this._GameListView.LargeImageList = this._LogoImageList;
            this._GameListView.Location = new System.Drawing.Point(0, 25);
            this._GameListView.MultiSelect = true;
            this._GameListView.Name = "_GameListView";
            this._GameListView.OwnerDraw = true;
            this._GameListView.Size = new System.Drawing.Size(742, 245);
            this._GameListView.SmallImageList = this._LogoImageList;
            this._GameListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._GameListView.TabIndex = 0;
            this._GameListView.TileSize = new System.Drawing.Size(184, 69);
            this._GameListView.UseCompatibleStateImageBehavior = false;
            this._GameListView.VirtualMode = true;
            this._GameListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OnGameListViewDrawItem);
            this._GameListView.ItemActivate += new System.EventHandler(this.OnActivateGame);
            this._GameListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnGameListViewClick);
            this._GameListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.OnGameListViewRetrieveVirtualItem);
            this._GameListView.SearchForVirtualItem += new System.Windows.Forms.SearchForVirtualItemEventHandler(this.OnGameListViewSearchForVirtualItem);
            //
            // _PickerStatusStrip
            //
            this._PickerStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._PickerStatusLabel,
            this._DownloadStatusLabel});
            this._PickerStatusStrip.Location = new System.Drawing.Point(0, 270);
            this._PickerStatusStrip.Name = "_PickerStatusStrip";
            this._PickerStatusStrip.Size = new System.Drawing.Size(742, 22);
            this._PickerStatusStrip.TabIndex = 2;
            this._PickerStatusStrip.Text = "statusStrip";
            //
            // _PickerStatusLabel
            //
            this._PickerStatusLabel.Name = "_PickerStatusLabel";
            this._PickerStatusLabel.Size = new System.Drawing.Size(727, 17);
            this._PickerStatusLabel.Spring = true;
            this._PickerStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _DownloadStatusLabel
            //
            this._DownloadStatusLabel.Image = global::SAM.Picker.Resources.Download;
            this._DownloadStatusLabel.Name = "_DownloadStatusLabel";
            this._DownloadStatusLabel.Size = new System.Drawing.Size(111, 17);
            this._DownloadStatusLabel.Text = "Download status";
            this._DownloadStatusLabel.Visible = false;
            //
            // _LogoWorker
            //
            this._LogoWorker.WorkerSupportsCancellation = true;
            this._LogoWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoDownloadLogo);
            this._LogoWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnDownloadLogo);
            //
            // _ListWorker
            //
            this._ListWorker.WorkerSupportsCancellation = true;
            this._ListWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoDownloadList);
            this._ListWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnDownloadList);
            //
            // GamePicker
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 292);
            this.Controls.Add(this._GameListView);
            this.Controls.Add(this._OtherGamesHeaderPanel);
            this.Controls.Add(this._SelectedListView);
            this.Controls.Add(this._SelectedHeaderPanel);
            this.Controls.Add(this._PickerStatusStrip);
            this.Controls.Add(this._PickerToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GamePicker";
            this.Text = "HxB SAM Enhanced 1.0 | Pick a game... Any game...";
            this._PickerToolStrip.ResumeLayout(false);
            this._PickerToolStrip.PerformLayout();
            this._PickerStatusStrip.ResumeLayout(false);
            this._PickerStatusStrip.PerformLayout();
            this._SelectedHeaderPanel.ResumeLayout(false);
            this._SelectedHeaderPanel.PerformLayout();
            this._OtherGamesHeaderPanel.ResumeLayout(false);
            this._OtherGamesHeaderPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private MyListView _GameListView;
        private System.Windows.Forms.ImageList _LogoImageList;
        private System.Windows.Forms.Timer _CallbackTimer;
        private System.Windows.Forms.ToolStrip _PickerToolStrip;
        private System.Windows.Forms.ToolStripButton _RefreshGamesButton;
        private System.Windows.Forms.ToolStripTextBox _AddGameTextBox;
        private System.Windows.Forms.ToolStripButton _AddGameButton;
        private System.Windows.Forms.ToolStripDropDownButton _FilterDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem _FilterGamesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _FilterJunkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _FilterDemosMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _FilterModsMenuItem;
        private System.Windows.Forms.StatusStrip _PickerStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _DownloadStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _PickerStatusLabel;
        private System.ComponentModel.BackgroundWorker _LogoWorker;
        private System.ComponentModel.BackgroundWorker _ListWorker;
        private System.Windows.Forms.ToolStripTextBox _SearchGameTextBox;
        private System.Windows.Forms.ToolStripLabel _FindGamesLabel;
        private System.Windows.Forms.ContextMenuStrip _GameContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _ToggleSelectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _ClearAllSelectionsMenuItem;
        private System.Windows.Forms.Panel _SelectedHeaderPanel;
        private System.Windows.Forms.Label _SelectedHeaderLabel;
        private System.Windows.Forms.Panel _OtherGamesHeaderPanel;
        private System.Windows.Forms.Label _OtherGamesHeaderLabel;
        private MyListView _SelectedListView;
        private System.Windows.Forms.ToolStripButton _SelectAllButton;
        private System.Windows.Forms.ToolStripButton _ClearAllButton;
        private System.Windows.Forms.ToolStripMenuItem _LaunchThisOnlyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _LaunchOneRandomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _RemoveFromSelectedMenuItem;
        private System.Windows.Forms.ToolStripButton _DonateButton;
        private System.Windows.Forms.ToolStripButton _SocialsButton;
        private System.Windows.Forms.ToolStripButton _DisclaimerButton;

        #endregion
    }
}
