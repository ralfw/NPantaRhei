using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    class PopCausality : IOperation
    {
        private readonly string _name;
        private readonly OperationAdapter _implementation;


        public PopCausality(string name)
        {
            _name = name;
            _implementation = (input, outputCont, _) =>
                                  {
                                      input.Causalities.Pop();
                                      outputCont(input);
                                  };
        }

        public string Name
        {
            get { return _name; }
        }

        public OperationAdapter Implementation
        {
            get { return _implementation; }
        }
    }
}