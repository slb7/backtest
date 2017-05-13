using Backtest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest
{
    public class MyTest : Backtest.Infrastructure.Test
    {
        public override double exec()
        {
            return MinuteOpen(1, 0);
        }

        public override string GetVerion()
        {
            return "1.0";
        }

        public override bool screen()
        {
            return true;
        }
        public override bool t(int symbolNumber, int dayNumber, int minuteNumber)
        //public override bool t(int i)
        {
            this.symbolNumber = symbolNumber; this.dayNumber = dayNumber; this.minuteNumber = minuteNumber;
            double mo1 = MinuteOpen(1, 1);
            double mo2 = MinuteOpen(1, 2);
            //if (symbols[symbolNumber] == "AMZN")
            //{
            //    Console.WriteLine($"d={dates[dayNumber]} sn={symbols[symbolNumber]} dn={dayNumber} mn={minuteNumber} time={TimeSpan.Parse("09:30").Add(new TimeSpan(0, 0, minuteNumber, 0, 0))} mo1={mo1.ToString("0.00")} mo2={mo2.ToString("0.00")}");
            //}
            //IBar b0 = GetDayBar(bd.Date, 0);
            tif = 10;
            profitExit = .1;
            StopTimeInterval = 20;
            maxOvernights = 0;
            CloseAll = (int)(TimeSpan.Parse("11:59").Subtract(TimeSpan.Parse("09:30")).TotalMinutes);
            bool res = false;
            if (mo1 > mo2 * 1.06 && mo1 != 0 && mo2 != 0)
            {
                res = true;
            }
            if (mo1 != 0 && mo2 != 0 && res)
            {
            }
            return res;




            //IBar b1 = GetDayBar(bd.Date,1);
            //IBar b2 = GetDayBar(bd.Date,2);
            //IBar b3 = GetDayBar(bd.Date,3);
            //if (dbs.Count < 10000)
            //{
            //    dbs.Add("b0.Open=" + b0.Open.ToString()  + " b1.Close=" + b1.Close.ToString());
            //}
            //File.AppendAllText(@"c:\src\diag.txt",$"b0.Open{b0.Open} b1.Close{b1.Close}\r\n
            //int t1 = Test.Time(i);
            ////////return MinuteClose(1, 1) < b0.Open * 0.995 && t1 > t931 && t1 < t1259;
            return true;
            //return b1.Close > b1.Open * 1.0075 && b0.Open < b1.Close * 1.03 && Time() > Time("09:31") && Time() < Time("09:33");
            //return b0.Open > b1.Close * 0.95 && MinuteClose(1, 1) > b1.Close * 1.005 && MinuteClose(1, 1) > b0.Open * 1.002 && Time() > Time("10:01") && Time() < Time("10:03");
            //double close = MinuteClose(5, 1);
            //MinuteClose(1, 1) > MinuteOpen(60, 4) * 0.95
            //double open = MinuteOpen(5, 1) - 1.8;
            //bool b = false;
            //b = close < open;
            //return b && Time() > Time("9:40");
        }
    }
}
