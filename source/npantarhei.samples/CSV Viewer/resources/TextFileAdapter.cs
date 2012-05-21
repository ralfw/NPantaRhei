using System.Collections.Generic;
using System.IO;

namespace CSV_Viewer.resources
{
    class TextFileAdapter
    {
        public IEnumerable<string> Read_all_lines(string filename)
        {
            return File.ReadAllLines(filename);
        }
    }
}
