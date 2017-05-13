using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IBackTest
{
    public abstract class Test
    {
        public TestContainer Container;
        bool underflow;
        int i;
        IBar[] bars;
        public IBarDay bd;
        public abstract bool t(int i);
        public abstract double exec();
        public abstract bool screen();

        public int tif = 0;
        public double profitExit = .01;
        public int maxOvernights = 0;
        public bool entering = false;
        public int CloseAll;
        public int StopTimeInterval;
        //int stopTime;
        public static int Time(int i)
        {
            int rv = (int)new TimeSpan(0, i, 0).TotalSeconds;
            return rv;
        }
        public static int Time(string t)
        {
            int rv = (int)((TimeSpan.Parse(t) - TimeSpan.Parse("09:30")).TotalSeconds);
            return rv;
        }
        public bool inPosition = false;
        bool orderInForce = false;
        int orderInForceExpiry = 0;
        double enterLimitPrice = 0;
        double exitLimitPrice = 0;
        double enterExecPrice = 0;
        double exitExecPrice = 0;
        public int inPositionDays = 0;
        int stopTime = 0;
        public void runMinute(int i, TestResult tr, IBarDay barday)
        {
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
            underflow = false;
            Container.messages.Clear();
            if (!inPosition && !orderInForce)
            {
                //double tt = MinuteOpen(1, 0);
                CheckEntryCriteria(Container.results, barday, ref orderInForce, ref orderInForceExpiry, ref enterLimitPrice, i);
            }
            else if (!inPosition)
            {
                if (orderInForce)
                {
                    if (i < orderInForceExpiry)
                    {
                        //ProcessTradeEntry(results, barday, ref inPosition, ref orderInForce, enterLimitPrice, ref exitLimitPrice, ref enterExecPrice, ref stopTime);
                        ProcessTradeEntry(Container.results, barday);// ref inPosition, ref orderInForce, enterLimitPrice, ref exitLimitPrice, ref enterExecPrice, ref stopTime);
                    }
                    else //tif expired
                    {
                        orderInForce = false;
                        Container.results.Add($"tif expired {i}");
                    }
                }
            }
            else if (inPosition)
            {
                ProcessTradeExit(Container.results, barday);//, ref inPosition, exitLimitPrice, enterExecPrice, ref exitExecPrice, stopTime);
            }

        }

        public double GetAverageDailyRange(int d)
        {
            Double tot = 0;
            for (int i = 1; i < d + 1; i++)
            {
                IBar b = GetDayBar(i);
                if (Container.uf) return 0;
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
                if (Container.uf) return 0;
                tot += b.Volume;
            }
            return tot / d;
        }
        public IBar GetDayBar(DateTime dt, int offset)
        {
            Container.screenDate = dt;
            return GetDayBar(offset);
        }
        IBar GetZeroBar()
        {
            IBar rv = (IBar)Activator.CreateInstance(Container.bartype,new object[] { 0, 0, 0, 0, 0 });
            return rv;
        }
        public IBar GetDayBar(int d)
        {
            return Container.dict[Container.screenDate][Container.screenSymbol];
            if (Container.dbk == null)
            {
                Container.dbk = Container.dict.Keys.ToArray();
                Array.Sort(Container.dbk);
            }
            int ind = Array.BinarySearch(Container.dbk, Container.screenDate);
            if (ind >= d) ind -= d;
            else
            {
                Container.uf = true;
                return GetZeroBar();
            }
            Dictionary<string, IBar> sb = Container.dict[Container.dbk[ind]];
            //File.AppendAllText(@"c:\src\diag.txt", $"open={sb["SPY"].Open} close={sb["SPY"].Close}");
            return sb[Container.screenSymbol];
        }
        public abstract string GetVerion();
        public string[] run(IBarDay[] barDays, Dictionary<DateTime, Dictionary<string, IBar>> dayDict)
        {
            Container.testDictionary.Clear();
            List<Tuple<string, DateTime>> screened = Container.ScreenAll(dayDict, this);
            Container.testResults = new List<TestResult>();
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
                TestResult tr = Container.TestDay(bdl, dt.Key);
                Container.testResults.Add(tr);

            }
            string[] s = { "hello", "world" };
            return s;
            //foreach (BarDay barday in barDays)
            //{
            //    Bar b = (from ba in barday.bars where ba != null select ba).First();
            //    double dayOpen = b.Open;
            //    testResult = new TestResult() { Date = barday.Date, DayOpen = dayOpen };
            //    Container.messages.Clear();
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
                if (b.Open > exitLimitPrice)
                {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    exitExecPrice = exitLimitPrice;
                    double pandl = exitExecPrice - enterExecPrice;
                    Container.testResult.NumTrades++;
                    Container.testResult.PandL += pandl;
                    //testResults.Add(testResult);
                    string str = $"{bd.Symbol} exit:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} {b.ToString()} exitExecPrice={exitExecPrice} p&l={(exitExecPrice - enterExecPrice).ToString("0.00")}";
                    results.Add(str);
                    results.AddRange(Container.messages);
                    inPosition = false;
                }
                else if (i >= stopTime || (i >= CloseAll && inPositionDays >= maxOvernights))
                {
                    //File.AppendAllText(@"c:\src\diag.txt", $"profitExit={profitExit} tif={tif} exitLimitPrice{exitLimitPrice} \r\n");
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    exitExecPrice = b.Open;
                    double pandl = exitExecPrice - enterExecPrice;
                    Container.testResult.NumTrades++;
                    Container.testResult.PandL += pandl;
                    //testResults.Add(testResult);
                    string str = $"{bd.Symbol} Stop Exit:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} {b.ToString()} exitExecPrice={exitExecPrice} p&l={(exitExecPrice - enterExecPrice).ToString("0.00")}";
                    results.Add(str);
                    results.AddRange(Container.messages);
                    inPosition = false;
                }
            }
        }

        //        private void ProcessTradeEntry(List<string> results, BarDay barday, ref bool inPosition, ref bool orderInForce, double enterLimitPrice, ref double exitLimitPrice, ref double enterExecPrice, ref int stopTime)
        private void ProcessTradeEntry(List<string> results, IBarDay barday) //, ref bool inPosition, ref bool orderInForce, double enterLimitPrice, ref double exitLimitPrice, ref double enterExecPrice, ref int stopTime)
        {
            IBar b = bd.GetMinuteBar(i, 1, 1);
            if (b.Open < enterLimitPrice)
            {
                DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);

                inPosition = true;
                inPositionDays = 0;
                enterExecPrice = b.Open;
                exitLimitPrice = enterExecPrice + profitExit;
                b = bd.GetMinuteBar(i, 1, 0);
                string str = $"{bd.Symbol} enter:{barTime.ToString("MM-dd-yy HH:mm:ss")} {i} {b.ToString()} enterExecPrice={enterExecPrice} exitLimitPrice={exitLimitPrice}";
                results.Add(str);
                results.AddRange(Container.messages);
                orderInForce = false;
                stopTime = i + StopTimeInterval;
                //File.AppendAllText(@"c:\src\diag.txt", $"stopTime={stopTime} i={i}\r\n");
            }
        }

        private void CheckEntryCriteria(List<string> results, IBarDay barday, ref bool orderInForce, ref int orderInForceExpiry, ref double enterLimitPrice,int i)
        {
            if (t(i))
            {
                IBar b = bd.GetMinuteBar(i, 1, 0);
                if (b == null) underflow = true;
                if (!underflow)
                {
                    DateTime barTime = barday.Date.AddHours(9.5).AddMinutes(i);
                    entering = true;
                    enterLimitPrice = exec();
                    //double d = MinuteClose(5, 0);
                    //double dd = bars[i].Close;
                    entering = false;
                    string str = $"{bd.Symbol} met: {barTime.ToString("MM-dd-yy HH:mm:ss")} {i} {b.ToString()} enterLimitPrice={enterLimitPrice}";
                    results.Add(str);
                    results.AddRange(Container.messages);
                    orderInForce = true;
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
        public double MinuteOpen(int mins, int p)
        {
            double rv = 0;
            IBar mb = bd.GetMinuteBar(i, mins, p);
            if (mb == null)
            {
                underflow = true;
            }
            else rv = mb.Open;
            return rv;
        }
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
        public double MinuteLow(int mins, int p)
        {
            return 0;
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
