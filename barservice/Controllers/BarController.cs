using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace barservice.Controllers
{
    public class BarController : ApiController
    {
        static float[] bars = null;
        static string[] dates;
        static string[] symbols;
        public void loadBars()
        {
            string btd = Environment.GetEnvironmentVariable("BackTestDir");
            byte[] b = File.ReadAllBytes(@"{btd}bindata\201601.bin");
            bars = new float[b.Length / 4];
            Buffer.BlockCopy(b, 0, bars, 0, b.Length);
            b = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            symbols = File.ReadAllLines(@"{btd}bindata\201601.sym");
            dates = File.ReadAllLines(@"{btd}bindata\201601.dates");
        }
        public Models.Bar[] Get(string symbol,string date)
        {
            if(bars == null)
            {
                loadBars();
            }
            //DateTime inDate = DateTime.Parse("01/26/2016");
            int dayNo = Array.BinarySearch(dates, date);
            int dayStart = 7 * symbols.Length * 390 * dayNo;
            int[] v = new int[1];
            int symbolNo = Array.BinarySearch(symbols,symbol);
            if (symbolNo < 0 || dayNo < 0) return null;
            //int symbolNo = 1;
            int b;
            List<Models.Bar> bz = new List<Models.Bar>();
            DateTime dt = DateTime.Parse(date).AddHours(9.5);
            for(int i = 0;i<390;i++)
            {
                int off = i * 7 * symbols.Length;
                b = (symbolNo * 7) + off + dayStart;
                Buffer.BlockCopy(bars, (b * 4) + 24, v, 0, 4);
                Models.Bar bar = new Models.Bar() { Open = bars[b + 0], Close = bars[b + 3], High = bars[b + 1], Low = bars[b + 2], Volume = v[0], Time = dt.AddMinutes(i), DayHigh = bars[b + 4], DayLow = bars[b + 5] };
                bz.Add(bar);
            }
            return bz.ToArray();
        }
    }
}
