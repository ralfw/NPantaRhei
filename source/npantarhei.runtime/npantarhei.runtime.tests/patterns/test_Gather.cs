using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_Gather
    {
        [Test]
        public void Without_correlationId()
        {
            IMessage result = null;

            var sut = new Gather<string>("x");

            sut.Implementation(new Message("x.count", 2), null, null);

            sut.Implementation(new Message("x.stream", "a"), null, null);
            sut.Implementation(new Message("x.stream", "b"), _ => result = _, null);

            Assert.That(result.Data, Is.EqualTo(new[]{"a", "b"}));
        }

        [Test]
        public void With_correlationId()
        {
            IMessage result = null;

            var sut = new Gather<string>("x");

            var corrId1 = Guid.NewGuid();
            var corrId2 = Guid.NewGuid();

            sut.Implementation(new Message("x.count", 2, corrId1), null, null);
            sut.Implementation(new Message("x.count", 3, corrId2), null, null);

            sut.Implementation(new Message("x.stream", "a", corrId1), null, null);
            sut.Implementation(new Message("x.stream", "x", corrId2), null, null);
            sut.Implementation(new Message("x.stream", "y", corrId2), null, null);
            sut.Implementation(new Message("x.stream", "b", corrId1), _ => result = _, null);
            Assert.That(result.Data, Is.EqualTo(new[] { "a", "b" }));

            sut.Implementation(new Message("x.stream", "z", corrId2), _ => result = _, null);
            Assert.That(result.Data, Is.EqualTo(new[] { "x", "y", "z" }));
        }
    }
}
