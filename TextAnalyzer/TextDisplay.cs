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
        public static void MenuWord(string menuText, bool newLine = false)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(menuText[0..1]);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(menuText[1..]);
            Console.ForegroundColor = previousColor;
            if (newLine) Console.WriteLine();
        }

        public static void MenuWord(string dim1, string highlight, string dim2, bool newLine = false)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(dim1);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(highlight);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(dim2);
            Console.ForegroundColor = previousColor;
            if (newLine) Console.WriteLine();
        }

        public static void NavigateText(string text, Dictionary<string, string> unicodeSymbols, bool useTextElements = true)
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
                for (int i = currentLine - 1; i < currentLine + 6; i++)
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
                        displayText = displayText.Replace('\t', '\u2B7E'); // tab symbol ⭾ 0x2B7E // ▓
                        //displayText = displayText.Replace('\uFFFD', '\u2370'); // box question mark ⍰ 0x2370 // █
                        Console.WriteLine(displayText);
                    }
                }

                Debug.WriteLine($"cl:{currentLine} / {lines.Length}   cc:{currentCol}");

                // Create unicode character list

                List<string> textElements = [];

                if (useTextElements)
                {
                    textElements = GetTextElements(lines[currentLine]);
                }
                else
                {
                    textElements = GetCharacters(lines[currentLine]);
                }

                int currentLineLength = textElements.Count;

                // DRAW SELECTED CHARACTER
                int clampedCol = Math.Min(currentCol, textElements.Count - 1);
                clampedCol = Math.Max(0, clampedCol); // prevent negative numbers
                clampedCol = Math.Min(clampedCol, ConsoleWidth - LineInfoPadding - 1);
                string SelectedChar = "";
                if (textElements.Count > 0)
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
                    try
                    {
                        hex = $"{char.ConvertToUtf32(SelectedChar, 0):X4}";
                    }
                    catch
                    {
                        Debug.WriteLine("Error decoding character");
                    }
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
                    displayChar = SelectedChar;
                    displayChar = displayChar.Replace('\t', '\u2B7E'); // tab symbol ⭾ 0x2B7E
                }
                Console.Write(displayChar);
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.SetCursorPosition(0, 14);
                MenuWord("Use ", "arrow keys", " to navigate", true);
                MenuWord("Line select", true);
                MenuWord("Quit", true);

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
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    Console.SetCursorPosition(0, 15);
                    Console.Write("Select line number: ");
                    string? gotoLine = Console.ReadLine();
                    if (int.TryParse(gotoLine, out int selectline))
                    {
                        currentLine = selectline;
                    }
                }
            }

            static List<string> GetTextElements(string text)
            {
                TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);

                int count = 0;
                List<string> textElements = [];
                while (enumerator.MoveNext())
                {
                    string sub = (string)enumerator.Current;
                    textElements.Add(sub);
                    count++;
                }

                return textElements;
            }
        }

        private static List<string> GetCharacters(string text)
        {
            CharEnumerator enumerator = text.GetEnumerator();

            int count = 0;
            List<string> textElements = [];
            while (enumerator.MoveNext())
            {
                string sub = enumerator.Current.ToString();
                textElements.Add(sub);
                count++;
            }

            return textElements;
        }
    }
}
