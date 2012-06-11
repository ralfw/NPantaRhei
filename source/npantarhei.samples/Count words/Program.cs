using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;
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
            using (var fr = new FlowRuntime())
            {
                var frc = new FlowRuntimeConfiguration();
                frc.AddStream(".in", "Pushc");
                frc.AddStream("Pushc", "Find Files");
                frc.AddStream("Pushc.exception", "Handle Exception");
                frc.AddStream("Find Files", "Count Words");
                frc.AddStream("Count Words", "popc");
                frc.AddStream("Popc", "Total");
                frc.AddStream("Total", ".out");

                frc.AddFunc<IEnumerable<string>, IEnumerable<int>>("Count Words", Count_words3)
                   .AddFunc<string, IEnumerable<String>>("Find Files", Find_files)
                   .AddAction<FlowRuntimeException>("Handle Exception", Handle_exception)
                   .AddPopCausality("Popc")
                   .AddPushCausality("Pushc")
                   .AddFunc<IEnumerable<int>, Tuple<int, int>>("Total", Total);
                fr.Configure(frc);

                fr.Process(new Message(".in", "x"));

                Tuple<int, int> result = null;
                fr.WaitForResult(5000, _ => result = (Tuple<int, int>)_.Data);

                if (result != null)
                    Console.WriteLine("{0} words in {1} files", result.Item2, result.Item1);
            }
        }

        static void Handle_exception(FlowRuntimeException ex)
        {
            Console.WriteLine("*** Exception during file processing: {0}", ex.InnerException);
        }

        static IEnumerable<int> Count_words3(IEnumerable<string> filenames)
        {
            throw new ApplicationException("arghhh!");
        }


        static void Main3(string[] args)
        {
            using (var fr = new FlowRuntime())
            {
                var frc = new FlowRuntimeConfiguration();
                frc.AddStream(".in", "Find files");
                frc.AddStream("Find files", "Count words");
                frc.AddStream("Count words", "Total");
                frc.AddStream("Total", ".out");

                frc.AddFunc<string, IEnumerable<String>>("Find files", Find_files)
                   .AddFunc<IEnumerable<string>, IEnumerable<int>>("Count words", Count_words2)
                   .AddFunc<IEnumerable<int>, Tuple<int, int>>("Total", Total);
                fr.Configure(frc);

                var start = DateTime.Now;
                fr.Process(new Message(".in", "x"));

                Tuple<int, int> result = null;
                fr.WaitForResult(5000, _ => result = (Tuple<int, int>)_.Data);
                var delta = DateTime.Now.Subtract(start);

                Console.WriteLine("{0} words in {1} files, {2}msec", result.Item2, result.Item1, delta);
            }
        }


        static void Main2(string[] args)
        {
            using(var fr = new FlowRuntime())
            {
                var frc = new FlowRuntimeConfiguration();
                frc.AddStream(".in", "Find_files");
                frc.AddStream("Find_files", "scatter");
                frc.AddStream("scatter.stream", "Count_words");
                frc.AddStream("scatter.count", "gather.count");
                frc.AddStream("Count_words", "gather.stream");
                frc.AddStream("gather", "Total");
                frc.AddStream("Total", ".out");

                frc.AddFunc<string, IEnumerable<String>>("Find_files", Find_files).MakeAsync()
                   .AddFunc<string,int>("Count_words", Count_words).MakeParallel()
                   .AddFunc<IEnumerable<int>, Tuple<int,int>>("Total", Total)
                   .AddOperation(new Scatter<string>("scatter"))
                   .AddOperation(new Gather<int>("gather"));

                fr.Configure(frc);

                var start = DateTime.Now;
                fr.Process(new Message(".in", "x"));

                Tuple<int,int> result = null;
                fr.WaitForResult(5000, _ => result = (Tuple<int,int>)_.Data);
                var delta = DateTime.Now.Subtract(start);

                Console.WriteLine("{0} words in {1} files, {2}msec", result.Item2, result.Item1, delta);
            }
        }

        static IEnumerable<string> Find_files(string path)
        {
            return new[] {path+"a", path+"bc", path+"def", path+"ghij", path+"klmno"};
        }

        static IEnumerable<int> Count_words2(IEnumerable<string> filenames)
        {
            return filenames.Select(fn => fn.Length);
        }

        static int Count_words(string filename)
        {
            int delay = filename.Length*50;
            Console.WriteLine("Count_words delay: {0} @ {1}", delay, Thread.CurrentThread.GetHashCode());
            Thread.Sleep(delay);
            return filename.Length;
        }

        static Tuple<int,int> Total(IEnumerable<int> wordCounts)
        {
            return new Tuple<int, int>(wordCounts.Count(), wordCounts.Sum());
        }
    }


    class Total : AOperation
    {
        public Total(string name) : base(name) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            Console.WriteLine("total class");
            var wordCounts = (IEnumerable<int>)input.Data;
            var result = new Tuple<int, int>(wordCounts.Count(), wordCounts.Sum());
            continueWith(new Message(base.Name, result));
        }
    }
}
