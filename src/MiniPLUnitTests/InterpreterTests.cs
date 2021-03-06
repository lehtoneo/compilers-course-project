using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniPLInterpreter.Scanner;
using MiniPLInterpreter.Parser;
using MiniPLInterpreter.Interfaces;
using MiniPLInterpreter.Interpreter;
using Moq;
namespace MiniPLUnitTests
{
    [TestClass]
    public class InterpreterTests
    {
        [TestMethod]
        public void TestProgram1()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program1 = new string[] { "var X : int := 4 + (6 * 2);", "print X;" };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program1);

            mockConsoleIO.Verify(t => t.Write("16"), Times.Once());

        }

        [TestMethod]
        public void TestProgram2()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var times = "2";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(times);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "var nTimes : int := 0;",
            "print \"How many times?\";",
            "read nTimes;",
            "var x : int;",
            "for x in 0..nTimes - 1 do",
            "        print x;",
            "      print \" : Hello, World!\n\";",
            "end for;",
            "assert(x = nTimes); " };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);
            mockConsoleIO.Verify(t => t.Write("How many times?"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("0"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("1"), Times.Once());
            mockConsoleIO.Verify(t => t.Write(" : Hello, World!\n"), Times.AtLeastOnce());
            mockConsoleIO.Verify(t => t.WriteLine("ASSERT false"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram3()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "9";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "print \"Give a number\";",
            "var n : int;",
            "read n;",
            "var v : int := 1;",
            "var i : int;",
            "for i in 1..n do",
            "   v:= v * i;",
            "end for;",
            "print \"The result is: \";",
            "print v;"
        };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);

            mockConsoleIO.Verify(t => t.Write("Give a number"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("The result is: "), Times.Once());
            mockConsoleIO.Verify(t => t.Write("362880"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram3WithComments()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "9";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "print \"Give a number\";",
            "var n : int; /*",
            "print \"Give a number\";",
            "var n : int; */",
            "read n;",
            "var v : int := 1;",
            "var i : int;",
            "for i in 1..n do",
            "   v:= v * i; // moi",
            "end for;",
            "print \"The result is: \";",
            "print v;"
        };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);

            mockConsoleIO.Verify(t => t.Write("Give a number"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("The result is: "), Times.Once());
            mockConsoleIO.Verify(t => t.Write("362880"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram4WithErrors()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "9";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "var _nTimes : int := 0;",
            "print \"How many times?\";",
            " ",
            "read _nTimes;",
            "var x : int;",
            "for x in 0..kk - 1 do",
            "   print Y;",
            "   print \" : Hello, World!\n\";",
            "end for;",
            "sdjijasdjiasd(x = _nTimes);"
        };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);

            mockConsoleIO.Verify(t => t.WriteLine("Errors:"), Times.Once());
            mockConsoleIO.Verify(t => t.WriteLine("Parser error: Invalid identifier '_nTimes' at row 1"), Times.Once());
            mockConsoleIO.Verify(t => t.WriteLine("Parser error: Undefined variable at row 6, col 14: Variable 'kk' is undefined"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram5WithEmptyForLoop()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "9";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "var n : int := 0;",
            "print \"How many times?\";",
            " ",
            "read n;",
            "var x : int;",
            "for x in 0..n-1 do",

            "end for;",
            "assert (x = n);"
        };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);

            mockConsoleIO.Verify(t => t.WriteLine("ASSERT false"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram5WithNestedForLoop()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "2";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            MiniPLScanner scanner = new MiniPLScanner();
            MiniPLParser mPLP = new MiniPLParser();

            string[] program3 = new string[] {
            "var n : int := 0;",
            "print \"How many times?\";",
            " ",
            "read n;",
            "var x : int;",
            "var y : int;",
            "for x in 0..n-1 do",
                "for y in 0..n-1 do",
                    "print 2 * (2 * 2);",
                "end for;",
            "end for;",
            "assert (x = n);"
        };

            var interpreter = new MPLInterpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);
            mockConsoleIO.Verify(t => t.Write("8"), Times.Exactly(4));
            mockConsoleIO.Verify(t => t.WriteLine("ASSERT false"), Times.AtLeastOnce());

        }


    }
}
