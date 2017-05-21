using MongoDB.Bson;
using MongoDB.Driver;
using MongoObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            IMongoClient client;
            IMongoDatabase database;
            string[] strings = File.ReadAllLines(@"c:\users\scott\Downloads\er_nyse.csv");
            string[] strings1 = File.ReadAllLines(@"c:\users\scott\Downloads\er_nsdq.csv");
            List<Earning> earnings = (from e in strings where Earning.isEarning(e) select new Earning(e)).ToList();
            earnings.AddRange(from e in strings1 where Earning.isEarning(e) select new Earning(e));
            client = new MongoClient();
            database = client.GetDatabase("Quotes");
            var earningsCollection = database.GetCollection<Earning>("Earnings");
            var eqCollection = database.GetCollection<Quote>("EarningsQuotes");
            //earningsCollection.DeleteMany(Builders<Earning>.Filter.Empty);
            //earningsCollection.InsertMany(earnings);
            //var symbolCollection = database.GetCollection<Symbol>("Symbols");
            //symbolCollection.DeleteMany(Builders<Symbol>.Filter.Empty);
            //string[] files = Directory.GetFiles(@"e:\dludone\", "*.txt");
            //files = (from f in files select f.Split('\\').Last()).ToArray();
            //files = (from f in files select f.Substring(0, f.Length - 4)).ToArray();
            //Symbol[] symbols = (from f in files select new Symbol() { _id = f }).ToArray();
            //symbolCollection.InsertMany(symbols);
            var earns = (from e in earningsCollection.AsQueryable<Earning>() select e).ToArray();
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
                var quotes = (from q in quotecoll.AsQueryable<Quote>() where q.d >= s && q.d <= en && syms.Contains(q.s) select q ).ToArray();
                foreach(var q in quotes)
                {
                    q.e = true;
                    q.date = q.d.Date;
                }

                //eqCollection.InsertMany(quotes);
                //Console.WriteLine(quotes.First().d.Ticks);
                var gquotes = quotes.GroupBy(q => q.s);
                foreach(var q in gquotes)
                {
                    var vals = q.ToArray();
                    var dquotes = vals.GroupBy(v=>v.date);
                    foreach(var itemGroup in dquotes)
                    {
                        foreach(var item in itemGroup)
                        {
                            item.dayHigh = (from items in itemGroup where items.d <= item.d select items.h).Max();
                            item.dayLow = (from items in itemGroup where items.d <= item.d select items.l).Min();
                        }
                    }
                }
                eqCollection.InsertMany(quotes);
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
