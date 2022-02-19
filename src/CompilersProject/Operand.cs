using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject
{
    public class Operand
    {

        public string type { get; set; }

        public string value { get; set; }
        public Operand(string value, string type)
        {
            this.value = value;
            this.type = type;
        }

        private int valueToInt()
        {
            if (this.type != "int")
            {
                throw new Exception($"Called toInt for identifier with type {type} and value {value}");
            }

            int.TryParse(this.value, out int numericValue);
            return numericValue;
        }

        public static Operand operator +(Operand x, Operand y)
        {
            if (x.type == "int")
            {
                int value = x.valueToInt() + y.valueToInt();
                return new Operand(value.ToString(), x.type);
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator -(Operand x, Operand y)
        {
            if (x.type == "int")
            {
                int value = x.valueToInt() - y.valueToInt();
                return new Operand(value.ToString(), x.type);
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator *(Operand x, Operand y)
        {
            if (x.type == "int")
            {
                int value = x.valueToInt() * y.valueToInt();
                return new Operand(value.ToString(), x.type);
            }
            else
            {
                throw new NotImplementedException();

            }
        }


    }
}
