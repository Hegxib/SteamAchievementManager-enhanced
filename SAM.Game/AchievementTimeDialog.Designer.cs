namespace SAM.Game
{
    partial class AchievementTimeDialog
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
            this._EnableScheduleCheckBox = new System.Windows.Forms.CheckBox();
            this._DateTimePicker = new System.Windows.Forms.DateTimePicker();
            this._OKButton = new System.Windows.Forms.Button();
            this._CancelButton = new System.Windows.Forms.Button();
            this._InstructionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _EnableScheduleCheckBox
            // 
            this._EnableScheduleCheckBox.AutoSize = true;
            this._EnableScheduleCheckBox.Location = new System.Drawing.Point(12, 50);
            this._EnableScheduleCheckBox.Name = "_EnableScheduleCheckBox";
            this._EnableScheduleCheckBox.Size = new System.Drawing.Size(157, 17);
            this._EnableScheduleCheckBox.TabIndex = 0;
            this._EnableScheduleCheckBox.Text = "Schedule unlock at specific time";
            this._EnableScheduleCheckBox.UseVisualStyleBackColor = true;
            this._EnableScheduleCheckBox.CheckedChanged += new System.EventHandler(this.OnEnableScheduleChanged);
            // 
            // _DateTimePicker
            // 
            this._DateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this._DateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._DateTimePicker.Location = new System.Drawing.Point(12, 73);
            this._DateTimePicker.Name = "_DateTimePicker";
            this._DateTimePicker.Size = new System.Drawing.Size(360, 20);
            this._DateTimePicker.TabIndex = 1;
            // 
            // _OKButton
            // 
            this._OKButton.Location = new System.Drawing.Point(216, 110);
            this._OKButton.Name = "_OKButton";
            this._OKButton.Size = new System.Drawing.Size(75, 23);
            this._OKButton.TabIndex = 2;
            this._OKButton.Text = "OK";
            this._OKButton.UseVisualStyleBackColor = true;
            this._OKButton.Click += new System.EventHandler(this.OnOK);
            // 
            // _CancelButton
            // 
            this._CancelButton.Location = new System.Drawing.Point(297, 110);
            this._CancelButton.Name = "_CancelButton";
            this._CancelButton.Size = new System.Drawing.Size(75, 23);
            this._CancelButton.TabIndex = 3;
            this._CancelButton.Text = "Cancel";
            this._CancelButton.UseVisualStyleBackColor = true;
            this._CancelButton.Click += new System.EventHandler(this.OnCancel);
            // 
            // _InstructionLabel
            // 
            this._InstructionLabel.Location = new System.Drawing.Point(12, 9);
            this._InstructionLabel.Name = "_InstructionLabel";
            this._InstructionLabel.Size = new System.Drawing.Size(360, 35);
            this._InstructionLabel.TabIndex = 4;
            this._InstructionLabel.Text = "Set a specific unlock time for this achievement. When you commit changes, the achievement will be unlocked with the specified timestamp.";
            // 
            // AchievementTimeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 145);
            this.Controls.Add(this._InstructionLabel);
            this.Controls.Add(this._CancelButton);
            this.Controls.Add(this._OKButton);
            this.Controls.Add(this._DateTimePicker);
            this.Controls.Add(this._EnableScheduleCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AchievementTimeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Achievement Unlock Time";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox _EnableScheduleCheckBox;
        private System.Windows.Forms.DateTimePicker _DateTimePicker;
        private System.Windows.Forms.Button _OKButton;
        private System.Windows.Forms.Button _CancelButton;
        private System.Windows.Forms.Label _InstructionLabel;
    }
}
