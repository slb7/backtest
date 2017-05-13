using MongoDB.Bson;
using MongoDB.Driver;
using MongoObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTradingDates
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime[] days = (from line in File.ReadAllLines(@"e:\dludone\ibm.txt") select DateTime.Parse(line.Split(',')[0])).Distinct().ToArray();
            TradingDay[] td = (from d in days select new TradingDay(d)).ToArray();
            IMongoClient cl = new MongoClient();
            IMongoDatabase db = cl.GetDatabase("Quotes");
            IMongoCollection<TradingDay> tdcoll = db.GetCollection<TradingDay>("TradingDays");
            //IMongoCollection < TradingDay > coll = new MongoClient().GetDatabase("quotes").GetCollection<TradingDay>("TradingDates");
            tdcoll.InsertMany(td);
        }
    }
}
