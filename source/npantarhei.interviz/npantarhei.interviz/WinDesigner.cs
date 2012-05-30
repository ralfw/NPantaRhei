using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace npantarhei.interviz
{
    public partial class WinDesigner : Form
    {
        private string _flow_filename;


        public WinDesigner()
        {
            InitializeComponent();

            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            Remember_filename("");
            txtSource.Text = "";
        }

        private void menuLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = Path.GetFileName(_flow_filename);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
                Load_flow(openFileDialog1.FileName);
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
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Remember_filename(saveFileDialog1.FileName);
                Save_flow(new Tuple<string, string>(saveFileDialog1.FileName, txtSource.Text));
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
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


        public void Display_flow(Tuple<string, string> flow)
        {
            Remember_filename(flow.Item1);
            txtSource.Text = flow.Item2;
        }

        private void Remember_filename(string filename)
        {
            _flow_filename = filename;
            this.Text = "NPantaRhei Interactive Visualizer [" + Path.GetFileName(_flow_filename) + "]";
        }


        public void Display_graph(Image graph)
        {
            picGraph.Image = graph;
        }


        public event Action<Tuple<string[], int>> Redraw;
        public event Action<string> Load_flow;
        public event Action<Tuple<string, string>> Save_flow;


    }
}
