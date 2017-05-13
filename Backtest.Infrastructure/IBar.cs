using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest.Infrastructure
{
    public interface IBar : IComparable
    {
        DateTime Time { get; }
        string Symbol { get; }
        double Open { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        double DayHigh { get; }
        double DayLow { get; }
        int Volume { get; }
        void SetDayValues(IBar b,bool first=false);
        IBar GetZeroBar();

    }
}
