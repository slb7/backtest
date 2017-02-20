using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest
{
    class BarStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string[] tx = x.Split(',');
            string[] ty = y.Split(',');
            if (tx[0] != ty[0]) return tx[0].CompareTo(ty[0]);
            if (tx[1] != ty[1]) return tx[1].CompareTo(ty[1]);
            if (tx[2] != ty[2]) return tx[2].CompareTo(ty[2]);
            return 0;
        }
    }
}
