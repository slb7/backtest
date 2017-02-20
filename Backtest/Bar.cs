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
        private short[] arr;
        int sc;
        private int offset;
        private double itod(short i)
        {
            double rv = (double)i;
            return rv / 100;
        }
        IBar IBar.GetZeroBar()
        {
            Bar rv = new Bar(arr, 0, 0);
            return rv;
            //throw new NotImplementedException();
        }
        public Bar(short[] a,int offset, int symbolCount)
        {
            arr = a;
            sc = symbolCount;
            this.offset = offset;
        }
        double IBar.Close
        {
            get
            {
                return itod(arr[offset + 3]);
            }
        }

        double IBar.DayHigh
        {
            get
            {
                return itod(arr[offset + 4]);
            }
        }

        double IBar.DayLow
        {
            get
            {
                return itod(arr[offset + 5]);
            }
        }

        double IBar.High
        {
            get
            {
                return itod(arr[offset + 1]);
            }
        }

        double IBar.Low
        {
            get
            {
                return itod(arr[offset + 2]);
            }
        }

        double IBar.Open
        {
            get
            {
                return itod(arr[offset + 0]);
            }
        }

        int IBar.Volume
        {
            get
            {
                int[] ia = new int[1];
                Buffer.BlockCopy(arr, (offset * 2) + 12, ia, 0, 4);
                return ia[0];
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
