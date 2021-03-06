﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using npantarhei.runtime;
using npantarhei.runtime.patterns.operations;

namespace Alarm_clock
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
             * The dialog/clock is part of the flow. 
             * Since it fires events without prior input it is defined as an [ActiveOperation]
             */
            using (var fr = new FlowRuntime())
            {
                var frc = new FlowRuntimeConfiguration();

                // Define flow
                // Feature: close application
                frc.AddStream("Dialog.closed", ".stop");

                // Feature: set alarm
                frc.AddStream("Dialog.setAlarm", "Join.in0");
                frc.AddStream("Dialog.setAlarm", "Alarm switched on");
                frc.AddStream("Clock.now", "Join.in1");
                frc.AddStream("Join", "Calc time diff");
                frc.AddStream("Calc time diff", "Display time diff");

                // Feature: stop alarm
                frc.AddStream("Dialog.stopAlarm", "Join.reset");
                frc.AddStream("Dialog.stopAlarm", "Alarm switched off");
                frc.AddStream("Dialog.stopAlarm", "Stop alarm");

                // Feature: sound alarm
                frc.AddStream("Calc time diff", "Alarm time reached");
                frc.AddStream("Alarm time reached", "Sound alarm");

                fr.Configure(frc);

                // Register operations
                var dlg = new Dialog();
                var clock = new npantarhei.runtime.patterns.operations.Clock();
                var player = new Soundplayer();

                frc.AddOperation(dlg)
                   .AddOperation(clock)
                   .AddAction("Alarm switched off", dlg.Alarm_switched_off).MakeSync()
                   .AddAction("Alarm switched on", dlg.Alarm_switched_on).MakeSync()
                   .AddAction<TimeSpan>("Alarm time reached", Alarm_time_reached)
                   .AddFunc<Tuple<DateTime,DateTime>,TimeSpan>("Calc time diff", Calc_time_diff)
                   .AddAction<TimeSpan>("Display time diff", dlg.Display_time_diff).MakeSync()
                   .AddManualResetJoin<DateTime, DateTime>("Join")
                   .AddAction("Sound alarm", player.Start_playing)
                   .AddAction("Stop alarm", player.Stop_playing);
                fr.Configure(frc);

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
