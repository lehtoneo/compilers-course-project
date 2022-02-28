﻿using System;
using System.IO;
using CompilersProject.Interfaces;

namespace CompilersProject.Implementations
{
    class MiniPLFileNameInput : IInput
    {

        private string getPath()
        {
            bool validNameFound = false;
            string miniPLPath = "";
            while (!validNameFound)
            {
                Console.WriteLine("Give the name of MiniPL program in folder (miniPL-programs):");

                string input = Console.ReadLine();
                string path = $"miniPL-programs\\{input}";

                string projectDir = System.AppDomain.CurrentDomain.BaseDirectory;
                projectDir = Path.Combine(projectDir, "..");
                projectDir = Path.Combine(projectDir, "..");
                projectDir = Path.Combine(projectDir, "..");

                miniPLPath = Path.Combine(projectDir, path);

                if (File.Exists(miniPLPath))
                {

                    validNameFound = true;
                }
                else
                {
                    Console.WriteLine($"File {miniPLPath} not found");
                    Console.WriteLine($"Try again or CTRL + C to exit");
                }
            }
            return miniPLPath;
        }
        public string[] readMiniPLProgram()
        {
            string miniPLPath = this.getPath();

            string[] lines = System.IO.File.ReadAllLines($@"{miniPLPath}");

            return lines;
        }
    }
}
