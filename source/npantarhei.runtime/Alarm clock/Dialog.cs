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

        void Show_dialog()
        {
            Application.Run(this);
        }


        private Action<IMessage> _continueWith;

        void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            switch(input.Port.Name.ToLower())
            {
                case "config":
                    _continueWith = continueWith;
                    break;
                case "show":
                    this.Show_dialog();
                    break;
            }
        }


        public OperationAdapter Implementation
        {
            get { return this.Process; }
        }
    }
}
