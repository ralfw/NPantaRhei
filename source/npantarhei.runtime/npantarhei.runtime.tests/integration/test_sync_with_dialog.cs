using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

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
