using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_EBCOperation
    {
        [Test]
        public void Wrap_EBC()
        {
            var sut = new EBCOperation("math", new MyEbc(), null, null);

            IMessage result = null;

            var input = new Message("math.Inc", 41);
            var ebcOp = sut.Create_method_operation(input);
            ebcOp.Implementation(input, _ => result = _, null);

            Assert.AreEqual("math.Result", result.Port.Fullname);
            Assert.AreEqual(42, (int)result.Data);


            input = new Message("math.Divide", new Tuple<int, int>(42, 7));
            ebcOp = sut.Create_method_operation(input);
            ebcOp.Implementation(input, _ => result = _, null);

            Assert.AreEqual("math.Result", result.Port.Fullname);
            Assert.AreEqual(6, (int)result.Data);

            input = new Message("math.Divide", new Tuple<int, int>(42, 0));
            ebcOp = sut.Create_method_operation(input);
            ebcOp.Implementation(input, _ => result = _, null);

            Assert.AreEqual("math.DivisionByZero", result.Port.Fullname);
            Assert.IsNull(result.Data);
        }


        [Test]
        public void Wrap_async_EBC_method()
        {
            var cache = new AsynchronizerCache();
            var sut = new EBCOperation("math", new MyAsyncEbc(), null, cache);

            var are = new AutoResetEvent(false);
            IMessage result = null;
            Thread methodThread = null;

            var input = new Message("math.Inc", 41);
            var methodOp = sut.Create_method_operation(input);
            Assert.IsInstanceOf<AsyncWrapperOperation>(methodOp);

            methodOp.Implementation(input, _ => { result = _; methodThread = Thread.CurrentThread; are.Set(); }, null);

            Assert.IsTrue(are.WaitOne(1000));
            Assert.AreEqual(42, (int)result.Data);
            Assert.AreNotSame(methodThread, Thread.CurrentThread);
        }


        [Test]
        public void Wrap_parallel_EBC_method()
        {
            var cache = new AsynchronizerCache();
            var sut = new EBCOperation("math", new MyParallelEbc(), null, cache);

            var are = new AutoResetEvent(false);
            IMessage result = null;
            Thread methodThread = null;

            var input = new Message("math.Inc", 41);
            var methodOp = sut.Create_method_operation(input);
            Assert.IsInstanceOf<AsyncWrapperOperation>(methodOp);

            methodOp.Implementation(input, _ => { result = _; methodThread = Thread.CurrentThread; are.Set(); }, null);

            Assert.IsTrue(are.WaitOne(1000));
            Assert.AreEqual(42, (int)result.Data);
            Assert.AreNotSame(methodThread, Thread.CurrentThread);
        }


        [Test]
        public void Async_EBC_method_throwing_exception()
        {
            var cache = new AsynchronizerCache();
            var sut = new EBCOperation("math", new MyAsyncEbc(), null, cache);

            var are = new AutoResetEvent(false);
            FlowRuntimeException result = null;

            var input = new Message("math.ThrowException", 41);
            var methodOp = sut.Create_method_operation(input);

            methodOp.Implementation(input, null, ex => { result = ex; are.Set(); });

            Assert.IsTrue(are.WaitOne(1000));
            Assert.IsInstanceOf<ApplicationException>(result.InnerException);
        }

        [Test]
        public void Stress_test_parallel_method()
        {
            var cache = new AsynchronizerCache();
            var sut = new EBCOperation("math", new MyParallelEbc(), null, cache);

            var are = new AutoResetEvent(false);
            var results = new List<int>();
            var threads = new Dictionary<long, int>();
            var exceptions = new List<string>();

            const int N = 2000;
            for (var i = 1; i <= N; i++)
            {
                var input = new Message("math.Inc", i);
                var methodOp = sut.Create_method_operation(input);
                Assert.IsInstanceOf<AsyncWrapperOperation>(methodOp);

                methodOp.Implementation(input, _ =>
                                                   {
                                                       lock(results)
                                                       {
                                                           results.Add((int)_.Data);

                                                           var thUsage = 0;
                                                           if (threads.TryGetValue(Thread.CurrentThread.GetHashCode(), out thUsage))
                                                               threads[Thread.CurrentThread.GetHashCode()] = thUsage + 1;
                                                           else
                                                               threads.Add(Thread.CurrentThread.GetHashCode(), 1);

                                                           if (results.Count == N) are.Set();
                                                       }
                                                   }, 
                                                   ex =>
                                                       {
                                                           lock(exceptions)
                                                           {
                                                               exceptions.Add(string.Format("data==[{0}] -> {1}", (int)input.Data, ex.ToString()));
                                                           }
                                                       });
            }

            var waitOne = are.WaitOne(2000);
            Console.WriteLine("count: {0}, thread count: {1}, ex count: {2}", results.Count, threads.Count, exceptions.Count);

            foreach(var th in threads)
                Console.WriteLine("{1} x thread #{0}", th.Key, th.Value);
            Console.WriteLine("thread usage total: {0}", threads.Values.Sum());

            for(var i = 0; i < Math.Min(5, exceptions.Count); i++)
                Console.WriteLine("*** {0}: {1}", i, exceptions[i]);
            Assert.IsTrue(waitOne);
        }
    }


    class MyEbc
    {
        public void Inc(int i)
        {
            Result(i + 1);
        }

        public void Divide(Tuple<int,int> input)
        {
            if (input.Item2 == 0)
                DivisionByZero();
            else
                Result(input.Item1/input.Item2);
        }

        public event Action<int> Result;
        public event Action DivisionByZero;
    }


    class MyAsyncEbc
    {
        [AsyncMethod]
        public void Inc(int i)
        {
            Result(i + 1);
        }

        [AsyncMethod]
        public void ThrowException(int i)
        {
            throw new ApplicationException("argghhh, a " + i.ToString());
        }

        public event Action<int> Result;
    }

    class MyParallelEbc
    {
        [ParallelMethod]
        public void Inc(int i)
        {
            Result(i + 1);
        }

        public event Action<int> Result;
    }
}
