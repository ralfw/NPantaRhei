using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    internal class ManualResetJoin<T0, T1> : ManualResetJoinBase
    {
        public ManualResetJoin(string name) : base(name, 2, Create_join_tuple) { }

        private static object Create_join_tuple(List<object> joinList)
        {
            return new Tuple<T0, T1>((T0)joinList[0],
                                     (T1)joinList[1]);
        }
    }

    internal class ManualResetJoin<T0, T1, T2> : ManualResetJoinBase
    {
        public ManualResetJoin(string name) : base(name, 3, Create_join_tuple) { }

        private static object Create_join_tuple(List<object> joinList)
        {
            return new Tuple<T0, T1, T2>((T0)joinList[0],
                                         (T1)joinList[1],
                                         (T2)joinList[2]);
        }
    }

    /*
     * Port names:
     *      input: .in0, .in1, ..., .in9 (last char in port name is index of input)
     *             .reset
     *      output: none (only name of operation)
     */
    internal class ManualResetJoinBase : IOperation
    {
        private readonly ManualResetJoin _mrj;

        public ManualResetJoinBase(string name, int numberOfInputs, Func<List<object>, object> createJoinTuple)
        {
            if (numberOfInputs>10) throw new ArgumentException("Maximum of 10 input ports exceeded!");

            _name = name;
            _mrj = new ManualResetJoin(numberOfInputs);

            _implementation = (input, continueWith, _) =>
                                  {
                                      if (!Regex.Match(input.Port.Name, "^in[0-9]$|^reset$", RegexOptions.IgnoreCase).Success)
                                          throw new ArgumentException("ManualResetJoin: Invalid port name! Use 'in0'..'in9' or 'reset'.");

                                      if (input.Port.Name.ToLower() == "reset")
                                          _mrj.Reset(input.CorrelationId);
                                      else
                                      {
                                          var inputIndex = int.Parse(input.Port.Name.Substring(input.Port.Name.Length - 1));
                                          _mrj.Process(inputIndex, input.Data, input.CorrelationId,
                                                       joinList => continueWith(new Message(_name, createJoinTuple(joinList), input.CorrelationId)));
                                      }
                                  };
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly OperationAdapter _implementation;
        public OperationAdapter Implementation
        {
            get { return _implementation; }
        }
    }
}