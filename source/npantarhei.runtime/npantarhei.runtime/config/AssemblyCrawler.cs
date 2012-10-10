using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.config
{
    public class AssemblyCrawler : IOperationCrawler
    {
        private readonly Assembly[] _assemblies;

        public AssemblyCrawler(params Assembly[] assemblies) { _assemblies = assemblies; }


        public void Register(Action<Type> registerStaticMethods, Action<object> registerInstanceMethods, Action<object> registerEventBasedComponent)
        {
            foreach(var assm in _assemblies)
                foreach(var t in assm.GetTypes())
                {
                    if (t.GetCustomAttributes(false).Any(attr => attr.GetType() == typeof(StaticOperations)))
                        registerStaticMethods(t);
                    if (t.GetCustomAttributes(false).Any(attr => attr.GetType() == typeof(InstanceOperations)))
                        registerInstanceMethods(Activator.CreateInstance(t));
                    if (t.GetCustomAttributes(false).Any(attr => attr.GetType() == typeof(EventBasedComponent)))
                        registerEventBasedComponent(Activator.CreateInstance(t));
                }
        }
    }
}
