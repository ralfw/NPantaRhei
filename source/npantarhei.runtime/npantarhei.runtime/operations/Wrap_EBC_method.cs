using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    internal class Wrap_EBC_method
    {
        public void Process(Task task)
        {
            var ebcOp = task.Operation as EBCOperation;
            if (ebcOp != null)
            {
                var methodOp = ebcOp.Create_method_operation(task.Message);
                task = new Task(task.Message, methodOp);
            }
            Result(task);
        }

        public event Action<Task> Result;
    }
}

