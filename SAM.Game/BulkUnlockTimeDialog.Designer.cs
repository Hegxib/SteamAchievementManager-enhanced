namespace SAM.Game
{
    partial class BulkUnlockTimeDialog
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
            this._InstructionLabel = new System.Windows.Forms.Label();
            this._StartTimeLabel = new System.Windows.Forms.Label();
            this._StartTimePicker = new System.Windows.Forms.DateTimePicker();
            this._IntervalLabel = new System.Windows.Forms.Label();
            this._IntervalNumeric = new System.Windows.Forms.NumericUpDown();
            this._MinutesLabel = new System.Windows.Forms.Label();
            this._ApplyToAllRadio = new System.Windows.Forms.RadioButton();
            this._ApplyToSelectedRadio = new System.Windows.Forms.RadioButton();
            this._OKButton = new System.Windows.Forms.Button();
            this._CancelButton = new System.Windows.Forms.Button();
            this._ExampleLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._IntervalNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // _InstructionLabel
            // 
            this._InstructionLabel.Location = new System.Drawing.Point(12, 9);
            this._InstructionLabel.Name = "_InstructionLabel";
            this._InstructionLabel.Size = new System.Drawing.Size(460, 40);
            this._InstructionLabel.TabIndex = 0;
            this._InstructionLabel.Text = "Set unlock times for multiple achievements sequentially. Each achievement will be scheduled at the specified interval after the previous one.";
            // 
            // _StartTimeLabel
            // 
            this._StartTimeLabel.AutoSize = true;
            this._StartTimeLabel.Location = new System.Drawing.Point(12, 60);
            this._StartTimeLabel.Name = "_StartTimeLabel";
            this._StartTimeLabel.Size = new System.Drawing.Size(83, 13);
            this._StartTimeLabel.TabIndex = 1;
            this._StartTimeLabel.Text = "Start Time:";
            // 
            // _StartTimePicker
            // 
            this._StartTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this._StartTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._StartTimePicker.Location = new System.Drawing.Point(120, 56);
            this._StartTimePicker.Name = "_StartTimePicker";
            this._StartTimePicker.Size = new System.Drawing.Size(352, 20);
            this._StartTimePicker.TabIndex = 2;
            // 
            // _IntervalLabel
            // 
            this._IntervalLabel.AutoSize = true;
            this._IntervalLabel.Location = new System.Drawing.Point(12, 90);
            this._IntervalLabel.Name = "_IntervalLabel";
            this._IntervalLabel.Size = new System.Drawing.Size(102, 13);
            this._IntervalLabel.TabIndex = 3;
            this._IntervalLabel.Text = "Interval between:";
            // 
            // _IntervalNumeric
            // 
            this._IntervalNumeric.Location = new System.Drawing.Point(120, 88);
            this._IntervalNumeric.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this._IntervalNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._IntervalNumeric.Name = "_IntervalNumeric";
            this._IntervalNumeric.Size = new System.Drawing.Size(80, 20);
            this._IntervalNumeric.TabIndex = 4;
            this._IntervalNumeric.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // _MinutesLabel
            // 
            this._MinutesLabel.AutoSize = true;
            this._MinutesLabel.Location = new System.Drawing.Point(206, 90);
            this._MinutesLabel.Name = "_MinutesLabel";
            this._MinutesLabel.Size = new System.Drawing.Size(50, 13);
            this._MinutesLabel.TabIndex = 5;
            this._MinutesLabel.Text = "minutes";
            // 
            // _ApplyToAllRadio
            // 
            this._ApplyToAllRadio.AutoSize = true;
            this._ApplyToAllRadio.Checked = true;
            this._ApplyToAllRadio.Location = new System.Drawing.Point(15, 120);
            this._ApplyToAllRadio.Name = "_ApplyToAllRadio";
            this._ApplyToAllRadio.Size = new System.Drawing.Size(160, 17);
            this._ApplyToAllRadio.TabIndex = 6;
            this._ApplyToAllRadio.TabStop = true;
            this._ApplyToAllRadio.Text = "Apply to all achievements";
            this._ApplyToAllRadio.UseVisualStyleBackColor = true;
            // 
            // _ApplyToSelectedRadio
            // 
            this._ApplyToSelectedRadio.AutoSize = true;
            this._ApplyToSelectedRadio.Location = new System.Drawing.Point(15, 143);
            this._ApplyToSelectedRadio.Name = "_ApplyToSelectedRadio";
            this._ApplyToSelectedRadio.Size = new System.Drawing.Size(200, 17);
            this._ApplyToSelectedRadio.TabIndex = 7;
            this._ApplyToSelectedRadio.Text = "Apply to selected achievements only";
            this._ApplyToSelectedRadio.UseVisualStyleBackColor = true;
            // 
            // _OKButton
            // 
            this._OKButton.Location = new System.Drawing.Point(316, 185);
            this._OKButton.Name = "_OKButton";
            this._OKButton.Size = new System.Drawing.Size(75, 23);
            this._OKButton.TabIndex = 8;
            this._OKButton.Text = "OK";
            this._OKButton.UseVisualStyleBackColor = true;
            this._OKButton.Click += new System.EventHandler(this.OnOK);
            // 
            // _CancelButton
            // 
            this._CancelButton.Location = new System.Drawing.Point(397, 185);
            this._CancelButton.Name = "_CancelButton";
            this._CancelButton.Size = new System.Drawing.Size(75, 23);
            this._CancelButton.TabIndex = 9;
            this._CancelButton.Text = "Cancel";
            this._CancelButton.UseVisualStyleBackColor = true;
            this._CancelButton.Click += new System.EventHandler(this.OnCancel);
            // 
            // _ExampleLabel
            // 
            this._ExampleLabel.ForeColor = System.Drawing.Color.Gray;
            this._ExampleLabel.Location = new System.Drawing.Point(12, 165);
            this._ExampleLabel.Name = "_ExampleLabel";
            this._ExampleLabel.Size = new System.Drawing.Size(460, 17);
            this._ExampleLabel.TabIndex = 10;
            this._ExampleLabel.Text = "Example: Achievement #1 at 5:00 PM, #2 at 5:05 PM, #3 at 5:10 PM, etc.";
            // 
            // BulkUnlockTimeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 220);
            this.Controls.Add(this._ExampleLabel);
            this.Controls.Add(this._CancelButton);
            this.Controls.Add(this._OKButton);
            this.Controls.Add(this._ApplyToSelectedRadio);
            this.Controls.Add(this._ApplyToAllRadio);
            this.Controls.Add(this._MinutesLabel);
            this.Controls.Add(this._IntervalNumeric);
            this.Controls.Add(this._IntervalLabel);
            this.Controls.Add(this._StartTimePicker);
            this.Controls.Add(this._StartTimeLabel);
            this.Controls.Add(this._InstructionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BulkUnlockTimeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bulk Set Achievement Unlock Times";
            ((System.ComponentModel.ISupportInitialize)(this._IntervalNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label _InstructionLabel;
        private System.Windows.Forms.Label _StartTimeLabel;
        private System.Windows.Forms.DateTimePicker _StartTimePicker;
        private System.Windows.Forms.Label _IntervalLabel;
        private System.Windows.Forms.NumericUpDown _IntervalNumeric;
        private System.Windows.Forms.Label _MinutesLabel;
        private System.Windows.Forms.RadioButton _ApplyToAllRadio;
        private System.Windows.Forms.RadioButton _ApplyToSelectedRadio;
        private System.Windows.Forms.Button _OKButton;
        private System.Windows.Forms.Button _CancelButton;
        private System.Windows.Forms.Label _ExampleLabel;
    }
}
