using Backtest.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest
{
    public class Utility
    {
        static string[] allDays = null;
        //static string btd = Environment.GetEnvironmentVariable("BackTestDir");
        public static string dayData;
        static string TransposeDate(string date)
        {
            return $"{date.Split('/')[2]}{date.Split('/')[0]}{date.Split('/')[1]}";
        }
        public static Dictionary<string,IDayCandle> LoadDayCandles(string date, string[] symbols)
        {
            string[] cdls = File.ReadAllLines($@"{dayData}\{TransposeDate(date)}.txt");
            cdls = (from c in cdls where symbols.Contains(c.Split(',')[0]) select c).ToArray();
            Dictionary<string, IDayCandle> rv = new Dictionary<string, IDayCandle>();
            foreach(string c in cdls)
            {
                IDayCandle dc = new DayCandle(c);
                rv[dc.Symbol] = dc;
            }
            return rv;
        }
        public static string GetTradingDay(string date, int offset)
        {
            if (allDays == null)
            {
                allDays = Directory.GetFiles(dayData);
                allDays = (from d in allDays select d.Split('.')[0].Split('\\').Last()).ToArray();
                //allDays = (from d in allDays select $"{d.Substring(4, 2)}/{d.Substring(6, 2)}/{d.Substring(0, 4)}").ToArray();
                Array.Sort(allDays);
            }
            string tdate = $"{date.Split('/')[2]}{date.Split('/')[0]}{date.Split('/')[1]}";
            int index = Array.BinarySearch(allDays, tdate);
            string rv = null;
            if(index > 0)
            {
                string d = allDays[index + offset];
                rv = $"{d.Substring(4, 2)}/{d.Substring(6, 2)}/{d.Substring(0, 4)}";
            }
            return rv;
        }
    }
}
