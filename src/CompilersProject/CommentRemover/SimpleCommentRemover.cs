using System;
using System.Collections.Generic;
using CompilersProject.Exceptions;
using CompilersProject.Interfaces;
namespace CompilersProject.Implementations
{
    public class SimpleCommentRemover : ICommentRemover
    {
        MiniPLExceptionThrower miniPLExceptionThrower;
        public SimpleCommentRemover()
        {
            this.miniPLExceptionThrower = new MiniPLExceptionThrower("CommentRemover");
        }
        public string[] removeComments(string[] miniPlProgram)
        {
            string[] bigCommentsRemoved = removeBigComments(miniPlProgram);
            string[] smallCommentsRemoved = removeSmallComments(bigCommentsRemoved); 
            return smallCommentsRemoved;
        }

        public string[] removeSmallComments(string[] miniPlProgram)
        {
            List<string> smallCommentsRemoved = new List<string>();
            foreach (string line in miniPlProgram)
            {
                string newLine = line;
                int commentStartIndex = line.IndexOf("//");
                if (commentStartIndex != -1)
                {
                    newLine = line.Substring(0, commentStartIndex);
                }
                smallCommentsRemoved.Add(newLine);
            }

            return smallCommentsRemoved.ToArray();
        }


        public string[] removeBigComments(string[] miniPlProgram)
        {
            List<string> bigCommentsRemoved = new List<string>();

            int i = 0;
            foreach (string line in miniPlProgram)
            {
                
                bigCommentsRemoved.Add(line);
                i++;
            }

            while (true)
            {
                int startIndex = 0;
                int[] commentIndexes = getFirstBigCommentIndexes(startIndex, bigCommentsRemoved);
                int commentStartIndex = commentIndexes[0];
                int commentEndIndex = commentIndexes[1];
                startIndex = commentEndIndex;
                if (commentEndIndex == -1 || commentStartIndex == -1)
                {
                    break;
                }
                int j = commentStartIndex;

                while (j <= commentEndIndex)
                {
                    string line = bigCommentsRemoved[j];
                    bool isFirstLine = j == commentStartIndex;
                    bool isLastLine = j == commentEndIndex;
                    if (isFirstLine && isLastLine)
                    {
                        int commentStartStringIndex = line.IndexOf("/*");
                        int commentEndStringIndex = line.IndexOf("*/");

                        int subStringStart = commentStartStringIndex + 2;
                        int subStringLength = commentEndStringIndex - subStringStart;
                        string newLine = line.Substring(subStringStart, subStringLength);
                        bigCommentsRemoved[j] = newLine;
                    } else if (isFirstLine)
                    {
                        int commentStartStringIndex = line.IndexOf("/*");

                        int subStringLength = commentStartStringIndex;
                        string newLine = line.Substring(0, subStringLength);
                        bigCommentsRemoved[j] = newLine;
                    } else if (isLastLine)
                    {
                        int commentEndStringIndex = line.IndexOf("*/");
                        int subStringStart = commentEndStringIndex + 2;
                        int subStringLength = line.Length - subStringStart;
                        string newLine = line.Substring(subStringStart, subStringLength);
                        bigCommentsRemoved[j] = newLine;
                    } else
                    {
                        string newLine = "";
                        bigCommentsRemoved[j] = newLine;

                    }
                    j++;
                }




            }

            
            return bigCommentsRemoved.ToArray();
        }

        // todo: check the comment syntax? e.g comment needs to start before it ends
        public int[] getFirstBigCommentIndexes(int startIndex, List<string> lines)
        {
            int[] result = new int[2];

            result[0] = -1;
            result[1] = -1;
            int i = startIndex;
            while (i < lines.Count)
            {
                string line = lines[i];
                int indexOfCommentStartInline = line.IndexOf("/*");
                int indexOfCommentEndInLine = line.IndexOf("*/");
                if (indexOfCommentStartInline != -1 && result[0] == -1)
                {
                    result[0] = i;
                }

                if (indexOfCommentEndInLine != -1)
                {
                    if (result[0] == -1)
                    {
                        miniPLExceptionThrower.throwUnExpectedSymbolError(i, 'a');
                    }

                    result[1] = i;
                    break;
                }


                i++;
            }
            if (result[0] != -1 && result[1] == -1)
            {
                miniPLExceptionThrower.throwUnExpectedSymbolError(result[0], '/');
            }
            return result;
        }


        public string removeComments(string miniPlProgram)
        {
            throw new NotImplementedException();
        }
    };
}
