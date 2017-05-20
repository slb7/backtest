using MongoDB.Driver;
using MongoObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadNextDay
{
        class Program
    {
        static void Main(string[] args)
        {
            IMongoClient client = new MongoClient();
            IMongoDatabase db = client.GetDatabase("Quotes");
            IMongoCollection<Earning> earningsCollection = db.GetCollection<Earning>("Earnings");
            IMongoCollection<TradingDay> tradingDayCollection = db.GetCollection<TradingDay>("TradingDays");
            DateTime[] tradingDays = (from td in tradingDayCollection.AsQueryable<TradingDay>() select td.Date).ToArray();
            tradingDays = (from td in tradingDays select td.Date).ToArray();
            Array.Sort(tradingDays);
            Earning[] earningsDays = (from ed in earningsCollection.AsQueryable<Earning>() select ed).ToArray();
            List<Earning> nextDay = new List<Earning>();
            foreach(Earning e in earningsDays)
            {
                DateTime date = e.date.Date;
                int dateInd = Array.BinarySearch(tradingDays, date);
                DateTime nd = tradingDays[dateInd + 1];
                Earning ear = new Earning(e.symbol, nd);
                nextDay.Add(ear); //lend me
            }
            IMongoCollection<Earning> DayAfterEarningcoll = db.GetCollection<Earning>("DayAfterEarnings");
            DayAfterEarningcoll.InsertMany(nextDay);
            IMongoCollection<Quote> ndQuotes = db.GetCollection<Quote>("DayAfterEarningsQuotes");

            var earns = (from e in DayAfterEarningcoll.AsQueryable<Earning>() select e).ToArray();
            IMongoDatabase testDB = client.GetDatabase("test");
            IMongoCollection<Quote> quotecoll = testDB.GetCollection<Quote>("quotes");
            int total = 0;
            var gearns = earns.GroupBy(g => g.date.Date);
            foreach (var g in gearns)
            {

                DateTime s = g.Key.Date.AddHours(9.5 + 5);
                //Console.WriteLine(s.Ticks);
                DateTime en = g.Key.Date.AddHours(16 + 5);
                var syms = from e in g select e.symbol;
                var quotes = (from q in quotecoll.AsQueryable<Quote>() where q.d >= s && q.d <= en && syms.Contains(q.s) select q).ToArray();
                foreach (var q in quotes)
                {
                    q.e = true;
                    q.date = q.d.Date;
                }

                //eqCollection.InsertMany(quotes);
                //Console.WriteLine(quotes.First().d.Ticks);
                var gquotes = quotes.GroupBy(q => q.s);
                foreach (var q in gquotes)
                {
                    var vals = q.ToArray();
                    var dquotes = vals.GroupBy(v => v.date);
                    foreach (var itemGroup in dquotes)
                    {
                        foreach (var item in itemGroup)
                        {
                            item.dayHigh = (from items in itemGroup where items.d <= item.d select items.h).Max();
                            item.dayLow = (from items in itemGroup where items.d <= item.d select items.h).Min();
                        }
                    }
                }
                ndQuotes.InsertMany(quotes);
                foreach (var q in gquotes)
                {
                    int count = q.Count();
                    total += count;
                    Console.WriteLine($"{g.Key.Date.ToString("MM-dd-yyyy")} {q.Key}  {count}");
                }
            }
            Console.WriteLine($"average candles per day = {total / earns.Count()}");
        }
    }
}
