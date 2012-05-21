using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSV_Viewer.data_model
{
    class PageBuffer
    {
        private readonly int _pageLength;
        private int _index_of_current_page_top_line;

        private readonly List<string> _lines;
 

        public PageBuffer(int pageLength)
        {
            _pageLength = pageLength;
            _lines = new List<string>();

            GotoFirst();
        }

        public void AddLine(string line) { _lines.Add(line); }

        public void GotoFirst()
        {
            _index_of_current_page_top_line = 1;
        }

        public void GotoLast()
        {
            int remainder;
            var _ = Math.DivRem(_lines.Count-1, _pageLength, out remainder);
            _index_of_current_page_top_line = remainder == 0 ? _lines.Count - _pageLength : _lines.Count - remainder;
        }

        public void GotoNext()
        {
            _index_of_current_page_top_line += _pageLength;
            if (_index_of_current_page_top_line > (_lines.Count-1)) GotoLast();
        }

        public void GotoPrev()
        {
            _index_of_current_page_top_line -= _pageLength;
            if (_index_of_current_page_top_line < 1) GotoFirst();
        }

        public Page Current 
        { 
            get
            {
                return new Page(_lines[0], 
                                _lines.GetRange(_index_of_current_page_top_line, Math.Min(_pageLength, _lines.Count - _index_of_current_page_top_line)));
            } 
        }
    }
}
