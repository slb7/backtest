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
        }
    }
}
