using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringAnalyzer
{
    class FileLoader
    {
        public static string? SelectFile()
        {
            bool fileFound = false;
            string? command = "";
            string? fileName = null;
            while (fileFound == false)
            {
                Console.Write("Load text file (q to quit): ");
                command = Console.ReadLine();
                if (command?.ToLower() == "q")
                {
                    return null;
                }
                fileName = command;
                if (File.Exists(fileName) == false)
                {
                    Console.WriteLine("File does not exist, try again");
                }
                else
                {
                    fileFound = true;
                }
            }
            return fileName;
        }

        public static string[] LoadLines(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            return lines;
        }
        public static string LoadText(string fileName)
        {
            string text = File.ReadAllText(fileName);
            return text;
        }
    }
}
