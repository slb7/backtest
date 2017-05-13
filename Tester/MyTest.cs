//hs2
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
        public override double CalculateShortEntryLimitPrice()
        { return 0; }
        public override double CalculateLongEntryLimitPrice()
        {
            double mo = MinuteOpen(1, 0);
            //string str = $"{symbols[symbolNumber]} {mo.ToString("0.00")}";
            //results.Add("hel;;p");
            return MinuteOpen(1, 0) + 0.5;
        }

        public override string GetVerion()
        {
            return "1.6";
        }

        public override bool screen()
        {
            return true;
        }
        public override double CalculateLongExitLimitPrice()
        {
            double rv = enterExecPrice + prev.ATR * 0.5;
            return rv;
        }
        //public override bool t(int symbolNumber, int dayNumber, int minuteNumber)
        public override bool CheckLongEntryCriteria(int symbolNumber, int dayNumber, int minuteNumber)
        //public override bool t(int i)
        {
            tif = 10;
            profitExit = 9.1;
            StopTimeInterval = 20;
            maxOvernights = 0;
            CloseAll = 385;
            if (symbols[symbolNumber] == "GBSN") return false;
            this.symbolNumber = symbolNumber; this.dayNumber = dayNumber; this.minuteNumber = minuteNumber;
            double mo1 = MinuteOpen(1, 1);
            double mo2 = MinuteOpen(1, 2);
            double mc2 = MinuteClose(1, 2);
            //foreach(string s in symbols)
            //{
            //    Console.Write(s + ' ');
            //}
            //Console.WriteLine();
            if (minuteNumber == 0)
            {
                try
                {
                    int mov = MinuteVolume(1, 0);
                    
                    results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " copen " + MinuteOpen(1, 0) + " vol " + mov);
                }
                catch (Exception e)
                {
                    results.Add("exc");
                }
                //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " copen " + MinuteOpen(1, 0));
            }
            if (mo1 == 0 || mo2 == 0) return false;

            //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " chigh " + MinuteHigh(1, 0));
            //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " clow " + MinuteLow(1, 0));
            //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " cclose " + MinuteClose(1, 0));

            if (prev != null && today != null)
            {
                double pc = prev.Close;
                double atr = prev.ATR;
                //mo1 < today.Open  - (4.5 * prev.ATR)
                if (mo1 < today.Open  - (0.5 * prev.ATR) && minuteNumber < 15 && minuteNumber > 2 && mo1 < 250)
                {
                    if(symbols[symbolNumber] == "XOM")
                    {

                    }
                    //results.Add("symbol=" + symbols[symbolNumber] + " date=" + dates[dayNumber] + " todopen=" + today.Open + " prevclose=" + prev.Close + " prev.ATR=" + prev.ATR + " mo1=" + mo1 + " closeall=" + CloseAll + " mc2 " + mc2);
                    //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " copen " + MinuteOpen(1, 0));
                    //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " chigh " + MinuteHigh(1, 0));
                    //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " clow " + MinuteLow(1, 0));
                    //results.Add("symbol=" + symbols[symbolNumber] + " minutenumber = " + minuteNumber + " date=" + dates[dayNumber] + " cclose " + MinuteClose(1, 0));

                    Console.WriteLine("symbol=" + symbols[symbolNumber] + " date=" + dates[dayNumber] + " todopen=" + today.Open + " prevclose=" + prev.Close + " prev.ATR=" + prev.ATR);
                    return true;
                }
                //Console.WriteLine("pc=" + pc + " atr=" + atr);
            }
            return false;
        }

        public override bool CheckShortEntryCriteria(int symbolNumber, int dayNumber, int minuteNumber)
        {
            return false;
        }

        public override double CalculateShortExitLimitPrice()
        {
            return 0;
        }
    }
}
