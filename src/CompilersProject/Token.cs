using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Implementations
{
    public class Token
    {
        public string value { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public string type { get; set; }


        public Token(string value, int row, int column)
        {
            this.value = value;
            this.row = row;
            this.column = column;
        }

        public Token(char value, int row, int column)
        {
            this.value = $"{value}";
            this.row = row;
            this.column = column;
        }

        public Token(string value, int row, int column, string type) : this(value, row, column)
        {
            this.type = type;
        }

        public Token(char value, int row, int column, string type) : this(value, row, column)
        {
            this.type = type;
        }

        public override string ToString()
        {
            return $"{{value: {this.value}}}";
        }
    }
}
