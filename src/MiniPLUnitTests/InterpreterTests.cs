using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompilersProject.Implementations;
using CompilersProject.Interfaces;
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
            SimpleCommentRemover scr = new SimpleCommentRemover();
            MiniPLScanner scanner = new MiniPLScanner(scr);
            MiniPLParser mPLP = new MiniPLParser();

            string[] program1 = new string[] { "var X : int := 4 + (6 * 2);", "print X;" };

            var interpreter = new Interpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program1);

            mockConsoleIO.Verify(t => t.Write("16"), Times.Once());

        }

        [TestMethod]
        public void TestProgram2()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var times = "2";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(times);

            SimpleCommentRemover scr = new SimpleCommentRemover();
            MiniPLScanner scanner = new MiniPLScanner(scr);
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

            var interpreter = new Interpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);
            mockConsoleIO.Verify(t => t.Write("How many times?"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("0"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("1"), Times.Once());
            mockConsoleIO.Verify(t => t.Write(" : Hello, World!\n"), Times.AtLeastOnce());

        }

        [TestMethod]
        public void TestProgram3()
        {
            var mockConsoleIO = new Mock<IConsoleIO>();
            var number = "9";
            mockConsoleIO.Setup(t => t.ReadLine()).Returns(number);

            SimpleCommentRemover scr = new SimpleCommentRemover();
            MiniPLScanner scanner = new MiniPLScanner(scr);
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

            var interpreter = new Interpreter(scanner, mPLP, mockConsoleIO.Object);

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

            SimpleCommentRemover scr = new SimpleCommentRemover();
            MiniPLScanner scanner = new MiniPLScanner(scr);
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

            var interpreter = new Interpreter(scanner, mPLP, mockConsoleIO.Object);

            interpreter.interpret(program3);

            mockConsoleIO.Verify(t => t.Write("Give a number"), Times.Once());
            mockConsoleIO.Verify(t => t.Write("The result is: "), Times.Once());
            mockConsoleIO.Verify(t => t.Write("362880"), Times.AtLeastOnce());

        }


    }
}
