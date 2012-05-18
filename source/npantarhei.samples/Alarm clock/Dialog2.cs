using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using npantarhei.runtime.contract;

namespace Alarm_clock
{
    public partial class Dialog2 : Form
    {
        public Dialog2()
        {
            InitializeComponent();
        }


        public event Action<DateTime> SetAlarm;
        public event Action ResetAlarm;
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text.StartsWith("Set"))
                SetAlarm(DateTime.Parse(this.txtAlarmTime.Text));
            else
                ResetAlarm();
        }


        public void Display_time_diff(TimeSpan timeDiff)
        {
            lblTimeDiff.Text = timeDiff.ToString(@"hh\:mm\:ss");
        }

        public void Alarm_switched_on()
        {
            btnStartStop.Text = "Stop Alarm";
        }

        public void Alarm_switched_off()
        {
            lblTimeDiff.Text = "<no alarm set>";
            btnStartStop.Text = "Set Alarm";
        }
    }
}
