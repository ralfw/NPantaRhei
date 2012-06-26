using System;
using System.Linq;

namespace TelegramProblem
{
    class Formatter
    {
        private int _newLineWidth;

        public void Config(int newLineWidth)
        {
            _newLineWidth = newLineWidth;
        }


        public void Decompose(string line, Action<string> onWord)
        {
            if (line == null) { onWord(null); return; }

            line.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries).ToList()
                .ForEach(onWord);
        }


        private string _currentLine = "";

        public void Concatenate(string word, Action<string> onLine)
        {
            if (word == null)
            {
                if (_currentLine != "") onLine(_currentLine);
                onLine(null); 
                _currentLine = ""; 
                return;
            }

            if (_currentLine.Length + 1 + word.Length > _newLineWidth)
            {
                onLine(_currentLine);
                _currentLine = "";
            }
            
            _currentLine += (_currentLine == "" ? "" : " ") + word;
        }
    }
}