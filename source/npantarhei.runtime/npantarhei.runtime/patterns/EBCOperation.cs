using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    [ActiveOperation]
    internal class EBCOperation : AOperation
    {
        private const string CONTINUATION_SLOT_NAME = "continueWith";

        private readonly object _eventBasedComponent;
        private readonly IDispatcher _dispatcher;
        private readonly AsynchronizerCache _asyncerCache;
        private readonly IEnumerable<MethodInfo> _inputPorts;
        private Action<IMessage> _active_continueWith;


        public EBCOperation(string name, object eventBasedComponent, IDispatcher dispatcher, AsynchronizerCache asyncerCache) : base(name)
        {
            _eventBasedComponent = eventBasedComponent;
            _dispatcher = dispatcher;
            _asyncerCache = asyncerCache;

            _inputPorts = Find_input_ports(_eventBasedComponent);
            var outputPorts = Find_output_ports(_eventBasedComponent);

            Assign_handlers_to_output_port_events(_eventBasedComponent,
                                                  outputPorts,
                                                  _ => {
                                                            var continueWith = (Action<IMessage>)Thread.GetData(Thread.GetNamedDataSlot(CONTINUATION_SLOT_NAME))
                                                                               ?? _active_continueWith;
                                                            continueWith(_);
                                                  });
        }


        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (input is ActivationMessage)
            {
                _active_continueWith = continueWith;
            }
        }


        public IOperation Create_method_operation(IMessage input)
        {
            return new EbcMethodOperation(base.Name + "." + input.Port.Name, _eventBasedComponent, _inputPorts, _dispatcher);
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
            return mi.ReturnType == typeof(void);
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


        #region Find output ports
        private IEnumerable<EventInfo> Find_output_ports(object ebc)
        {
            var ebcType = ebc.GetType();

            var candidateEvents = ebcType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            var outputPorts = new List<EventInfo>();
            foreach (var ei in candidateEvents)
                if (Is_output_port_event(ei))
                {
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


        #region Assign handlers to output port events
        private void Assign_handlers_to_output_port_events(object ebc, IEnumerable<EventInfo> outputPorts, Action<IMessage> continueWith)
        {
            foreach (var eiOutput in outputPorts)
            {
                var builder = new EventHandlerBuilder(base.Name + "." + eiOutput.Name, continueWith);
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


        class EbcMethodOperation : AOperation
        {
            private readonly object _eventBasedComponent;
            private readonly IEnumerable<MethodInfo> _inputPorts;
            private readonly IDispatcher _dispatcher;

            public EbcMethodOperation(string name, object eventBasedComponent, IEnumerable<MethodInfo> inputPorts, IDispatcher dispatcher)
                : base(name)
            {
                _eventBasedComponent = eventBasedComponent;
                _inputPorts = inputPorts;
                _dispatcher = dispatcher;
            }


            protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
            {
                var method = Find_input_port_method(_inputPorts, input.Port.Name);
                var call = Build_input_port_method_call(_eventBasedComponent, method, input.Data, continueWith);
                Schedule_input_port_method_call(method, call);
            }


            #region Call input port method
            private MethodInfo Find_input_port_method(IEnumerable<MethodInfo> inputPorts, string portName)
            {
                var miInput = inputPorts.FirstOrDefault(mi => mi.Name.ToLower() == portName.ToLower());
                if (miInput == null) throw new ArgumentException(string.Format("EBC-Operation {0}: Unknown input port name '{1}'!",
                                                                                base.Name,
                                                                                portName));
                return miInput;
            }

            private Action Build_input_port_method_call(object ebc, MethodInfo miInput, object parameter, Action<IMessage> continueWith)
            {
                var parameters = new[] { parameter };
                if (!miInput.GetParameters().Any()) parameters = null;

                return () =>
                {
                    Thread.SetData(Thread.GetNamedDataSlot(CONTINUATION_SLOT_NAME), continueWith);
                    try
                    {
                        miInput.Invoke(ebc, parameters);
                    }
                    finally
                    {
                        Thread.FreeNamedDataSlot(CONTINUATION_SLOT_NAME);
                    }
                };
            }

            private void Schedule_input_port_method_call(MethodInfo miInput, Action input_call)
            {
                if (DispatchedMethodAttribute.HasBeenApplied(miInput))
                    _dispatcher.Process(input_call);
                else
                    input_call();
            }
            #endregion
        }
    }
}
