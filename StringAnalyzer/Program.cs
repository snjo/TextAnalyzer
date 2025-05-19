// See https://aka.ms/new-console-template for more information
using StringAnalyzer;
using System.Text;
using System.Text.Unicode;
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
        if (fileName == null) return;
        lines = FileLoader.LoadLines(fileName);
        text = FileLoader.LoadText(fileName);
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
    Console.WriteLine("S > Search for text.  A > analyze line chars.  Q > quit.  L > load file.  [number] > select line.  t > browse text");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"[{lineNumber}]: ");
    string? command = Console.ReadLine();
    
    if (command == null) continue;
    command = command.Trim();
    if (command.Length < 1) continue;

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
        Console.WriteLine("Search for text in file. use 'error' to search for '0xFFFD' (Replacement Char) or '0x[hex]' for unicode");
        Console.Write("Search for: ");
        string? search = Console.ReadLine();
        if (search == null) continue;

        if (search.Contains("0x"))
        {
            search = HexStringToUnicodeChar(search);
        }

        Console.WriteLine($"first char: '{search[0]}' int:{(int)search[0]}");

        if (search.ToLower() == "error")
        {
            search = '\uFFFD'.ToString();
        }
        
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
    else if (command.ToLower() == "t")
    {
        TextDisplay.NavigateText(text, unicodeSymbols);
    }
    else if (command.ToLower() == "save")
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
    else if (command.ToLower() == "a")
    {
        StringBuilder sb = new();
        Console.WriteLine($"Analyzing line number {lineNumber}");
        if (lineNumber < 0 || lineNumber >= lines.Length)
        {
            Console.WriteLine("Invalid line number");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
            sb.AppendLine($"CHAR \tCOL \tHEX");
            //Console.WriteLine($"CHAR \tCOL \tHEX");
            for (int i = 0; i < lines[lineNumber].Length; i++)// char c in lines[lineNumber])
            {
                char c = lines[lineNumber][i];
                string hex = ((int)c).ToString("X4");
                string displayChar = c.ToString().Replace("\t", "TAB");
                sb.AppendLine($"{displayChar}\t#{i.ToString().PadLeft(3, '0')}\t0x{hex}");
                //Console.WriteLine($"{c} \t#{i.ToString().PadLeft(3,'0')} \t0x{hex}");
            }
            result = sb.ToString();
            Console.WriteLine(result);
            Console.WriteLine("type 'save' to export result");
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
                Console.WriteLine($"Line number {lineNumber} is out of range, use values 0-{lines.Length-1}");
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

string HexStringToUnicodeChar(string search)
{
    Console.WriteLine($"Unicode character search: {search}");
    // from https://stackoverflow.com/questions/8608489/net-convert-from-string-of-hex-values-into-unicode-characters-support-differen
    string? result = search;
    if (search.Length >= 6 && search.Substring(0, 2) == "0x")
    {
        string hexString = search.Substring(2).Trim();
        if ((int)hexString.Length /2 != (float)hexString.Length/2)
        {
            Console.WriteLine("hex string is odd, padding");
            hexString = "0" + hexString;
        }
        int length = hexString.Length;
        byte[] bytes = new byte[length / 2];

        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            Console.WriteLine($"byte {i}: {bytes[i / 2]} {(int)bytes[i / 2]}");
        }

        try
        {
            char[] chars = Encoding.Latin1.GetChars(bytes);
            result = "";
            foreach(char c in chars)
            {
                Console.WriteLine($"char: {c} {(int)c}");
                if ((int)c > 0)
                    result += c;
            }
            Console.WriteLine($"Unicode character {search}: '{result}'");
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