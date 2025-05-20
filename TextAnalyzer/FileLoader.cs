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
            string? fileName = null;
            while (fileFound == false)
            {
                Console.Write("Load text file (q to quit): ");
                string? command = Console.ReadLine();
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

        public static string[] LoadLines(string fileName, Encoding encoding)
        {
            string[] lines = File.ReadAllLines(fileName, encoding);
            return lines;
        }
        public static string LoadText(string fileName, Encoding encoding)
        {
            string text = File.ReadAllText(fileName, encoding);
            return text;
        }

        internal static Encoding SelectEncoding()
        {
            Console.WriteLine("Select Encoding (Enter for UTF8):");
            Console.WriteLine("1: UTF-8");
            Console.WriteLine("2: Latin1");
            Console.WriteLine("3: UTF-16");
            Console.WriteLine("4: ASCII");
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (key)
            {
                case '1':
                    Console.WriteLine("UTF8 selected");
                    return Encoding.UTF8;
                case '2':
                    Console.WriteLine("Latin1 selected");
                    return Encoding.Latin1;
                case '3':
                    return Encoding.Unicode;
                case '4':
                    return Encoding.ASCII;
                default:
                    Console.WriteLine("UTF8 selected");
                    return Encoding.UTF8;
            }
        }
    }
}
