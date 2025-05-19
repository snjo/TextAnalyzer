using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
                for (int i = currentLine - 1; i < currentLine+6; i++)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    if (i < 0)
                    {
                        Console.WriteLine("START OF FILE");
                    }
                    else if (i == lines.Length)
                    {
                        Console.WriteLine($"END OF FILE");
                    }
                    else if (i > lines.Length)
                    {
                        Console.WriteLine($".");
                    }
                    else
                    {
                        Console.Write($"[{(currentLine + (i - currentLine)).ToString().PadLeft(4, '0')}] ");
                        Console.ForegroundColor = ConsoleColor.White;
                        string displayText = lines[i].Substring(0, Math.Min(lines[i].Length, ConsoleWidth - LineInfoPadding));
                        displayText = displayText.Replace('\t', '▓');
                        displayText = displayText.Replace('\uFFFD', '█'); // make unicode replacement characters obvious
                        Console.WriteLine(displayText);
                    }
                }

                Debug.WriteLine($"cl:{currentLine} / {lines.Length}   cc:{currentCol}");

                // Create unicode character list
                TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(lines[currentLine]);
                int count = 0;
                List<string> textElements = [];
                while (enumerator.MoveNext())
                {
                    string sub = (string)enumerator.Current;
                    sub = sub.Replace('\t', '▓');
                    sub = sub.Replace('\uFFFD', '█');
                    textElements.Add(sub);
                    count++;
                }
                int currentLineLength = textElements.Count;

                // DRAW SELECTED CHARACTER
                int clampedCol = Math.Min(currentCol, textElements.Count - 1);
                clampedCol = Math.Max(0, clampedCol); // prevent negative numbers
                clampedCol = Math.Min(clampedCol, ConsoleWidth - LineInfoPadding - 1);
                string SelectedChar = "";
                if (textElements.Count > 0)//lines[currentLine].Length > 0)
                {
                    SelectedChar = textElements[clampedCol];
                }
                else
                {
                    Debug.WriteLine("line is empty");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"-----");
                Console.WriteLine($"Line:{currentLine} Col:{clampedCol}");
                Console.WriteLine($"Character: {SelectedChar}");
                

                if (clampedCol != currentCol) currentCol = clampedCol;

                string hex = "...";
                if (SelectedChar != null && SelectedChar.Length > 0)
                {
                    //hex = ((int)SelectedChar).ToString("X4");
                    //int codepoint = char.ConvertToUtf32(sub, 0);
                    hex = $"{char.ConvertToUtf32(SelectedChar, 0):X4}";
                }
                Console.WriteLine($"Hex code: {hex}");
                string symbolname = "UNKNOWN";
                if (unicodeSymbols.TryGetValue(hex, out string? value))
                {
                    symbolname = value;
                }
                Console.WriteLine($"Unicode name: {symbolname}");

                

                Console.SetCursorPosition(clampedCol + LineInfoPadding, 2);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Green;
                string displayChar = "";
                if (SelectedChar != null)
                {
                    displayChar = SelectedChar; //SelectedChar.ToString() + "";
                    displayChar = displayChar.Replace('\t', '▓');
                }
                Console.Write(displayChar);
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
                    if (currentCol >= currentLineLength)
                    {
                        currentCol = currentLineLength - 1;
                    }
                }
            }
        }
    }
}
