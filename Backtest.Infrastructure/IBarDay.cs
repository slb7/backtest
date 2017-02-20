using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest.Infrastructure
{
    public interface IBarDay
    {
        List<IBarDay> LoadAll(string dn);
        Dictionary<DateTime, Dictionary<string, IBar>> LoadDayCandles(string dn);
        IBar GetMinuteBar(int index, int minutes, int p);
        string Symbol { get; }
        DateTime Date { get; }
        int underflow { get; set;}
    }
}
