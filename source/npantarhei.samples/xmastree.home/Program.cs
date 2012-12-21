using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using npantarhei.distribution.wcf;
using npantarhei.runtime;

namespace xmastree.home
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var lr = new LivingRoom();
            var config = new FlowRuntimeConfiguration()
                            .AddEventBasedComponent("livingroom", lr)
                            .AddOperation(new WcfStandInOperation("stand-in", "localhost:8100", "localhost:8000"))
                            .AddStreamsFrom(@"
                                                /
                                                .run, stand-in#xmasfactory.Build_tree

                                                livingroom.Order_tree, stand-in#xmasfactory.Build_tree
                                                stand-in#xmasfactory.Deliver_tree, livingroom.Setup_tree
                                             ");
            using (var fr = new FlowRuntime(config))
            {
                fr.Process(".run", 2);
                Application.Run(lr);
            }
        }
    }
}
