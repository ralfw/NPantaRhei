using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using npantarhei.interviz.graphviz.adapter;

namespace npantarhei.interviz
{
    public partial class WinDesigner : Form
    {
        private string _flow_filename;
        private IEnumerable<string> _flownames = new string[0];
        private NodeMap _nodeMap;
        private NodeMap.NodeArea _currentNode;


        public WinDesigner()
        {
            InitializeComponent();

            openTextfileDialog.InitialDirectory = Environment.CurrentDirectory;
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            Remember_filename("");
            txtSource.Text = "";
        }

        private void menuLoad_Click(object sender, EventArgs e)
        {
            openTextfileDialog.FileName = Path.GetFileName(_flow_filename);
            if (openTextfileDialog.ShowDialog() != DialogResult.OK) return;

            openTextfileDialog.InitialDirectory = Path.GetDirectoryName(openTextfileDialog.FileName);
            Load_flow_from_textfile(openTextfileDialog.FileName);
        }

        private void menuLoadFromAssembly_Click(object sender, EventArgs e)
        {
            if (openAssemblyDialog.ShowDialog() == DialogResult.OK)
            {
                Load_flow_from_assembly(openAssemblyDialog.FileName);
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            if (_flow_filename == "")
                menuSaveAs_Click(sender, e);
            else
                Save_flow(new Tuple<string, string>(_flow_filename, txtSource.Text));
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Path.GetFileName(_flow_filename);
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            Remember_filename(saveFileDialog1.FileName);
            Save_flow(new Tuple<string, string>(saveFileDialog1.FileName, txtSource.Text));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNavigateBack_Click(object sender, EventArgs e)
        {
            Navigate_backward(txtSource.Lines);
        }

        private void btnNavigateForward_Click(object sender, EventArgs e)
        {
            Navigate_forward(txtSource.Lines);
        }

        private void cboFlows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFlows.IsAccessible) Jump_to_flow(new Tuple<string[], string>(txtSource.Lines, cboFlows.Text));
        }

        private void refreshGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Request_redraw();
            
        }

        private void txtSource_SelectionChanged(object sender, EventArgs e)
        {
            Request_redraw();
        }

        private void Request_redraw()
        {
            Redraw(new Tuple<string[], int>(txtSource.Lines, txtSource.GetLineFromCharIndex(txtSource.SelectionStart)));
        }


        private void picGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (_nodeMap == null) return;

            var mr = new Rectangle(e.X, e.Y, 1, 1);
            _currentNode = _nodeMap.NodeAreas.FirstOrDefault(a => !a.Name.StartsWith(".") && 
                                                                  _flownames.Contains(a.Name) &&
                                                                  a.Rectangle.IntersectsWith(mr));
            this.Cursor = _currentNode == null ? Cursors.Arrow : Cursors.Hand;
        }

        private void picGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (_currentNode != null) Jump_to_flow(new Tuple<string[], string>(txtSource.Lines, _currentNode.Name));
        }


        public void Display_flow(Tuple<string, string> flow)
        {
            Remember_filename(flow.Item1);
            txtSource.Text = flow.Item2;
            Redraw(new Tuple<string[], int>(txtSource.Lines, 0));
        }

        private void Remember_filename(string filename)
        {
            _flow_filename = filename;
            this.Text = "NPantaRhei Interactive Visualizer [" + Path.GetFileName(_flow_filename) + "]";
        }


        public void Display_graph(Tuple<Image,NodeMap> graph)
        {
            picGraph.Image = graph.Item1;
            _nodeMap = graph.Item2;

            //using (var g = Graphics.FromImage(picGraph.Image))
            //{
            //    foreach (var a in graph.Item2.NodeAreas)
            //        g.DrawRectangle(Pens.Red, a.Rectangle);
            //}
        }

        public void Display_flownames(Tuple<string[], int> flownames)
        {
            _flownames = flownames.Item1;

            cboFlows.IsAccessible = false;
            cboFlows.Items.Clear();
            cboFlows.Items.AddRange(flownames.Item1);
            cboFlows.SelectedIndex = flownames.Item2;
            cboFlows.IsAccessible = true;
        }

        public void Move_cursor_to_flow_header(int linenumber)
        {
            if (linenumber == 0)
                txtSource.SelectionStart = 0;
            else
            {
                var charCount = 0;
                var row = 0;
                foreach (var line in txtSource.Lines)
                {
                    charCount += line.Length + 1; row++;
                    if (row == linenumber) 
                    { 
                        txtSource.SelectionStart = charCount;
                        txtSource.SelectionLength = txtSource.Lines[linenumber].Length; 
                        break; 
                    }
                }
            }
            txtSource.ScrollToCaret();
            txtSource.Focus();
        }


        public event Action<Tuple<string[], int>> Redraw;
        public event Action<string> Load_flow_from_textfile;
        public event Action<string> Load_flow_from_assembly;
        public event Action<Tuple<string, string>> Save_flow;
        public event Action<Tuple<string[], string>> Jump_to_flow;
        public event Action<string[]> Navigate_backward;
        public event Action<string[]> Navigate_forward;
    }
}
