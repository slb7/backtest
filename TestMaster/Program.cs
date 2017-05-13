using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;

namespace TestMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            //string dir = Environment.GetEnvironmentVariable("BackTestDir");
            //string dir = ConfigurationManager.AppSettings["datadirectory"];
            //string bindir = ConfigurationManager.AppSettings["bindirectory"];
            string bindir = args[0];
        //e:\backtest\testslave\bin\release\ 
        //e:\daysbydate\ 
        //e:\uearningsbin\ 
        //E:\Backtest\Backtest.Infrastructure\bin\Release\ 
        //e:\Mytest.cs
            //string datadir = args[1];
            List<string> dates = new List<string>();
            for(int y = 2014;y < 2018;y++)
            {
                for(int m = 1;m < 13;m++)
                {
                    if (y == 2017 && m > 2) { }
                    else
                    {
                        dates.Add($"{y.ToString("0000")} {m.ToString("00")}");
                    }
                }
            }
            string[] ags = dates.ToArray();
            List<Process> processes = new List<Process>();
            foreach (string s in ags)
            {
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.FileName = $@"{bindir}testslave";
                ps.Arguments = $"{s} {args[1]} {args[2]} {args[3]} {args[4]}";
                Console.WriteLine(ps.Arguments);
                ps.WindowStyle = ProcessWindowStyle.Minimized;
                //processes.Add(Process.Start($@"{dir}BackTest\TestSlave\bin\Release\testslave",s));
                processes.Add(Process.Start(ps));
            }
            Console.ReadLine();
            foreach(Process p in processes)
            {
                p.Kill();
            }
        }
    }
}
