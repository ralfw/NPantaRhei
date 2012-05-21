using System.Collections.Generic;

namespace CSV_Viewer.data_model
{
    class Page
    {
        public Page(string headerLine, IEnumerable<string> lines)
        {
            HeaderLine = headerLine;
            Lines = lines;
        }

        public string HeaderLine { get; set; }
        public IEnumerable<string> Lines { get; private set; } 
    }
}