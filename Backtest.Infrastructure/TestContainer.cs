using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtest.Infrastructure
{
    public class TestContainer
    {
        public  List<string> messages = new List<string>();
        public  bool uf;
        public  List<TestResult> testResults = new List<TestResult>();
        public  TestResult testResult;
        public  System.Type type;
        public  Dictionary<string, Test> testDictionary = new Dictionary<string, Test>();
        public Dictionary<DateTime, Dictionary<string, IBar>> dict;
        public string screenSymbol;
        public DateTime screenDate;
        public DateTime[] dbk = null;
        public  List<string> dbs = new List<string>();
        public  List<string> results = new List<string>();
        public  Type bartype;
        public List<string> resultList = new List<string>();


        public  TestResult TestDay(ILookup<DateTime, IBarDay> bdl, DateTime date)
        {
            TestResult tr = new TestResult();
            testResult = tr;
            tr.Date = date;
            for (int i = 0; i < 390; i++)
            {
                foreach (IBarDay bd in bdl[date])
                {
                    string symbol = bd.Symbol;
                    Test t;
                    if (!testDictionary.ContainsKey(symbol))
                    {
                        t = (Test)Activator.CreateInstance(type);
                        t.Container = this;
                        testDictionary[symbol] = t;
                    }
                    t = testDictionary[symbol];
                    t.runMinute(i, tr, bd);
                }
            }
            foreach (Test t in testDictionary.Values)
            {
                if (t.inPosition && t.maxOvernights > 0)
                {
                    t.inPositionDays++;
                }
            }
            return tr;
        }
        public  List<Tuple<string, DateTime>> ScreenAll(Dictionary<DateTime, Dictionary<string, IBar>> indict, Test inTest)
        {

            List<Tuple<string, DateTime>> screened = new List<Tuple<string, DateTime>>();
            dict = indict;
            foreach (DateTime dt in dict.Keys)
            {
                foreach (string sym in dict[dt].Keys)
                {
                    screenSymbol = sym;
                    screenDate = dt;
                    uf = false;
                    if (inTest.screen())
                    {
                        if (!uf)
                        {
                            screened.Add(new Tuple<string, DateTime>(sym, dt));
                        }
                    }
                }
            }
            return screened;
        }

    }
}
