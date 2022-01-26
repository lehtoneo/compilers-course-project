using System;
using System.Collections.Generic;
using System.Text;
using CompilersProject.Interfaces;
namespace CompilersProject.Implementations
{
    public class Interpreter : IInterpreter
    {
        public string[] MiniPlProgram { get; set; }
        private ICommentRemover CommentRemover;

        

        public Interpreter(ICommentRemover commentRemover)
        {
            this.CommentRemover = commentRemover;
        }

        public Interpreter(string[] miniPLProgram, ICommentRemover commentRemover) : this(commentRemover)
        {
            this.MiniPlProgram = miniPLProgram;
        }

        
        public void interpret(string[] miniPlProgram)
        {
            try
            {
                string[] commentsRemoved = CommentRemover.removeComments(miniPlProgram);
                Console.WriteLine("");
                Console.WriteLine("Comments removed:");
                foreach (string line in commentsRemoved)
                {
                    Console.WriteLine(line);
                }
            } catch (MiniPLException e)
            {
                Console.WriteLine("There was an error interpreting the minipl program");
                Console.WriteLine(e.Message);
            }
        }

        public void interpret()
        {
            if (this.MiniPlProgram == null)
            {
                throw new InvalidOperationException("MiniPlProgram not defined");
            }

            interpret(this.MiniPlProgram);
        }
    }
}
