using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.distribution.wcf;
using npantarhei.runtime;

namespace xmastree.factory
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FlowRuntimeConfiguration()
                            .AddInstanceOperations(new TreeFactory())
                            .AddStreamsFrom(@"
                                                /
                                                .Build_tree@xmasfactory, xmasfactory.Build_tree
                                                xmasfactory.Deliver_tree, .Deliver_tree@xmasfactory


                                                xmasfactory
                                                .Build_tree, grow_branches
                                                grow_branches, erect_tree
                                                erect_tree, add_stem
                                                add_stem, .Deliver_tree
                                             ");

            using (var fr = new FlowRuntime(config))
            using(new WcfOperationHost(fr, "localhost:8000"))
            {
                Console.WriteLine("Waiting for xmas tree orders...");
                Console.ReadLine();
            }
        }
    }
}
