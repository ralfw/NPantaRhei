namespace Alarm_clock
{
    partial class Dialog2
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
            this.lblTimeDiff = new System.Windows.Forms.Label();
            this.txtAlarmTime = new System.Windows.Forms.TextBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTimeDiff
            // 
            this.lblTimeDiff.AutoSize = true;
            this.lblTimeDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeDiff.Location = new System.Drawing.Point(12, 58);
            this.lblTimeDiff.Name = "lblTimeDiff";
            this.lblTimeDiff.Size = new System.Drawing.Size(158, 26);
            this.lblTimeDiff.TabIndex = 0;
            this.lblTimeDiff.Text = "<no alarm set>";
            // 
            // txtAlarmTime
            // 
            this.txtAlarmTime.Location = new System.Drawing.Point(12, 12);
            this.txtAlarmTime.Name = "txtAlarmTime";
            this.txtAlarmTime.Size = new System.Drawing.Size(100, 20);
            this.txtAlarmTime.TabIndex = 1;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(118, 10);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(78, 23);
            this.btnStartStop.TabIndex = 2;
            this.btnStartStop.Text = "Set Alarm";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // Dialog2
            // 
            this.AcceptButton = this.btnStartStop;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 107);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.txtAlarmTime);
            this.Controls.Add(this.lblTimeDiff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog2";
            this.Text = "Alarm Clock 2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTimeDiff;
        private System.Windows.Forms.TextBox txtAlarmTime;
        private System.Windows.Forms.Button btnStartStop;

    }
}

