using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
    internal class Create_activation_task_for_operation
    {
        public void Process(IOperation operation)
        {
            if (operation.GetType().GetCustomAttributes(typeof(ActiveOperationAttribute), true).Length == 0) return;
            
            Result(new Task(new ActivationMessage(), operation));
        }

        public event Action<Task> Result;
    }
}