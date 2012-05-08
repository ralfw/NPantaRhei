using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using npantarhei.runtime;
using npantarhei.runtime.messagetypes;

namespace Alarm_clock
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

            using(var fr = new FlowRuntime())
            {
                fr.AddStream(".config", "dialog.config");
                fr.AddStream(".run", "dialog.show");
                fr.AddStream("dialog.closed", ".stop");

                fr.AddOperation(new Dialog());

                fr.Message += Console.WriteLine; 
                fr.UnhandledException += Console.WriteLine;

                fr.Start();

                fr.Process(new npantarhei.runtime.messagetypes.Message(".config", null));
                fr.Process(new npantarhei.runtime.messagetypes.Message(".run", null));

                fr.WaitForResult(Timeout.Infinite);
            }
        }
    }
}
