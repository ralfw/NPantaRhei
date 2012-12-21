namespace xmastree.home
{
    partial class LivingRoom
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
            this.label1 = new System.Windows.Forms.Label();
            this.lstTree = new System.Windows.Forms.ListBox();
            this.trackTreeHeight = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackTreeHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // lstTree
            // 
            this.lstTree.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTree.ForeColor = System.Drawing.Color.Green;
            this.lstTree.FormattingEnabled = true;
            this.lstTree.ItemHeight = 18;
            this.lstTree.Location = new System.Drawing.Point(18, 12);
            this.lstTree.Name = "lstTree";
            this.lstTree.Size = new System.Drawing.Size(211, 202);
            this.lstTree.TabIndex = 1;
            // 
            // trackTreeHeight
            // 
            this.trackTreeHeight.Location = new System.Drawing.Point(235, 12);
            this.trackTreeHeight.Minimum = 2;
            this.trackTreeHeight.Name = "trackTreeHeight";
            this.trackTreeHeight.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackTreeHeight.Size = new System.Drawing.Size(45, 202);
            this.trackTreeHeight.TabIndex = 2;
            this.trackTreeHeight.Value = 2;
            this.trackTreeHeight.ValueChanged += new System.EventHandler(this.trackTreeHeight_ValueChanged);
            // 
            // LivingRoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 225);
            this.Controls.Add(this.trackTreeHeight);
            this.Controls.Add(this.lstTree);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LivingRoom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Order and set up your xmas tree";
            ((System.ComponentModel.ISupportInitialize)(this.trackTreeHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstTree;
        private System.Windows.Forms.TrackBar trackTreeHeight;
    }
}

