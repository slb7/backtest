using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

namespace dayHighTroubleshooter
{
    public class Quote
    {
        public string s;
        public ObjectId _id;
        public DateTime d;
        public double o;
        public double h;
        public double l;
        public double c;
        public double dh;
        public double dl;
        public int v;
    }
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = File.ReadAllBytes(@"e:\uearningsbin\20161031.bin");
            string[] symbols = File.ReadAllLines(@"e:\uearningsbin\20161031.sym");
            int symNo = Array.BinarySearch(symbols, "SB");
            float[] arr = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
            double[][] SB = new double[390][];
            int dpb = 7;
            int dpm = dpb * symbols.Length;
            for (int i = 0; i < 390; i++)
            {
                SB[i] = new double[7];
                int offset = (i * dpm) + (symNo * dpb);
                for (int j = 0; j < dpb; j++)
                {
                    SB[i][j] = arr[offset + j];
                }
            }
            IMongoClient client;
            IMongoDatabase database;

            client = new MongoClient();
            database = client.GetDatabase("test");
            var collection = database.GetCollection<Quote>("quotes");
            DateTime start = DateTime.Parse("2016/10/31 09:31:00");
            DateTime end = DateTime.Parse("2016/10/31 16:00:00");
            string[] syms = (from q in collection.AsQueryable<Quote>() select q.s).Distinct().ToArray();

            var foo = (from q in collection.AsQueryable<Quote>() where q.s == "SB" && (q.d >= start && q.d <= end) select q).ToArray();
            foreach(var f in foo)
            {
                double dayHigh = (from b in foo where b.d <= f.d select b.h).Max();
                double dayLow = (from b in foo where b.d <= f.d select b.l).Min();
                f.dh = dayHigh;
                f.dl = dayLow;
            }

            foreach(var f in foo)
            {
                DateTime dt = f.d.ToLocalTime();
                Console.WriteLine($"{dt.ToString("MM/dd/yyyy,HH:mm")},{f.o},{f.h},{f.l},{f.c},{f.v},{f.dh},{f.dl}");
            }

            for (int i = 0; i < 390; i++)
            {
                DateTime dt = DateTime.Parse("10/31/2016 09:30:00").AddMinutes(i);
                if (SB[i][2] != 0)
                {
                    Console.WriteLine($"{dt.ToString("MM/dd/yyyy,HH:mm")},{SB[i][0].ToString("0.000")},{SB[i][1].ToString("0.000")},{SB[i][2].ToString("0.000")},{SB[i][3].ToString("0.000")},{SB[i][4].ToString("0.000")},{SB[i][5].ToString("0.000")}");
                }
            }
        }
    }
}
