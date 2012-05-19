using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    public abstract class Flow : AOperation
    {
        protected Flow(string name) : base(name) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (IsFlowInputMessage(input))
            {
                var output = new Message(string.Format("{0}/{0}.{1}", input.Port.OperationName, input.Port.Name), input.Data)
                                {
                                    Causalities = input.Causalities,
                                    FlowStack = input.FlowStack
                                };

                if (input.Port.Path != "") output.FlowStack.Push(input.Port.Path);

                continueWith(output);
            }
            else
            {
                var parentFlowname = "";
                if (!input.FlowStack.IsEmpty) parentFlowname = input.FlowStack.Pop() + "/";

                var output = new Message(string.Format("{0}{1}.{2}", parentFlowname, input.Port.OperationName, input.Port.Name), input.Data)
                                {
                                    Causalities = input.Causalities,
                                    FlowStack = input.FlowStack
                                };

                continueWith(output);
            }
        }

        private static bool IsFlowInputMessage(IMessage input)
        {
            return input.Port.Path != input.Port.OperationName;
        }


        public IEnumerable<IStream> Streams { get { return Map_streams_to_flow(BuildStreams()); } }
        public virtual IEnumerable<IOperation> Operations { get { return BuildOperations(new FlowOperationContainer()); } }

        private IEnumerable<IStream> Map_streams_to_flow(IEnumerable<IStream> streams)
        {
            return streams.Select(s => new Stream(Map_port_to_flow(s.FromPort), Map_port_to_flow(s.ToPort)));
        }

        private string Map_port_to_flow(IPort port)
        {
            return port.IsOperationPort ? string.Format("{0}/{1}{2}", base.Name, port.OperationName, Map_portname(port)) 
                                        : string.Format("{0}/{0}{1}", base.Name, Map_portname(port));
        }

        private object Map_portname(IPort port)
        {
            return port.Name == "" ? "" : "." + port.Name;
        }


        protected abstract IEnumerable<IStream> BuildStreams();
        protected virtual IEnumerable<IOperation> BuildOperations(FlowOperationContainer container) { return new IOperation[]{};}
    }
}
