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
using System.Windows.Forms;

namespace SAM.Game
{
    internal partial class BulkUnlockTimeDialog : Form
    {
        public DateTime StartTime { get; private set; }
        public int IntervalMinutes { get; private set; }
        public bool ApplyToAll { get; private set; }
        public bool UseSmartDistribution { get; private set; }
        public int TotalDurationMinutes { get; private set; }

        public BulkUnlockTimeDialog(int? autoCloseMinutes = null)
        {
            InitializeComponent();
            this._StartTimePicker.Value = DateTime.Now;
            this._IntervalNumeric.Value = 5;
            this._ApplyToAllRadio.Checked = true;
            
            // Default to Smart distribution with auto-close timer if available
            if (autoCloseMinutes.HasValue && autoCloseMinutes.Value > 0)
            {
                this._UseSmartCheck.Checked = true;
                this._TotalDurationNumeric.Value = autoCloseMinutes.Value;
                this._UseSmartCheck.Enabled = false; // Lock to auto mode
                this._TotalDurationNumeric.Enabled = false; // Lock to auto-close time
                
                // Update instruction to show auto mode
                double hours = autoCloseMinutes.Value / 60.0;
                this._InstructionLabel.Text = 
                    $"AUTO MODE: Using remaining auto-close time ({hours:F1}h). Achievements will unlock realistically based on rarity across this time period. Common achievements unlock quickly, rare ones take most of the time.";
            }
            else
            {
                this._UseSmartCheck.Checked = true; // Default to Smart
                this._TotalDurationNumeric.Value = 60; // default 1 hour
            }
        }

        private void OnOK(object sender, EventArgs e)
        {
            this.StartTime = this._StartTimePicker.Value;
            this.IntervalMinutes = (int)this._IntervalNumeric.Value;
            this.ApplyToAll = this._ApplyToAllRadio.Checked;
            this.UseSmartDistribution = this._UseSmartCheck.Checked;
            this.TotalDurationMinutes = (int)this._TotalDurationNumeric.Value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
