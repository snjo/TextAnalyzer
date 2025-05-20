using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringAnalyzer
{
    class UnicodeData
    {
        public static Dictionary<string, string> GetUnicodeData()
        {
            Dictionary<string, string> symbolDict = [];
            string[] lines;
            try
            {
                lines = File.ReadAllLines("data\\symbols.txt");
            }
            catch
            {
                Console.WriteLine("Could not load unicode data file .\\data\\symbols.txt");
                return symbolDict;
            }

            

            foreach(string line in lines)
            {
                string[] values = line.Split(';');
                string description = values[1];
                if (description == "<control>")
                {
                    description = values[3];
                }
                symbolDict.Add(values[0], description);
            }

            return symbolDict;
        }
    }
}
