using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject.Interfaces
{
    public interface ICommentRemover
    {
        public string[] removeComments(string[] miniPlProgram);
        public string removeComments(string miniPlProgram);
    }
}
