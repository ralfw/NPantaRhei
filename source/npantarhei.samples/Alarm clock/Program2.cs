using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using npantarhei.runtime;

namespace Alarm_clock
{
    /*
     * Dialog and clock are implemented as event sources, not operations.
     * They are not part of a flow for the purpose of starting it.
     * Flow initiation is viewed as happening in the environment.
     */
    static class Program2
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
                // Define flow
                // Feature: set alarm
                fr.AddStream(".setAlarm", "Join.in0");
                fr.AddStream(".setAlarm", "Alarm switched on");
                fr.AddStream(".now", "Join.in1");
                fr.AddStream("Join", "Calc time diff");
                fr.AddStream("Calc time diff", "Display time diff");

                // Feature: stop alarm
                fr.AddStream(".resetAlarm", "Join.reset");
                fr.AddStream(".resetAlarm", "Alarm switched off");
                fr.AddStream(".resetAlarm", "Stop alarm");

                // Feature: sound alarm
                fr.AddStream("Calc time diff", "Alarm time reached");
                fr.AddStream("Alarm time reached", "Sound alarm");

                // Register operations
                var dlg = new Dialog2();
                var clock = new Clock2();
                var player = new Soundplayer();

                fr.AddOperations(new FlowOperationContainer()
                                     .AddAction("Alarm switched off", dlg.Alarm_switched_off).MakeSync()
                                     .AddAction("Alarm switched on", dlg.Alarm_switched_on).MakeSync()
                                     .AddAction<TimeSpan>("Alarm time reached", Alarm_time_reached)
                                     .AddFunc<Tuple<DateTime,DateTime>,TimeSpan>("Calc time diff", Calc_time_diff)
                                     .AddAction<TimeSpan>("Display time diff", dlg.Display_time_diff).MakeSync()
                                     .AddManualResetJoin<DateTime, DateTime>("Join")
                                     .AddAction("Sound alarm", player.Start_playing)
                                     .AddAction("Stop alarm", player.Stop_playing)
                                     .Operations);

                // Wire-up event sources
                dlg.SetAlarm += fr.CreateEventProcessor<DateTime>(".setAlarm");
                dlg.ResetAlarm += fr.CreateEventProcessor(".resetAlarm");
                clock.CurrentTime += fr.CreateEventProcessor<DateTime>(".now");

                fr.Message += Console.WriteLine; 
                fr.UnhandledException += Console.WriteLine;

                // Execute flow
                // Feature: start application
                Application.Run(dlg); // needs to run on this thread; cannot be done on flow runtime thread.
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
