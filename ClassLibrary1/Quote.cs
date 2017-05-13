using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoObjects
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
}
