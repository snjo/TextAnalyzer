using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringAnalyzer
{
    class TextDisplay
    {
        public static void NavigateText(string text, Dictionary<string, string> unicodeSymbols)
        {
            bool quit = false;
            string[] lines = text.Split(Environment.NewLine);
            int currentLine = 0;
            if (text.Length < 1) return;
            if (lines.Length < 1) return;
            int currentCol = 0;
            int LineInfoPadding = 7;

            ConsoleKeyInfo keyInfo;

            while (quit == false)
            {
                int ConsoleWidth = Console.WindowWidth;
                Console.Clear();
                Console.WriteLine("TEXT NAVIGATOR");
                for (int i = currentLine - 1; i < currentLine+2; i++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    if (i < 0)
                    {
                        Console.WriteLine("START OF FILE");
                    }
                    else if (i >= lines.Length)
                    {
                        Console.WriteLine($"END OF FILE");
                    }
                    else
                    {
                        Console.Write($"[{(currentLine + (i-currentLine)).ToString().PadLeft(4, '0')}] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(lines[i].Substring(0,Math.Min(lines[i].Length,ConsoleWidth-LineInfoPadding)));
                    }
                }
                
                

                // DRAW SELECTED CHARACTER
                int clampedCol = Math.Min(currentCol, lines[currentLine].Length - 1);
                clampedCol = Math.Max(0, clampedCol);
                clampedCol = Math.Min(clampedCol, ConsoleWidth - LineInfoPadding - 1);
                char SelectedChar = lines[currentLine][clampedCol];

                Console.WriteLine($"-----");
                Console.WriteLine($"Line:{currentLine} Col:{clampedCol}");
                Console.WriteLine($"Character: {SelectedChar}");
                

                if (clampedCol != currentCol) currentCol = clampedCol;

                string hex = ((int)SelectedChar).ToString("X4");
                Console.WriteLine($"Hex code: {hex}");
                string symbolname = "UNKNOWN";
                if (unicodeSymbols.ContainsKey(hex))
                {
                    symbolname = unicodeSymbols[hex];
                }
                Console.WriteLine($"Unicode name: {symbolname}");


                Console.SetCursorPosition(clampedCol + LineInfoPadding, 2);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(SelectedChar);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Q || keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                    return;
                }
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    currentLine--;
                    if (currentLine < 0)
                    {
                        currentLine = 0;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    currentLine++;
                    if (currentLine >= lines.Length)
                    {
                        currentLine = lines.Length - 1;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    currentCol--;
                    if (currentCol < 0)
                    {
                        currentCol = 0;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    currentCol++;
                    if (currentCol >= lines[currentLine].Length)
                    {
                        currentCol = lines[currentLine].Length - 1;
                    }
                }
            }
        }
    }
}
