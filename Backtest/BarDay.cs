using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backtest.Infrastructure;
using System.IO;

namespace Backtest
{
    public class BarDay : IBarDay
    {
        DateTime IBarDay.Date
        {
            get
            {
                return date;
            }
        }

        string IBarDay.Symbol
        {
            get
            {
                return symbol;
            }
        }

        int IBarDay.underflow
        {
            get
            {
                //throw new NotImplementedException();
                return underflow;
            }

            set
            {
                underflow = value;
                //throw new NotImplementedException();
            }
        }
        public int symbolNumber = 1;
        public int day;
        //public float[] arr;
        public static int shortsPerBar = 7;
        public static int symbolCount = 0;
        public static int minutesPerDay = 390;
        public static int shortsPerDay = symbolCount * shortsPerBar * minutesPerDay;
        public static int shortsPerInterval = symbolCount * shortsPerBar;
        //public void setup(int symbolCount, int shortsPerBar, int day, int symbolNumber, int minutesPerDay)
        //{
        //    this.symbolCount = symbolCount;
        //    this.shortsPerBar = shortsPerBar;
        //    this.minutesPerDay = minutesPerDay;
        //    this.shortsPerDay = symbolCount * shortsPerBar * minutesPerDay;
        //    this.shortsPerMinute = symbolCount * shortsPerBar;
        //}
        public static void LoadMonth(int year,int month)
        {
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            string baseDir = $@"{btd}uearningsbin\";
            BarDay.dates = File.ReadAllLines($"{baseDir}{year}{month.ToString("00")}.dates");
            BarDay.symbols = File.ReadAllLines($"{baseDir}{year}{month.ToString("00")}.sym");
            byte[] ba = File.ReadAllBytes($"{baseDir}{year}.{month.ToString("00")}bin");
            float[] sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr = sa;
            //ba = File.ReadAllBytes($"{ baseDir}{ year}.bin5");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr5 = sa;
            //ba = File.ReadAllBytes($"{baseDir}{year}.bin15");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr15 = sa;
            //ba = File.ReadAllBytes($"{baseDir}{year}.bin390");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr390 = sa;
            BarDay.symbolCount = symbols.Count();
            shortsPerInterval = symbolCount * shortsPerBar;
        }
        public static void bdinit()
        {
            BarDay.symbolCount = symbols.Count();
            shortsPerInterval = symbolCount * shortsPerBar;
        }
        public static string minuteData;
        public static void LoadDay(int year, int month, int day,bool nextDay = false)
        {
            string baseDir = $@"{minuteData}";
            //BarDay.dates = File.ReadAllLines($"{baseDir}{year}{month.ToString("00")}.dates");
            BarDay.symbols = File.ReadAllLines($"{baseDir}{year}{month.ToString("00")}{day.ToString("00")}.sym");
            string bfn = $"{baseDir}{year}{month.ToString("00")}{day.ToString("00")}.bin";
            if (nextDay) bfn += "nd";
            byte[] ba = File.ReadAllBytes(bfn);
            float[] sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr = sa;
            //ba = File.ReadAllBytes($"{ baseDir}{ year}.bin5");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr5 = sa;
            //ba = File.ReadAllBytes($"{baseDir}{year}.bin15");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr15 = sa;
            //ba = File.ReadAllBytes($"{baseDir}{year}.bin390");
            //sa = new float[ba.Length / 4];
            //Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            //BarDay.arr390 = sa;
            BarDay.symbolCount = symbols.Count();
            shortsPerInterval = symbolCount * shortsPerBar;
        }
        public static void LoadYear(int year)
        {
            string baseDir = @"c:\src\data\";
            BarDay.dates = File.ReadAllLines($"{baseDir}{year}.dates");
            BarDay.symbols = File.ReadAllLines($"{baseDir}{year}.sym");
            byte[] ba = File.ReadAllBytes($"{baseDir}{year}.bin");
            float[] sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr = sa;
            ba = File.ReadAllBytes($"{ baseDir}{ year}.bin5");
            sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr5 = sa;
            ba = File.ReadAllBytes($"{baseDir}{year}.bin15");
            sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr15 = sa;
            ba = File.ReadAllBytes($"{baseDir}{year}.bin390");
            sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            BarDay.arr390 = sa;
            BarDay.symbolCount = symbols.Count();
            shortsPerInterval = symbolCount * shortsPerBar;
        }
        IBar IBarDay.GetMinuteBar(int indi, int minutes, int p)
        {
            //crap to test xoffset
            int dayNumber = 0;
            int adjustedMinuteNumber = indi - (p * minutes);
            int shortsPerMinute = shortsPerBar * symbolCount;
            int xoffset = (dayNumber * shortsPerDay) + (adjustedMinuteNumber * shortsPerMinute) + (symbolNumber * shortsPerBar);
            //end crap
            int offset = (day * shortsPerDay / minutes) + ((indi - (p * minutes)) * shortsPerInterval) + (symbolNumber * shortsPerBar);
            if(offset > arr.Length - 7)
            {

            }

            if (offset < 0) return null;
            float[] a = arr;
            if (minutes == 5) a = arr5;
            if (minutes == 15) a = arr15;
            if (minutes == 390) a = arr390;
            IBar rv = new Bar(a, offset, symbolCount, indi - p, symbolNumber, minutes);
            // DateTime dt = DateTime.Parse("01/02/1998 09:30:00");
            //Console.WriteLine($"{rv.Time} {rv.Symbol} {rv.Open}");
            return rv;
        }

        List<IBarDay> IBarDay.LoadAll(string dn)
        {
            List<IBarDay> rv = new List<IBarDay>();
            string[] files = Directory.GetFiles(dn);
            return rv;
        }

        Dictionary<DateTime, Dictionary<string, IBar>> IBarDay.LoadDayCandles(string dn)
        {
            throw new NotImplementedException();
        }
        private DateTime date;
        private string symbol;
        public static float[] arr;
        public static float[] arr5;
        public static float[] arr15;
        public static float[] arr390;
        public static string[] symbols;
        public static string[] dates;
        int underflow = 0;
        public BarDay(int day, int sym)
        {
            this.symbol = symbols[sym];
            this.date = DateTime.Parse(dates[day]);
            this.symbolNumber = sym;
        }
        public static void Load(string fn)
        {
            int foo = daySlot("09:34");

            byte[] indata = File.ReadAllBytes(@"c:\src\foo.bin");
            float[] symx = new float[Buffer.ByteLength(indata) / 4];
            Buffer.BlockCopy(indata, 0, symx, 0, Buffer.ByteLength(symx));

            int[] a1 = { 0, 1, 2, 3, 4, 5, 6, 7 };
            byte[] a2 = new byte[32];
            Buffer.BlockCopy(a1, 0, a2, 0, 32);
            string sym = fn.Split('\\').Last();
            sym = sym.Substring(0, sym.Length - 4);
            //List<BarDay> rv = new List<BarDay>();
            string[] data = File.ReadAllLines(fn);
            data = (from d in data where d.Trim() != "" select d).ToArray();
            string dataString = data[0].Substring(0, 10);
            //data = (from d in data where d.StartsWith(dataString) select d).ToArray();
            string[] filteredData = (from d in data where regularSession(d) select d).ToArray();
            var days = filteredData.GroupBy(k => k.Split(',')[0]);
            List<IGrouping<string, string>> daylist = days.ToList();
            int numDays = days.Count();
            int numslots = numDays * (390 + 78) * 8;
            float[] arr = new float[numslots];
            for (int i = 0; i < numDays; i++)
            {
                IGrouping<string, string> day = daylist[i];
                string key = day.Key;
                int dayslot = 0;
                double dayLow = double.MaxValue;
                double dayHigh = double.MinValue;
                foreach (string s in day)
                {
                    string[] toks = s.Split(',');
                    double open = Double.Parse(toks[2]);
                    double high = Double.Parse(toks[3]);
                    double low = Double.Parse(toks[4]);
                    double close = Double.Parse(toks[5]);
                    int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                    dayslot++;
                    arr[slotno + 0] = dtoi(open);
                    arr[slotno + 1] = dtoi(high);
                    arr[slotno + 2] = dtoi(low);
                    arr[slotno + 3] = dtoi(close);
                    if (high > dayHigh) dayHigh = high;
                    if (dayLow < low) dayLow = low;
                    arr[slotno + 4] = dtoi(dayHigh);
                    arr[slotno + 5] = dtoi(dayLow);
                    Buffer.BlockCopy(new int[] { Int32.Parse(toks[6]) }, 0, arr, slotno * 2 + 12, 4);
                }
            }
            int bl = Buffer.ByteLength(arr);
            byte[] b = new byte[bl];
            Buffer.BlockCopy(arr, 0, b, 0, bl);
            File.WriteAllBytes(@"c:\src\foo.bin", b);
        }
        private static double itod(short i)
        {
            double rv = (double)i;
            return rv / 100;
        }
        public static short dtoi(double d)
        {
            int i = (int)(d * 10000);
            short s = (short)(i / 100);
            return s;
        }
        static TimeSpan mktOpen = new TimeSpan(9, 30, 0);
        static TimeSpan mktClose = new TimeSpan(16, 0, 0);
        public static bool regularSession(string d)
        {
            string[] toks = d.Split(',');
            string[] tsToks = toks[1].Split(':');
            int hh = Int32.Parse(tsToks[0]);
            int mm = Int32.Parse(tsToks[1]);
            //int ss = Int32.Parse(tsToks[2]);
            TimeSpan ts = new TimeSpan(hh, mm, 0);
            bool rv = false;
            if (ts >= mktOpen && ts < mktClose) rv = true;
            return rv;
        }
        static TimeSpan t930 = new TimeSpan(9, 30, 0);
        public static int daySlot(string d)
        {
            int rv = 0;
            DateTime dt = DateTime.Parse(d);
            TimeSpan ts = dt.TimeOfDay;
            ts -= t930;
            rv = (int)ts.TotalMinutes;
            return rv;
        }
        static int getYear(string s)
        {
            int year = Int32.Parse(s.Split(',')[0].Split('/')[2]);
            return year;
        }
        static int getYearMonth(string s)
        {
            int year = Int32.Parse(s.Split(',')[0].Split('/')[2]);
            int month = Int32.Parse(s.Split(',')[0].Split('/')[0]);
            return (year * 100) + month;
        }
        static int getEarliestYear(string sym)
        {
            StreamReader sr = File.OpenText($@"e:\{sym}.txt");
            string s = sr.ReadLine();
            return getYear(s);
        }
        public static void BreakFileIntoMonths(string sym)
        {
            string dir = Environment.GetEnvironmentVariable("BackTestDir");
            string[] lines = File.ReadAllLines($@"{dir}dl2\{sym}.txt");
            var groups = lines.GroupBy(getYearMonth);
            foreach (var group in groups)
            {
                int x = group.Key;
                int year = x / 100;
                int month = x - (year * 100);
                if (!Directory.Exists($@"{dir}rawdata\{year}")) Directory.CreateDirectory($@"{dir}rawdata\{year}");
                string dirName = $@"{dir}rawdata\{year}\{month}";
                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
                File.WriteAllLines($@"{dirName}\{sym}.txt", group);
                Console.WriteLine($@"creating file {dirName}\{sym}.txt");
            }
        }
        public static void BreakFileIntoYears(string sym)
        {
            string[] lines = File.ReadAllLines($@"e:\{sym}.txt");
            var groups = lines.GroupBy(getYear);
            foreach (var group in groups)
            {
                string dirName = $@"e:\{group.Key}";
                if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
                File.WriteAllLines($@"{dirName}\{sym}.txt", group);
                Console.WriteLine($@"creating file {dirName}\{sym}.txt");
            }
        }
        public static int GetSlot2(string s, string[] symbols, DateTime[] dates, out int minSlot, string symbol)
        {
            int slotsPerBar = 7;
            string[] toks = s.Split(',');
            int dateIndex = Array.BinarySearch(dates, DateTime.Parse(toks[0]));
            int symbolIndex = Array.BinarySearch(symbols, symbol);
            if (symbol == "AA.B")
            {

            }
            TimeSpan t930 = TimeSpan.Parse("9:30");
            TimeSpan ts = TimeSpan.Parse(toks[1]);
            if (ts.TotalHours < 9.5 || ts.TotalMinutes > new TimeSpan(15, 59, 1).TotalMinutes)
            {
                minSlot = -1;
                return -1;
            }
            int totalMinutes = (int)(ts - t930).TotalMinutes;
            minSlot = totalMinutes;
            int rv = (dateIndex * 390 * slotsPerBar * symbols.Count()) + (symbolIndex * slotsPerBar) + (totalMinutes * slotsPerBar * symbols.Count());
            return rv;
        }
        public static int GetSlot3(string s, string[] symbols, out int minSlot, string symbol,string date)
        {
            if(!s.StartsWith(date))
            {
                minSlot = -1;
                return -1;
            }
            int slotsPerBar = 7;
            string[] toks = s.Split(',');
            int dateIndex = 0;// Array.BinarySearch(dates, DateTime.Parse(toks[0]));
            int symbolIndex = Array.BinarySearch(symbols, symbol);
            if (symbol == "AA.B")
            {

            }
            TimeSpan t930 = TimeSpan.Parse("9:30");
            TimeSpan ts = TimeSpan.Parse(toks[1]);
            if (ts.TotalHours < 9.5 || ts.TotalMinutes > new TimeSpan(15, 59, 1).TotalMinutes || symbolIndex < 0)
            {
                minSlot = -1;
                return -1;
            }
            int totalMinutes = (int)(ts - t930).TotalMinutes;
            minSlot = totalMinutes;
            int rv = (dateIndex * 390 * slotsPerBar * symbols.Count()) + (symbolIndex * slotsPerBar) + (totalMinutes * slotsPerBar * symbols.Count());
            return rv;
        }
        public static int GetSlot(string s, string[] symbols, DateTime[] dates, out int minSlot)
        {
            int slotsPerBar = 7;
            string[] toks = s.Split(',');
            int dateIndex = Array.BinarySearch(dates, DateTime.Parse(toks[0]));
            int symbolIndex = Array.BinarySearch(symbols, toks[2]);
            TimeSpan t930 = TimeSpan.Parse("9:30");
            TimeSpan ts = TimeSpan.Parse(toks[1]);
            int totalMinutes = (int)(ts - t930).TotalMinutes;
            minSlot = totalMinutes;
            return (dateIndex * 390 * slotsPerBar * symbols.Count()) + (symbolIndex * slotsPerBar) + (totalMinutes * slotsPerBar * symbols.Count());
        }
        public static float[] BuildYearBin(string dir)
        {
            List<string> strings = new List<string>();
            int year = 0;
            HashSet<string> symbolHash = new HashSet<string>();
            HashSet<string> dateHash = new HashSet<string>();
            string[] fileStrings = null;
            float[] arr = null;
            string[] symbols = null;
            DateTime[] dates = null;
            int count = 0;
            if (Int32.TryParse(dir.Split('\\').Last(), out year))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    fileStrings = File.ReadAllLines(file);
                    Console.WriteLine($"reading {file} {count++}");
                    string symbol = file.Split('\\').Last().Split('.')[0];
                    symbolHash.Add(symbol);
                    fileStrings = (from fs in fileStrings select $"{fs.Split(',')[0]},{fs.Split(',')[1]},{symbol},{fs}").ToArray();
                    var dateStrings = (from fs in fileStrings select $"{fs.Split(',')[0]}").OrderBy(t => t).ToArray();
                    foreach (string d in dateStrings) dateHash.Add(d);
                    strings.AddRange(fileStrings);
                }
                strings = new List<string>(from s in strings where s.Split(',')[1].CompareTo("09:29") > 0 && s.Split(',')[1].CompareTo("16:00") < 0 select s);
                strings.Sort(new BarStringComparer());
                string[] a = strings.ToArray();
                int symbolCount = symbolHash.Count();
                dates = (from d in dateHash select DateTime.Parse(d)).OrderBy(d => d).ToArray();
                symbols = symbolHash.OrderBy(d => d).ToArray();
                arr = new float[symbolCount * 7 * 390 * dates.Count()];
                foreach (string s in strings)
                {
                    int minSlot = 0;
                    int slot = GetSlot(s, symbols, dates, out minSlot);

                    string[] toks = s.Split(',');
                    double open = Double.Parse(toks[3 + 2]);
                    double high = Double.Parse(toks[3 + 3]);
                    double low = Double.Parse(toks[3 + 4]);
                    double close = Double.Parse(toks[3 + 5]);
                    int volume = Int32.Parse(toks[3 + 6]);
                    double dayHigh;
                    double dayLow;
                    if (minSlot == 0)
                    {
                        dayHigh = high;
                        dayLow = low;
                        if (low == 0 || high == 0)
                        {

                        }
                    }
                    else
                    {
                        int prevMinuteSlot = slot - (symbolCount * 7);
                        double oldDayHigh = arr[prevMinuteSlot + 4];
                        double oldDayLow = arr[prevMinuteSlot + 5];
                        if (high > oldDayHigh || oldDayHigh == 0) dayHigh = high;
                        else dayHigh = oldDayHigh;
                        if (low < oldDayLow || oldDayLow == 0) dayLow = low;
                        else dayLow = oldDayLow;
                        if (dayLow == 0 || dayHigh == 0)
                        {

                        }
                    }
                    //if (high > dayHigh) dayHigh = high;
                    //if (low < dayLow) dayLow = low;
                    //dayslot = daySlot(toks[1]);
                    //int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                    //dayslot++;
                    arr[slot + 0] = (float)open;
                    arr[slot + 1] = (float)high;
                    arr[slot + 2] = (float)low;
                    arr[slot + 3] = (float)close;
                    arr[slot + 4] = (float)dayHigh;
                    arr[slot + 5] = (float)dayLow;
                    //if (dayslot % 5 == 0 || dayslot == 1)
                    //{
                    //    open5 = open;
                    //    high5 = high;
                    //    low5 = low;

                    //}
                    //else
                    //{
                    //    close5 = close;

                    //}
                    Buffer.BlockCopy(new int[] { volume }, 0, arr, slot * 4 + 24, 4);

                }
            }
            if (arr != null)
            {
                FileStream fs0 = File.OpenWrite($@"c:\src\data\{year}.bin");
                byte[] fsdata = new byte[arr.Length * 4];
                Buffer.BlockCopy(arr, 0, fsdata, 0, fsdata.Length);
                fs0.Write(fsdata, 0, fsdata.Length);
                fs0.Close();
                File.WriteAllLines($@"c:\src\data\{year}.sym", symbols);
                string[] dateStrings = (from d in dates select d.ToString("MM/dd/yyyy")).ToArray();
                File.WriteAllLines($@"c:\src\data\{year}.dates", dateStrings);
            }
            return arr;
        }
        public static float[] BuildMonthBin(string dir)
        {
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            int year = Int32.Parse(dir.Split('\\')[2]);
            int month = Int32.Parse(dir.Split('\\')[3]);
            string outfn = $@"{btd}bindata\{year.ToString("0000")}{month.ToString("00")}";
            Console.WriteLine(outfn);
            //FileStream fsx = File.OpenWrite($@"{outfn}.bin");

            //return null;
            List<string> strings = new List<string>();
            //int year = 0;
            HashSet<string> symbolHash = new HashSet<string>();
            HashSet<string> dateHash = new HashSet<string>();
            string[] fileStrings = null;
            float[] arr = null;
            string[] symbols = null;
            DateTime[] dates = null;
            int count = 0;
            if (Int32.TryParse(dir.Split('\\').Last(), out year))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    fileStrings = File.ReadAllLines(file);
                    Console.WriteLine($"reading {file} {count++}");
                    string symbol = file.Split('\\').Last().Split('.')[0];
                    symbolHash.Add(symbol);
                    fileStrings = (from fs in fileStrings select $"{fs.Split(',')[0]},{fs.Split(',')[1]},{symbol},{fs}").ToArray();
                    var dateStrings = (from fs in fileStrings select $"{fs.Split(',')[0]}").OrderBy(t => t).ToArray();
                    foreach (string d in dateStrings) dateHash.Add(d);
                    strings.AddRange(fileStrings);
                }
                strings = new List<string>(from s in strings where s.Split(',')[1].CompareTo("09:29") > 0 && s.Split(',')[1].CompareTo("16:00") < 0 select s);
                strings.Sort(new BarStringComparer());
                string[] a = strings.ToArray();
                int symbolCount = symbolHash.Count();
                dates = (from d in dateHash select DateTime.Parse(d)).OrderBy(d => d).ToArray();
                symbols = symbolHash.OrderBy(d => d).ToArray();
                arr = new float[symbolCount * 7 * 390 * dates.Count()];
                foreach (string s in strings)
                {
                    int minSlot = 0;
                    int slot = GetSlot(s, symbols, dates, out minSlot);

                    string[] toks = s.Split(',');
                    double open = Double.Parse(toks[3 + 2]);
                    double high = Double.Parse(toks[3 + 3]);
                    double low = Double.Parse(toks[3 + 4]);
                    double close = Double.Parse(toks[3 + 5]);
                    int volume = Int32.Parse(toks[3 + 6]);
                    double dayHigh;
                    double dayLow;
                    if (minSlot == 0)
                    {
                        dayHigh = high;
                        dayLow = low;
                        if (low == 0 || high == 0)
                        {

                        }
                    }
                    else
                    {
                        int prevMinuteSlot = slot - (symbolCount * 7);
                        double oldDayHigh = arr[prevMinuteSlot + 4];
                        double oldDayLow = arr[prevMinuteSlot + 5];
                        if (high > oldDayHigh || oldDayHigh == 0) dayHigh = high;
                        else dayHigh = oldDayHigh;
                        if (low < oldDayLow || oldDayLow == 0) dayLow = low;
                        else dayLow = oldDayLow;
                        if (dayLow == 0 || dayHigh == 0)
                        {

                        }
                    }
                    //if (high > dayHigh) dayHigh = high;
                    //if (low < dayLow) dayLow = low;
                    //dayslot = daySlot(toks[1]);
                    //int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                    //dayslot++;
                    arr[slot + 0] = (float)open;
                    arr[slot + 1] = (float)high;
                    arr[slot + 2] = (float)low;
                    arr[slot + 3] = (float)close;
                    arr[slot + 4] = (float)dayHigh;
                    arr[slot + 5] = (float)dayLow;
                    //if (dayslot % 5 == 0 || dayslot == 1)
                    //{
                    //    open5 = open;
                    //    high5 = high;
                    //    low5 = low;

                    //}
                    //else
                    //{
                    //    close5 = close;

                    //}
                    Buffer.BlockCopy(new int[] { volume }, 0, arr, slot * 4 + 24, 4);

                }
            }
            if (arr != null)
            {
                //FileStream fs0 = File.OpenWrite($@"c:\src\data\{year}.bin");
                FileStream fs0 = File.OpenWrite($@"{outfn}.bin");
                byte[] fsdata = new byte[arr.Length * 4];
                Buffer.BlockCopy(arr, 0, fsdata, 0, fsdata.Length);
                fs0.Write(fsdata, 0, fsdata.Length);
                fs0.Close();
                //File.WriteAllLines($@"c:\src\data\{year}.sym", symbols);
                File.WriteAllLines($@"{outfn}.sym", symbols);
                string[] dateStrings = (from d in dates select d.ToString("MM/dd/yyyy")).ToArray();
                File.WriteAllLines($@"{outfn}.dates", symbols);
                //File.WriteAllLines($@"c:\src\data\{year}.dates", dateStrings);
            }
            return arr;
        }
        public static float[] BuildMonthBin2(string dir)
        {
            int year = Int32.Parse(dir.Split('\\')[2]);
            int month = Int32.Parse(dir.Split('\\')[3]);
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            string outfn = $@"{btd}bindata\{year.ToString("0000")}{month.ToString("00")}";
            Console.WriteLine(outfn);
            //FileStream fsx = File.OpenWrite($@"{outfn}.bin");

            //return null;
            //List<string> strings = new List<string>();
            //int year = 0;
            HashSet<string> symbolHash = new HashSet<string>();
            HashSet<string> dateHash = new HashSet<string>();
            string[] fileStrings = null;
            float[] arr = null;
            string[] symbols = (from s in Directory.GetFiles(dir, "*.txt") select s.Split('\\').Last()).ToArray();
            symbols = (from s in symbols select s.Substring(0, s.Length - 4)).ToArray();
            Array.Sort(symbols);
            symbolCount = symbols.Count();
            int count = 0;
            DateTime[] dates = (from s in File.ReadAllLines($@"{dir}\ibm.txt") select DateTime.Parse(s.Split(',')[0])).Distinct().ToArray();
            arr = new float[symbolCount * 7 * 390 * dates.Count()];
            if (Int32.TryParse(dir.Split('\\').Last(), out year))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string symbol = file.Split('\\').Last();
                    symbol = symbol.Substring(0, symbol.Length - 4);
                    //    fileStrings = File.ReadAllLines(file);
                    //    Console.WriteLine($"reading {file} {count++}");
                    //    string symbol = file.Split('\\').Last().Split('.')[0];
                    //    symbolHash.Add(symbol);
                    //    fileStrings = (from fs in fileStrings select $"{fs.Split(',')[0]},{fs.Split(',')[1]},{symbol},{fs}").ToArray();
                    //    var dateStrings = (from fs in fileStrings select $"{fs.Split(',')[0]}").OrderBy(t => t).ToArray();
                    //    foreach (string d in dateStrings) dateHash.Add(d);
                    //    strings.AddRange(fileStrings);
                    //}
                    //strings = new List<string>(from s in strings where s.Split(',')[1].CompareTo("09:29") > 0 && s.Split(',')[1].CompareTo("16:00") < 0 select s);
                    //strings.Sort(new BarStringComparer());
                    //string[] a = strings.ToArray();
                    //int symbolCount = symbolHash.Count();
                    //dates = (from d in dateHash select DateTime.Parse(d)).OrderBy(d => d).ToArray();
                    //symbols = symbolHash.OrderBy(d => d).ToArray();
                    //arr = new float[symbolCount * 7 * 390 * dates.Count()];
                    string[] strings = File.ReadAllLines(file);
                    Console.WriteLine(file);
                    foreach (string s in strings)
                    {
                        int minSlot = 0;
                        int slot = GetSlot2(s, symbols, dates, out minSlot, symbol);
                        if (slot < 0)
                        {
                            continue;
                        }
                        string[] toks = s.Split(',');
                        double open = Double.Parse(toks[2]);
                        double high = Double.Parse(toks[3]);
                        double low = Double.Parse(toks[4]);
                        double close = Double.Parse(toks[5]);
                        int volume = Int32.Parse(toks[6]);
                        double dayHigh;
                        double dayLow;
                        if (minSlot == 0)
                        {
                            dayHigh = high;
                            dayLow = low;
                            if (low == 0 || high == 0)
                            {

                            }
                        }
                        else
                        {
                            int prevMinuteSlot = slot - (symbolCount * 7);
                            double oldDayHigh = arr[prevMinuteSlot + 4];
                            double oldDayLow = arr[prevMinuteSlot + 5];
                            if (high > oldDayHigh || oldDayHigh == 0) dayHigh = high;
                            else dayHigh = oldDayHigh;
                            if (low < oldDayLow || oldDayLow == 0) dayLow = low;
                            else dayLow = oldDayLow;
                            if (dayLow == 0 || dayHigh == 0)
                            {

                            }
                        }
                        //if (high > dayHigh) dayHigh = high;
                        //if (low < dayLow) dayLow = low;
                        //dayslot = daySlot(toks[1]);
                        //int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                        //dayslot++;
                        arr[slot + 0] = (float)open;
                        arr[slot + 1] = (float)high;
                        arr[slot + 2] = (float)low;
                        arr[slot + 3] = (float)close;
                        arr[slot + 4] = (float)dayHigh;
                        arr[slot + 5] = (float)dayLow;
                        //if (dayslot % 5 == 0 || dayslot == 1)
                        //{
                        //    open5 = open;
                        //    high5 = high;
                        //    low5 = low;

                        //}
                        //else
                        //{
                        //    close5 = close;

                        //}
                        Buffer.BlockCopy(new int[] { volume }, 0, arr, slot * 4 + 24, 4);

                    }
                }
            }
            if (arr != null)
            {
                //FileStream fs0 = File.OpenWrite($@"c:\src\data\{year}.bin");
                FileStream fs0 = File.OpenWrite($@"{outfn}.bin");
                byte[] fsdata = new byte[arr.Length * 4];
                Buffer.BlockCopy(arr, 0, fsdata, 0, fsdata.Length);
                fs0.Write(fsdata, 0, fsdata.Length);
                fs0.Close();
                //File.WriteAllLines($@"c:\src\data\{year}.sym", symbols);
                File.WriteAllLines($@"{outfn}.sym", symbols);
                string[] dateStrings = (from d in dates select d.ToString("MM/dd/yyyy")).ToArray();
                File.WriteAllLines($@"{outfn}.dates", dateStrings);
                //File.WriteAllLines($@"c:\src\data\{year}.dates", dateStrings);
            }
            return arr;
        }
        public static float[] BuildEarningsBin(string date, string[] symbols, string nd = "", string realDate = "")
        {
            string[] dateToks = date.Split('/');
            //if(realDate.Length > 0) dateToks = realDate.Split('/'); // name the next day file same as curr day
            int year = Int32.Parse(dateToks[2]);
            int month = Int32.Parse(dateToks[0]);
            int day = Int32.Parse(dateToks[1]);
            //string btd = Environment.GetEnvironmentVariable("BackTestDir");
            string btd = @"e:\";
            string dir = $@"{btd}um\{year}\{month}";
            //int year = Int32.Parse(dir.Split('\\')[2]);
            //int month = Int32.Parse(dir.Split('\\')[3]);
            //string outfn = $@"f:\bindata\{year.ToString("0000")}{month.ToString("00")}";
            string ds = $@"{year.ToString("0000")}{month.ToString("00")}{day.ToString("00")}";
            string outfn = $@"{btd}newuearningsBin\{ds}";
            if(realDate.Length > 0) outfn = $@"{btd}uearningsBin\{realDate.Split('/')[2]}{realDate.Split('/')[0]}{realDate.Split('/')[1]}";
            Console.WriteLine(outfn);
            //FileStream fsx = File.OpenWrite($@"{outfn}.bin");

            //return null;
            //List<string> strings = new List<string>();
            //int year = 0;
            HashSet<string> symbolHash = new HashSet<string>();
            HashSet<string> dateHash = new HashSet<string>();
            string[] fileStrings = null;
            float[] arr = null;
            //string[] symbols = (from s in Directory.GetFiles(dir, "*.txt") select s.Split('\\').Last()).ToArray();
            //symbols = (from s in symbols select s.Substring(0, s.Length - 4)).ToArray();
            Array.Sort(symbols);
            symbolCount = symbols.Count();
            int count = 0;
            //DateTime[] dates = (from s in File.ReadAllLines($@"{dir}\ibm.txt") select DateTime.Parse(s.Split(',')[0])).Distinct().ToArray();
            //arr = new float[symbolCount * 7 * 390 * dates.Count()];
            arr = new float[symbolCount * 7 * 390];
            foreach (string sym in symbols)
            {
                string file = $@"{dir}\{sym}.txt";
                string symbol = sym;// file.Split('\\').Last();
                //symbol = symbol.Substring(0, symbol.Length - 4);
                //    fileStrings = File.ReadAllLines(file);
                //    Console.WriteLine($"reading {file} {count++}");
                //    string symbol = file.Split('\\').Last().Split('.')[0];
                //    symbolHash.Add(symbol);
                //    fileStrings = (from fs in fileStrings select $"{fs.Split(',')[0]},{fs.Split(',')[1]},{symbol},{fs}").ToArray();
                //    var dateStrings = (from fs in fileStrings select $"{fs.Split(',')[0]}").OrderBy(t => t).ToArray();
                //    foreach (string d in dateStrings) dateHash.Add(d);
                //    strings.AddRange(fileStrings);
                //}
                //strings = new List<string>(from s in strings where s.Split(',')[1].CompareTo("09:29") > 0 && s.Split(',')[1].CompareTo("16:00") < 0 select s);
                //strings.Sort(new BarStringComparer());
                //string[] a = strings.ToArray();
                //int symbolCount = symbolHash.Count();
                //dates = (from d in dateHash select DateTime.Parse(d)).OrderBy(d => d).ToArray();
                //symbols = symbolHash.OrderBy(d => d).ToArray();
                //arr = new float[symbolCount * 7 * 390 * dates.Count()];
                if(!File.Exists(file))
                {
                    Console.WriteLine($@"{file} does not exist");
                    continue;
                }
                string[] strings = File.ReadAllLines(file);
                Console.WriteLine(file);
                foreach (string s in strings)
                {
                    int minSlot = 0;
                    int slot = GetSlot3(s, symbols, out minSlot, symbol, date);
                    if (slot < 0)
                    {
                        continue;
                    }
                    string[] toks = s.Split(',');
                    double open = Double.Parse(toks[2]);
                    double high = Double.Parse(toks[3]);
                    double low = Double.Parse(toks[4]);
                    double close = Double.Parse(toks[5]);
                    int volume = Int32.Parse(toks[6]);
                    double dayHigh;
                    double dayLow;
                    if (minSlot == 0)
                    {
                        dayHigh = high;
                        dayLow = low;
                        if (low == 0 || high == 0)
                        {

                        }
                    }
                    else
                    {
                        int prevMinuteSlot = slot - (symbolCount * 7);
                        double oldDayHigh = arr[prevMinuteSlot + 4];
                        double oldDayLow = arr[prevMinuteSlot + 5];
                        if (high > oldDayHigh || oldDayHigh == 0) dayHigh = high;
                        else dayHigh = oldDayHigh;
                        if (low < oldDayLow || oldDayLow == 0) dayLow = low;
                        else dayLow = oldDayLow;
                        if (dayLow == 0 || dayHigh == 0)
                        {

                        }
                    }
                    //if (high > dayHigh) dayHigh = high;
                    //if (low < dayLow) dayLow = low;
                    //dayslot = daySlot(toks[1]);
                    //int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                    //dayslot++;
                    arr[slot + 0] = (float)open;
                    arr[slot + 1] = (float)high;
                    arr[slot + 2] = (float)low;
                    arr[slot + 3] = (float)close;
                    arr[slot + 4] = (float)dayHigh;
                    arr[slot + 5] = (float)dayLow;
                    //if (dayslot % 5 == 0 || dayslot == 1)
                    //{
                    //    open5 = open;
                    //    high5 = high;
                    //    low5 = low;

                    //}
                    //else
                    //{
                    //    close5 = close;

                    //}
                    Buffer.BlockCopy(new int[] { volume }, 0, arr, slot * 4 + 24, 4);

                }
            }
            if (arr != null)
            {
                //FileStream fs0 = File.OpenWrite($@"c:\src\data\{year}.bin");
                FileStream fs0 = File.OpenWrite($@"{outfn}.bin{nd}");
                byte[] fsdata = new byte[arr.Length * 4];
                Buffer.BlockCopy(arr, 0, fsdata, 0, fsdata.Length);
                fs0.Write(fsdata, 0, fsdata.Length);
                fs0.Close();
                //File.WriteAllLines($@"c:\src\data\{year}.sym", symbols);
                if(nd.Length == 0) File.WriteAllLines($@"{outfn}.sym", symbols);
                //string[] dateStrings = (from d in dates select d.ToString("MM/dd/yyyy")).ToArray();
                //File.WriteAllLines($@"{outfn}.dates", dateStrings);
                //File.WriteAllLines($@"c:\src\data\{year}.dates", dateStrings);
            }
            return arr;
        }
        public static string BarString(IBar b, string symbol, string[] dateStrings, int symbolCount, int i, int minsPerCandle)
        {
            int dubsPerBar = 7;
            int barsPerDay = 390 * symbolCount;
            barsPerDay /= minsPerCandle;
            int dubsPerDay = dubsPerBar * barsPerDay;
            int dayNo = i / dubsPerDay;
            string date = dateStrings[dayNo];
            int dubsPerMinute = dubsPerBar * symbolCount;
            int minuteNumber = (i % dubsPerDay) / dubsPerMinute;
            TimeSpan ts = new TimeSpan(0, minuteNumber * minsPerCandle, 0) + TimeSpan.Parse("09:30");
            //if (minsPerCandle > 1) ts -= new TimeSpan(0, minsPerCandle, 0);
            string time = ts.ToString(@"hh\:mm");
            if (b.Open == 0 && b.Volume == 0) return null;
            return $"{symbol}|{date},{time},{b.High},{b.Low},{b.Open},{b.Close},{b.DayHigh},{b.DayLow},{b.Volume}";
        }
        static Bar[] getNBars(float[] arr, int day, int symbolNo, int period, int symbolCount, int oneMinCandlePerConsolidatedCandle)
        {
            int barsPerSymbolPerDay = 390;
            int shortsPerBar = 7;
            int barsPerDay = barsPerSymbolPerDay * symbolCount;
            int shortsPerDay = barsPerDay * shortsPerBar;
            int shortsPerMinute = shortsPerBar * symbolCount;
            Bar[] bars = new Bar[oneMinCandlePerConsolidatedCandle];
            for (int i = 0; i < oneMinCandlePerConsolidatedCandle; i++)
            {
                int off = day * shortsPerDay + (symbolNo * shortsPerBar) + (period * shortsPerMinute * oneMinCandlePerConsolidatedCandle) + (shortsPerMinute * i);
                bars[i] = new Bar(arr, off, symbolCount, i, symbolNo, oneMinCandlePerConsolidatedCandle);
            }
            return bars;
        }
        public static float[] BuildConsolidatedBarsyy(string binName, string symbolName, string dateName, int oneMinCandlePerConsolidatedCandle)
        {
            int oneMinCandlesPerDayPerSymbol = 390;
            //int oneMinCandlePerConsolidatedCandle = 5;
            string[] symbols = File.ReadAllLines(symbolName);
            int consolidatedCandlesPerDayPerSymbol = (oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle) / symbols.Count();
            //int consolidatedCandlesPerDay = (oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle);
            int consolidatedCandlesPerDay = (oneMinCandlesPerDayPerSymbol / oneMinCandlePerConsolidatedCandle) * symbols.Count();//(oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle);
            string[] dates = File.ReadAllLines(dateName);
            byte[] b = File.ReadAllBytes(binName);
            float[] arr = new float[b.Length / 4];
            Buffer.BlockCopy(b, 0, arr, 0, b.Length);
            int shortsPerBar = 7;
            int oneMinCandles = arr.Length / shortsPerBar;
            //int consolidatedCandleCount = consolidatedCandlesPerDay * dates.Count();
            int consolidatedCandleCount = arr.Length / shortsPerBar / oneMinCandlePerConsolidatedCandle;
            float[] arrC = new float[arr.Length / oneMinCandlePerConsolidatedCandle];
            int periodsPerDay = oneMinCandlesPerDayPerSymbol / oneMinCandlePerConsolidatedCandle;
            for (int dayNo = 0; dayNo < dates.Count(); dayNo++)
            {
                for (int period = 0; period < periodsPerDay; period++)
                {
                    int symbolCount = symbols.Count();
                    for (int symNo = 0; symNo < symbolCount; symNo++)
                    {
                        IBar[] bars = getNBars(arr, dayNo, symNo, period, symbols.Count(), oneMinCandlePerConsolidatedCandle);
                        double high = (from bar in bars select bar.High).Max();
                        double low = (from bar in bars select bar.High).Max();
                        double open = (from bar in bars where bar.Open != 0 select bar.Open).FirstOrDefault();
                        double close = (from bar in bars where bar.Close != 0 select bar.Close).LastOrDefault();
                        double dayHigh = (from bar in bars select bar.DayHigh).Max();
                        double dayLow = 0;
                        if ((from bar in bars where bar.DayLow != 0 select bar.DayLow).Count() > 0)
                        {
                            dayLow = (from bar in bars where bar.DayLow != 0 select bar.DayLow).Min();
                        }
                        int volume = Int32.MaxValue;
                        //try { dayLow = (from bar in bars where bar.DayLow != 0 select bar.DayLow).Min(); } catch (Exception) { }
                        try
                        {
                            volume = (from bar in bars select bar.Volume).Sum();
                        }
                        catch (OverflowException of)
                        {

                        }
                        int dayBase = (dayNo * periodsPerDay * symbolCount * shortsPerBar);
                        int periodBase = period * symbolCount * shortsPerBar;
                        int symbolOffset = symNo * shortsPerBar;
                        int slot = dayBase + periodBase + symbolOffset;
                        arrC[slot + 0] = dtoi(open);
                        arrC[slot + 1] = dtoi(high);
                        arrC[slot + 2] = dtoi(low);
                        arrC[slot + 3] = dtoi(close);
                        arrC[slot + 4] = dtoi(dayHigh);
                        arrC[slot + 5] = dtoi(dayLow);
                        Buffer.BlockCopy(new int[] { volume }, 0, arrC, slot * 2 + 12, 4);
                    }

                }
            }
            byte[] arrCb = new byte[arrC.Length * 2];
            Buffer.BlockCopy(arrC, 0, arrCb, 0, arrCb.Length);
            File.WriteAllBytes($"{binName}{oneMinCandlePerConsolidatedCandle}", arrCb);
            return arrC;
        }

        public static float[] BuildConsolidatedBars(string binName, string symbolName, string dateName, int oneMinCandlePerConsolidatedCandle)
        {
            int oneMinCandlesPerDayPerSymbol = 390;
            //int oneMinCandlePerConsolidatedCandle = 5;
            string[] symbols = File.ReadAllLines(symbolName);
            int consolidatedCandlesPerDayPerSymbol = (oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle) / symbols.Count();
            //int consolidatedCandlesPerDay = (oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle);
            int consolidatedCandlesPerDay = (oneMinCandlesPerDayPerSymbol / oneMinCandlePerConsolidatedCandle) * symbols.Count();//(oneMinCandlesPerDayPerSymbol * oneMinCandlePerConsolidatedCandle);
            string[] dates = File.ReadAllLines(dateName);
            byte[] b = File.ReadAllBytes(binName);
            float[] arr = new float[b.Length / 4];
            Buffer.BlockCopy(b, 0, arr, 0, b.Length);
            int shortsPerBar = 7;
            int oneMinCandles = arr.Length / shortsPerBar;
            //int consolidatedCandleCount = consolidatedCandlesPerDay * dates.Count();
            int consolidatedCandleCount = arr.Length / shortsPerBar / oneMinCandlePerConsolidatedCandle;
            float[] arrC = new float[arr.Length / oneMinCandlePerConsolidatedCandle];
            for (int i = 0; i < consolidatedCandleCount; i++)
            {
                int period = i % consolidatedCandlesPerDay;
                //int period = (i / symbols.Count()) % consolidatedCandlesPerDay;
                int symbolNo = i % symbols.Count();
                int dayno = i / consolidatedCandlesPerDay;
                if (dayno > 248 && period > 73 && symbolNo > 24)
                {

                }
                IBar[] bars = getNBars(arr, dayno, symbolNo, period, symbols.Count(), oneMinCandlePerConsolidatedCandle);
                double high = (from bar in bars select bar.High).Max();
                double low = (from bar in bars select bar.High).Max();
                double open = (from bar in bars where bar.Open != 0 select bar.Open).FirstOrDefault();
                double close = (from bar in bars where bar.Close != 0 select bar.Close).LastOrDefault();
                double dayHigh = (from bar in bars select bar.DayHigh).Max();
                double dayLow = 0;
                if ((from bar in bars where bar.DayLow != 0 select bar.DayLow).Count() > 0)
                {
                    dayLow = (from bar in bars where bar.DayLow != 0 select bar.DayLow).Min();
                }
                //try { dayLow = (from bar in bars where bar.DayLow != 0 select bar.DayLow).Min(); } catch (Exception) { }
                int volume = (from bar in bars select bar.Volume).Sum();
                int slot = i * shortsPerBar;
                arrC[slot + 0] = dtoi(open);
                arrC[slot + 1] = dtoi(high);
                arrC[slot + 2] = dtoi(low);
                arrC[slot + 3] = dtoi(close);
                arrC[slot + 4] = dtoi(dayHigh);
                arrC[slot + 5] = dtoi(dayLow);
                Buffer.BlockCopy(new int[] { volume }, 0, arrC, slot * 2 + 12, 4);
            }
            byte[] arrCb = new byte[arrC.Length * 2];
            Buffer.BlockCopy(arrC, 0, arrCb, 0, arrCb.Length);
            File.WriteAllBytes($"{binName}{oneMinCandlePerConsolidatedCandle}", arrCb);
            return arrC;
        }
        public static void validateBinFile(string binName, string symName, string dateName)
        {
            string[] symbols = File.ReadAllLines(symName);
            int floatsPerBar = 7;
            byte[] ba = File.ReadAllBytes(binName);
            float[] sa = new float[ba.Length / 4];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            string[] dateStrings = File.ReadAllLines(dateName);
            Dictionary<string, List<string>> valData = new Dictionary<string, List<string>>();
            foreach (string sym in symbols) valData[sym] = new List<string>();
            for (int i = 0; i < sa.Length; i += floatsPerBar)
            {
                int symbolnumber = (i / floatsPerBar) % symbols.Length;
                string symbol = symbols[(i / floatsPerBar) % symbols.Length];
                Bar b = new Bar(sa, i, symbols.Length, i, symbolnumber, 1);
                string bs = BarString(b, symbol, dateStrings, symbols.Count(), i, 1);
                if (bs != null)
                {
                    string[] toks = bs.Split('|');
                    valData[toks[0]].Add(toks[1]);
                }
            }
            foreach (string sym in symbols)
            {
                File.WriteAllLines($@"c:\src\data\val\{sym}.txt", valData[sym].ToArray());
            }
        }
        public static void validateBinConsolidatedFile(string binName, string symName, string dateName, int oneMinCandlePerConsolidatedCandle)
        {
            int year = Int32.Parse(symName.Split('\\').Last().Split('.')[0]);
            string[] symbols = File.ReadAllLines(symName);
            byte[] ba = File.ReadAllBytes(binName);
            float[] sa = new float[ba.Length / 2];
            Buffer.BlockCopy(ba, 0, sa, 0, ba.Length);
            string[] dateStrings = File.ReadAllLines(dateName);
            Dictionary<string, List<string>> valData = new Dictionary<string, List<string>>();
            foreach (string sym in symbols) valData[sym] = new List<string>();
            int floatsPerBar = 7;
            for (int i = 0; i < sa.Length; i += floatsPerBar)
            {
                int symbolNumber = (i / floatsPerBar) % symbols.Length;
                string symbol = symbols[(i / floatsPerBar) % symbols.Length];
                Bar b = new Bar(sa, i, symbols.Length, i / floatsPerBar, symbolNumber, oneMinCandlePerConsolidatedCandle);
                string bs = BarString(b, symbol, dateStrings, symbols.Count(), i, oneMinCandlePerConsolidatedCandle);
                if (bs != null)
                {
                    string[] toks = bs.Split('|');
                    valData[toks[0]].Add(toks[1]);
                }
            }
            foreach (string sym in symbols)
            {
                File.WriteAllLines($@"c:\src\data\val\{sym}{year}{oneMinCandlePerConsolidatedCandle}.txt", valData[sym].ToArray());
            }
        }

        public static void BuildAllBins()
        {
            //string[] syms = Directory.GetFiles(@"c:\src\data");
            string[] syms = Directory.GetFiles(@"E:\");
            syms = (from s in syms select s.Split('\\').Last()).ToArray();
            syms = (from s in syms select s.Split('.').First()).ToArray();
            Dictionary<string, int> yearDict = new Dictionary<string, int>();
            foreach (string sym in syms)
            {
                //BreakFileIntoYears(sym);
            }
            float[] arrz = BuildYearBin(@"e:\2016");
            foreach (string dir in Directory.GetDirectories(@"e:\"))
            {
                float[] arr = BuildYearBin(dir);
            }
        }
        static DateTime epoch = DateTime.Parse("1/1/1970");
        public static short dayNum(string day)
        {
            DateTime dt = DateTime.Parse(day);
            short rv = (short)(dt - epoch).TotalDays;
            return rv;
        }
        public static string getDay(short d)
        {
            return epoch.AddDays(d).ToString("MM/dd/yyyy");
        }
        public static void setInt(int val, float[] arr, int offset)
        {
            int[] ia = { val };
            Buffer.BlockCopy(ia, 0, arr, offset * 2, 4);
        }
        public static int getInt(float[] arr, int offset)
        {
            int[] dst = new int[1];
            Buffer.BlockCopy(arr, offset * 2, dst, 0, 4);
            return dst[0];
        }
        public static List<IBarDay> LoadSymbolBin(string sym)
        {
            FileStream fs = File.OpenRead($@"c:\src\symbin\{sym}.bin");
            byte[] ia = new byte[4];
            fs.Read(ia, 0, 4);
            int dayMapLen = BitConverter.ToInt32(ia, 0);
            fs.Read(ia, 0, 4);
            int arrLen = BitConverter.ToInt32(ia, 0);
            byte[] bb = new byte[dayMapLen];
            fs.Read(bb, 0, dayMapLen);
            float[] dayMap = new float[dayMapLen / 2];
            Buffer.BlockCopy(bb, 0, dayMap, 0, dayMapLen);
            bb = new byte[arrLen];
            fs.Read(bb, 0, arrLen);
            float[] arr = new float[arrLen / 2];
            Buffer.BlockCopy(bb, 0, arr, 0, arrLen);
            List<IBarDay> rv = new List<IBarDay>();
            int numDays = dayMap.Length / 3;
            for (int i = 0; i < numDays; i++)
            {
                float day = dayMap[i * 3];
                int offset = getInt(dayMap, (i * 3) + 1);
                //rv.Add(new BarDay(day, offset, sym, arr));
            }
            return rv;
        }
        static int getSlot(string s, int mins)
        {
            int rv = 0;
            TimeSpan ts = TimeSpan.Parse(s.Split(',')[1]);
            rv = (int)((ts - t930).TotalMinutes) / mins;
            return rv;
        }
        public static void BuildSymbolBin(string sym)
        {
            //int[] a1 = { 0, 1, 2, 3, 4, 5, 6, 7 };
            //byte[] a2 = new byte[32];
            //Buffer.BlockCopy(a1, 0, a2, 0, 32);
            //string sym = fn.Split('\\').Last();
            //sym = sym.Substring(0, sym.Length - 4);
            //List<BarDay> rv = new List<BarDay>();
            //string[] data = File.ReadAllLines($@"c:\src\data\{sym}.txt");
            string[] data = File.ReadAllLines($@"E:\{sym}.txt");
            data = (from d in data where d.Trim() != "" select d).ToArray();
            string dataString = data[0].Substring(0, 10);
            //data = (from d in data where d.StartsWith(dataString) select d).ToArray();
            string[] filteredData = (from d in data where regularSession(d) select d).ToArray();
            var days = filteredData.GroupBy(k => k.Split(',')[0]);
            List<IGrouping<string, string>> daylist = days.ToList();
            int numDays = days.Count();
            int numslots = numDays * (390 + 78) * 8;
            float[] arr = new float[numslots];
            Array.Clear(arr, 0, numslots);
            DateTime t0 = DateTime.Parse("1/1/1970");
            short daze = (short)(DateTime.Now - t0).TotalDays;
            float[] dayMap = new float[numDays * 3];
            for (int i = 0; i < numDays; i++)
            {
                IGrouping<string, string> day = daylist[i];
                string key = day.Key;
                dayMap[i * 3] = dayNum(key);
                int firstSlotOfDay = i * 8 * (390 + 78);
                string td = null;// getDay(dayMap[i * 3]);
                setInt(firstSlotOfDay, dayMap, (i * 3) + 1);
                int tfsod = getInt(dayMap, (i * 3) + 1);
                if (td != key || tfsod != firstSlotOfDay)
                {

                }
                int dayslot = 0;
                double dayHigh = double.MinValue;
                double dayLow = double.MaxValue;
                //double open5 = 0;
                //double high5 = 0;
                //double low5 = 0;
                //double close5 = 0;
                //double dayHigh5 = double.MinValue;
                //double dayLow5 = double.MaxValue;
                //int Volume = 0;
                //int Volume5 = 0;
                //int bar5 = 0;
                //bool first = true;
                var mg5 = (from s in day group s by getSlot(s, 5));
                {
                    double high = 0;
                    double low = 0;
                    double open = 0;
                    double close = 0;
                    double ddayHigh = double.MinValue;
                    double ddayLow = double.MaxValue;
                    int volume = 0;
                    foreach (IGrouping<int, string> g in mg5)
                    {
                        int ind = g.Key;
                        ind *= 8;
                        ind += firstSlotOfDay;
                        ind += (390 * 8);
                        //not sure why this is necessary - grouping somethines includes null strings
                        open = (from gs in g where gs != null orderby (gs) select double.Parse(gs.Split(',')[2])).First();
                        high = (from gs in g where gs != null select Double.Parse(gs.Split(',')[3])).Max();
                        low = (from gs in g where gs != null select Double.Parse(gs.Split(',')[4])).Min();
                        close = (from gs in g where gs != null orderby (gs) select double.Parse(gs.Split(',')[5])).Last();
                        volume = (from gs in g where gs != null orderby (gs) select Int32.Parse(gs.Split(',')[6])).Sum();
                        if (high > ddayHigh) ddayHigh = high;
                        if (low < ddayLow) ddayLow = low;
                        arr[ind + 0] = dtoi(open);
                        arr[ind + 1] = dtoi(high);
                        arr[ind + 2] = dtoi(low);
                        arr[ind + 3] = dtoi(close);
                        arr[ind + 4] = dtoi(ddayHigh);
                        arr[ind + 5] = dtoi(ddayLow);
                        Buffer.BlockCopy(new int[] { volume }, 0, arr, ind * 2 + 12, 4);
                        int[] ans = new int[1];
                        Buffer.BlockCopy(arr, ind * 2 + 12, ans, 0, 4);
                    }
                }

                foreach (string s in day)
                {
                    string[] toks = s.Split(',');
                    double open = Double.Parse(toks[2]);
                    double high = Double.Parse(toks[3]);
                    double low = Double.Parse(toks[4]);
                    double close = Double.Parse(toks[5]);
                    int volume = Int32.Parse(toks[6]);
                    if (high > dayHigh) dayHigh = high;
                    if (low < dayLow) dayLow = low;
                    dayslot = daySlot(toks[1]);
                    //int curr5 = dayslot / 5;
                    //if(first)
                    //{
                    //    open5 = open;
                    //    close5 = close;
                    //    Volume5 = Volume;                        
                    //    first = false;
                    //}
                    //if(curr5 != bar5)
                    //{
                    //    //write bar5
                    //    string mess = $"{bar5} open={open5} high={high5} low={low5} close={close5} volume={Volume5} dayhigh={dayHigh5} daylow={dayLow5}";
                    //    //init curr5
                    //    open5 = open;
                    //    close5 = close;
                    //    dayHigh5 = dayHigh;
                    //    dayLow5 = dayLow;
                    //    Volume5 = 0;

                    //    //bar5 = curr5
                    //    bar5 = curr5;
                    //}
                    //close5 = close;
                    //if (high > high5) high5 = high;
                    //if (high > dayHigh5) dayHigh5 = high;
                    //if (low < low5) low5 = low;
                    //if (low < dayLow5) dayLow5 = low;
                    //Volume5 += Volume;
                    int slotno = (i * 8 * (390 + 78)) + (dayslot * 8);
                    //dayslot++;
                    arr[slotno + 0] = dtoi(open);
                    arr[slotno + 1] = dtoi(high);
                    arr[slotno + 2] = dtoi(low);
                    arr[slotno + 3] = dtoi(close);
                    arr[slotno + 4] = dtoi(dayHigh);
                    arr[slotno + 5] = dtoi(dayLow);
                    //if (dayslot % 5 == 0 || dayslot == 1)
                    //{
                    //    open5 = open;
                    //    high5 = high;
                    //    low5 = low;

                    //}
                    //else
                    //{
                    //    close5 = close;

                    //}
                    Buffer.BlockCopy(new int[] { volume }, 0, arr, slotno * 2 + 12, 4);

                }

            }
            int arrLen = Buffer.ByteLength(arr);
            int dayMapLen = Buffer.ByteLength(dayMap);
            byte[] b = new byte[arrLen + dayMapLen];
            Buffer.BlockCopy(dayMap, 0, b, 0, dayMapLen);
            Buffer.BlockCopy(arr, 0, b, dayMapLen, arrLen);
            FileStream fs = File.OpenWrite($@"c:\src\symbin\{sym}.bin");
            fs.Write(BitConverter.GetBytes(dayMapLen), 0, 4);
            fs.Write(BitConverter.GetBytes(arrLen), 0, 4);
            fs.Write(b, 0, arrLen + dayMapLen);
            fs.Close();
        }
    }
}
