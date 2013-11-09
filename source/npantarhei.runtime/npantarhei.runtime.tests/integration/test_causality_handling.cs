using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_causality_handling
    {
        private FlowRuntime _fr;
        private FlowRuntimeException _exCausality, _exUnhandled;
        private AutoResetEvent _are;

        [SetUp]
        public void Setup()
        {
            var frc = new FlowRuntimeConfiguration();
            frc.AddPushCausality("push");
            frc.AddPopCausality("pop");

            frc.AddFunc<int, int>("exOn0", i =>
                                                {
                                                    if (i == 0)
                                                        throw new ApplicationException("on0");
                                                    return i;
                                                });
            frc.AddFunc<int, int>("exOn1", i =>
                                                {
                                                    if (i == 1)
                                                        throw new ApplicationException("on1");
                                                    return i;
                                                });
            frc.AddAction<int>("exOn2", i =>
                                                {
                                                    if (i == 2) 
                                                        throw new ApplicationException("on2");
                                                });

            _exCausality = null;
            _are = new AutoResetEvent(false);
            frc.AddAction<FlowRuntimeException>("handleEx", _ =>
                                                                   {
                                                                       _exCausality = _;
                                                                       _are.Set();
                                                                   });

            frc.AddStream(new Stream(".in", "push"));
            frc.AddStream(new Stream("push", "exOn0"));
            frc.AddStream(new Stream("push.exception", "handleEx"));
            frc.AddStream(new Stream("exOn0", "exOn1"));
            frc.AddStream(new Stream("exOn1", "pop"));
            frc.AddStream(new Stream("pop", "exOn2"));

            _fr = new FlowRuntime(frc);

            _exUnhandled = null;
            _fr.UnhandledException += _ => { 
                                                _exUnhandled = _;
                                                _are.Set();
                                           };
        }

        [TearDown]
        public void Teardown() { _fr.Dispose(); }


        [Test]
        public void Exception_after_push()
        {
            _fr.Process(new Message(".in", 0));

            Assert.IsTrue(_are.WaitOne(500));
            Assert.AreEqual("exOn0", _exCausality.Context.Port.Fullname);
        }

        [Test]
        public void Exception_after_a_correct_op()
        {
            _fr.Process(new Message(".in", 1));

            Assert.IsTrue(_are.WaitOne(500));
            Assert.AreEqual("exOn1", _exCausality.Context.Port.Fullname);
        }

        [Test]
        public void Exception_after_pop()
        {
            _fr.Process(new Message(".in", 2));

            Assert.IsTrue(_are.WaitOne(500));
            Assert.AreEqual("exOn2", _exUnhandled.Context.Port.Fullname);
        }


        [Test]
        public void Causality_exception_messages_bypasses_other_pending_messages()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                    /
                                                    .in, push
                                                    push, doit
                                                    push.exception, .error
                                                    doit, .result
                                                 ")
                                .AddPushCausality("push")
                                .AddPopCausality("pop")
                                .AddAction<int, string>("doit", (int d, Action<string> continueWith) =>
                                                       {
                                                           continueWith(d + "x");
                                                           continueWith(d + "y");
                                                           throw new ApplicationException("arrrghhh!");
                                                       });

            using(var fr = new FlowRuntime(config, new Schedule_for_async_breadthfirst_processing()))
            {
                var results = new List<IMessage>();
                var are = new AutoResetEvent(false);

                fr.Result += _ =>
                                 {
                                     results.Add(_);
                                     if (results.Count == 3 || _.Port.Name == ".error") are.Set();
                                 };

                fr.Process(".in", 42);

                Assert.IsTrue(are.WaitOne(1000));
                Assert.AreEqual("error", results[0].Port.Name);
            }
        }
    }
}
