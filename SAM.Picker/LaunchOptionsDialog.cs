/* Launch Options Dialog */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAM.Picker
{
    internal partial class LaunchOptionsDialog : Form
    {
        public int DelaySeconds { get; private set; }
        public bool UseQueue { get; private set; }

        public LaunchOptionsDialog()
        {
            this.DelaySeconds = 2; // Default 2 seconds
            this.UseQueue = true;

            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Launch Options";
            this.Size = new Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(15)
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Queue option
            this._UseQueueCheckBox = new CheckBox
            {
                Text = "Use launch queue with progress dialog",
                Checked = true,
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            this._UseQueueCheckBox.CheckedChanged += (s, e) =>
            {
                this._DelayPanel.Enabled = this._UseQueueCheckBox.Checked;
            };

            // Delay panel
            this._DelayPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(20, 10, 0, 0)
            };

            var delayLabel = new Label
            {
                Text = "Delay between launches:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 3, 10, 0)
            };

            this._DelayNumeric = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 60,
                Value = 2,
                Width = 60
            };

            var secondsLabel = new Label
            {
                Text = "seconds",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 3, 0, 0)
            };

            this._DelayPanel.Controls.Add(delayLabel);
            this._DelayPanel.Controls.Add(this._DelayNumeric);
            this._DelayPanel.Controls.Add(secondsLabel);

            // Info label
            var infoLabel = new Label
            {
                Text = "Queue mode shows a progress dialog and launches games one at a time.\n" +
                       "Without queue, all games launch simultaneously (like before).",
                AutoSize = true,
                Dock = DockStyle.Fill,
                ForeColor = SystemColors.GrayText
            };

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Height = 35
            };

            var okButton = new Button
            {
                Text = "Launch",
                Width = 80,
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

            buttonPanel.Controls.Add(okButton);
            buttonPanel.Controls.Add(cancelButton);

            mainPanel.Controls.Add(this._UseQueueCheckBox, 0, 0);
            mainPanel.Controls.Add(this._DelayPanel, 0, 1);
            mainPanel.Controls.Add(infoLabel, 0, 2);
            mainPanel.Controls.Add(buttonPanel, 0, 3);

            this.Controls.Add(mainPanel);
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private CheckBox _UseQueueCheckBox;
        private FlowLayoutPanel _DelayPanel;
        private NumericUpDown _DelayNumeric;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (this.DialogResult == DialogResult.OK)
            {
                this.UseQueue = this._UseQueueCheckBox.Checked;
                this.DelaySeconds = (int)this._DelayNumeric.Value;
            }
        }
    }
}
