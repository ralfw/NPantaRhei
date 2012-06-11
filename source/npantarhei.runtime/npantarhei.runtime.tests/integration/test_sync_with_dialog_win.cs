using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.integration
{
    public partial class test_sync_with_dialog_win : Form
    {
        private readonly IFlowRuntime _fr;

        public test_sync_with_dialog_win()
        {
            InitializeComponent();

            var frc = new FlowRuntimeConfiguration();
            frc.AddStream(new Stream(".in", "gettime"));
            frc.AddStream(new Stream("gettime", "showtime"));

            var opcont = new FlowRuntimeConfiguration();
            opcont.AddFunc("gettime", () => DateTime.Now);
            opcont.AddAction<DateTime>("showtime", (DateTime _) =>
                                                       {
                                                           Console.WriteLine("event handler thread: {0}",
                                                                             Thread.CurrentThread.GetHashCode());
                                                           label1.Text = _.ToString();
                                                           listBox1.Items.Add(_.ToString());
                                                       })
                  .MakeSync();

            frc.AddOperations(opcont.Operations);

            _fr = new FlowRuntime(frc);
        }


        protected override void OnClosed(EventArgs e)
        {
            _fr.Dispose();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button thread: {0}", Thread.CurrentThread.GetHashCode());
            _fr.Process(new npantarhei.runtime.messagetypes.Message(".in", null));
        }
    }
}
