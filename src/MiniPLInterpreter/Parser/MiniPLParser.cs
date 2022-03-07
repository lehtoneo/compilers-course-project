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
        public MiniPLParser()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("Parser");
            this.miniPLHelper = new MiniPLHelper(miniPLExceptionThrower);
        }
        public Node<String> parse(List<Token> tokens)
        {
            this.identifiers = new Dictionary<string, ParserIdentifier>();
            this.forLoopIndex = 0;

            this.Tokens = tokens;

            Node<String> parseTree = new Node<String>("program");

            stmt_list(parseTree, tokens);

            return parseTree;

        }



        public void stmt_list(Node<String> parent, List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                parent.children.Add(new Node<string>("$$"));
                return;
            }

            List<Token> firstStatement = new List<Token>();
            bool inLoop = false;
            int innerLoopCount = 0;
            int i = -1;
            while (i + 1 < tokens.Count)
            {

                i++;
                Token t = tokens[i];
                string tokenValue = t.value;
                if (i == 0)
                {
                    if (tokenValue == "for")
                    {
                        firstStatement.Add(t);
                        inLoop = true;
                        continue;
                    }
                }

                if (inLoop)
                {
                    if (tokenValue == "end")
                    {
                        firstStatement.Add(t);

                        i++;
                        if (tokens.Count < i)
                        {
                            miniPLExceptionThrower
                                .throwExpectedSomethingFoundNothingError(t.row, "for");
                        }
                        Token shouldBeFor = tokens[i];
                        miniPLHelper.checkTokenThrowsMiniPLError(shouldBeFor, "for");

                        i++;
                        if (tokens.Count < i)
                        {
                            miniPLExceptionThrower
                                .throwExpectedSomethingFoundNothingError(t.row, ";");
                        }

                        Token shouldBeEndOfLine = tokens[i];
                        miniPLHelper.checkTokenThrowsMiniPLError(shouldBeEndOfLine, ";");

                        firstStatement.Add(shouldBeFor);

                        if (innerLoopCount == 0)
                        {

                            break;
                        }
                        else
                        {
                            firstStatement.Add(shouldBeEndOfLine);
                            innerLoopCount--;
                        }
                    }
                    else if (tokenValue == "for")
                    {
                        innerLoopCount++;
                        firstStatement.Add(t);
                    }
                    else
                    {
                        firstStatement.Add(t);
                    }

                }
                else
                {
                    if (tokenValue == ";")
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        firstStatement.Add(t);
                    }
                }
            }
            stmt(parent, firstStatement);
            List<Token> restOfStatements = new List<Token>();
            while (i < tokens.Count)
            {
                Token t = tokens[i];
                restOfStatements.Add(t);
                i++;
            }

            stmt_list(parent, restOfStatements);
        }
        public void stmt(Node<String> parent, List<Token> statement)
        {

            Node<string> statementNode = parent;

            if (statement.Count == 0)
            {
                statementNode.children.Add(new Node<string>("$$"));
                return;
            }
            Token firstToken = statement[0];

            if (firstToken.value == "var")
            {
                this.var(statementNode, statement);
            }
            else if (firstToken.value == "read")
            {
                read(statementNode, statement);
            }
            else if (firstToken.value == "print")
            {
                print(statementNode, statement);
            }
            else if (firstToken.value == "assert")
            {
                assert(statementNode, statement);
            }
            else if (firstToken.value == "for")
            {
                forLoop(statementNode, statement);
            }
            else
            {
                identAssignment(statementNode, statement);
            }
        }

        public void forLoop(Node<String> parent, List<Token> tokens)
        {
            forLoopIndex++;

            Node<String> loopNode = new Node<string>("forloop");
            Token first = tokens[0];
            parent.children.Add(loopNode);
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "identifier");
            }
            Token identifierToken = tokens[1];
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

            if (tokens.Count < 3)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "in");
            }
            Token inToken = tokens[2];
            if (inToken.value != "in")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    first.row,
                    inToken.value,
                    "in"
                );
            }


            if (tokens.Count < 4)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(
                    first.row,
                    "For loop expression"
                );
            }
            int firstExpressionStartIndex = 3;
            int i = firstExpressionStartIndex;
            List<Token> firstExpressionTokens = new List<Token>();

            while (i < tokens.Count)
            {
                Token t = tokens[i];
                if (t.value == "..")
                {


                    break;
                }
                else
                {
                    firstExpressionTokens.Add(t);
                }
                i++;
            }
            if (firstExpressionTokens.Count == 0)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(tokens[i - 1].row, "expression");
            }

            expr(loopNode, firstExpressionTokens);
            Token shouldBeDotsToken = tokens[i];
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeDotsToken, "..");

            i++;
            List<Token> secondExpressionTokens = new List<Token>();
            while (i < tokens.Count)
            {
                Token t = tokens[i];

                if (t.value == "do")
                {
                    break;
                }
                else
                {
                    secondExpressionTokens.Add(t);
                }
                i++;
            }

            if (secondExpressionTokens.Count == 0)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(tokens[i - 1].row, "expression");
            }

            expr(loopNode, secondExpressionTokens);
            Token shouldBeDoToken = tokens[i];

            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeDoToken, "do");

            List<Token> statements = new List<Token>();
            i++;
            if (i >= tokens.Count)
            {
                miniPLExceptionThrower
                    .throwExpectedSomethingFoundNothingError(first.row + 1, "statements");
            }

            while (i < tokens.Count - 2)
            {
                Token t = tokens[i];
                statements.Add(t);
                i++;
            }

            Token shouldBeEndToken = tokens[i];
            miniPLHelper.checkTokenThrowsMiniPLError(shouldBeEndToken, "end");

            stmt_list(loopNode, statements);


            loopNode.children.Add(new Node<string>("end"));
            i++;
            if (i >= tokens.Count)
            {
                miniPLExceptionThrower
                    .throwExpectedSomethingFoundNothingError(shouldBeEndToken.row, "for");
            }

            Token shouldBeForToken = tokens[i];
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
        }

        public void identAssignment(Node<String> parent, List<Token> tokens)
        {
            Token ident = tokens[0];
            identifierCheck(ident);
            string identifier = ident.value;

            Node<string> assignmentNode = new Node<string>("assignment");

            parent.children.Add(assignmentNode);

            if (identifiers.GetValueOrDefault(identifier, new ParserIdentifier("undefined", -1)).type == "undefined")
            {
                miniPLExceptionThrower.throwUndefinedVariableError(ident.row, identifier);
            }
            assignmentNode.children.Add(new Node<string>(identifier));
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(ident.row, ":=");
            }
            Token assignmentToken = tokens[1];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    assignmentToken.row,
                    assignmentToken.value,
                    ":="
                );
            }

            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token curToken = tokens[i];
                expressionTokens.Add(curToken);
                i++;
            }
            string type = expr(assignmentNode, expressionTokens);
            ParserIdentifier pI = identifiers.GetValueOrDefault(identifier);
            if (type != pI.type)
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid assignment at row {ident.row}: cannot convert type '{pI.type}' to type '{type}' ");
            }

        }
        public void assert(Node<String> parent, List<Token> tokens)
        {

            Node<String> assertNode = new Node<string>("assert");

            parent.children.Add(assertNode);

            Token first = tokens[0];
            if (tokens.Count < 2)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "(");
            }
            Token leftParenthesis = tokens[1];
            if (leftParenthesis.value != "(")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(
                    first.row,
                    leftParenthesis.value,
                    "("
                );
            }
            List<Token> expressionTokens = new List<Token>();
            int i = 2;
            while (i < tokens.Count)
            {
                Token currentT = tokens[i];
                if (currentT.value != ")")
                {
                    expressionTokens.Add(currentT);
                }
                else
                {
                    string expressionType = expr(assertNode, expressionTokens);
                    if (expressionType != "bool")
                    {
                        miniPLExceptionThrower
                            .throwMiniPLException($"expected expression return value 'bool' at row {first.row}, found {expressionType}");
                    }
                    return;
                }
                i++;
            }
            miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, ")");


        }
        public void print(Node<String> parent, List<Token> tokens)
        {
            Node<String> printNode = new Node<string>("print");
            parent.children.Add(printNode);
            List<Token> expressionTokens = tokens.GetRange(1, tokens.Count - 1);
            expr(printNode, expressionTokens);

        }

        public void read(Node<String> parent, List<Token> tokens)
        {
            Node<String> readNode = new Node<string>("read");

            parent.children.Add(readNode);

            if (tokens.Count != 2)
            {
                miniPLExceptionThrower.throwMiniPLException("Invalid usage of read");
            }
            else
            {
                Token varIdent = tokens[1];

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
            }

        }

        public void var(Node<String> parent, List<Token> tokens)
        {
            Token first = tokens[0];
            Node<String> varAssignmentNode = new Node<string>("var_assignment");
            parent.children.Add(varAssignmentNode);

            if (tokens.Count < 4)
            {
                miniPLExceptionThrower.throwMiniPLException($"Invalid usage of var at row {first.row}");
            }
            int identI = 1;
            Token identToken = tokens[identI];
            int typeI = 3;
            int assignMentI = 4;

            this.identifierCheck(identToken);

            varAssignmentNode.children.Add(new Node<String>(identToken.value));

            if (tokens[2].value != ":")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.row, tokens[2].value, ":");
            }

            string type = this.validTypeCheck(tokens[typeI]);

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
            if (tokens.Count == 4)
            {
                return;
            }

            Token assignmentToken = tokens[assignMentI];
            if (assignmentToken.value != ":=")
            {
                miniPLExceptionThrower.throwUnExpectedValueError(first.row, assignmentToken.value, ":=");
            }

            if (tokens.Count < 6)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, "expression");
            }

            List<Token> expression = new List<Token>();
            int i = assignMentI + 1;
            while (i < tokens.Count)
            {
                expression.Add(tokens[i]);
                i++;
            }
            expr(varAssignmentNode, expression);
        }

        public string expr(Node<String> parent, List<Token> tokens)
        {
            if (tokens.Count < 1)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(-1, "expression");
            }

            Token first = tokens[0];
            if (miniPLHelper.isUnaryOperator(first.value))
            {
                if (tokens.Count < 2)
                {
                    miniPLExceptionThrower.throwInvalidError(first.row, "expression");
                }
                Node<string> unaryNode = new Node<string>("!");
                parent.children.Add(unaryNode);

                string operandType = Operand(unaryNode, tokens.GetRange(1, tokens.Count - 1));
                if (operandType != "bool")
                {
                    miniPLExceptionThrower.throwInvalidUsageOfOperatorError(first.row, "!", operandType);
                }
                return "bool";
            }
            else
            {
                List<Token> firstOperandTokens = new List<Token>();
                Token operatorToken = null;
                int i = 0;
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if (miniPLHelper.isOperator(t.value))
                    {
                        operatorToken = t;
                        i++;
                        break;
                    }
                    else
                    {
                        firstOperandTokens.Add(t);
                        i++;
                    }
                }
                if (operatorToken == null)
                {

                    return this.Operand(parent, firstOperandTokens);
                }
                else
                {
                    string op = operatorToken.value;
                    Node<string> operatorNode = new Node<string>(op);
                    parent.children.Add(operatorNode);
                    string firstOperandType = this.Operand(operatorNode, firstOperandTokens);


                    if (!miniPLHelper.isValidOperatorForType(op, firstOperandType))
                    {
                        miniPLExceptionThrower.throwInvalidOperatorError(operatorToken.row, op, firstOperandType);
                    }

                    List<Token> secondOperandTokens = new List<Token>();

                    while (i < tokens.Count)
                    {
                        Token t = tokens[i];


                        secondOperandTokens.Add(t);
                        i++;
                    }
                    string secondOperandType = Operand(operatorNode, secondOperandTokens);
                    if (firstOperandType != secondOperandType)
                    {
                        miniPLExceptionThrower.throwInvalidExpressionError(operatorToken.row, firstOperandType, secondOperandType);
                    }

                    if (op == "+")
                    {
                        return firstOperandType;
                    }
                    else
                    {
                        return miniPLHelper.getReturnTypeFromOperator(op);
                    }


                }
            }



        }

        public string Operand(Node<String> parent, List<Token> tokens)
        {
            Node<String> operandNode = parent;

            if (tokens.Count < 1)
            {
                miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(-1, "operand");
            }
            Token first = tokens[0];
            if (first.value == "(")
            {
                int i = 1;
                List<Token> expression = new List<Token>();
                bool rightParFound = false;
                while (i < tokens.Count)
                {
                    Token t = tokens[i];
                    if (t.value == ")")
                    {
                        rightParFound = true;
                        break;
                    }
                    else
                    {
                        expression.Add(t);
                    }
                    i++;
                }
                // ")" not found
                if (rightParFound)
                {
                    return expr(operandNode, expression);
                }
                else
                {
                    miniPLExceptionThrower.throwExpectedSomethingFoundNothingError(first.row, ")");
                    // to satisfy c# compiler
                    return "";
                }


            }
            else
            {
                if (tokens.Count != 1)
                {
                    miniPLExceptionThrower.throwMiniPLException($"Invalid operand at row {first.row} near '{first.value}'");
                }

                string value = first.value;

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
