using System;

namespace IOperation_wrapper_for_EBC
{
    class ToUpperEBC
    {
        public void Process(string text)
        {
            Result(text.ToUpper());
        }

        public event Action<string> Result;
    }
}