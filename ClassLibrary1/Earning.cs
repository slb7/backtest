using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoObjects
{
    public class Earning
    {
        public string symbol;
        public DateTime date;
        public double openingPrint;
        public ObjectId _id;
        public Earning(string s)
        {
            string[] toks = s.Split('\"');
            string ss = s;
            if (toks.Length > 1)
            {
                ss = $"{toks[0]}{toks[1].Replace(",", "")}{toks[2]}";
            }
            toks = ss.Split(',');
            symbol = toks[1];
            date = DateTime.Parse(toks[2].Split(' ')[0] + " 09:30");
            openingPrint = double.Parse(toks[5].Replace("$", ""));
        }
        public Earning(string symbol, DateTime date)
        {
            this.symbol = symbol;
            this.date = date;
            _id = new ObjectId();
            openingPrint = 0;
        }
        public static bool isEarning(string s)
        {
            string[] toks = s.Split(',');
            bool rv = false;
            if (toks.Length > 3 && toks[3][0] == 'B') rv = true;
            return rv;
        }
    }

}
