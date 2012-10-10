using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_OperationsFactory
    {
        [Test]
        public void Instance_method_operations()
        {
            var operations = OperationsFactory.Find_instance_method_operations(new InstanceMethodOperations());
            Assert.That(operations.Select(op => op.Name).ToArray(), 
                        Is.EquivalentTo(new[]{"Procedure", "ProcedureV", "ProcedureC", "ProcedureCv", "ProcedureVC", "ProcedureCC", 
                                              "Function", "FunctionV"}));
        }

        [Test]
        public void Static_method_operations()
        {
            var operations = OperationsFactory.Find_static_method_operations(typeof(StaticMethodOperations));
            Assert.That(operations.Select(op => op.Name).ToArray(),
                        Is.EquivalentTo(new[]{"SProcedure", "SFunction"}));
        }
    }


    class InstanceMethodOperations
    {
        public void Procedure() {}
        public void ProcedureV(int a) {}
        public void ProcedureC(Action continueWith) {}
        public void ProcedureCv(Action<int> continueWith) {}
        public void ProcedureVC(int a, Action continueWith) {}
        public void ProcedureCC(Action continueWith0, Action continueWith1) {}
        public int Function() { return 0; }
        public int FunctionV(int a) { return 0; }

        // invalid operations
        public void ProcedureCC(int a, int b) {}
        public void ProcedureVC(Action continueWith, int a) {}
        public void ProcedureVCCC(int a, Action continueWith0, Action continueWith1, Action continueWith2) {}
        public int FunctionC(Action continueWith) { return 0; }

        public static void SProcedure() {}
    }

    class StaticMethodOperations
    {
        public static void SProcedure() { }
        public static int SFunction() { return 0; }   
    }
}
