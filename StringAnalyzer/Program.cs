// See https://aka.ms/new-console-template for more information
using StringAnalyzer;
using System.Drawing;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("String analyzer");

//bool fileFound = false;
//string? file = null;

//while (fileFound == false)
//{
//    Console.Write("Load text file: ");
//    file = Console.ReadLine();
//    if (File.Exists(file) == false)
//    {
//        Console.WriteLine("File does not exist, try again");
//    }
//    else
//    {
//        fileFound = true;
//    }
//}

//if (file == null)
//{
//    Console.WriteLine("File name is null, exiting");
//    return;
//}

bool fileLoaded = false;
string[] lines = { };


bool quit = false;
int lineNumber = -1;

while (!quit)
{
    if (fileLoaded == false)
    {
        string? fileName = FileLoader.SelectFile();
        if (fileName == null) return;
        lines = FileLoader.LoadLines(fileName);
        if (lines.Length > 0)
        {
            fileLoaded = true;
        }
        else
        {
            Console.WriteLine("File empty!");
        }
    }
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.WriteLine("s > Search for text.  a > analyze line chars.  q > quit.  l > load file.  [number] > select line.");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("s/q/a/l/number: ");
    string? command = Console.ReadLine();
    if (command == null) continue;
    if (command.ToLower() == "q")
    { 
        quit = true;
    }
    else if (command.ToLower() == "l")
    {
        fileLoaded = false;
        // go back to file select
    }
    else if (command.ToLower() == "s")
    {
        Console.Write("Search for: ");
        string? search = Console.ReadLine();
        if (search == null) continue;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToLower().Contains(search))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{i}: {lines[i]}");
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
    }
    else if (command.ToLower() == "a")
    {
        if (lineNumber < 0)
        {
            Console.WriteLine("Specify a line number first");
        }
        else
        {
            Console.WriteLine($"Analyzing line number {lineNumber}");
            if (lineNumber < 0 || lineNumber >= lines.Length)
            {
                Console.WriteLine("Invalid line number");
            }
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < lines[lineNumber].Length; i++)// char c in lines[lineNumber])
            {
                char c = lines[lineNumber][i];
                string hex = ((int)c).ToString("X4");
                Console.WriteLine($"{c} : {(int)c} / {hex}");
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
    else
    {

        try
        {
            lineNumber = int.Parse(command);
            if (lineNumber < lines.Length && lineNumber >= 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{lineNumber}: {lines[lineNumber]}");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.WriteLine($"Line number {lineNumber} is out of range, total lines {lines.Length}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Invalid input");
        }
    }
}