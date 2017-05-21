using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backtest.Infrastructure;

namespace Backtest
{
    public class Bar : IBar
    {
        //private BarDay bd;
        private float[] arr;
        int sc;
        private int offset;
        private double itod(short i)
        {
            double rv = (double)i;
            return rv / 100;
        }
        IBar IBar.GetZeroBar()
        {
            Bar rv = new Bar(arr, 0, 0,0,0,0);
            return rv;
            //throw new NotImplementedException();
        }
        public Bar(float[] a,int offset, int symbolCount, int index, int symbolnumber, int numMinutes)
        {
            arr = a;
            sc = symbolCount;
            this.offset = offset;
            this.numMinutes = numMinutes;
            this.interval = index;
            this.symbolNumber = symbolnumber;
        }
        double IBar.Close
        {
            get
            {
                return arr[offset + 3];
            }
        }

        double IBar.DayHigh
        {
            get
            {
                return arr[offset + 4];
            }
        }

        double IBar.DayLow
        {
            get
            {
                return arr[offset + 5];
            }
        }

        double IBar.High
        {
            get
            {
                return arr[offset + 1];
            }
        }

        double IBar.Low
        {
            get
            {
                return arr[offset + 2];
            }
        }

        double IBar.Open
        {
            get
            {
                return arr[offset + 0];
            }
        }

        int IBar.Volume
        {
            get
            {
                int[] ia = new int[1];
                Buffer.BlockCopy(arr, (offset * 4) + 24, ia, 0, 4);
                return ia[0];
            }
        }
        int symbolNumber;
        int numMinutes;
        int interval;
        int dayNumber;

        DateTime IBar.Time
        {
            get
            {
                DateTime rv = DateTime.Parse(BarDay.dates[dayNumber]).AddHours(9.5).AddMinutes(numMinutes * interval + 1);
                return rv;
            }
        }

        string IBar.Symbol
        {
            get
            {
                return BarDay.symbols[symbolNumber];
            }
        }

        int IComparable.CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
        void IBar.SetDayValues(IBar b, bool first)
        {
            throw new NotImplementedException();
        }
    }
}
