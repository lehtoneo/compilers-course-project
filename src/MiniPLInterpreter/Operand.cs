using System;
using MiniPLInterpreter.Exceptions;
using MiniPLInterpreter.Utils;
namespace MiniPLInterpreter
{
    public class Operand
    {

        public string type { get; set; }

        public string value { get; set; }


        private static MiniPLExceptionThrower miniPLExceptionThrower = new MiniPLExceptionThrower("Runtime");
        private static MiniPLHelper miniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
        public Operand(string value, string type)
        {
            this.value = value;
            this.type = type;
        }

        public int valueToInt()
        {
            if (this.type != "int")
            {
                throw new Exception($"Called toInt for identifier with type {type} and value {value}");
            }

            int.TryParse(this.value, out int numericValue);
            return numericValue;
        }

        public override string ToString()
        {

            string returnString = "";
            bool backSlashBefore = false;
            foreach (char c in this.value)
            {
                if (backSlashBefore)
                {
                    if (c == 'n')
                    {
                        returnString = returnString + System.Environment.NewLine;
                    }
                    else
                    {
                        returnString = returnString + '\\' + c;
                    }
                    backSlashBefore = false;
                }
                else if (c == '\\')
                {
                    backSlashBefore = true;
                }
                else
                {
                    returnString = returnString + c;
                }
            }

            return returnString;
        }


        private static void checkSameTypeThrowsError(Operand first, Operand second, string op)
        {
            if (first.type != second.type)
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid operation '{op}' for type '{first.type}' and type {second.type}.");
            }
        }

        private static void checkValidOperatorForOperandThrowsError(string op, Operand opnd)
        {
            if (!miniPLHelper.isValidOperatorForType(op, opnd.type))
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid operation '{op}' for type '{opnd.type}'.");
            }
        }

        public static Operand operator +(Operand x, Operand y)
        {
            string miniPLOperator = "+";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            if (x.type == "int")
            {
                int value = x.valueToInt() + y.valueToInt();
                return new Operand(value.ToString(), "int");
            }
            else if (x.type == "string")
            {
                string value = x.value + y.value;
                return new Operand(value, "string");
            }
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator -(Operand x, Operand y)
        {

            string miniPLOperator = "-";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);

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
            string miniPLOperator = "*";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
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

        public static Operand operator /(Operand x, Operand y)
        {
            string miniPLOperator = "/";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            if (x.type == "int")
            {
                int value = x.valueToInt() / y.valueToInt();
                return new Operand(value.ToString(), "int");
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator !(Operand x)
        {
            string miniPLOperator = "!";
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            if (x.type == "bool")
            {
                if (x.value == "false")
                {
                    return new Operand("true", "bool");
                }
                else
                {
                    return new Operand("false", "bool");
                }
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator &(Operand x, Operand y)
        {
            string miniPLOperator = "&";
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            if (x.type == "bool")
            {
                bool valueBool = x.value == "true" && y.value == "true";
                string value = valueBool.ToString().ToLower();

                return new Operand(value, "bool");
            }
            else
            {
                throw new NotImplementedException();

            }
        }

        public static Operand operator ==(Operand x, Operand y)
        {
            string miniPLOperator = "=";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            bool valueBool = x.value == y.value;
            string value = valueBool.ToString().ToLower();
            return new Operand(value, "bool");

        }

        public static Operand operator <(Operand x, Operand y)
        {
            string miniPLOperator = "<";
            checkSameTypeThrowsError(x, y, miniPLOperator);
            checkValidOperatorForOperandThrowsError(miniPLOperator, x);
            string type = x.type;
            string value;
            if (type == "int")
            {
                bool boolValue = x.valueToInt() < y.valueToInt();
                value = boolValue.ToString().ToLower();
            }
            else if (type == "string")
            {
                bool boolValue = x.value.Length < y.value.Length;
                value = boolValue.ToString().ToLower();
            }
            else if (type == "bool")
            {
                if (x.value == "false" && y.value == "true")
                {
                    value = "true";
                }
                else
                {
                    value = "false";
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Operand(value, "bool");

        }

        public static Operand operator >(Operand x, Operand y)
        {
            throw new NotImplementedException();

        }

        public static Operand operator !=(Operand x, Operand y)
        {
            throw new NotImplementedException();

        }

    }
}
