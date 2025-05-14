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
                symbolDict.Add(values[0], values[1]);
            }

            return symbolDict;
        }
    }
}
