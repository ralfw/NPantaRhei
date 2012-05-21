using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSV_Viewer.frontend
{
    class Frontend
    {
        public void Output(IEnumerable<string> lines)
        {
            Console.WriteLine();
            lines.ToList()
                 .ForEach(Console.WriteLine);
        }

        public void Menu(Action<int> exit)
        {
            while (true)
            {
                Console.Write("F(irst, L(ast, N(ext, P(rev, eX(it: ");
                var k = Console.ReadKey();
                Console.WriteLine();

                switch(k.KeyChar)
                {
                    case 'f':
                    case 'F':
                        displayFirstPage();
                        return;

                    case 'l':
                    case 'L':
                        displayLastPage();
                        return;

                    case 'n':
                    case 'N':
                        displayNextPage();
                        return;

                    case 'p':
                    case 'P':
                        displayPrevPage();
                        return;

                    case 'x':
                    case 'X':
                        exit(0);
                        return;
                }
            }
        }

        public event Action displayFirstPage;
        public event Action displayLastPage;
        public event Action displayNextPage;
        public event Action displayPrevPage;
    }
}
