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
    public partial class Dialog : Form, IOperation
    {
        public Dialog()
        {
            InitializeComponent();
        }

        private void Dialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _continueWith(new npantarhei.runtime.messagetypes.Message(this.Name + ".closed", null));
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text.StartsWith("Set"))
                _continueWith(new npantarhei.runtime.messagetypes.Message(this.Name + ".setAlarm",
                                                                          DateTime.Parse(this.txtAlarmTime.Text)));
            else
                _continueWith(new npantarhei.runtime.messagetypes.Message(this.Name + ".stopAlarm", null));
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


        private Action<IMessage> _continueWith;

        void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            switch(input.Port.Name.ToLower())
            {
                case "config":
                    _continueWith = continueWith;
                    break;
            }
        }


        public OperationAdapter Implementation
        {
            get { return this.Process; }
        }
    }
}
