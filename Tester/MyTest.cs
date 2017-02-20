using Backtest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class MyTest : Backtest.Infrastructure.Test
    {
        public override double exec()
        {
            return 1;
        }

        public override string GetVerion()
        {
            return "1.0";
        }

        public override bool screen()
        {
            return true;
        }

        public override bool t(int i)
        {
            IBar b0 = GetDayBar(bd.Date, 0);
            return false;
            tif = 10;
            profitExit = 11.75;
            StopTimeInterval = 99380;
            maxOvernights = 4;



            CloseAll = (int)(TimeSpan.Parse("11:59").Subtract(TimeSpan.Parse("09:30")).TotalMinutes);
            //IBar b1 = GetDayBar(bd.Date,1);
            //IBar b2 = GetDayBar(bd.Date,2);
            //IBar b3 = GetDayBar(bd.Date,3);
            //if (dbs.Count < 10000)
            //{
            //    dbs.Add("b0.Open=" + b0.Open.ToString()  + " b1.Close=" + b1.Close.ToString());
            //}
            //File.AppendAllText(@"c:\src\diag.txt",$"b0.Open{b0.Open} b1.Close{b1.Close}\r\n
            int t1 = Test.Time(i);
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
