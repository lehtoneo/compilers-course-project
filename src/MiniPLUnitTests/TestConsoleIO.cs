using System;
using System.Collections.Generic;
using CompilersProject.Interfaces;

namespace MiniPLUnitTests
{
    public class TestConsoleIO : IConsoleIO
    {
        public List<string> writes;
        public List<string> reads;
        public int readI;
        public TestConsoleIO(List<string> reads)
        {
            this.writes = new List<string>();
            this.reads = reads;
            readI = 0;
        }
        public string ReadLine()
        {
            string returnValue = reads[readI];
            readI++;
            return returnValue;
        }

        public void Write(string s)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string s)
        {
            throw new NotImplementedException();
        }
    }
}
