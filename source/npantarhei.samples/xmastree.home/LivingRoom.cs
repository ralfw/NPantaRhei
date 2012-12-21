using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using npantarhei.runtime.contract;

namespace xmastree.home
{
    public partial class LivingRoom : Form
    {
        public LivingRoom()
        {
            InitializeComponent();
        }


        private void trackTreeHeight_ValueChanged(object sender, EventArgs e)
        {
            Order_tree(trackTreeHeight.Value);
        }


        public event Action<int> Order_tree;


        [DispatchedMethod]
        public void Setup_tree(string[] tree)
        {
            lstTree.Items.Clear();
            foreach (var b in tree)
                lstTree.Items.Add(b);
        }
    }
}
