using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Operation_activation
    {
        [Test]
        public void No_activation_if_attribute_not_present()
        {
            using (var fr = new FlowRuntime())
            {
                var op = new InactiveOp();

                fr.AddOperation(op);

                Assert.AreEqual(0, op.messages.Count);
            }   
        }

        [Test]
        public void Activation_if_attribute_present()
        {
            using (var fr = new FlowRuntime())
            {
                var op = new ActiveOp();

                fr.AddOperation(op);

                Assert.AreEqual(1, op.messages.Count);
                Assert.IsInstanceOf<ActivationMessage>(op.messages[0]);
            }
        }


        class InactiveOp : AOperation
        {
            public readonly List<IMessage> messages = new List<IMessage>();

            public InactiveOp() : base("InactiveOp") {}

            protected override void Process(runtime.contract.IMessage input, Action<runtime.contract.IMessage> continueWith, Action<runtime.contract.FlowRuntimeException> unhandledException)
            {
                this.messages.Add(input);
            }
        }
    
        [ActiveOperation]
        class ActiveOp : AOperation
        {
            public readonly List<IMessage> messages = new List<IMessage>();

            public ActiveOp() : base("InactiveOp") { }

            protected override void Process(runtime.contract.IMessage input, Action<runtime.contract.IMessage> continueWith, Action<runtime.contract.FlowRuntimeException> unhandledException)
            {
                this.messages.Add(input);
            }
        }
    }
}
