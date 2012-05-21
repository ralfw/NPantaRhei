using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;

namespace CSV_Viewer.formatter
{
    class Formatter
    {
        public IEnumerable<string> Format(Page csvLinePage)
        {
            yield return csvLinePage.HeaderLine;
            yield return "---";
            foreach (var l in csvLinePage.Lines) yield return l;
        }
    }
}
