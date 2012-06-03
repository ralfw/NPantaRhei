using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.interviz.data;

namespace npantarhei.interviz
{
    class Navigator
    {
        private readonly NavigationHistory _history = new NavigationHistory();
        private string _previousFlowname;


        public Navigator() { _history.Extend("/"); }


        public void Extend_history(Tuple<string[], string> flow)
        {
            var flowname = flow.Item2;
            if (flowname == _previousFlowname) return;

            _history.Extend(flowname);
            _previousFlowname = flowname;
        }

        public void Navigate_backward(string[] flowSource, Action<Tuple<string[],string>> continueWith)
        {
            string flowname;
            if (_history.GoBack(out flowname)) continueWith(new Tuple<string[], string>(flowSource, flowname));
        }

        public void Navigate_forward(string[] flowSource, Action<Tuple<string[], string>> continueWith)
        {
            string flowname;
            if (_history.GoForward(out flowname)) continueWith(new Tuple<string[], string>(flowSource, flowname));
        }
    }
}
