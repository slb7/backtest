using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Backtest.Infrastructure
{
    public abstract class Test
    {

        bool underflow;
        int i;
        IBar[] bars;
        public IBarDay bd;
        public bool testNextDay = false;
        public static List<string> messages = new List<string>();
        public abstract bool CheckLongEntryCriteria(int symbolNumber, int dayNumber, int minuteNumber);
        public abstract bool CheckShortEntryCriteria(int symbolNumber, int dayNumber, int minuteNumber);
        public abstract double CalculateLongEntryLimitPrice();
        public abstract double CalculateShortEntryLimitPrice();
        public abstract bool screen();
        public abstract double CalculateLongExitLimitPrice();
        public abstract double CalculateShortExitLimitPrice();

        public int tif = 0;
        public double profitExit = .01;
        public int maxOvernights = 0;
        public bool entering = false;
        public int CloseAll;
        public int StopTimeInterval;
        public IDayCandle today;
        public IDayCandle prev;
        public static List<TestResult> testResults = new List<TestResult>();
        public static TestResult testResult;
        //int stopTime;
        public int Time()
        {
            int rv = (int)new TimeSpan(0, i, 0).TotalSeconds;
            return rv;
        }
        public int Time(string t)
        {
            int rv = (int)((TimeSpan.Parse(t) - TimeSpan.Parse("09:30")).TotalSeconds);
            return rv;
        }
        public static Dictionary<string, Test> testDictionary = new Dictionary<string, Test>();
        public static System.Type type;
        public int symbolNumber;
        public string symbol;
        public float[] arr;
        public short[] arr5;
        public short[] arr15;
        public short[] arr390;
        public int symbolCount;
        int shortsPerBar = 7;
        int minutesPerDay = 390;
        int barsPerDay1;
        int barsPerDay5;
        int barsPerDay15;
        int shortsPerDay;
        int shortsPerMinute;
        public void calcBPD()
        {
            barsPerDay1 = (minutesPerDay * symbolCount * shortsPerBar);
            barsPerDay5 = ((minutesPerDay / 5) * symbolCount * shortsPerBar);
            barsPerDay15 = ((minutesPerDay / 15) * symbolCount * shortsPerBar);
            shortsPerMinute = symbolCount * shortsPerBar;
            shortsPerDay = shortsPerMinute * minutesPerDay;
        }
        //int symbolNumber;
        public int dayNumber;
        public int minuteNumber;
        private double itod(short i)
        {
            double rv = (double)i;
            return rv / 100;
        }
        public double MinuteOpen(int mins, int period)
        {
            try
            {
                int offset = 0;
                int adjustedMinuteNumber = minuteNumber - period;
                if (mins == 1)
                {
                    double rv = 0;
                    bool uf = false;
                    do
                    {
                        offset = (dayNumber * shortsPerDay) + (adjustedMinuteNumber * shortsPerMinute) + (symbolNumber * shortsPerBar);
                        if (offset < 0)
                        {
                            if (adjustedMinuteNumber >= 0)
                            {

                            }
                            uf = true;
                        }
                        else
                        {
                            rv = arr[offset + 2];
                        }
                        adjustedMinuteNumber -= 1; // if open price = 0, go to previous minte next time
                        if(Math.Abs(rv) > 100000) results.Add("offset " + offset + " rv " + rv);
                    } while (!uf && rv == 0);
                    return rv;
                }
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            throw (new Exception("not supposed to be here"));
            return 0;
        }
        public double CurrentDayLow()
        {
            int offset = 0;
            int adjustedMinuteNumber = minuteNumber;
            offset = (dayNumber * shortsPerDay) + (adjustedMinuteNumber * shortsPerMinute) + (symbolNumber * shortsPerBar);
            if (offset < 0)
            {
                if (adjustedMinuteNumber >= 0)
                {

                }
                return 0;
            }
            double rv = arr[offset + 5];
            return rv;
        }
        public double CurrentDayHigh()
        {
            int offset = 0;
            int adjustedMinuteNumber = minuteNumber;
            offset = (dayNumber * shortsPerDay) + (adjustedMinuteNumber * shortsPerMinute) + (symbolNumber * shortsPerBar);
            if (offset < 0)
            {
                if (adjustedMinuteNumber >= 0)
                {

                }
                return 0;
            }
            double rv = arr[offset + 4];
            return rv;
        }
        public string[] dates;
        public string[] symbols;
        public static TestResult TestDay(ILookup<DateTime, IBarDay> bdl, DateTime date,string[] dates, string[] symbols, float[][] arrs, Dictionary<string,IDayCandle> dcPrev, Dictionary<string, IDayCandle> dcToday)
        {
            TestResult tr = new TestResult();
            Test.testResult = tr;
            tr.Date = date;
            string dateString = date.ToString("MM/dd/yyyy");
            int dayNumber = Array.BinarySearch(dates, dateString);
            for (int i = 0; i < 390; i++)
            {
                foreach (IBarDay bd in bdl[date])
                {
                    string symbol = bd.Symbol;
                    Test t;
                    if (!testDictionary.ContainsKey(symbol))
                    {
                        t = (Test)Activator.CreateInstance(type);
                        t.symbolNumber = Array.BinarySearch(symbols, symbol);
                        t.symbolCount = symbols.Count();
                        t.minutesPerDay = 390;
                        t.shortsPerBar = 7;
                        t.calcBPD();
                        t.dates = dates;
                        t.symbols = symbols;
                        t.arr = arrs[0];
                        testDictionary[symbol] = t;
                    }
                    t = testDictionary[symbol];
                    try
                    {
                        if (dcPrev.ContainsKey(symbol) && dcToday.ContainsKey(symbol))
                        {

                            IDayCandle prev = dcPrev[symbol];
                            IDayCandle today = dcToday[symbol];
                            t.runMinute(i, tr, bd, dayNumber, prev, today);
                        }
                        else
                        {
                            if (i == 0)
                            {
                                results.Add($"missing day candle for {symbol} + {date}");
                                Console.WriteLine($"missing day candle for {symbol} + {date}");
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                        //Console.WriteLine($"crap {symbol} {t.dates[0]}" );

                    }
                }
            }
            foreach (Test t in testDictionary.Values)
            {
                if (t.inPosition && t.maxOvernights > 0)
                {
                    t.inPositionDays++;
                }
            }
            return tr;
        }
        bool inPosition = false;
        bool orderInForce = false;
        char activeOrderSide = ' ';
        int orderInForceExpiry = 0;
        public double enterLimitPrice = 0;
        public double exitLimitPrice = 0;
        public double enterExecPrice = 0;
        double exitExecPrice = 0;
        int inPositionDays = 0;
        int stopTime = 0;
        public static List<string> results = new List<string>();
        public void runMinute(int i, TestResult tr, IBarDay barday, int dayNumber, IDayCandle prev, IDayCandle today)
        {
            //Console.WriteLine($"dayno={dayNumber} minteNumber={i}");
            this.today = today;
            this.prev = prev;
            if (i == 3)
            {
                string ip = inPosition ? "t" : "f";
                string oif = orderInForce ? "t" : "f";
                //results.Add($"{today.Symbol} {today.Open}  {today.Date} {i} {ip} {oif}");
            }
            if (today.Symbol == "XOM") { }
            this.i = i;
            if (i == 0)
            {
                //inPosition = false;
                orderInForce = false;
                orderInForceExpiry = 0;
                enterLimitPrice = 0;
                //exitLimitPrice = 0;
                //enterExecPrice = 0;
                exitExecPrice = 0;
                //stopTime = 0;
            }
            bd = barday;
            //Console.WriteLine($"{i} {bd.Symbol}");
            underflow = false;
            Test.messages.Clear();
            if (!inPosition && !orderInForce)
            {
                //double tt = MinuteOpen(1, 0);
                int minuteNumber = i;
                CheckEntryCriteria(results, barday, ref orderInForce, ref orderInForceExpiry, ref enterLimitPrice,dayNumber,minuteNumber);
            }
            if (!inPosition)
            {
                if (orderInForce)
                {
                    if (i < orderInForceExpiry)
                    {
                        //ProcessTradeEntry(results, barday, ref inPosition, ref orderInForce, enterLimitPrice, ref exitLimitPrice, ref enterExecPrice, ref stopTime);
                        ProcessTradeEntry(results, barday);// ref inPosition, ref orderInForce, enterLimitPrice, ref exitLimitPrice, ref enterExecPrice, ref stopTime);
                    }
                    else //tif expired
                    {
                        orderInForce = false;
                        results.Add($"{barday.Symbol} tif expired {i}");
                    }
                }
            }
            if (inPosition)
            {
                ProcessTradeExit(results, barday);//, ref inPosition, exitLimitPrice, enterExecPrice, ref exitExecPrice, stopTime);
            }

        }
        static Dictionary<DateTime, Dictionary<string, IBar>> dict;
        public static bool uf;
        static string screenSymbol;
        static DateTime screenDate;
        static DateTime[] dbk = null;
        public static List<string> dbs = new List<string>();
        public double GetAverageDailyRange(int d)
        {
            Double tot = 0;
            for (int i = 1; i < d + 1; i++)
            {
                IBar b = GetDayBar(i);
                if (Test.uf) return 0;
                tot += (b.High - b.Low);
            }
            return tot / d;
        }
        public long GetAverageDailyVolume(int d)
        {
            long tot = 0;
            for (int i = 1; i < d + 1; i++)
            {
                IBar b = GetDayBar(i);
                if (Test.uf) return 0;
                tot += b.Volume;
            }
            return tot / d;
        }
        public IBar GetDayBar(DateTime dt, int offset)
        {
            screenDate = dt;
            return GetDayBar(offset);
        }
        public static Type bartype;
        IBar GetZeroBar()
        {
            IBar rv = (IBar)Activator.CreateInstance(bartype,new object[] { 0, 0, 0, 0, 0 });
            return rv;
        }
        public IBar GetDayBar(int d)
        {
            IBar rv = bd.GetMinuteBar(0, 390, 0);
            return rv;
            if (dbk == null)
            {
                dbk = dict.Keys.ToArray();
                Array.Sort(dbk);
            }
            int ind = Array.BinarySearch(dbk, screenDate);
            if (ind >= d) ind -= d;
            else
            {
                uf = true;
                return GetZeroBar();
            }
            Dictionary<string, IBar> sb = dict[dbk[ind]];
            //File.AppendAllText(@"c:\src\diag.txt", $"open={sb["SPY"].Open} close={sb["SPY"].Close}");
            return sb[screenSymbol];
        }
        public abstract string GetVerion();
        public static List<Tuple<string, DateTime>> ScreenAll(string[] symbols, string[] dates, Test inTest)
        {

            List<Tuple<string, DateTime>> screened = new List<Tuple<string, DateTime>>();
            //dict = indict;
            DateTime[] dateArray = (from d in dates select DateTime.Parse(d)).ToArray();
            foreach (DateTime dt in dateArray)
            {
                foreach (string sym in symbols)
                {
                    screenSymbol = sym;
                    screenDate = dt;
                    uf = false;
                    if (inTest.screen())
                    {
                        if (!uf)
                        {
                            screened.Add(new Tuple<string, DateTime>(sym, dt));
                        }
                    }
                }
            }
            return screened;
        }
        //public string[] run(IBarDay[] barDays, Dictionary<DateTime, Dictionary<string, IBar>> dayDict)
        public string[] run(IBarDay[] barDays, string[] symbols, string[] dates,float[][] arrs, Dictionary<string,Dictionary<string, IDayCandle>> dcPrev, Dictionary<string, Dictionary<string, IDayCandle>> dcToday)
        {
            testDictionary.Clear();
            List<Tuple<string, DateTime>> screened = ScreenAll(symbols, dates, this);
            //Test.testResults = new List<TestResult>();
            //List<string> results = new List<string>();
            //List<TestResult> testResults = new List<TestResult>();
            int goodDay = 0;
            int badDay = 0;
            int exceptions = 0;
            double runningPandL = 0;
            ILookup<DateTime, IBarDay> bdl = barDays.ToLookup(k => k.Date);
            List<IBarDay> barDayList = new List<IBarDay>();
            foreach (var tp in screened)
            {
                var bdg = bdl[tp.Item2];
                var item = (from bd in bdg where bd.Symbol == tp.Item1 select bd).FirstOrDefault();
                if (item != null)
                {
                    barDayList.Add(item);
                }
            }
            bdl = barDayList.ToLookup(k => k.Date);
            foreach (var dt in bdl)
            {
                Dictionary<string, IDayCandle> today = dcToday[dt.Key.ToString("MM/dd/yyyy")];
                Dictionary<string, IDayCandle> prev = dcPrev[dt.Key.ToString("MM/dd/yyyy")];
                TestResult tr = TestDay(bdl, dt.Key,dates,symbols, arrs, prev, today);
                testResults.Add(tr);

            }
            string[] s = { "hello", "world" };
            return s;
            //foreach (BarDay barday in barDays)
            //{
            //    Bar b = (from ba in barday.bars where ba != null select ba).First();
            //    double dayOpen = b.Open;
            //    testResult = new TestResult() { Date = barday.Date, DayOpen = dayOpen };
            //    Test.messages.Clear();
            //    bd = barday;

            //    this.bars = barday.bars;

            //    int barCount = bars.Count();
            //    if (barCount == 390) goodDay++; else badDay++;
            //    for (i = 0; i < barCount; i++)
            //    {
            //        TestDay(bdl, bd.Date);
            //    }
            //    runningPandL += testResult.PandL;
            //    testResult.RunningPandL = runningPandL;
            //    if (testResult.NumTrades > 0) testResults.Add(testResult);
            //}
            //results.Insert(0, $"good: {goodDay} bad: {badDay} exceptions: {exceptions}");
            //return results.ToArray();
        }

        //        private void ProcessTradeExit(List<string> results, BarDay barday, ref bool inPosition, double exitLimitPrice, double enterExecPrice, ref double exitExecPrice, int stopTime)
        private void ProcessTradeExit(List<string> results, IBarDay barday) //, ref bool inPosition, double exitLimitPrice, double enterExecPrice, ref double exitExecPrice, int stopTime)
        {
            IBar b = bd.GetMinuteBar(i, 1, 0);
            //File.AppendAllText(@"c:\src\diag.txt", $"profitExit={profitExit} tif={tif} exitLimitPrice{exitLimitPrice}\r\n");
            if (b != null)
            {
                if (b.Open > exitLimitPrice && activeOrderSide == 'B')
                {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    exitExecPrice = exitLimitPrice;
                    double pandl = exitExecPrice - enterExecPrice;
                    testResult.NumTrades++;
                    testResult.PandL += pandl;
                    //testResults.Add(testResult);
                    string str = $"{bd.Symbol} exitlong:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} exitExecPrice={exitExecPrice.ToString("0.00")} p&l={(exitExecPrice - enterExecPrice).ToString("0.00")}";
                    results.Add(str);
                    results.AddRange(Test.messages);
                    inPosition = false;
                } else if(b.Open != 0 && b.Open < exitLimitPrice && activeOrderSide == 'S') {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    exitExecPrice = exitLimitPrice;
                    double pandl = enterExecPrice - exitExecPrice;
                    testResult.NumTrades++;
                    testResult.PandL += pandl;
                    //testResults.Add(testResult);
                    string str = $"{bd.Symbol} exitsell:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterExecPrice={enterExecPrice.ToString("0.00")} exitExecPrice={exitExecPrice.ToString("0.00")} p&l={pandl.ToString("0.00")}";
                    results.Add(str);
                    results.AddRange(Test.messages);
                    inPosition = false;
                }
                else if ((i >= stopTime || (i >= CloseAll && inPositionDays >= maxOvernights)) && b.Open != 0)
                {
                    results.Add($"stop time{stopTime}");
                    //File.AppendAllText(@"c:\src\diag.txt", $"profitExit={profitExit} tif={tif} exitLimitPrice{exitLimitPrice} \r\n");
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    exitExecPrice = b.Open;
                    double pandl = enterExecPrice - exitExecPrice;
                    if (activeOrderSide == 'B') pandl *= -1;
                    testResult.NumTrades++;
                    testResult.PandL += pandl;
                    //testResults.Add(testResult);
                    string str = $"{bd.Symbol} Stop Exit:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterExecPrice={enterExecPrice.ToString("0.00")} exitExecPrice={exitExecPrice.ToString("0.00")} p&l={(pandl).ToString("0.00")}";
                    results.Add(str);
                    results.AddRange(Test.messages);
                    inPosition = false;
                }
            }
        }

        //        private void ProcessTradeEntry(List<string> results, BarDay barday, ref bool inPosition, ref bool orderInForce, double enterLimitPrice, ref double exitLimitPrice, ref double enterExecPrice, ref int stopTime)
        private void ProcessTradeEntry(List<string> results, IBarDay barday) //, ref bool inPosition, ref bool orderInForce, double enterLimitPrice, ref double exitLimitPrice, ref double enterExecPrice, ref int stopTime)
        {
            IBar b = bd.GetMinuteBar(i, 1, 0);
            if (b.Open == 0 && b.Volume == 0) return;
            if (b.Open < enterLimitPrice && activeOrderSide == 'B')
            {
                DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);

                inPosition = true;
                inPositionDays = 0;
                enterExecPrice = b.Open;
                double mc1 = MinuteClose(1, 1);
                double elp = mc1 + (prev.ATR * .1);
                exitLimitPrice = CalculateLongExitLimitPrice();
                b = bd.GetMinuteBar(i, 1, 0);
                string str = $"{bd.Symbol} enterlong:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterExecPrice={enterExecPrice.ToString("0.00")} exitLimitPrice={exitLimitPrice.ToString("0.00")}";
                results.Add(str);
                results.AddRange(Test.messages);
                orderInForce = false;
                stopTime = i + StopTimeInterval;
                //File.AppendAllText(@"c:\src\diag.txt", $"stopTime={stopTime} i={i}\r\n");
            }
            if (b.Open > enterLimitPrice && activeOrderSide == 'S')
            {
                DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);

                inPosition = true;
                inPositionDays = 0;
                enterExecPrice = b.Open;
                double mc1 = MinuteClose(1, 1);
                double elp = mc1 + (prev.ATR * .1);
                exitLimitPrice = CalculateShortExitLimitPrice();
                b = bd.GetMinuteBar(i, 1, 0);
                string str = $"{bd.Symbol} entershort:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterExecPrice={enterExecPrice.ToString("0.00")} exitLimitPrice={exitLimitPrice.ToString("0.00")}";
                results.Add(str);
                results.AddRange(Test.messages);
                orderInForce = false;
                stopTime = i + StopTimeInterval;
                results.Add($"current time {i} stop time {stopTime}");
                //File.AppendAllText(@"c:\src\diag.txt", $"stopTime={stopTime} i={i}\r\n");
            }
        }

        private void CheckEntryCriteria(List<string> results, IBarDay barday, ref bool orderInForce, ref int orderInForceExpiry, ref double enterLimitPrice, int dayNumber, int minuteNumber)
        {
            if (CheckLongEntryCriteria(symbolNumber,dayNumber,minuteNumber))
            {
                IBar b = bd.GetMinuteBar(i, 1, 0);
                if (b == null) underflow = true;
                if (!underflow)
                {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    entering = true;
                    enterLimitPrice = CalculateLongEntryLimitPrice();
                    //double d = MinuteClose(5, 0);
                    //double dd = bars[i].Close;
                    entering = false;
                    int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                    string str = $"{bd.Symbol} metlong: {barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterLimitPrice={enterLimitPrice.ToString("0.00")} pid={pid}";
                    results.Add(str);
                    results.AddRange(Test.messages);
                    orderInForce = true;
                    activeOrderSide = 'B';
                    orderInForceExpiry = i + tif;
                    //File.AppendAllText(@"c:\src\diag.txt", $"profitExit={profitExit} tif={tif}\r\n");
                }
            }
            if (CheckShortEntryCriteria(symbolNumber, dayNumber, minuteNumber))
            {
                IBar b = bd.GetMinuteBar(i, 1, 0);
                if (b == null) underflow = true;
                if (!underflow)
                {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    entering = true;
                    enterLimitPrice = CalculateShortEntryLimitPrice();
                    //double d = MinuteClose(5, 0);
                    //double dd = bars[i].Close;
                    entering = false;
                    int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                    string str = $"{bd.Symbol} metshort: {barTime.ToString("MM-dd-yy HH:mm:ss")} {i} enterLimitPrice={enterLimitPrice.ToString("0.00")} pid={pid}";
                    results.Add(str);
                    results.AddRange(Test.messages);
                    orderInForce = true;
                    activeOrderSide = 'S';
                    orderInForceExpiry = i + tif;
                    //File.AppendAllText(@"c:\src\diag.txt", $"profitExit={profitExit} tif={tif}\r\n");
                }
            }
        }

        //public BarList GetMinuteBar(int minutes, int prev)
        //{

        //}
        //public void getParms(ref int start, ref int len, ref int interval, ref bool underflow, int mins, int p)
        //{
        //    interval = i / mins;
        //    interval -= p;
        //    start = interval * mins;
        //    len = start + mins > i ? i - start + 1 : mins;
        //    if (len == 0)
        //    {

        //    }
        //    //end = start + mins;
        //    //if (end > i + 1) end = i + 1;
        //    if (interval < 0)
        //    {
        //        underflow = true;
        //    }
        //    if (start + mins > 389) underflow = true;
        //    double open = 0, close = 0, high = 0, low = 0;
        //    int volume = 0;
        //    bool debug = true;
        //    if (!underflow && debug)
        //    {
        //        open = bars[start].Open;
        //        close = bars[start + len].Close;
        //        BarList bl = new BarList(bars, start, len);
        //        //Bar[] tBars = bars.SubArray(start, len);
        //        //high = (from b in tBars select b.High).Max();
        //        //low = (from b in tBars select b.Low).Min();
        //        //volume = (from b in tBars select b.Volume).Sum();
        //        high = (from b in bl select b.High).Max();
        //        low = (from b in bl select b.Low).Min();
        //        volume = (from b in bl select b.Volume).Sum();
        //        string rep = $"underflow={underflow} mins={mins} i={i} interval={interval} start={start} end={start + len} len={len} p={p} open={open} high={high} low={low} close={close} volume={volume}";
        //    }
        //}
        //public double MinuteOpen(int mins, int p)
        //{
        //    double rv = 0;
        //    IBar mb = bd.GetMinuteBar(i, mins, p);
        //    if (mb == null)
        //    {
        //        underflow = true;
        //    }
        //    else rv = mb.Open;
        //    return rv;
        //}
        public double MinuteHigh(int mins, int p)
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, mins, p);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.High;
            return rv;
        }
        public double DayHigh()
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, 1, 0);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.DayHigh;
            return rv;
        }
        public double DayLow()
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, 1, 0);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.DayLow;
            return rv;
        }
        public double MinuteClose(int mins, int p)
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, mins, p);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.Close;
            return rv;
            //Bar b1 = GetHa
            //if (entering)
            //{

            //}
            //int interval = 0, start = 0, len = 0;
            //getParms(ref start, ref len, ref interval, ref underflow, mins, p);
            //if (i < mins * p)
            //{
            //    underflow = true;
            //    return 0;
            //}
            //rv = 0;
            //if (!underflow)
            //{
            //    rv = bars[start + len - 1].Close;
            //}
            //return rv;
        }
        public int MinuteVolume(int mins, int p)
        {
            int rv = 0;
            IBar mb = bd.GetMinuteBar(i, mins, p);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.Volume;
            return rv;
        }
        public double MinuteLow(int mins, int p)
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, mins, p);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.Low;
            return rv;
        }
        public double DayHigh(int p)
        {
            return 0;
        }
        public double DayLow(int p)
        {
            return 0;
        }
    }
    public class TestResult
    {
        public int NumTrades;
        public DateTime Date;
        public double DayOpen;
        public double RunningPandL;
        public double PandL;
        public override string ToString()
        {
            return $"{Date.ToString("MM/dd/yyyy")},{NumTrades},{PandL.ToString("0.00")},{DayOpen.ToString("0.00")},{RunningPandL.ToString("0.00")}";
        }
    }
}
