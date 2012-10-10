using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    static class OperationsFactory
    {
        public static IEnumerable<MethodInfo> Find_input_ports(object ebc)
        {
            var ebcType = ebc.GetType();

            var candidateMethods = ebcType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var inputPorts = new List<MethodInfo>();
            foreach (var mi in candidateMethods)
                if (Is_input_port_method(mi))
                    inputPorts.Add(mi);
            return inputPorts;
        }

        public static IEnumerable<EventInfo> Find_output_ports(object ebc)
        {
            var ebcType = ebc.GetType();

            var candidateEvents = ebcType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            var outputPorts = new List<EventInfo>();
            foreach (var ei in candidateEvents)
                if (Is_output_port_event(ei))
                    outputPorts.Add(ei);

            return outputPorts;
        }


        public static IEnumerable<MethodInfo> Find_instance_method_operations(object operationsInstance) { return Find_method_operations(operationsInstance.GetType(), false); }
        public static IEnumerable<MethodInfo> Find_static_method_operations(Type operationsType) { return Find_method_operations(operationsType, true); }
        private static IEnumerable<MethodInfo> Find_method_operations(Type operationsType, bool findStatic)
        {
            var candidateMethods = operationsType.GetMethods(BindingFlags.Public | (findStatic ? BindingFlags.Static : BindingFlags.Instance));
            var methodOperations = new List<MethodInfo>();
            foreach (var mi in candidateMethods)
                if (Is_method_operation(mi))
                    methodOperations.Add(mi);
            return methodOperations;   
        } 


        // Creating continuations again and again for every output message is very inefficient.
        // But right now it´s simple and serves the purpose. It can be optimized later on.
        public static IOperation Create_method_operation(object instance, MethodInfo operationMethod)
        {
            var name = operationMethod.Name;

            if (Is_a_procedure(operationMethod))
            {
                if (operationMethod.GetParameters().Length == 0)
                    return new Operation(name, (input, outputCont, _) =>
                    {
                        operationMethod.Invoke(instance, null);
                        outputCont(new Message(name, null, input.CorrelationId));
                    });
 

                if (Parameter_types_ok(operationMethod, "c"))
                    return new Operation(name, (input, outputCont, _) => operationMethod.Invoke(instance, new[] {Create_continuation(operationMethod.GetParameters()[0], name, input, outputCont)}));

                if (Parameter_types_ok(operationMethod, "v"))
                    return new Operation(name, (input, outputCont, _) =>
                    {
                        operationMethod.Invoke(instance, new[] { input.Data });
                        outputCont(new Message(name, null, input.CorrelationId));
                    });

                if (Parameter_types_ok(operationMethod, "cc"))
                    return new Operation(name, (input, outputCont, _) => operationMethod.Invoke(instance, new[] {Create_continuation(operationMethod.GetParameters()[0], name + ".out0", input, outputCont),
                                                                                                                 Create_continuation(operationMethod.GetParameters()[1], name + ".out1", input, outputCont)}));
                if (Parameter_types_ok(operationMethod, "vc"))
                    return new Operation(name, (input, outputCont, _) => operationMethod.Invoke(instance, new[] {input.Data, Create_continuation(operationMethod.GetParameters()[1], name, input, outputCont)}));

                if (Parameter_types_ok(operationMethod, "vcc"))
                    return new Operation(name, (input, outputCont, _) => operationMethod.Invoke(instance, new[] {input.Data, 
                                                                                                Create_continuation(operationMethod.GetParameters()[1], name + ".out0", input, outputCont),
                                                                                                Create_continuation(operationMethod.GetParameters()[2], name + ".out1", input, outputCont)}));

                throw new NotImplementedException(string.Format("{0}.{1}: Procedure signature not supported as an operation!", instance.GetType().Name, operationMethod.Name));
            }


            if (operationMethod.GetParameters().Length == 0)
                return new Operation(name, (input, outputCont, _) =>
                {
                    var result = operationMethod.Invoke(instance, null);
                    outputCont(new Message(name, result, input.CorrelationId));
                });
            if (Parameter_types_ok(operationMethod, "v"))
                return new Operation(name, (input, outputCont, _) =>
                {
                    var result = operationMethod.Invoke(instance, new[] { input.Data });
                    outputCont(new Message(name, result, input.CorrelationId));
                });
            throw new NotImplementedException(string.Format("{0}.{1}: Function signature not supported as an operation!", instance.GetType().Name, operationMethod.Name));
        }


        private static bool Is_input_port_method(MethodInfo mi)
        {
            return Is_a_procedure(mi) &&
                   Is_not_a_property_accessor(mi) &&
                   Is_not_an_event_modifier(mi) &&
                   Has_EBC_input_port_signature(mi);
        }

        private static bool Is_method_operation(MethodInfo mi)
        {
            return Is_not_a_property_accessor(mi) &&
                   Is_not_an_event_modifier(mi) &&
                   Has_method_operation_signature(mi) &&
                   Is_not_an_object_type_method(mi);
        }


        private static bool Is_a_procedure(MethodInfo mi)
        {
            return mi.ReturnType == typeof(void);
        }

        private static bool Is_not_a_property_accessor(MethodInfo mi)
        {
            return !mi.DeclaringType.GetProperties().Any(p => p.GetSetMethod() == mi);
        }

        private static bool Is_not_an_event_modifier(MethodInfo mi)
        {
            return !mi.DeclaringType.GetEvents().Any(e => e.GetAddMethod() == mi || e.GetRemoveMethod() == mi);
        }

        private static bool Is_not_an_object_type_method(MethodInfo mi)
        {
            return "Equals ToString GetType GetHashCode".IndexOf(mi.Name) < 0;
        }

        private static bool Has_EBC_input_port_signature(MethodInfo mi)
        {
            return mi.GetParameters().Count() <= 1;
        }

        private static bool Has_method_operation_signature(MethodInfo mi)
        {
            return (Is_a_procedure(mi) && (
                                              mi.GetParameters().Length == 0 ||
                                              Parameter_types_ok(mi, "v", "c") ||
                                              Parameter_types_ok(mi, "vc", "cc") ||
                                              Parameter_types_ok(mi, "vcc"))) 
                   ||
                   (!Is_a_procedure(mi) && (
                                              mi.GetParameters().Length == 0 ||
                                              Parameter_types_ok(mi, "v")));
        }

        private static bool Parameter_types_ok(MethodInfo mi, params string[] signatures)
        {
            if (mi.GetParameters().Length != signatures[0].Length) return false;

            foreach(var signature in signatures)
            {
                var numberOfParamsOk = 0;
                for(var i=0; i<signature.Length; i++)
                    switch (signature[i])
                    {
                        case 'v':
                            numberOfParamsOk += Is_continuation_parameter(mi.GetParameters()[i]) ? 0 : 1;
                            break;
                        case 'c':
                            numberOfParamsOk += Is_continuation_parameter(mi.GetParameters()[i]) ? 1 : 0;
                            break;
                    }
                if (numberOfParamsOk == signature.Length) return true;
            }
            return false;
        }


        private static bool Is_continuation_parameter(ParameterInfo paramInfo)
        {
            return paramInfo.ParameterType == typeof(Action) ||
                   (paramInfo.ParameterType.IsGenericType && paramInfo.ParameterType.GetGenericTypeDefinition() == typeof(Action<>));
        }


        private static bool Is_output_port_event(EventInfo ei)
        {
            var mi = ei.EventHandlerType.GetMethod("Invoke");
            return mi.GetParameters().Count() <= 1;
        }


        private static Delegate Create_continuation(ParameterInfo param, string name, IMessage input, Action<IMessage> outputCont)
        {
            if (param.ParameterType == typeof(Action)) return new Action(() => outputCont(new Message(name, null, input.CorrelationId)));

            //return new Action<object>(output => outputCont(new Message(name, output, input.CorrelationId)));
            var continuationType = typeof (Continuation<>).MakeGenericType(param.ParameterType.GetGenericArguments()[0]);
            var continuationObject = Activator.CreateInstance(continuationType, name, input, outputCont);

            var delegateType = typeof (Action<>).MakeGenericType(param.ParameterType.GetGenericArguments()[0]);
            return Delegate.CreateDelegate(delegateType, continuationObject, "ContinueWith");
        }
    }


    class Continuation<T>
    {
        private readonly Action<object> _continueWith;

        public Continuation(string name, IMessage input, Action<IMessage> outputCont)
        {
            _continueWith = output => outputCont(new Message(name, output, input.CorrelationId));
        } 

        public void ContinueWith(T output)
        {
            _continueWith(output);
        }
    }
}
