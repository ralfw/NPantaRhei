using System.Collections.Generic;

namespace npantarhei.interviz.data
{
    internal class NavigationHistory
    {
        private readonly Stack<string> _thePast = new Stack<string>();
        private string _thePresent;
        private Stack<string> _theFuture = new Stack<string>();

        public void Extend(string item)
        {
            if (_thePresent != null) _thePast.Push(_thePresent);
            _thePresent = item;
            _theFuture = new Stack<string>();
        }

        public bool GoBack(out string item)
        {
            item = "";
            if (_thePast.Count == 0) return false;

            item = _thePast.Pop();
            if (_thePresent != null) _theFuture.Push(_thePresent);
            _thePresent = item;

            return true;
        }

        public bool GoForward(out string item)
        {
            item = "";
            if (_theFuture.Count == 0) return false;

            item = _theFuture.Pop();
            if (_thePresent != null) _thePast.Push(_thePresent);
            _thePresent = item;

            return true;
        }
    }
}
