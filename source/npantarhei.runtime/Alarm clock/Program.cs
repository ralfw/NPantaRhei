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

                // Define flow
                // Configure active operations
                fr.AddStream(".config", "dialog.config");
                fr.AddStream(".config", "clock.config");

                // Feature: close application
                fr.AddStream("dialog.closed", ".stop");

                // Feature: set alarm
                fr.AddStream("dialog.setAlarm", "join.in0");
                fr.AddStream("dialog.setAlarm", "Alarm switched on");
                fr.AddStream("clock.now", "join.in1");
                fr.AddStream("join", "calc time diff");
                fr.AddStream("calc time diff", "Display time diff");

                // Feature: stop alarm
                fr.AddStream("dialog.stopAlarm", "join.reset");
                fr.AddStream("dialog.stopAlarm", "Alarm switched off");

                // Feature: sound alarm
                fr.AddStream("calc time diff", "alarm time reached");
                fr.AddStream("alarm time reached", "sound alarm");
                fr.AddStream("alarm time reached", "join.reset");
                fr.AddStream("alarm time reached", "Alarm switched off");

                // Register operations
                var player = new Soundplayer();

                fr.AddOperation(dlg);
                fr.AddOperation(new Clock());
                fr.AddOperations(new FlowOperationContainer()
                                     .AddManualResetJoin<DateTime, DateTime>("join")
                                     .AddFunc<Tuple<DateTime,DateTime>,TimeSpan>("calc time diff", Calc_time_diff)
                                     .AddAction("Alarm switched on", dlg.Alarm_switched_on).MakeSync()
                                     .AddAction("Alarm switched off", dlg.Alarm_switched_off).MakeSync()
                                     .AddAction<TimeSpan>("Display time diff", dlg.Display_time_diff).MakeSync()
                                     .AddAction<TimeSpan>("alarm time reached", Alarm_time_reached)
                                     .AddAction("sound alarm", player.Start_playing)
                                     .Operations);

                fr.Message += Console.WriteLine; 
                fr.UnhandledException += Console.WriteLine;

                // Execute flow
                fr.Start();

                fr.Process(new npantarhei.runtime.messagetypes.Message(".config", null));

                // Feature: start application
                Application.Run(dlg);
            }
        }


        static TimeSpan Calc_time_diff(Tuple<DateTime, DateTime> input)
        {
            var alarm_time = input.Item1;
            var current_time = input.Item2;

            return alarm_time.Subtract(current_time);
        }

        static void Alarm_time_reached(TimeSpan timeDiff, Action continueWith)
        {
            if (timeDiff.Hours == 0 && timeDiff.Minutes == 0 && timeDiff.Seconds == 0) 
                continueWith();
        }
    }
}
