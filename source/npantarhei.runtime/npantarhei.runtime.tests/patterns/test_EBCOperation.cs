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

        public event Action<int> Result;
    }
}
