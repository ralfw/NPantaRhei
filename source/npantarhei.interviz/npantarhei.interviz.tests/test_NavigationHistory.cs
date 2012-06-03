using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.interviz.data;

namespace npantarhei.interviz.tests
{
    [TestFixture]
    public class test_NavigationHistory
    {
        [Test]
        public void Go_back_from_end()
        {
            var sut = new NavigationHistory();
            sut.Extend("a");
            sut.Extend("b");
            sut.Extend("c");

            string item;
            Assert.IsTrue(sut.GoBack(out item));
            Assert.AreEqual("b", item);
            Assert.IsTrue(sut.GoBack(out item));
            Assert.AreEqual("a", item);
            Assert.IsFalse(sut.GoBack(out item));
        }


        [Test]
        public void Go_forward_from_beginning()
        {
            var sut = new NavigationHistory();
            sut.Extend("a");
            sut.Extend("b");
            sut.Extend("c");

            string item;
            sut.GoBack(out item);
            sut.GoBack(out item);

            Assert.IsTrue(sut.GoForward(out item));
            Assert.AreEqual("b", item);
            Assert.IsTrue(sut.GoForward(out item));
            Assert.AreEqual("c", item);
        }


        [Test]
        public void Extend_after_going_back()
        {
            var sut = new NavigationHistory();
            sut.Extend("a");
            sut.Extend("b");
            sut.Extend("c");

            string item;
            sut.GoBack(out item); // b

            sut.Extend("x");
            sut.Extend("y");

            Assert.IsTrue(sut.GoBack(out item));
            Assert.AreEqual("x", item);
            Assert.IsTrue(sut.GoBack(out item));
            Assert.AreEqual("b", item);

            Assert.IsTrue(sut.GoForward(out item));
            Assert.AreEqual("x", item);
            Assert.IsTrue(sut.GoForward(out item));
            Assert.AreEqual("y", item);
        }
    }
}
