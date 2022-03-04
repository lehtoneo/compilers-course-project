using System;
using System.IO;
using MiniPLInterpreter.Interfaces;

namespace MiniPLInterpreter.Implementations
{
    class MiniPLFileNameInput : IInput
    {

        private string getPath()
        {
            string projectDir = System.AppDomain.CurrentDomain.BaseDirectory;
            projectDir = Path.Combine(projectDir, "..");
            projectDir = Path.Combine(projectDir, "..");
            projectDir = Path.Combine(projectDir, "..");

            string miniPLPath = Path.Combine(projectDir, "miniPL-programs");
            while (true)
            {

                Console.WriteLine("Give the name of MiniPL program in folder (miniPL-programs):");

                string input = Console.ReadLine();

                string finalPath = Path.Combine(miniPLPath, input);
                if (File.Exists(finalPath))
                {

                    return finalPath;
                }
                else
                {
                    Console.WriteLine($"File {finalPath} not found");
                    Console.WriteLine($"Try again or CTRL + C to exit");
                }
            }

        }
        public string[] readMiniPLProgram()
        {
            string miniPLPath = this.getPath();

            string[] lines = System.IO.File.ReadAllLines($@"{miniPLPath}");

            return lines;
        }
    }
}
