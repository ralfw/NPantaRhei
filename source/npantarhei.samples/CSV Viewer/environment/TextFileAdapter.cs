using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSV_Viewer.environment
{
    class TextFileAdapter
    {
        public IEnumerable<string> Read_all_lines(string filename)
        {
            return File.ReadAllLines(filename);
        }
    }
}
