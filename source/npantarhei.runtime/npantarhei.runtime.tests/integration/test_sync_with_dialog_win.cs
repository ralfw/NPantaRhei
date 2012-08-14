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
            frc.AddStream(new Stream(".inMakeSync", "gettime"));
            frc.AddStream(new Stream("gettime", "showtimeMakeSync"));
            frc.AddStream(new Stream(".inAttribute", "gettime2"));
            frc.AddStream(new Stream("gettime2", "showtimeAttribute"));

            var opcont = new FlowRuntimeConfiguration();
            opcont.AddFunc("gettime", () => DateTime.Now);
            opcont.AddFunc("gettime2", () => DateTime.Now);
            opcont.AddAction<DateTime>("showtimeMakeSync", (DateTime _) =>
                                                       {
                                                           Console.WriteLine("event handler thread: {0}",
                                                                             Thread.CurrentThread.GetHashCode());
                                                           label1.Text = _.ToString();
                                                           listBox1.Items.Add(_.ToString());
                                                       })
                  .MakeDispatched();
            opcont.AddAction<DateTime>("showtimeAttribute", this.ShowTimeAttribute);

            frc.AddOperations(opcont.Operations);

            _fr = new FlowRuntime(frc);
            _fr.UnhandledException += Console.WriteLine;
        }



        protected override void OnClosed(EventArgs e)
        {
            _fr.Dispose();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button thread: {0}", Thread.CurrentThread.GetHashCode());
            _fr.Process(".inMakeSync", null);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button thread: {0}", Thread.CurrentThread.GetHashCode());
            _fr.Process(".inAttribute", null);
        }


        [DispatchedMethod]
        public void ShowTimeAttribute(DateTime dt)
        {
            Console.WriteLine("event handler thread: {0}", Thread.CurrentThread.GetHashCode());
            label1.Text = dt.ToString();
            listBox1.Items.Add(dt.ToString() + "/attr");
        }

    }
}
