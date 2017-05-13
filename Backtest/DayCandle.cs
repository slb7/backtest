using Backtest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest
{
    public class DayCandle : IDayCandle
    {
        DateTime date;
        string symbol;
        double high;
        double low;
        double open;
        double close;
        int volume;
        int atv;
        double atr;
        public DayCandle(string pl)
        {
            string[] toks = pl.Split(',');
            symbol = toks[0];
            date = DateTime.Parse(toks[1]);
            open = Double.Parse(toks[2]);
            high = Double.Parse(toks[3]);
            low = Double.Parse(toks[4]);
            close = Double.Parse(toks[5]);
            volume = Int32.Parse(toks[6]);
            atr = Double.Parse(toks[7]);
            atv = (int)Double.Parse(toks[8]);
            if(toks[1] == "07/29/2016" && symbol == "XOM")
            {
                Console.WriteLine($"XOM open {open} ");

            }
        }
        double IDayCandle.ATR
        {
            get
            {
                return atr;
            }
        }

        int IDayCandle.ATV
        {
            get
            {
                return atv;
            }
        }

        double IDayCandle.Close
        {
            get
            {
                return close;
            }
        }

        DateTime IDayCandle.Date
        {
            get
            {
                return date;
            }
        }

        double IDayCandle.High
        {
            get
            {
                return high;
            }
        }

        double IDayCandle.Low
        {
            get
            {
                return low;
            }
        }

        double IDayCandle.Open
        {
            get
            {
                return open;
            }
        }

        string IDayCandle.Symbol
        {
            get
            {
                return symbol;
            }
        }

        int IDayCandle.Volume
        {
            get
            {
                return volume;
            }
        }
    }
}
