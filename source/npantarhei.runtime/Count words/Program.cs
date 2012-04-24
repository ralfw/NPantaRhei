using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace Count_words
{
    /*
     * Find all .txt files in a directory tree.
     * Count the words in each file and sum up the counts.
     * Show the number of files and the total word count as a result.
     */
    class Program
    {
        static void Main(string[] args)
        {
            using(var fr = new FlowRuntime())
            {
                fr.AddStream(".in", "findFiles");
                fr.AddStream("findFiles", "scatter");
                fr.AddStream("scatter.stream", "countWords");
                fr.AddStream("scatter.count", "gather.count");
                fr.AddStream("countWords", "gather.stream");
                fr.AddStream("gather", "total");
                fr.AddStream("total", ".out");

                var foc = new FlowOperationContainer()
                    .AddFunc<string, IEnumerable<String>>("findFiles", findFiles)
                    .AddFunc<string,int>("countWords", countWords)
                    .AddFunc<IEnumerable<int>, Tuple<int,int>>("total", total);
                fr.AddOperations(foc.Operations);

                fr.AddOperation(new Scatter<string>("scatter"));
                fr.AddOperation(new Gather<int>("gather"));

                fr.Start();

                fr.Process(new Message(".in", "x"));

                Tuple<int,int> result = null;
                fr.WaitForResult(1000, _ => result = (Tuple<int,int>)_.Data);

                Console.WriteLine("{0} words in {1} files", result.Item2, result.Item1);
            }
        }

        static IEnumerable<string> findFiles(string path)
        {
            return new[] {path+"a", path+"bc", path+"def", path+"ghij", path+"klmno"};
        }

        static int countWords(string filename)
        {
            return filename.Length;
        }

        static Tuple<int,int> total(IEnumerable<int> wordCounts)
        {
            return new Tuple<int, int>(wordCounts.Count(), wordCounts.Sum());
        }
    }
}
