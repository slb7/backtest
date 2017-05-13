using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backtest;
using System.IO;

namespace EarningsBinBuilder
{
    class Program

    {
        public static ushort dtoi(double d)
        {
            int i = (int)(d * 10000);
            ushort s = (ushort)(i / 100);
            return s;
        }
        static void Main(string[] args)
        {
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            btd = @"e:\";
            int buildArgs = 1;
            if (buildArgs == 1)
            {
                string[] strings = File.ReadAllLines(@"c:\users\scott\Downloads\er_nyse.csv");
                string[] strings1 = File.ReadAllLines(@"c:\users\scott\Downloads\er_nsdq.csv");
                List<string> allStrings = new List<string>(strings);
                allStrings.AddRange(strings1);
                strings = allStrings.ToArray();

                //strings = (from st in strings where st.Split(',').Count() > 1 select $"{st.Split(',')[0]},{st.Split(',')[1].Split(' ')[0]}").ToArray();
                string[][] tokArray = (from st in strings select st.Split(',')).ToArray();
                tokArray = (from t in tokArray where t.Length == 8 select t).ToArray();
                tokArray = tokArray.Skip(1).ToArray();
                tokArray = (from t in tokArray where t[3] == "B" select t).ToArray();
                var groups = tokArray.GroupBy(k => k[2].Split(' ')[0]);
                //string[] allDays = Directory.GetFiles($@"{btd}daysbydate");
                //allDays = (from d in allDays select d.Split('.')[0].Split('\\').Last()).ToArray();
                //allDays = (from d in allDays select $"{d.Substring(4, 2)}/{d.Substring(6, 2)}/{d.Substring(0, 4)}").ToArray();
                //Array.Sort(allDays);
                foreach (var g in groups)
                {
                    string date = g.Key;
                    date = date.Split(' ')[0];
                    string[] dateToks = date.Split('/');
                    date = $@"{ Int32.Parse(dateToks[0]).ToString("00")}/{Int32.Parse(dateToks[1]).ToString("00")}/{Int32.Parse(dateToks[2]).ToString("0000")}";
                    string[][] toka = g.ToArray();
                    string[] symbols = (from tok in toka select tok[1]).ToArray();
                    Array.Sort(symbols);
                    BarDay.BuildEarningsBin(date, symbols);
                    //string tdate = $"{date.Split('/')[2]}{date.Split('/')[0]}{date.Split('/')[1]}";
                    //int index = Array.BinarySearch(allDays, tdate);
                    string prevDate = Utility.GetTradingDay(date, 1);
                    if (prevDate != null)
                    {
                        //string d = allDays[index + 1];
                        BarDay.BuildEarningsBin(prevDate, symbols, "nd", date);
                    }
                    else
                    {
                        Console.WriteLine("next day not found");
                    }
                }
                //BarDay.BuildEarningsBin("01/26/2016", new string[] { "AAPL", "SCHW" });
                return;
            }
            //byte[] b = File.ReadAllBytes(@"c:\src\data\1.bin");
            //DLChecker(@"f:\dl2");
            //return;
            //foreach (string symfn in Directory.GetFiles(@"f:\dl2\"))
            //{
            //    string sym = symfn.Split('\\').Last();
            //    sym = sym.Substring(0, sym.Length - 4);
            //    BarDay.BreakFileIntoMonths(sym);
            //}
            //return;
            //BarDay.BreakFileIntoMonths("IMNP");
            BarDay.BuildMonthBin2(args[0]);
            return;
            foreach (string dir in Directory.GetDirectories($@"{btd}rawdata\2016"))
            {
                BarDay.BuildMonthBin(dir);
            }
            ushort s = dtoi(658.99);
            //BarDay.BuildAllBins();
            for (int i = 2016; i < 2017; i++)
            {
                BarDay.BuildConsolidatedBarsyy($@"c:\src\data\{i}.bin", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 5);
                BarDay.BuildConsolidatedBarsyy($@"c:\src\data\{i}.bin", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 15);
                BarDay.BuildConsolidatedBarsyy($@"c:\src\data\{i}.bin", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 390);
                BarDay.validateBinConsolidatedFile($@"c:\src\data\{i}.bin5", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 5);
                BarDay.validateBinConsolidatedFile($@"c:\src\data\{i}.bin15", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 15);
                BarDay.validateBinConsolidatedFile($@"c:\src\data\{i}.bin390", $@"c:\src\data\{i}.sym", $@"c:\src\data\{i}.dates", 390);
            }
            return;
            //BarDay.validateBinFile(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym",@"c:\src\data\1998.dates");
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 5);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin5", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 5);
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 15);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin15", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 15);
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 390);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin390", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 390);
        }
        static void DLChecker(string dir)
        {
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            foreach (string fn in Directory.GetFiles(dir, "*.txt"))
            {
                FileStream fs = File.OpenRead(fn);
                long length = new FileInfo(fn).Length;
                if (length > 200)
                    fs.Seek(-200, SeekOrigin.End);
                StreamReader sr = new StreamReader(fs);
                string s = null;
                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                }
                fs.Close();
                if (s.Split(',').Count() != 7)
                {
                    Console.WriteLine($"bad ----------{fn}");
                    File.AppendAllLines(@"c:\src\badfiles.txt", new string[] { fn });
                    string tail = fn.Split('\\').Last();
                    string nfn = $@"{btd}bad\{tail}";
                    File.Move(fn, nfn);
                }
                else
                {
                    Console.WriteLine($"good {fn}");
                }
            }
        }
    }
}
