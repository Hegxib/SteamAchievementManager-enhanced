/* Launch Queue Progress Dialog */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SAM.Picker
{
    internal partial class LaunchQueueDialog : Form
    {
        private readonly List<GameInfo> _GamesToLaunch;
        private readonly int _DelayBetweenLaunches;
        private readonly Action<uint> _OnGameClosed;
        private int _CurrentIndex;
        private int _SuccessCount;
        private int _FailCount;
        private bool _IsCancelled;

        public LaunchQueueDialog(List<GameInfo> games, int delaySeconds, Action<uint> onGameClosed = null)
        {
            this._GamesToLaunch = games;
            this._DelayBetweenLaunches = delaySeconds;
            this._OnGameClosed = onGameClosed;
            this._CurrentIndex = 0;
            this._SuccessCount = 0;
            this._FailCount = 0;
            this._IsCancelled = false;

            this.InitializeComponent();
            this.UpdateProgress();
        }

        private void InitializeComponent()
        {
            this.Text = "Launching Games...";
            this.Size = new Size(500, 250);
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

            this._StatusLabel = new Label
            {
                Text = "Preparing to launch games...",
                AutoSize = false,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            this._ProgressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = this._GamesToLaunch.Count,
                Value = 0,
                Height = 25,
                Dock = DockStyle.Fill
            };

            this._ProgressLabel = new Label
            {
                Text = $"0 / {this._GamesToLaunch.Count}",
                AutoSize = false,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            this._ResultsTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9)
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Height = 35
            };

            this._CancelButton = new Button
            {
                Text = "Cancel",
                Width = 80,
                Height = 30
            };
            this._CancelButton.Click += OnCancelClick;

            this._CloseButton = new Button
            {
                Text = "Close",
                Width = 80,
                Height = 30,
                Enabled = false
            };
            this._CloseButton.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(this._CloseButton);
            buttonPanel.Controls.Add(this._CancelButton);

            mainPanel.Controls.Add(this._StatusLabel, 0, 0);
            mainPanel.Controls.Add(this._ProgressBar, 0, 1);
            mainPanel.Controls.Add(this._ProgressLabel, 0, 2);
            mainPanel.Controls.Add(this._ResultsTextBox, 0, 3);
            mainPanel.Controls.Add(buttonPanel, 0, 4);

            this.Controls.Add(mainPanel);
        }

        private Label _StatusLabel;
        private ProgressBar _ProgressBar;
        private Label _ProgressLabel;
        private TextBox _ResultsTextBox;
        private Button _CancelButton;
        private Button _CloseButton;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var bgWorker = new BackgroundWorker { WorkerReportsProgress = true };
            bgWorker.DoWork += LaunchGamesInBackground;
            bgWorker.ProgressChanged += OnProgressChanged;
            bgWorker.RunWorkerCompleted += OnLaunchCompleted;
            bgWorker.RunWorkerAsync();
        }

        private void LaunchGamesInBackground(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            
            for (int i = 0; i < this._GamesToLaunch.Count; i++)
            {
                if (this._IsCancelled)
                {
                    worker.ReportProgress(i, "CANCELLED");
                    break;
                }

                this._CurrentIndex = i;
                var game = this._GamesToLaunch[i];

                try
                {
                    var gamePath = Path.Combine(Application.StartupPath, "SAM.Game.exe");
                    var process = Process.Start(gamePath, game.Id.ToString(CultureInfo.InvariantCulture));
                    
                    // Monitor process to trigger callback when it exits
                    if (process != null && this._OnGameClosed != null)
                    {
                        var gameId = game.Id;
                        process.EnableRaisingEvents = true;
                        process.Exited += (s, args) =>
                        {
                            this._OnGameClosed(gameId);
                        };
                    }
                    
                    this._SuccessCount++;
                    worker.ReportProgress(i + 1, $"✓ Launched: {game.Name}");
                }
                catch (Exception ex)
                {
                    this._FailCount++;
                    worker.ReportProgress(i + 1, $"✗ Failed: {game.Name} - {ex.Message}");
                }

                // Add delay between launches (except for last one)
                if (i < this._GamesToLaunch.Count - 1 && !this._IsCancelled && this._DelayBetweenLaunches > 0)
                {
                    for (int delay = 0; delay < this._DelayBetweenLaunches; delay++)
                    {
                        if (this._IsCancelled) break;
                        Thread.Sleep(1000);
                        worker.ReportProgress(i + 1, $"Waiting {this._DelayBetweenLaunches - delay}s before next launch...");
                    }
                }
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this._ProgressBar.Value = e.ProgressPercentage;
            this._ProgressLabel.Text = $"{e.ProgressPercentage} / {this._GamesToLaunch.Count}";

            var message = e.UserState as string;
            if (message != null)
            {
                if (message == "CANCELLED")
                {
                    this._StatusLabel.Text = "Launch cancelled by user";
                }
                else if (message.StartsWith("Waiting"))
                {
                    this._StatusLabel.Text = message;
                }
                else
                {
                    this._ResultsTextBox.AppendText(message + Environment.NewLine);
                    if (this._CurrentIndex < this._GamesToLaunch.Count)
                    {
                        this._StatusLabel.Text = $"Launching game {this._CurrentIndex + 1} of {this._GamesToLaunch.Count}...";
                    }
                }
            }

            this.UpdateProgress();
        }

        private void OnLaunchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this._CancelButton.Enabled = false;
            this._CloseButton.Enabled = true;
            this._StatusLabel.Text = this._IsCancelled 
                ? $"Cancelled! Launched {this._SuccessCount}, Failed {this._FailCount}" 
                : $"Complete! Launched {this._SuccessCount}, Failed {this._FailCount}";
            this._ResultsTextBox.AppendText(Environment.NewLine + "=== Launch Complete ===" + Environment.NewLine);
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this._IsCancelled = true;
            this._CancelButton.Enabled = false;
        }

        private void UpdateProgress()
        {
            // Visual feedback
            if (this._IsCancelled)
            {
                this._ProgressBar.ForeColor = Color.Gray;
            }
            else if (this._FailCount > 0)
            {
                this._ProgressBar.ForeColor = Color.Orange;
            }
            else
            {
                this._ProgressBar.ForeColor = Color.Green;
            }
        }
    }
}
