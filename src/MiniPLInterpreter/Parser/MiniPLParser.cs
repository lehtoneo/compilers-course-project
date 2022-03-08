using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Utils;
using MiniPLInterpreter.Interfaces;
using MiniPLInterpreter.Exceptions;

namespace MiniPLInterpreter.Implementations
{
    public class MiniPLParser : IParser
    {
        public List<Token> Tokens;
        public Dictionary<string, ParserIdentifier> identifiers = new Dictionary<string, ParserIdentifier>();
        public MiniPLExceptionThrower miniPLExceptionThrower;
        public MiniPLHelper miniPLHelper;
        private int forLoopIndex = 0;
        private List<string> errors;
        private int tokenI;
        public MiniPLParser()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("Parser");
            this.miniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
        }

        private Token CurrentToken()
        {
            return Tokens[tokenI];
        }
        private Token NextToken()
        {
            Token current = CurrentToken();
            if (current.value == "EOF")
            {
                return current;
            }
            tokenI++;
            return Tokens[tokenI];
        }

        public Node<String> parse(List<Token> tokens)
        {
            this.identifiers = new Dictionary<string, ParserIdentifier>();
            this.forLoopIndex = 0;
            this.errors = new List<string>();
            this.Tokens = tokens;
            this.tokenI = 0;
            Node<String> parseTree = new Node<String>("program");

            stmt_list(parseTree);

            return parseTree;

        }



        public void stmt_list(Node<String> parent)
        {

            if (CurrentToken().value == "EOF" || CurrentToken().value == "end")
            {
                return;
            }


            stmt(parent);

            stmt_list(parent);
        }
        public void stmt(Node<String> parent)
        {

            Node<string> statementNode = parent;


            Token currentToken = CurrentToken();


            if (currentToken.value == ";")
            {
                currentToken = NextToken();
            }

            if (currentToken.value == "EOF" || currentToken.value == "end")
            {
                return;
            }

            if (currentToken.value == "var")
            {
                this.var(statementNode);
            }
            else if (currentToken.value == "read")
            {
                read(statementNode);
            }
            else if (currentToken.value == "print")
            {
                print(statementNode);
            }
            else if (currentToken.value == "assert")
            {
                assert(statementNode);
            }
            else if (currentToken.value == "for")
            {
                forLoop(statementNode);
            }
            else
            {
                Console.WriteLine("Curr: " + currentToken.value);
                identAssignment(statementNode);
            }
        }

        public void forLoop(Node<String> parent)
        {
            forLoopIndex++;

            Node<String> loopNode = new Node<string>("forloop");
            parent.children.Add(loopNode);

            NextToken();

            Token identifierToken = CurrentToken();
            this.identifierCheck(identifierToken);
            ParserIdentifier pI = this.identifiers.GetValueOrDefault(identifierToken.value, new ParserIdentifier("undefined", -1));
            if (pI.type == "undefined")
            {
                miniPLExceptionThrower
                    .throwUndefinedVariableError(identifierToken.row, identifierToken.value);
            }

            if (pI.type != "int")
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Identifier error at row {identifierToken.row}: '{identifierToken.value}' has invalid type '{pI.type}', expected 'int'");
            }

            loopNode.children.Add(new Node<string>(identifierToken.value));

            NextToken();

            Token inToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(inToken, "in");


            NextToken();
            string typeOfFirstExpr = expr(loopNode);

            Token shouldBeDotsToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeDotsToken, "..");
            NextToken();




            string typeOfSecondExpr = expr(loopNode);
            if (typeOfFirstExpr != "int" || typeOfSecondExpr != "int")
            {
                miniPLExceptionThrower.throwInvalidExpressionError(CurrentToken().row, "int", typeOfSecondExpr);
            }
            Token shouldBeDoToken = CurrentToken();

            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeDoToken, "do");

            NextToken();

            stmt_list(loopNode);

            Token shouldBeEndToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeEndToken, "end");



            loopNode.children.Add(new Node<string>("end"));
            NextToken();

            Token shouldBeForToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeForToken, "for");

            loopNode.children.Add(new Node<string>("for"));
            foreach (string key in identifiers.Keys)
            {
                ParserIdentifier pi = identifiers.GetValueOrDefault(key, new ParserIdentifier("undefined", -1));
                if (pi.forLoopIndex == forLoopIndex)
                {
                    identifiers.Remove(key);
                }
            }
            forLoopIndex--;
            NextToken();
        }

        public void identAssignment(Node<String> parent)
        {
            Token ident = CurrentToken();
            identifierCheck(ident);
            string identifier = ident.value;
            NextToken();
            Node<string> assignmentNode = new Node<string>("assignment");

            parent.children.Add(assignmentNode);

            if (identifiers.GetValueOrDefault(identifier, new ParserIdentifier("undefined", -1)).type == "undefined")
            {
                miniPLExceptionThrower.throwUndefinedVariableError(ident.row, identifier);
            }
            assignmentNode.children.Add(new Node<string>(identifier));

            Token assignmentToken = CurrentToken();

            miniPLHelper.checkTokenThrowsMiniPLError(assignmentToken, ":=");

            NextToken();
            string type = expr(assignmentNode);
            ParserIdentifier pI = identifiers.GetValueOrDefault(identifier);
            if (type != pI.type)
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid assignment at row {ident.row}: cannot convert type '{pI.type}' to type '{type}' ");
            }
            NextToken();

        }
        public void assert(Node<String> parent)
        {

            Node<String> assertNode = new Node<string>("assert");

            parent.children.Add(assertNode);
            Token first = CurrentToken();
            NextToken();
            Token leftParenthesis = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(leftParenthesis, "(");


            string expressionType = expr(assertNode);
            if (expressionType != "bool")
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"expected expression return value 'bool' at row {first.row}, found {expressionType}");
            }

            NextToken();


        }
        public void print(Node<String> parent)
        {
            Node<String> printNode = new Node<string>("print");
            parent.children.Add(printNode);
            NextToken();
            expr(printNode);

        }

        public void read(Node<String> parent)
        {
            Node<String> readNode = new Node<string>("read");

            parent.children.Add(readNode);
            NextToken();
            if (1 == 2)
            {

            }
            else
            {
                Token varIdent = CurrentToken();

                ParserIdentifier pI = this.identifiers.GetValueOrDefault(varIdent.value, new ParserIdentifier("undefined", -1));

                readNode.children.Add(new Node<string>(varIdent.value));

                if (pI.type == "undefined")
                {
                    miniPLExceptionThrower.throwUndefinedVariableError(varIdent.row, varIdent.value);
                }

                List<string> validReadTypes = new List<string> { "int", "string" };
                if (!validReadTypes.Contains(pI.type))
                {
                    miniPLExceptionThrower
                        .throwMiniPLException($"Invalid read at row {varIdent.row}. Cannot read variable of type '{pI.type}'");
                }
                NextToken();
            }

        }

        public void var(Node<String> parent)
        {
            Token first = CurrentToken();
            Node<String> varAssignmentNode = new Node<string>("var_assignment");
            parent.children.Add(varAssignmentNode);

            NextToken();
            Token identToken = CurrentToken();

            this.identifierCheck(identToken);
            NextToken();
            varAssignmentNode.children.Add(new Node<String>(identToken.value));

            miniPLHelper.checkTokenThrowsMiniPLError(CurrentToken(), ":");
            NextToken();
            Token shouldBeType = CurrentToken();
            string type = this.validTypeCheck(shouldBeType);

            if (this.identifiers.GetValueOrDefault(identToken.value, new ParserIdentifier("undefined", -1)).type != "undefined")
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Duplicate variable assignment at row {identToken.row}");
            }
            else
            {
                this.identifiers.Add(identToken.value, new ParserIdentifier(type, forLoopIndex));
            }

            varAssignmentNode.children.Add(new Node<String>(type));
            NextToken();
            if (CurrentToken().value == ";")
            {
                NextToken();
                return;
            }

            Token assignmentToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(assignmentToken, ":=");
            NextToken();
            expr(varAssignmentNode);
            NextToken();
        }

        public string expr(Node<String> parent)
        {


            Token first = CurrentToken();

            if (miniPLHelper.isUnaryOperator(first.value))
            {

                Node<string> unaryNode = new Node<string>("!");
                parent.children.Add(unaryNode);

                NextToken();
                string operandType = Operand(unaryNode);
                if (operandType != "bool")
                {
                    miniPLExceptionThrower.throwInvalidUsageOfOperatorError(first.row, "!", operandType);
                }
                return "bool";
            }
            else
            {
                Node<string> operatorNode = new Node<string>("___");
                parent.children.Add(operatorNode);
                string firstOperandType = Operand(operatorNode);
                Token currentToken = CurrentToken();
                if (miniPLHelper.isOperator(currentToken.value) && currentToken.value != "!")
                {
                    operatorNode.value = currentToken.value;
                    string finalType = expr_tail(operatorNode, parent, firstOperandType);
                    return finalType;
                }

                else
                {
                    parent.children.RemoveAt(parent.children.Count - 1);
                    foreach (Node<string> child in operatorNode.children)
                    {
                        parent.children.Add(child);
                    }

                    return firstOperandType;


                }
            }



        }

        public string expr_tail(Node<String> operatorNode, Node<String> parentNode, string previousType)
        {
            Token operatorToken = CurrentToken();
            string op = operatorToken.value;
            Console.WriteLine("OP " + op);

            if (!miniPLHelper.isValidOperatorForType(op, previousType))
            {
                miniPLExceptionThrower.throwInvalidOperatorError(operatorToken.row, op, previousType);
            }

            NextToken();

            Token currentToken = CurrentToken();
            string type = Operand(operatorNode);

            if (type != previousType)
            {
                miniPLExceptionThrower.throwInvalidExpressionError(currentToken.row, previousType, type);
            }

            if (miniPLHelper.isOperator(currentToken.value))
            {
                Node<string> newOperatorNode = new Node<string>(currentToken.value);
                parentNode.children.Add(newOperatorNode);
                expr_tail(newOperatorNode, operatorNode, type);
            }

            if (op == "+")
            {
                return previousType;
            }
            else
            {
                Console.WriteLine("HERE");
                Console.WriteLine(miniPLHelper.getReturnTypeFromOperator(op));
                return miniPLHelper.getReturnTypeFromOperator(op);
            }


        }

        public string Operand(Node<String> parent)
        {
            Node<String> operandNode = parent;


            Token first = CurrentToken();
            if (first.value == "(")
            {
                NextToken();
                string type = expr(operandNode);
                Token rightParenth = CurrentToken();
                miniPLHelper.checkTokenThrowsMiniPLError(rightParenth, ")");
                NextToken();
                return type;
            }
            else
            {


                string value = first.value;

                NextToken();
                bool isNumeric = int.TryParse(value, out int numericValue);
                if (isNumeric)
                {
                    operandNode.children.Add(new Node<String>($"int({numericValue})"));
                    return "int";
                }
                else if (miniPLHelper.isString(value))
                {
                    string splitted = value.Substring(1, value.Length - 2);
                    operandNode.children.Add(new Node<String>($"string({splitted})"));
                    return "string";
                }
                else if (miniPLHelper.isBoolean(value))
                {
                    operandNode.children.Add(new Node<String>($"bool({value})"));
                    return "bool";
                }
                else
                {

                    identifierCheck(first);

                    bool isIdentDefined = isIdentifierDefined(value);
                    if (!isIdentDefined)
                    {
                        miniPLExceptionThrower.throwUndefinedVariableError(first.row, value);
                    }
                    ParserIdentifier p = this.identifiers.GetValueOrDefault(value);

                    operandNode.children.Add(new Node<String>($"id({value})"));
                    return p.type;
                }

            }
        }

        public string identifierCheck(Token t)
        {
            string value = t.value;
            Console.WriteLine($"In ident check: {t.value}");
            if (miniPLHelper.isReservedKeyword(value))
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid identifier: invalid usage of keyword '{value}' as identifier at row {t.row}");
            }

            char firstChar = value[0];
            if (!Char.IsLetter(firstChar))
            {
                miniPLExceptionThrower.throwMiniPLException($"Invalid identifier '{value}' at row {t.row}");
            }

            foreach (char c in value)
            {
                if (!Char.IsNumber(c) && !Char.IsLetter(c) && c != '_')
                {
                    miniPLExceptionThrower.throwMiniPLException($"Invalid identifier '{value}' at row {t.row}");
                }
            }


            return value;
        }

        public string validTypeCheck(Token t)
        {

            if (!miniPLHelper.isValidType(t))
            {
                miniPLExceptionThrower.throwMiniPLException($"Invalid type '{t.value}' at row {t.row}");
            }

            return t.value;


        }

        private bool isIdentifierDefined(string ident)
        {
            ParserIdentifier p = this.identifiers.GetValueOrDefault(ident, new ParserIdentifier("undefined", -1));

            return p.type != "undefined";
        }
    }


}
