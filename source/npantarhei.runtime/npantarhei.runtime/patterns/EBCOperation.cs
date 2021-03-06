﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        static readonly ConcurrentDictionary<Thread, Stack<Action<IMessage>>> _tls = new ConcurrentDictionary<Thread, Stack<Action<IMessage>>>();


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

            _inputPorts = OperationsFactory.Find_input_ports(_eventBasedComponent);
            var outputPorts = OperationsFactory.Find_output_ports(_eventBasedComponent);

            Assign_handlers_to_output_port_events(_eventBasedComponent,
                                                  outputPorts,
                                                  _ =>
                                                      {
                                                            Action<IMessage> continueWith;
                                                            Stack<Action<IMessage>> continuationStack;
                                                            if (_tls.TryGetValue(Thread.CurrentThread, out continuationStack) &&
                                                                continuationStack.Count > 0)
                                                                continueWith = continuationStack.Peek();
                                                            else
                                                                continueWith = _active_continueWith;
                                                            
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
            var input_port_method = Find_input_port_method(_inputPorts, input.Port.Name);
            var ebcOp = new EbcMethodOperation(base.Name + "." + input.Port.Name, _eventBasedComponent, input_port_method, _dispatcher);
            return OperationsFactory.Schedule_operation_according_to_attributes(_asyncerCache, input_port_method, ebcOp);
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
            private readonly MethodInfo _inputPortMethod;
            private readonly IDispatcher _dispatcher;

            public EbcMethodOperation(string name, object eventBasedComponent, MethodInfo inputPortMethod, IDispatcher dispatcher)
                : base(name)
            {
                _eventBasedComponent = eventBasedComponent;
                _inputPortMethod = inputPortMethod;
                _dispatcher = dispatcher;
            }


            protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
            {
                var call = Build_input_port_method_call(_eventBasedComponent, _inputPortMethod, input.Data, continueWith);
                Dispatch_input_port_method_call(_inputPortMethod, call);
            }


            #region Call input port method
            private Action Build_input_port_method_call(object ebc, MethodInfo miInput, object parameter, Action<IMessage> continueWith)
            {
                Debug.Assert(continueWith != null, "No continuation passed to EBC method call builder!");

                var parameters = new[] { parameter };
                if (!miInput.GetParameters().Any()) parameters = null;

                return () =>
                           {
                                Stack<Action<IMessage>> continuationStack;
                                if (!_tls.TryGetValue(Thread.CurrentThread, out continuationStack))
                                {
                                    continuationStack = new Stack<Action<IMessage>>();
                                    _tls.TryAdd(Thread.CurrentThread, continuationStack);
                                }

                                continuationStack.Push(continueWith);
                                try
                                {
                                    miInput.Invoke(ebc, parameters);
                                }
                                finally
                                {
                                    continuationStack.Pop();
                                }
                            };
            }

            private void Dispatch_input_port_method_call(MethodInfo miInput, Action input_call)
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
