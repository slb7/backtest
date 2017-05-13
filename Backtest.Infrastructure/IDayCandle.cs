using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest.Infrastructure
{
    public interface IDayCandle
    {
        string Symbol { get; }
        DateTime Date { get; }
        Double High { get; }
        Double Low { get; }
        Double Open { get; }
        Double Close { get; }
        int Volume { get; }
        Double ATR { get; }
        int ATV { get; }
    }
}
