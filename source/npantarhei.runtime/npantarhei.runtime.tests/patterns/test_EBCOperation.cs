using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
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
}
