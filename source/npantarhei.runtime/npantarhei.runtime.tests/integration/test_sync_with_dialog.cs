using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    class test_sync_with_dialog
    {
        [Test, Explicit]
        public void Run()
        {
            var dlg = new test_sync_with_dialog_win();
            dlg.ShowDialog();
        }
    }
}
