using System;
using System.Linq;
using NUnit.Framework;
using npantarhei.distribution.translators;

namespace npantarhei.distribution.tests
{
    [TestFixture]
    public class test_CorrelationCache
    {
        [Test]
        public void Add()
        {
            var sut = new CorrelationCache<int>();
            var corrId = Guid.NewGuid();
            var now = DateTime.Now;
            sut.Add(corrId, 42);

            Assert.AreEqual(1, sut._cache.Count());
            Assert.AreEqual(corrId, sut._cache[0].CorrelationId);
            Assert.AreEqual(42, sut._cache[0].Data);
            Assert.IsTrue(sut._cache[0].ExpiresAt.Subtract(now).TotalSeconds > 58 && sut._cache[0].ExpiresAt.Subtract(now).TotalSeconds < 62);
        }

        [Test]
        public void Get()
        {
            var sut = new CorrelationCache<int>();
            var corrId = Guid.NewGuid();
            sut.Add(corrId, 42);
            Assert.AreEqual(42, sut.Get(corrId));

            Assert.Throws<InvalidOperationException>(() => sut.Get(Guid.NewGuid()));
        }

        [Test]
        public void CollectGarbage()
        {
            var sut = new CorrelationCache<int>();
            sut._cache.Add(new CorrelationCache<int>.Element{CorrelationId = Guid.NewGuid(), Data = 42, ExpiresAt = DateTime.Now.AddSeconds(42)});
            sut._cache.Add(new CorrelationCache<int>.Element { CorrelationId = Guid.NewGuid(), Data = -99, ExpiresAt = DateTime.Now.AddSeconds(-99) });

            sut.CollectGarbage();

            Assert.AreEqual(1, sut._cache.Count());
            Assert.AreEqual(42, sut._cache[0].Data);
        }
    }
}