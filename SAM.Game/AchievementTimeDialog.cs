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
    internal partial class AchievementTimeDialog : Form
    {
        public DateTime? ScheduledTime { get; private set; }

        public AchievementTimeDialog(string achievementName, DateTime? currentTime)
        {
            InitializeComponent();
            this.Text = "Set Unlock Time - " + achievementName;
            
            if (currentTime.HasValue)
            {
                this._DateTimePicker.Value = currentTime.Value;
                this._EnableScheduleCheckBox.Checked = true;
            }
            else
            {
                this._DateTimePicker.Value = DateTime.Now;
                this._EnableScheduleCheckBox.Checked = false;
            }

            UpdateControls();
        }

        private void UpdateControls()
        {
            this._DateTimePicker.Enabled = this._EnableScheduleCheckBox.Checked;
        }

        private void OnEnableScheduleChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void OnOK(object sender, EventArgs e)
        {
            if (this._EnableScheduleCheckBox.Checked)
            {
                this.ScheduledTime = this._DateTimePicker.Value;
            }
            else
            {
                this.ScheduledTime = null;
            }

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
