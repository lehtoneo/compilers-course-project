﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Implementations
{
    public class Token
    {
        public string value { get; set; }
        public int line { get; set; }
        public int column { get; set; }
        public string type { get; set;  }

        
        public Token(string value, int line, int column)
        {
            this.value = value;
            this.line = line;
            this.column = column;
        }

        public Token(char value, int line, int column)
        {
            this.value = $"{value}";
            this.line = line;
            this.column = column;
        }

        public Token(string value, int line, int column, string type) : this(value, line, column)
        {
            this.type = type;
        }

        public Token(char value, int line, int column, string type) : this(value, line, column)
        {
            this.type = type;
        }
    }
}
