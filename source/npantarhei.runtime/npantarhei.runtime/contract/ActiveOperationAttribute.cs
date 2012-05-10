using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.contract
{
    public class ActiveOperationAttribute : Attribute
    {}

    public class ActivationMessage : Message
    {
        public ActivationMessage() : base(".activate", null) {}
    }
}
