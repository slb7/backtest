using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildBinFromMongo
{
    public class Quote
    {
        public ObjectId _id;
        public double h;
        public double o;
        public double l;
        public double c;
        public int v;
        public string s;
        public DateTime d;
        public DateTime date;
        public bool e = false;
        public double dayHigh;
        public double dayLow;
    }
    class Program
    {
        static TimeSpan first = TimeSpan.Parse("08:31").Add(new TimeSpan(6,0,0));
        static TimeSpan last = TimeSpan.Parse("16:00").Add(new TimeSpan(6, 0, 0));
        static void storeQuote(string[] symbols,Quote q, float[] arr)
        {
            int slotsPerMin = symbols.Length * 7;
            int rv = 0;
            DateTime begin = q.d.Date + first;
            int min = (int)(q.d - begin).TotalMinutes;
            int slot = (min * slotsPerMin) + Array.BinarySearch(symbols, q.s);
            if (slot >= 0 && slot < arr.Length)
            {
                arr[slot + 0] = (float)q.o;
                arr[slot + 1] = (float)q.h;
                arr[slot + 2] = (float)q.l;
                arr[slot + 3] = (float)q.c;
                arr[slot + 4] = (float)q.dayHigh;
                arr[slot + 5] = (float)q.dayLow;
                int[] ia = new int[] { q.v };
                Buffer.BlockCopy(ia, 0, arr, slot + 6, 4);
            }
            else
            {

            }
        }
        static void Main(string[] args)
        {
            IMongoClient client;
            IMongoDatabase database;
            client = new MongoClient();
            database = client.GetDatabase("Quotes");
            var coll = database.GetCollection<Quote>("EarningsQuotes");
            var qColl = coll.AsQueryable<Quote>();
            DateTime[] dates = (from d in qColl select d.date).Distinct().ToArray();
            Array.Sort(dates);
            foreach(DateTime dt in dates)
            {
                string[] symbols = (from q in qColl where q.date == dt select q.s).Distinct().ToArray();
                Array.Sort(symbols);
                float[] arr = new float[symbols.Count() * 390 * 7];
                for (int i = 0; i < arr.Length; i++) arr[i] = 0;
                var quotes = (from q in qColl where q.date == dt select q);
                foreach(var q in quotes)
                {
                    storeQuote(symbols, q, arr);
                }
                byte[] ba = new byte[arr.Length * 4];
                Buffer.BlockCopy(arr, 0, ba, 0, ba.Length);
                File.WriteAllBytes($@"e:\muearningsbin\{dt.ToString("yyyyMMdd")}.bin", ba);
                File.WriteAllLines($@"e:\muearningsbin\{dt.ToString("yyyyMMdd")}.sym", symbols);
            }
        }
    }
}
