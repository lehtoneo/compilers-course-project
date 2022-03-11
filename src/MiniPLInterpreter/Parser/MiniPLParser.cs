using System;
using System.Collections.Generic;
using System.Text;
using MiniPLInterpreter.Utils;
using MiniPLInterpreter.Interfaces;
using MiniPLInterpreter.Exceptions;

namespace MiniPLInterpreter.Parser
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

        private void GoToNextStatement()
        {
            while (true)
            {
                Token currToken = CurrentToken();
                string currTokenValue = currToken.value;
                if (currTokenValue == ";" || currTokenValue == "EOF")
                {
                    break;
                }
                else
                {
                    NextToken();
                }
            }
        }

        public ParserResult parse(List<Token> tokens)
        {
            this.identifiers = new Dictionary<string, ParserIdentifier>();
            this.forLoopIndex = 0;
            this.errors = new List<string>();
            this.Tokens = tokens;
            this.tokenI = 0;

            Node<String> ast = program(tokens);

            ParserResult parserResult = new ParserResult(ast, errors);

            return parserResult;

        }

        public Node<string> program(List<Token> tokens)
        {
            Node<String> ast = new Node<String>("program");

            stmt_list(ast);

            ast.children.Add(new Node<string>("$$"));



            return ast;
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


            try
            {

                switch (currentToken.value)
                {
                    case "var":
                        this.var(statementNode);
                        break;
                    case "read":
                        read(statementNode);
                        break;
                    case "print":
                        print(statementNode);
                        break;
                    case "assert":
                        assert(statementNode);
                        break;
                    case "for":
                        forLoop(statementNode);
                        break;
                    default:
                        identAssignment(statementNode);
                        break;
                }

            }
            catch (MiniPLException e)
            {

                errors.Add(e.Message);
                // basic case
                if (forLoopIndex == 0)
                {

                    GoToNextStatement();
                    return;
                }

                // handle errors in forloop, exit all forloops if they are nested

                while (true)
                {
                    Token currToken = CurrentToken();
                    string currTokenValue = currToken.value;
                    if (currTokenValue == "EOF")
                    {
                        return;
                    }

                    if (currToken.value == "end")
                    {
                        if (forLoopIndex == 1)
                        {
                            forLoopIndex--;
                            return;
                        }
                        else
                        {
                            forLoopIndex--;
                        }
                    }

                    NextToken();
                }
            }


        }

        public void forLoop(Node<String> parent)
        {
            forLoopIndex++;

            Node<String> loopNode = new Node<string>("forloop");
            parent.children.Add(loopNode);

            NextToken();

            Token identifierToken = CurrentToken();
            bool isIdentDefined = isIdentifierDefined(identifierToken.value);

            if (!isIdentDefined)
            {
                try
                {
                    miniPLExceptionThrower
                    .throwUndefinedVariableError(identifierToken.row, identifierToken.column, identifierToken.value);
                }
                catch (MiniPLException e)
                {
                    errors.Add(e.Message);
                }
            }

            ParserIdentifier pI = this.identifiers.GetValueOrDefault(identifierToken.value, new ParserIdentifier("undefined", -1));
            if (isIdentDefined && pI.type != "int")
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Identifier error at row {identifierToken.row} col {identifierToken.column}: '{identifierToken.value}' has invalid type '{pI.type}', expected 'int'");
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
            NextToken();
            checkCurrentTokenIsRowsEndOfLine(shouldBeForToken.row);
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

        public void identAssignment(Node<String> parent)
        {
            Token ident = CurrentToken();

            string identifier = ident.value;
            NextToken();
            Node<string> assignmentNode = new Node<string>("assignment");

            parent.children.Add(assignmentNode);
            bool isIdentDefined = isIdentifierDefined(identifier);

            if (!isIdentDefined)
            {
                try
                {
                    miniPLExceptionThrower.throwUndefinedVariableError(ident.row, ident.column, identifier);
                }
                catch (MiniPLException e)
                {
                    errors.Add(e.Message);
                }
            }

            assignmentNode.children.Add(new Node<string>(identifier));

            Token assignmentToken = CurrentToken();

            miniPLHelper.checkTokenThrowsMiniPLError(assignmentToken, ":=");

            NextToken();
            string type = expr(assignmentNode);
            ParserIdentifier pI = identifiers.GetValueOrDefault(identifier);
            if (type != pI.type && isIdentDefined)
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid assignment at row {ident.row}: cannot convert type '{pI.type}' to type '{type}' ");
            }
            checkCurrentTokenIsRowsEndOfLine(assignmentToken.row);


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

            checkCurrentTokenIsRowsEndOfLine(first.row);


        }
        public void print(Node<String> parent)
        {
            Node<String> printNode = new Node<string>("print");
            parent.children.Add(printNode);
            Token first = CurrentToken();
            NextToken();
            expr(printNode);
            checkCurrentTokenIsRowsEndOfLine(first.row);

        }

        public void read(Node<String> parent)
        {
            Node<String> readNode = new Node<string>("read");

            parent.children.Add(readNode);
            NextToken();

            Token varIdent = CurrentToken();

            ParserIdentifier pI = this.identifiers.GetValueOrDefault(varIdent.value, new ParserIdentifier("undefined", -1));

            readNode.children.Add(new Node<string>(varIdent.value));

            if (pI.type == "undefined")
            {
                try
                {
                    miniPLExceptionThrower.throwUndefinedVariableError(varIdent.row, varIdent.column, varIdent.value);
                }
                catch (MiniPLException e)
                {
                    errors.Add(e.Message);

                }
            }

            List<string> validReadTypes = new List<string> { "int", "string" };
            if (pI.type != "undefined" && !validReadTypes.Contains(pI.type))
            {
                miniPLExceptionThrower
                    .throwMiniPLException($"Invalid read at row {varIdent.row}. Cannot read variable of type '{pI.type}'");
            }
            NextToken();

            checkCurrentTokenIsRowsEndOfLine(varIdent.row);


        }

        public void var(Node<String> parent)
        {
            Token first = CurrentToken();
            Node<String> varAssignmentNode = new Node<string>("var_assignment");
            parent.children.Add(varAssignmentNode);

            NextToken();
            Token identToken = CurrentToken();

            identifierCheck(identToken);
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
                return;
            }

            Token assignmentToken = CurrentToken();
            miniPLHelper.checkTokenThrowsMiniPLError(assignmentToken, ":=");
            NextToken();
            expr(varAssignmentNode);
            checkCurrentTokenIsRowsEndOfLine(assignmentToken.row);
        }

        public string expr(Node<String> parent)
        {

            Token first = CurrentToken();

            if (miniPLHelper.isUnaryOperator(first.value))
            {

                return unary_expr(parent);
            }
            else
            {
                Node<string> operatorNode = new Node<string>("___");
                parent.children.Add(operatorNode);
                string firstOperandType = Operand(operatorNode);

                Token currentToken = CurrentToken();
                if (miniPLHelper.isOperator(currentToken.value))
                {
                    Token operatorToken = CurrentToken();
                    string op = operatorToken.value;
                    operatorNode.value = op;

                    NextToken();
                    if (!miniPLHelper.isValidOperatorForType(op, firstOperandType))
                    {
                        miniPLExceptionThrower.throwInvalidOperatorError(operatorToken.row, op, firstOperandType);
                    }


                    string secondOperandType = Operand(operatorNode);
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

        public string unary_expr(Node<string> parent)
        {
            Node<string> unaryNode = new Node<string>("!");
            parent.children.Add(unaryNode);
            Token first = CurrentToken();
            NextToken();
            string operandType = Operand(unaryNode);
            if (operandType != "bool")
            {
                miniPLExceptionThrower.throwInvalidUsageOfOperatorError(first.row, "!", operandType);
            }
            return "bool";
        }

        public string Operand(Node<String> parent)
        {
            Node<String> operandNode = parent;


            Token first = CurrentToken();
            NextToken();
            if (first.value == "(")
            {
                string type = expr(operandNode);
                Token rightParenth = CurrentToken();
                miniPLHelper.checkTokenThrowsMiniPLError(rightParenth, ")");
                NextToken();
                return type;
            }
            else
            {


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

                    bool isIdentDefined = isIdentifierDefined(value);
                    if (!isIdentDefined)
                    {
                        miniPLExceptionThrower.throwUndefinedVariableError(first.row, first.column, value);
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

            try
            {
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
            }
            catch (MiniPLException e)
            {
                errors.Add(e.Message);
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

        private void checkCurrentTokenIsRowsEndOfLine(int row)
        {
            Token curr = CurrentToken();
            if (curr.row == row && curr.value != ";")
            {
                miniPLExceptionThrower.throwMiniPLException($"SYNTAX ERROR: Expected ';', found {curr.value}, at row {row}");
            }
            else if (curr.value != ";")
            {
                miniPLExceptionThrower.throwMiniPLException($"SYNTAX ERROR: Expected ';', found nothing, at row {row}");
            }
        }

        private bool isIdentifierDefined(string ident)
        {
            ParserIdentifier p = this.identifiers.GetValueOrDefault(ident, new ParserIdentifier("undefined", -1));

            return p.type != "undefined";
        }
    }


}
