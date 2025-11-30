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

/* Enhanced Features - Copyright (c) 2025 HEGXIB (hegxib.me)
 * Auto-close countdown timer with persistence and visual countdown display.
 * See LICENSE.txt for full enhanced features copyright and usage terms.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAM.Game
{
    internal partial class AutoCloseTimerDialog : Form
    {
        public int CountdownSeconds { get; private set; }
        public bool EnableAutoClose { get; private set; }

        public AutoCloseTimerDialog()
        {
            this.CountdownSeconds = 3600; // Default: 1 hour
            this.EnableAutoClose = false;

            this.InitializeComponent();
            this.UpdateDisplay();
        }

        private void InitializeComponent()
        {
            this.Text = "Auto-Close Timer";
            this.Size = new Size(450, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(15)
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Title
            var titleLabel = new Label
            {
                Text = "⏰ Auto-Close Countdown Timer",
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            // Countdown setting panel
            var countdownPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 10)
            };

            var countdownLabel = new Label
            {
                Text = "Close game after:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 5, 10, 0)
            };

            this._HoursNumeric = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 999,
                Value = 1,
                Width = 70
            };
            this._HoursNumeric.ValueChanged += (s, e) => UpdateCountdown();

            var hoursLabel = new Label
            {
                Text = "hours",
                AutoSize = true,
                Padding = new Padding(5, 5, 15, 0)
            };

            this._MinutesNumeric = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 59,
                Value = 0,
                Width = 70
            };
            this._MinutesNumeric.ValueChanged += (s, e) => UpdateCountdown();

            var minutesLabel = new Label
            {
                Text = "minutes",
                AutoSize = true,
                Padding = new Padding(5, 5, 15, 0)
            };

            this._SecondsNumeric = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 59,
                Value = 0,
                Width = 70
            };
            this._SecondsNumeric.ValueChanged += (s, e) => UpdateCountdown();

            var secondsLabel = new Label
            {
                Text = "seconds",
                AutoSize = true,
                Padding = new Padding(5, 5, 0, 0)
            };

            countdownPanel.Controls.Add(countdownLabel);
            countdownPanel.Controls.Add(this._HoursNumeric);
            countdownPanel.Controls.Add(hoursLabel);
            countdownPanel.Controls.Add(this._MinutesNumeric);
            countdownPanel.Controls.Add(minutesLabel);
            countdownPanel.Controls.Add(this._SecondsNumeric);
            countdownPanel.Controls.Add(secondsLabel);

            // Countdown display
            this._CountdownLabel = new Label
            {
                Text = "Game will close in: 1h 0m 0s",
                ForeColor = Color.Green,
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 10)
            };

            // Info text
            var infoLabel = new Label
            {
                Text = "The game window will automatically close when the countdown reaches zero.\n\n" +
                       "This is a simple timer - set your desired duration and this window will\n" +
                       "close automatically when the time is up.",
                ForeColor = SystemColors.GrayText,
                AutoSize = true,
                MaximumSize = new Size(400, 0)
            };

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Height = 40
            };

            var enableButton = new Button
            {
                Text = "Enable Timer",
                Width = 100,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            buttonPanel.Controls.Add(enableButton);
            buttonPanel.Controls.Add(cancelButton);

            mainPanel.Controls.Add(titleLabel, 0, 0);
            mainPanel.Controls.Add(countdownPanel, 0, 1);
            mainPanel.Controls.Add(this._CountdownLabel, 0, 2);
            mainPanel.Controls.Add(infoLabel, 0, 3);
            mainPanel.Controls.Add(buttonPanel, 0, 4);

            this.Controls.Add(mainPanel);
            this.AcceptButton = enableButton;
            this.CancelButton = cancelButton;
        }

        private Label _CountdownLabel;
        private NumericUpDown _HoursNumeric;
        private NumericUpDown _MinutesNumeric;
        private NumericUpDown _SecondsNumeric;

        private void UpdateCountdown()
        {
            int hours = (int)this._HoursNumeric.Value;
            int minutes = (int)this._MinutesNumeric.Value;
            int seconds = (int)this._SecondsNumeric.Value;
            
            this.CountdownSeconds = (hours * 3600) + (minutes * 60) + seconds;
            this.UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (this.CountdownSeconds <= 0)
            {
                this._CountdownLabel.Text = "⚠ Please set a countdown time!";
                this._CountdownLabel.ForeColor = Color.Red;
            }
            else
            {
                this._CountdownLabel.Text = $"⏱ Game will close in: {FormatSeconds(this.CountdownSeconds)}";
                this._CountdownLabel.ForeColor = Color.Green;
            }
        }

        private string FormatSeconds(int totalSeconds)
        {
            if (totalSeconds == 0)
                return "0 seconds";

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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.DialogResult == DialogResult.OK)
            {
                if (this.CountdownSeconds <= 0)
                {
                    MessageBox.Show(
                        this,
                        "Please set a countdown time greater than zero!",
                        "Invalid Countdown",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                this.EnableAutoClose = true;
            }
        }
    }
}
