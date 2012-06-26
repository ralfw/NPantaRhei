using System;
using System.IO;
using System.Linq;

namespace TelegramProblem
{
    class TextfileAdapter
    {
        private string _inputFilename, _outputFilename;

        public void Config(Tuple<string,string> filenames)
        {
            _inputFilename = filenames.Item1;
            _outputFilename = filenames.Item2;

            if (File.Exists(_outputFilename)) File.Delete(_outputFilename);
        }

        public void Read(Action<string> onLine)
        {
            File.ReadAllLines(_inputFilename).ToList()
                .ForEach(onLine);
            onLine(null);
        }

        public void Write(string line)
        {
            if (line == null) return;

            File.AppendAllLines(_outputFilename, new[]{line});
        }
    }
}