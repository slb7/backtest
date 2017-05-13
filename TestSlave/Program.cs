using Backtest;
using Backtest.Infrastructure;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;


namespace TestSlave
{
    class TestDay
    {
        public string[] symbols;
        public float[] arr;
        public float[] ndArr;
        public List<IBarDay> bdlist = new List<IBarDay>();
        public Dictionary<string, IDayCandle> prevDay;
        public Dictionary<string, IDayCandle> nextDay;
        public Dictionary<string, IDayCandle> toDay;
    }
    class Program
    {
        //static List<IBarDay> bdlist = new List<IBarDay>();
        static float[][] arrs = new float[4][];
        static Tuple<int,int,int> getymd(string s)
        {
            string[] toks = s.Split('\\');
            int y = Int32.Parse(toks.Last().Substring(0, 4));
            int m = Int32.Parse(toks.Last().Substring(4, 2));
            int d = Int32.Parse(toks.Last().Substring(6, 2));
            return new Tuple<int, int, int>(y, m, d);
        }
        static Dictionary<string, TestDay> dictionary = new Dictionary<string,TestDay>();
        //static string dir = Environment.GetEnvironmentVariable("BackTestDir");
        static string daydata;
        static string minuteData;
        static string bindir;
        static string codefile;
        static void setup(int year, int month)
        {
            
            string filter = $@"{year.ToString("0000")}{month.ToString("00")}*.bin";
            string[] files = Directory.GetFiles($@"{minuteData}", filter);
            files = (from f in files where f.EndsWith(".bin") select f).ToArray();
            foreach(string file in files)
            {
                var ymd = getymd(file);
                BarDay.LoadDay(ymd.Item1, ymd.Item2, ymd.Item3);
                float[] tarr = BarDay.arr;
                BarDay.LoadDay(ymd.Item1, ymd.Item2, ymd.Item3,true);
                string date = $"{ymd.Item2.ToString("00")}/{ymd.Item3.ToString("00")}/{ymd.Item1.ToString("0000")}";
                
                List<IBarDay> bdlist = new List<IBarDay>();
                for (int i = 0; i < BarDay.symbols.Count(); i++)
                {
                    BarDay.dates = new string[] { date };
                    for (int j = 0; j < BarDay.dates.Count(); j++)
                    {
                        bdlist.Add(new BarDay(j, i));
                    }
                }
                string prev = Utility.GetTradingDay(date, -1);
                string nxt = Utility.GetTradingDay(date, 1);
                Dictionary<string,IDayCandle> prevDay = Utility.LoadDayCandles(prev, BarDay.symbols);
                Dictionary<string, IDayCandle> nxtDay = Utility.LoadDayCandles(nxt, BarDay.symbols);
                Dictionary<string, IDayCandle> today = Utility.LoadDayCandles(date, BarDay.symbols);
                dictionary[date] = new TestDay() { arr = tarr, symbols = BarDay.symbols, bdlist = bdlist,ndArr = BarDay.arr, nextDay = nxtDay, prevDay = prevDay, toDay = today };

            }

            //arrs[0] = BarDay.arr;
            //arrs[1] = BarDay.arr5;
            //arrs[2] = BarDay.arr15;
            //arrs[3] = BarDay.arr390;
        }
        static Assembly GetAssembly()
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add($@"{bindir}Backtest.Infrastructure.dll");
            Console.WriteLine($@"{bindir}Backtest.Infrastructure.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = true;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = false;
            string code = File.ReadAllText($@"{codefile}");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
            Console.WriteLine($"comple results {results.Errors.Count}");
            foreach(CompilerError e in results.Errors)
            {
                Console.WriteLine($"compiler error {e.ErrorText}");
            }
             return results.CompiledAssembly;
        }
        static void Main(string[] args)
        {
            HttpListener list = new HttpListener();
            string url = $"http://localhost:8001/{args[0]}{args[1]}/";
            Console.WriteLine($"listening at {url}");
            daydata = args[2];
            minuteData = args[3];
            bindir = args[4];
            codefile = args[5];
            BarDay.minuteData = minuteData;
            Utility.dayData = daydata;
            list.Prefixes.Add(url);
            int year = Int32.Parse(args[0]);
            int month = Int32.Parse(args[1]);
            setup(year, month);
            try
            {
                list.Start();
            }
            catch (Exception) {
                return;
            }
            Assembly a = null;
            for (int i = 0;;i++)
            {
                HttpListenerContext ctx = list.GetContext();
                //a = Assembly.LoadFrom(@"c:\src\MyTest\Mytest\bin\debug\mytest.dll");
                Type ty = null;
                try
                {
                    a = GetAssembly();
                    ty = a.GetType("MyTest.MyTest");
                } catch(Exception e)
                {
                    continue;
                }
                StreamWriter sw = new StreamWriter(ctx.Response.OutputStream);
                Test.messages.Clear();
                Test.testResults.Clear();
                Test.results.Clear();
                foreach (string date in dictionary.Keys)
                {
                    TestDay td = dictionary[date];
                    BarDay.symbols = td.symbols;
                    arrs[0] = td.arr;
                    string testdate = date;
                    Test t = (Test)Activator.CreateInstance(ty);// null;  //new MyTest();
                    BarDay.arr = td.arr;
                    if (t.testNextDay)
                    {
                        testdate = Utility.GetTradingDay(date, 1);
                        BarDay.arr = td.ndArr;
                    }
                    BarDay.dates = new string[] { testdate };
                    //ILookup<DateTime, IBarDay> bdl = bdlist.ToLookup(k => DateTime.Parse("01/02/1998"));
                    //t.Container = new TestContainer();
                    //Test.type = typeof(MyTest);
                    //Test.type = typeof(Test);
                    Test.type = ty;
                    //System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    Console.WriteLine($"running {date} {i}");
                    sw.WriteLine($"Testing version {t.GetVerion()}");
                    BarDay.bdinit();
                    Dictionary<string, Dictionary<string, IDayCandle>> prev = new Dictionary<string, Dictionary<string, IDayCandle>>();
                    prev[testdate] = td.prevDay;
                    Dictionary<string, Dictionary<string, IDayCandle>> today = new Dictionary<string, Dictionary<string, IDayCandle>>();
                    today[testdate] = td.toDay;
                    t.run(td.bdlist.ToArray(), BarDay.symbols, BarDay.dates, arrs,prev,today);
                    //Test.results.Clear();
                }
                string d = "nodate";
                foreach (string s in Test.messages)
                {
                    //Console.WriteLine(s);
                    sw.WriteLine($"Message^{d}^{s}");
                }
                foreach (string s in Test.results)
                {
                    //Console.WriteLine(s);
                    sw.WriteLine($"results^{d}^{s}");
                }
                foreach (TestResult s in Test.testResults)
                {
                    //Console.WriteLine(s.ToString());
                    sw.WriteLine($"testResults^{d}^{s}");
                }
                string dir = ctx.Request.Url.AbsolutePath;

                //StreamWriter sw = new StreamWriter(ctx.Response.OutputStream);
                sw.Write(dir);
                sw.Flush();
                ctx.Response.Close();
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
