using Backtest;
using Backtest.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //BarDay.LoadYear(2016);
            BarDay.LoadDay(2015,11,13);
            float[][] arrs = new float[4][];
            arrs[0] = BarDay.arr;
            arrs[1] = BarDay.arr5;
            arrs[2] = BarDay.arr15;
            arrs[3] = BarDay.arr390;
            BarDay.dates = new string[] { "11/13/2015" };
            List<IBarDay> bdlist = new List<IBarDay>();

            for(int i = 0;i<BarDay.symbols.Count();i++)
            {
                for (int j = 0; j < BarDay.dates.Count();j++) {
                    bdlist.Add(new BarDay(j, i));
                }
            }
            Test t = new MyTest.MyTest();
            //ILookup<DateTime, IBarDay> bdl = bdlist.ToLookup(k => DateTime.Parse("01/02/1998"));
            //t.Container = new TestContainer();
            Test.type = typeof(MyTest.MyTest);
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            t.run(bdlist.ToArray(), BarDay.symbols, BarDay.dates, arrs, null,null);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            List<string> testr = new List<string>();
            foreach(TestResult tr in Test.testResults)
            {
                testr.Add(tr.ToString());
            }
            File.WriteAllLines(@"c:\src\r.txt", testr);
            File.WriteAllLines(@"c:\src\m.txt", Test.results);

            //tc.type = typeof(MyTest);
            //TestResult tr = tc.TestDay(bdl, DateTime.Parse("01/02/1998"));
            PostToGoogle(new string[] { "a b c d e f g h i j" });
        }

        static string[] writeScope = { SheetsService.Scope.Drive, SheetsService.Scope.Spreadsheets };
        static void PostToGoogle(string[] args)
        {
            string udpate = args[0];
            UserCredential credential = null;

            string credPath = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Personal);
            credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
            string fn = @"C:\Users\scott\Documents\.credentials\sheets.googleapis.com-dotnet-quickstart.json\Google.Apis.Auth.OAuth2.Responses.TokenResponse-user";
            if (DateTime.Now - File.GetCreationTime(fn) > new TimeSpan(0, 30, 0))
            {
                File.Delete(fn);
                using (var stream =
                    new FileStream(@"c:\src\client_secret.json", FileMode.Open, FileAccess.Read))
                {

                    //ClientSecrets secrets = GoogleClientSecrets.Load(stream).Secrets;
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        writeScope,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }
            }
            string tf = File.ReadAllText(fn);
            string tok = tf.Split(',')[0].Split(':')[1].Split('"')[1];
            HttpClient cl = new HttpClient();
            cl.DefaultRequestHeaders.Add("Authorization", $"Bearer {tok}");
            //string url = "https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values/sheet1!a1:a2?valueInputOption=USER_ENTERED";
            string url = "https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values/sheet1!a1:e255?valueInputOption=USER_ENTERED";
            string updatereq = File.ReadAllText(@"c:\src\googleUpdate.txt");
            StringBuilder sb = new StringBuilder();
            string[] lines = File.ReadAllLines(@"c:\src\m.txt");
            sb.Append("{\"values\": [");
            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                string[] toks = l.Split(',');
                //if(toks.Length == 1) sb.Append($"[\"{toks[0]}\"],");
                if (i < lines.Length - 1)
                {
                    if (toks.Length == 1) sb.Append($"[\"{toks[0]}\"],");
                    else sb.Append($"[\"{toks[0]}\",{toks[1]},{toks[2]},{toks[3]},{toks[4]}],");
                }
                else
                {
                    if (toks.Length == 1) sb.Append($"[\"{toks[0]}\"]");
                    else sb.Append($"[\"{toks[0]}\",{toks[1]},{toks[2]},{toks[3]},{toks[4]}]");
                }
            }
            sb.Append("]}");
            File.WriteAllText(@"c:\src\res.txt", sb.ToString());

            HttpContent content = new ByteArrayContent(ASCIIEncoding.ASCII.GetBytes(sb.ToString()));
            HttpResponseMessage resp = cl.PutAsync(url, content).Result;
        }
    }
}
