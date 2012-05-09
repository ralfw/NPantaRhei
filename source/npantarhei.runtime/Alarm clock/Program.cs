using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using npantarhei.runtime;

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
                var dlg = new Dialog();

                fr.AddStream(".config", "dialog.config");
                fr.AddStream(".config", "clock.config");

                fr.AddStream("dialog.closed", ".stop");

                fr.AddStream("dialog.setAlarm", "join.in0");
                fr.AddStream("dialog.setAlarm", "Alarm switched on");
                fr.AddStream("clock.now", "join.in1");
                fr.AddStream("join", "calc time diff");
                fr.AddStream("calc time diff", "Display time diff");

                fr.AddStream("dialog.stopAlarm", "join.reset");
                fr.AddStream("dialog.stopAlarm", "Alarm switched off");

                fr.AddOperation(dlg);
                fr.AddOperation(new Clock());
                fr.AddOperations(new FlowOperationContainer()
                                     .AddManualResetJoin<DateTime, DateTime>("join")
                                     .AddFunc<Tuple<DateTime,DateTime>,TimeSpan>("calc time diff", Calc_time_diff)
                                     .AddAction("Alarm switched on", dlg.Alarm_switched_on).MakeSync()
                                     .AddAction("Alarm switched off", dlg.Alarm_switched_off).MakeSync()
                                     .AddAction<TimeSpan>("Display time diff", dlg.Display_time_diff).MakeSync()
                                     .Operations);

                fr.Message += Console.WriteLine; 
                fr.UnhandledException += Console.WriteLine;

                fr.Start();

                fr.Process(new npantarhei.runtime.messagetypes.Message(".config", null));

                Application.Run(dlg);
            }
        }


        static TimeSpan Calc_time_diff(Tuple<DateTime, DateTime> input)
        {
            var alarm_time = input.Item1;
            var current_time = input.Item2;

            return alarm_time.Subtract(current_time);
        }
    }
}
