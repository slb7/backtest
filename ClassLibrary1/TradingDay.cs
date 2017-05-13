using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoObjects
{
    public class TradingDay
    {
        public ObjectId _id = new ObjectId();
        public DateTime Date;
        public TradingDay(DateTime d)
        {
            Date = d;
        }
    }
}
