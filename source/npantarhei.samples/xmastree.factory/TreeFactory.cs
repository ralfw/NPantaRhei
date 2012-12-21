using System;
using System.Collections.Generic;
using System.Linq;

namespace xmastree.factory
{
    class TreeFactory
    {
        public List<string> Grow_branches(int numberOfbranches)
        {
            Console.WriteLine("  producing a {0} branch tree...", numberOfbranches);

            var branches = new List<string>();
            for (var i = 1; i <= numberOfbranches; i++ )
                branches.Add("".PadLeft(2*i-1, '*'));
            return branches;
        }

        public List<string> Erect_tree(List<string> branches)
        {
            return branches.Select((t, i) => "".PadLeft(branches.Count - i - 1) + t).ToList();
        }

        public string[] Add_stem(List<string> tree)
        {
            tree.Add("".PadLeft(tree.Count-1) + "I");
            return tree.ToArray();
        }
    }
}
