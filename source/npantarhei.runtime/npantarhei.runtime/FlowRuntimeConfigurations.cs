using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using npantarhei.runtime.config;
using npantarhei.runtime.operations;

namespace npantarhei.runtime
{
    public class FlowRuntimeFactory
    {
        public static FlowRuntime Beginner
        {
            get
            {
                return new FlowRuntime(DefaultConfig, new Schedule_for_sync_depthfirst_processing());
            }
        }

        public static FlowRuntime Basic
        {
            get
            {
                return new FlowRuntime(DefaultConfig);
            }
        }


        private static FlowRuntimeConfiguration DefaultConfig
        {
            get
            {
                var programAssm = Assembly.GetEntryAssembly();
                var programType = programAssm.GetTypes().FirstOrDefault(t => t.Name == "Program");
                if (programType == null) throw new InvalidOperationException("Cannot create default configuration. Missing class Program in entry assembly!");

                return new FlowRuntimeConfiguration()
                            .AddStreamsFrom(programType.Namespace + ".root.flow", programAssm)
                            .AddOperations(new AssemblyCrawler(programAssm));
            }
        }
    }
}
