using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.contract
{
    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class InstanceOperations : Attribute {}

    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class StaticOperations : Attribute { }

    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class EventBasedComponent : Attribute { }


    [Serializable, AttributeUsage(AttributeTargets.Method)]
    public class AsyncMethodAttribute : Attribute
    {
        private readonly string _threadPoolName;

        public AsyncMethodAttribute() : this("$$$asyncmethod$$$") { }
        public AsyncMethodAttribute(string threadPoolName)
        {
            _threadPoolName = threadPoolName;
        }

        public string ThreadPoolName { get { return _threadPoolName; } }
    }


    [Serializable, AttributeUsage(AttributeTargets.Method)]
    public class DispatchedMethodAttribute : Attribute
    {
        internal static bool HasBeenApplied(Delegate a)
        {
            var mi = a.Method;
            return HasBeenApplied(mi);
        }

        internal static bool HasBeenApplied(MethodInfo mi)
        {
            return mi.GetCustomAttributes(true)
                     .Select(attr => attr.GetType())
                     .Any(attrType => attrType == typeof(DispatchedMethodAttribute));
        }
    }


    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class ActiveOperationAttribute : Attribute
    { }

    public class ActivationMessage : Message
    {
        public ActivationMessage() : base(".activate", null) { }
    }
}
