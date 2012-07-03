using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace npantarhei.runtime.tests.spikes
{
    [TestFixture]
    public class spike_EBC_wrapping
    {
        [Test]
        public void Recognize_input_ports()
        {
            var inputPorts = Find_input_ports(new Simple_EBC(0));
            Assert.AreEqual(3, inputPorts.Count());
        }

        #region Find input ports
        private IEnumerable<MethodInfo> Find_input_ports(object ebc)
        {
            var ebcType = ebc.GetType();

            var candidateMethods = ebcType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var inputPorts = new List<MethodInfo>();
            foreach (var mi in candidateMethods)
                if (Is_input_port_method(mi))
                {
                    Console.WriteLine("{0}, {1}", mi.Name, mi.ReturnType);
                    inputPorts.Add(mi);
                }
            return inputPorts;
        }

        private bool Is_input_port_method(MethodInfo mi)
        {
            return Is_a_procedure(mi) &&
                   Is_not_a_property_accessor(mi) &&
                   Is_not_an_event_modifier(mi) &&
                   Has_the_right_number_of_params(mi);
        }

        private static bool Is_a_procedure(MethodInfo mi)
        {
            return mi.ReturnType == typeof (void);
        }

        private bool Is_not_a_property_accessor(MethodInfo mi)
        {
            return !mi.DeclaringType.GetProperties().Any(p => p.GetSetMethod() == mi);
        }

        private bool Is_not_an_event_modifier(MethodInfo mi)
        {
            return !mi.DeclaringType.GetEvents().Any(e => e.GetAddMethod() == mi || e.GetRemoveMethod() == mi);
        }

        private bool Has_the_right_number_of_params(MethodInfo mi)
        {
            return mi.GetParameters().Count() <= 1;
        }
        #endregion



        [Test]
        public void Recognize_output_ports()
        {
            var outputPorts = Find_output_ports(new Simple_EBC(0));

            Assert.AreEqual(2, outputPorts.Count());
        }


        #region Find output ports
        private IEnumerable<EventInfo> Find_output_ports(object ebc)
        {
            var ebcType = ebc.GetType();

            var candidateEvents = ebcType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            var outputPorts = new List<EventInfo>();
            foreach (var ei in candidateEvents)
                if (Is_output_port_event(ei))
                {
                    Console.WriteLine("{0}", ei.Name);
                    outputPorts.Add(ei);
                }

            return outputPorts;
        }

        private bool Is_output_port_event(EventInfo ei)
        {
            var mi = ei.EventHandlerType.GetMethod("Invoke");
            return mi.GetParameters().Count() <= 1;
        }
        #endregion
    }


    class Simple_EBC
    {
        public Simple_EBC(int a) {}

        public int noPortField;

        public string NoPortProperty { get; set; }

        public void InPort1(string input) {}
        public void InPort2(Uri input) {}
        public void InPort3() {}

        private void NoInPort1(string a) {}
        protected  void NoInPort2() {}
        public void NoInPort3(int a, string b) {}
        public int NoInPort4() { return 0; }
        public static void NoInPort5(int a) {}

        public event Action OutPort1;
        public event Action<int> OutPort2;

        public event Action<int, string> NoOutPort1;
        internal event Action NoOutPort2;
        public Action<string> NoOutPort3;
    }
}
