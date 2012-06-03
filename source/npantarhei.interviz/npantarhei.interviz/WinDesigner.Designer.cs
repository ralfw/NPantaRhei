namespace npantarhei.interviz
{
    partial class WinDesigner
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnNavigateForward = new System.Windows.Forms.Button();
            this.btnNavigateBack = new System.Windows.Forms.Button();
            this.cboFlows = new System.Windows.Forms.ComboBox();
            this.txtSource = new System.Windows.Forms.RichTextBox();
            this.picGraph = new System.Windows.Forms.PictureBox();
            this.openTextfileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openAssemblyDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(997, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem1,
            this.toolStripMenuItem6,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(245, 22);
            this.toolStripMenuItem5.Text = "New";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.menuNew_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItem1.Size = new System.Drawing.Size(245, 22);
            this.toolStripMenuItem1.Text = "&Load flow from textfile...";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.menuLoad_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(245, 22);
            this.toolStripMenuItem6.Text = "Load flows from assembly...";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.menuLoadFromAssembly_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(245, 22);
            this.toolStripMenuItem2.Text = "&Save flow";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(245, 22);
            this.toolStripMenuItem3.Text = "Save flow as...";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(242, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshGraphToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // refreshGraphToolStripMenuItem
            // 
            this.refreshGraphToolStripMenuItem.Name = "refreshGraphToolStripMenuItem";
            this.refreshGraphToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshGraphToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.refreshGraphToolStripMenuItem.Text = "&Refresh graph";
            this.refreshGraphToolStripMenuItem.Click += new System.EventHandler(this.refreshGraphToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 429);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(997, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnNavigateForward);
            this.splitContainer1.Panel1.Controls.Add(this.btnNavigateBack);
            this.splitContainer1.Panel1.Controls.Add(this.cboFlows);
            this.splitContainer1.Panel1.Controls.Add(this.txtSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.picGraph);
            this.splitContainer1.Size = new System.Drawing.Size(997, 405);
            this.splitContainer1.SplitterDistance = 401;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnNavigateForward
            // 
            this.btnNavigateForward.Location = new System.Drawing.Point(43, 1);
            this.btnNavigateForward.Name = "btnNavigateForward";
            this.btnNavigateForward.Size = new System.Drawing.Size(42, 23);
            this.btnNavigateForward.TabIndex = 3;
            this.btnNavigateForward.Text = ">";
            this.btnNavigateForward.UseVisualStyleBackColor = true;
            this.btnNavigateForward.Click += new System.EventHandler(this.btnNavigateForward_Click);
            // 
            // btnNavigateBack
            // 
            this.btnNavigateBack.Location = new System.Drawing.Point(1, 1);
            this.btnNavigateBack.Name = "btnNavigateBack";
            this.btnNavigateBack.Size = new System.Drawing.Size(43, 23);
            this.btnNavigateBack.TabIndex = 2;
            this.btnNavigateBack.Text = "<";
            this.btnNavigateBack.UseVisualStyleBackColor = true;
            this.btnNavigateBack.Click += new System.EventHandler(this.btnNavigateBack_Click);
            // 
            // cboFlows
            // 
            this.cboFlows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFlows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFlows.FormattingEnabled = true;
            this.cboFlows.Location = new System.Drawing.Point(85, 2);
            this.cboFlows.Name = "cboFlows";
            this.cboFlows.Size = new System.Drawing.Size(314, 21);
            this.cboFlows.TabIndex = 1;
            this.cboFlows.SelectedIndexChanged += new System.EventHandler(this.cboFlows_SelectedIndexChanged);
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(0, 24);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(401, 378);
            this.txtSource.TabIndex = 0;
            this.txtSource.Text = "";
            this.txtSource.WordWrap = false;
            this.txtSource.SelectionChanged += new System.EventHandler(this.txtSource_SelectionChanged);
            // 
            // picGraph
            // 
            this.picGraph.Location = new System.Drawing.Point(8, 8);
            this.picGraph.Name = "picGraph";
            this.picGraph.Size = new System.Drawing.Size(592, 405);
            this.picGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picGraph.TabIndex = 1;
            this.picGraph.TabStop = false;
            this.picGraph.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picGraph_MouseClick);
            this.picGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picGraph_MouseMove);
            // 
            // openTextfileDialog
            // 
            this.openTextfileDialog.DefaultExt = "FLOW";
            this.openTextfileDialog.Filter = "Flow *.flow|*.flow|Text *.txt|*.txt";
            this.openTextfileDialog.RestoreDirectory = true;
            this.openTextfileDialog.Title = "Load flow from textfile";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "FLOW";
            this.saveFileDialog1.Filter = "Flow *.flow|*.flow|Text *.txt|*.txt";
            this.saveFileDialog1.Title = "Save flow";
            // 
            // openAssemblyDialog
            // 
            this.openAssemblyDialog.DefaultExt = "DLL";
            this.openAssemblyDialog.Filter = "Assembly *.dll|*.dll|Executable *.exe|*.exe";
            this.openAssemblyDialog.RestoreDirectory = true;
            this.openAssemblyDialog.Title = "Load flows from assembly";
            // 
            // WinDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 451);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "WinDesigner";
            this.Text = "NPantaRhei Interactive Visualizer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshGraphToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox txtSource;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.OpenFileDialog openTextfileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ComboBox cboFlows;
        private System.Windows.Forms.PictureBox picGraph;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.OpenFileDialog openAssemblyDialog;
        private System.Windows.Forms.Button btnNavigateForward;
        private System.Windows.Forms.Button btnNavigateBack;

    }
}

