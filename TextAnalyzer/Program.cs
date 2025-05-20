// See https://aka.ms/new-console-template for more information
using StringAnalyzer;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Unicode;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Text analyzer");

Console.OutputEncoding = System.Text.Encoding.Unicode;

bool fileLoaded = false;
string[] lines = [];
string text = "";
string result = "";


bool quit = false;
int lineNumber = 0;

Dictionary<string, string> unicodeSymbols = UnicodeData.GetUnicodeData();

while (!quit)
{
    if (fileLoaded == false)
    {
        string? fileName = FileLoader.SelectFile();
        Encoding encoding = FileLoader.SelectEncoding();
        if (fileName == null) return;
        lines = FileLoader.LoadLines(fileName, encoding);
        text = FileLoader.LoadText(fileName, encoding);
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
    //Console.WriteLine("S > Search for text.  A > analyze line chars.  Q > quit.  L > load file.  [number] > select line.  t > browse text");

    TextDisplay.MenuWord("Search   ");
    TextDisplay.MenuWord("Analyze line   ");
    TextDisplay.MenuWord("Browse text   ");
    TextDisplay.MenuWord("Load file   ");
    TextDisplay.MenuWord("[","number", "] select line   ");
    TextDisplay.MenuWord("Quit");
    Console.WriteLine();


    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"[{lineNumber}]: ");
    string? command = Console.ReadLine();
    
    if (command == null) continue;
    command = command.Trim();
    if (command.Length < 1) continue;

    if (command.Equals("q", StringComparison.CurrentCultureIgnoreCase))
    { 
        quit = true;
    }
    else if (command.Equals("l", StringComparison.CurrentCultureIgnoreCase))
    {
        fileLoaded = false;
        // go back to file select
    }
    else if (command.Equals("s", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.WriteLine("Search for text in file. use 'error' to search for '0xFFFD' (Replacement Char) or '0x[hex]' for unicode");
        Console.Write("Search for: ");
        string? search = Console.ReadLine();
        if (search == null) continue;

        if (search.Contains("0x"))
        {
            search = HexStringToUnicodeChar(search);
        }

        if (search.Equals("error", StringComparison.CurrentCultureIgnoreCase))
        {
            search = '\uFFFD'.ToString();
        }
        
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(search, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{i}: {lines[i]}");
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
    }
    else if (command.Equals("b", StringComparison.CurrentCultureIgnoreCase))
    {
        TextDisplay.NavigateText(text, unicodeSymbols, true);
    }
    else if (command.Equals("save", StringComparison.CurrentCultureIgnoreCase))
    {
        string fileDestination = Path.GetFullPath("result.txt");
        try
        {
            File.WriteAllText(fileDestination, result);
            Console.WriteLine($"File saved to: {fileDestination}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not save file: {fileDestination}");
            Console.WriteLine(ex.Message);
        }
    }
    else if (command.Equals("at", StringComparison.CurrentCultureIgnoreCase)) // not shown in command list, not useful
    {
        result = AnalyzeTextLine(lines, result, lineNumber, unicodeSymbols, true);
    }
    else if (command.Equals("a", StringComparison.CurrentCultureIgnoreCase))
    {
        result = AnalyzeTextLine(lines, result, lineNumber, unicodeSymbols, false);
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
                Console.WriteLine($"Line number {lineNumber} is out of range, use values 0-{lines.Length - 1}");
            }
        }
        catch (Exception ex)
        {
            if (ex is ArgumentNullException)
            {
                Console.WriteLine("Invalid input: Argument is null");
            }
            else
            {
                Console.WriteLine($"Invalid input: {ex.Message}");
            }
            lineNumber = 0;
        }
    }
}

static string HexStringToUnicodeChar(string search)
{
    Console.WriteLine($"Unicode character search: {search}");
    if (search.Length >= 6 && search[..2] == "0x")
    {
        string hexString = search[2..].Trim();
        if ((int)hexString.Length /2 != (float)hexString.Length/2)
        {
            Console.WriteLine("hex string is odd, padding");
            hexString = "0" + hexString;
        }

        try
        {            
            int charnum = Convert.ToInt32(hexString, 16);
            string result = char.ConvertFromUtf32(charnum);

            Debug.WriteLine($"char num: {charnum}: {result}");
            Console.WriteLine($"Unicode character {search}: '{result}'");
            Debug.WriteLine($"Unicode character {search}: '{result}'");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception parsing hex {hexString}: {ex.Message}");
        }
    }
    Console.WriteLine($"Unicode hex not recognized, searching for literal text {search}");
    //if (result == null) return search;
    return search;
}

static string AnalyzeTextLine(string[] lines, string result, int lineNumber, Dictionary<string, string> unicodeSymbols, bool useTextElements)
{
    string line = lines[lineNumber];
    StringBuilder sb = new();
    Console.WriteLine($"Analyzing line number {lineNumber}");
    if (lineNumber < 0 || lineNumber >= lines.Length)
    {
        Console.WriteLine("Invalid line number");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.White;
        sb.AppendLine($"CHAR\tCOL\tHEX\tNAME");
        //Console.WriteLine($"CHAR \tCOL \tHEX");

        //for (int i = 0; i < lines[lineNumber].Length; i++)// char c in lines[lineNumber])

        List<string> dislayStrings = [];
        if (useTextElements)
        {
            TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(line);
            while (elementEnumerator.MoveNext())
            {
                dislayStrings.Add((string)elementEnumerator.Current);
            }
        }
        else
        {
            //foreach (char c in lines[lineNumber])
            //{
            //    dislayStrings.Add(c.ToString());
            //}
            for (int i = 0; i < line.Length; i++)
            {
                dislayStrings.Add(line.Substring(i,1));
            }
        }

        //TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(lines[lineNumber]);
        int count = 0;
        var enumerator = dislayStrings.GetEnumerator();
        bool errorInPrevious = false;
        string previousSub = "";
        while (enumerator.MoveNext())
        {
            string sub = enumerator.Current;
            string displayChar = sub.Replace("\t", "TAB");
            int codepoint = 0;

            if (errorInPrevious)
            {
                Debug.WriteLine($"{count} compile: {previousSub} {(int)previousSub[0]}+ {sub} {(int)sub[0]}");
                int compiledCodepoint = char.ConvertToUtf32(previousSub[0], sub[0]);
                Debug.WriteLine($"compiled: {compiledCodepoint}");
                errorInPrevious = false;
                codepoint = compiledCodepoint;
                displayChar = char.ConvertFromUtf32(codepoint);
            }
            else
            {
                try
                {
                    codepoint = char.ConvertToUtf32(sub, 0);
                }
                catch
                {
                    errorInPrevious = true;
                    previousSub = sub;
                    codepoint = (int)sub[0];
                    Debug.WriteLine("Error getting codepoint");
                }
            }

            string hex = $"{codepoint:X4}";
            string symbolname = "UNKNOWN";
            if (unicodeSymbols.TryGetValue(hex, out string? value))
            {
                symbolname = value;
                Console.WriteLine($"Unicode name: {symbolname}");

                sb.AppendLine($"{displayChar}\t#{count.ToString().PadLeft(3, '0')}\t0x{codepoint:X4}\t{symbolname}");
                count++;
            }
            
        }

        result = sb.ToString();
        Console.WriteLine(result);
        Console.WriteLine("type 'save' to export result");
        Console.ForegroundColor = ConsoleColor.Green;
    }

    return result;
}