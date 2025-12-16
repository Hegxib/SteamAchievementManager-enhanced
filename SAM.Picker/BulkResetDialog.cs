/* Copyright (c) 2024 HEGXIB
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SAM.Picker
{
    internal class BulkResetDialog : Form
    {
        private CheckedListBox _GameListBox;
        private Button _ResetButton;
        private Button _CancelButton;
        private Button _SelectAllButton;
        private Button _DeselectAllButton;
        private Label _InfoLabel;
        
        private readonly List<GameInfo> _AllGames;
        private readonly HashSet<uint> _PreSelectedGameIds;
        
        public List<uint> SelectedGameIds { get; private set; }

        public BulkResetDialog(List<GameInfo> allGames, HashSet<uint> preSelectedGameIds)
        {
            this._AllGames = allGames.OrderBy(g => g.Name).ToList();
            this._PreSelectedGameIds = preSelectedGameIds;
            this.SelectedGameIds = new List<uint>();
            
            InitializeComponent();
            PopulateGameList();
        }

        private void InitializeComponent()
        {
            this.Text = "🔄 Bulk Reset Achievements";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Info Label
            this._InfoLabel = new Label
            {
                Text = "Select games to reset achievements (all achievements will be locked):",
                Location = new Point(10, 10),
                Size = new Size(560, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Game List Box
            this._GameListBox = new CheckedListBox
            {
                Location = new Point(10, 55),
                Size = new Size(560, 330),
                CheckOnClick = true,
                IntegralHeight = false
            };

            // Select All Button
            this._SelectAllButton = new Button
            {
                Text = "Select All",
                Location = new Point(10, 395),
                Size = new Size(100, 30)
            };
            this._SelectAllButton.Click += (s, e) => SelectAll();

            // Deselect All Button
            this._DeselectAllButton = new Button
            {
                Text = "Deselect All",
                Location = new Point(120, 395),
                Size = new Size(100, 30)
            };
            this._DeselectAllButton.Click += (s, e) => DeselectAll();

            // Reset Button
            this._ResetButton = new Button
            {
                Text = "🔄 Reset Selected",
                Location = new Point(360, 395),
                Size = new Size(120, 30),
                DialogResult = DialogResult.OK
            };
            this._ResetButton.Click += OnResetClick;

            // Cancel Button
            this._CancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(490, 395),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(this._InfoLabel);
            this.Controls.Add(this._GameListBox);
            this.Controls.Add(this._SelectAllButton);
            this.Controls.Add(this._DeselectAllButton);
            this.Controls.Add(this._ResetButton);
            this.Controls.Add(this._CancelButton);
            
            this.AcceptButton = this._ResetButton;
            this.CancelButton = this._CancelButton;
        }

        private void PopulateGameList()
        {
            foreach (var game in this._AllGames)
            {
                int index = this._GameListBox.Items.Add(game.Name);
                
                // Pre-select games that are in SELECTED section
                if (this._PreSelectedGameIds.Contains(game.Id))
                {
                    this._GameListBox.SetItemChecked(index, true);
                }
            }
        }

        private void SelectAll()
        {
            for (int i = 0; i < this._GameListBox.Items.Count; i++)
            {
                this._GameListBox.SetItemChecked(i, true);
            }
        }

        private void DeselectAll()
        {
            for (int i = 0; i < this._GameListBox.Items.Count; i++)
            {
                this._GameListBox.SetItemChecked(i, false);
            }
        }

        private void OnResetClick(object sender, EventArgs e)
        {
            // Collect selected game IDs
            this.SelectedGameIds.Clear();
            
            for (int i = 0; i < this._GameListBox.CheckedIndices.Count; i++)
            {
                int checkedIndex = this._GameListBox.CheckedIndices[i];
                if (checkedIndex >= 0 && checkedIndex < this._AllGames.Count)
                {
                    this.SelectedGameIds.Add(this._AllGames[checkedIndex].Id);
                }
            }

            if (this.SelectedGameIds.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Please select at least one game to reset.",
                    "No Games Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Confirmation dialog
            var result = MessageBox.Show(
                this,
                $"⚠️ Are you sure you want to reset achievements for {this.SelectedGameIds.Count} game(s)?\n\n" +
                "This will lock ALL achievements in the selected games.\n\n" +
                "This action cannot be undone!",
                "Confirm Bulk Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                this.DialogResult = DialogResult.None;
            }
        }
    }
}
