using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.contract
{
    // Called like this after the same named attribute in the PostSharp Threading Toolkit
    public class DispatchedMethodAttribute : Attribute
    {
        internal static bool HasBeenApplied(Delegate a)
        {
            var mi = a.Method;
            return mi.GetCustomAttributes(true)
                     .Select(attr => attr.GetType())
                     .Any(attrType => attrType == typeof(DispatchedMethodAttribute));
        }
    }
}
