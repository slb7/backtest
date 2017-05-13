using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTradingDays
{
    class Program
    {
        static void Main(string[] args)
        {
            IMongoClient client = new MongoClient();
            MongoDatabase testDB = client.GetDatabase("test");
            //IMongoCollection<Quote> quotecoll = testDB.GetCollection<Quote>("quotes");
            DateTime[] days = (from line in File.ReadAllLines(@"e:\dludone\ibm.txt") select DateTime.Parse(line.Split(',')[0])).Distinct().ToArray();
        }
    }
}
