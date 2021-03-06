﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.spikes
{
    [TestFixture]
    public class spike_EBC_wrapping
    {
        [Test]
        public void Recognize_input_ports()
        {
            var inputPorts = Find_input_ports(new Simple_EBC(0));
            Assert.AreEqual(2, inputPorts.Count());
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


        [Test]
        public void Call_input_port()
        {
            var ebc = new Simple_EBC(0);
            var inputPorts = Find_input_ports(ebc);

            Call_input_port_method(ebc, inputPorts, "InPort2", null);
            Assert.AreEqual("x", ebc.noPortField);

            Call_input_port_method(ebc, inputPorts, "InPort1", "42");
            Assert.AreEqual("42", ebc.noPortField);
        }

        #region Call input port method
        private void Call_input_port_method(object ebc, IEnumerable<MethodInfo> inputPorts, string portName, object parameter)
        {
            var miInput = inputPorts.First(mi => mi.Name.ToLower() == portName.ToLower());
            var parameters = new[] {parameter};
            if (!miInput.GetParameters().Any()) parameters = null;
            miInput.Invoke(ebc, parameters);
        }
        #endregion


        [Test]
        public void Continue_from_events()
        {
            var ebc = new Simple_EBC(0);
            var outputPorts = Find_output_ports(ebc);

            var outputMessages = new List<IMessage>();

            Assign_handlers_to_output_port_events(ebc, outputPorts, outputMessages.Add);

            ebc.InPort1("hello");

            Assert.AreEqual("ebc.OutPort1", outputMessages[0].Port.Fullname);
            Assert.AreEqual("ebc.OutPort2", outputMessages[1].Port.Fullname);
            Assert.AreEqual("hello", (string)outputMessages[1].Data);
        }


        #region Assign handlers to output port events
        private void Assign_handlers_to_output_port_events(object ebc, IEnumerable<EventInfo> outputPorts, Action<IMessage> continueWith)
        {
            foreach (var eiOutput in outputPorts)
            {
                var builder = new EventHandlerBuilder("ebc." + eiOutput.Name, continueWith);
                if (eiOutput.EventHandlerType.GetMethod("Invoke").GetParameters().Count() == 0)
                    eiOutput.AddEventHandler(ebc, builder.CreateEventHandlerForAction());
                else
                    eiOutput.AddEventHandler(ebc, builder.CreateEventHandlerForActionOf(eiOutput.EventHandlerType.GetGenericArguments()[0]));
            }
        }


        class EventHandlerBuilder
        {
            private readonly string _portName;
            private readonly Action<IMessage> _continueWith;

            public EventHandlerBuilder(string portName, Action<IMessage> continueWith)
            {
                _portName = portName;
                _continueWith = continueWith;
            }


            public Delegate CreateEventHandlerForAction()
            {
                return Delegate.CreateDelegate(typeof(Action), this, "ContinueAction");
            }

            public Delegate CreateEventHandlerForActionOf(Type parameterType)
            {
                var miAction = this.GetType().GetMethod("ContinueActionOf").MakeGenericMethod(parameterType);
                return Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(new[] { parameterType }), this, miAction);
            }

            public void ContinueAction() { _continueWith(new Message(_portName, null)); }
            public void ContinueActionOf<T>(T output) { _continueWith(new Message(_portName, output)); }
        }
        #endregion
    }


    class Simple_EBC
    {
        public Simple_EBC(int a) {}

        public string noPortField;

        public string NoPortProperty { get; set; }

        public void InPort1(string input) { noPortField = input; OutPort1(); OutPort2(input); }
        public void InPort2() { noPortField = "x"; }

        private void NoInPort1(string a) {}
        protected  void NoInPort2() {}
        public void NoInPort3(int a, string b) {}
        public int NoInPort4() { return 0; }
        public static void NoInPort5(int a) {}

        public event Action OutPort1 = () => { };
        public event Action<string> OutPort2 = _ => { };

        public event Action<int, string> NoOutPort1;
        internal event Action NoOutPort2;
        public Action<string> NoOutPort3;
    }
}
