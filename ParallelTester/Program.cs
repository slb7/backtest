using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelTester
{
    [DataContract]
    internal class Rows
    {
        [DataMember]
        internal String [][] values;
    }
    class Program
    {
        static string dir;
        static void Main(string[] args)
        {
            dir = args[0];
            List<string> dates = new List<string>();
            for (int y = 2014; y < 2018; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    if (y == 2017 && m > 2) { }
                    else
                    {
                        dates.Add($"{y.ToString("0000")}{m.ToString("00")}");
                    }
                }
            }
            string[] ags = dates.ToArray();
            //string[] ags = new string[] { "201502", "201503", "201504", "201505", "201507", "201508", "201510", "201511" };
            List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
            foreach (string s in ags)
            {
                HttpClient cl = new HttpClient();
                tasks.Add( cl.GetAsync($"http://localhost:8001/{s}/"));
            }
            Task.WaitAll(tasks.ToArray());
            List<string> strings = new List<string>();
            foreach(var t in tasks)
            {
                string output = t.Result.Content.ReadAsStringAsync().Result;
                output = output.Replace("\r", "");
                string[] toks = output.Split('\n');
                strings.AddRange(toks);
            }
            var groups = strings.GroupBy(k => k.Split('^')[0]);

            var testResults = from s in strings where s.Split('^')[0] == "testResults" select s.Split('^').Last().Split(',');
            List<string[]> mappedResults = new List<string[]>();
            double totalpandl = 0;
            foreach(string[] tr in testResults)
            {
                totalpandl += Double.Parse(tr[2]);
                mappedResults.Add(new string[] {tr[0],tr[1],tr[2],totalpandl.ToString("0.00")});
            }
            Rows r = new Rows() { values = mappedResults.ToArray() };
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Rows));
            ser.WriteObject(stream1, r);
            stream1.Position = 0;
            string gs = new StreamReader(stream1).ReadToEnd();
            PostToGoogle(gs,testResults.Count());
            if (Directory.Exists(@"c:\googres"))
            {
                File.WriteAllText(@"c:\googres\testResults.txt", gs);
            }

            var results = from s in strings where s.Split('^')[0] == "results" select s.Split('^').Last();
            //mappedResults = new List<string[]>();
            //foreach (string[] tr in results)
            //{
            //    mappedResults.Add(new string[] { tr[0]});
            //}
            //r = new Rows() { values = mappedResults.ToArray() };
            //stream1 = new MemoryStream();
            //ser = new DataContractJsonSerializer(typeof(Rows));
            //ser.WriteObject(stream1, r);
            //stream1.Position = 0;
            //gs = new StreamReader(stream1).ReadToEnd();
            if (Directory.Exists(@"c:\googres"))
            {
                //File.WriteAllText(@"c:\googres\results.txt", gs);
                File.WriteAllLines(@"c:\googres\results.txt", results.ToArray());
            }
            //PostToGoogle(gs);
            //Console.WriteLine(gs);
        }
        static string[] writeScope = { SheetsService.Scope.Drive, SheetsService.Scope.Spreadsheets };
        static void PostToGoogle(string s ,int rowCount)
        {
            //string udpate = args[0];
            UserCredential credential = null;

            //string credPath = System.Environment.GetFolderPath(
            //    System.Environment.SpecialFolder.Personal);
            string credPath = dir;
            //credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
            string fn = $@"{dir}Google.Apis.Auth.OAuth2.Responses.TokenResponse-user";
            TimeSpan age = new TimeSpan(1, 0, 0);
            if (File.Exists(fn))
            {
                DateTime fct = File.GetLastWriteTime(fn);
                DateTime now = DateTime.Now;
                age = now - fct;
            }
            TimeSpan maxAge = new TimeSpan(0, 30, 0);
            if (age > maxAge)
            {
                File.Delete(fn);
                using (var stream =
                    new FileStream($@"{dir}client_secret.json", FileMode.Open, FileAccess.Read))
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
            Console.WriteLine(fn);
            Console.WriteLine(tok);
            HttpContent clearContent = new StringContent("{\"ranges\": [\"Sheet1!A1:Z99999\"]}");
            HttpResponseMessage clResp  = cl.PostAsync("https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values:batchClear", clearContent).Result;
            string url = $"https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values/sheet1!a1:e{rowCount + 1}?valueInputOption=USER_ENTERED";
            HttpContent content = new ByteArrayContent(ASCIIEncoding.ASCII.GetBytes(s));
            HttpResponseMessage resp = cl.PutAsync(url, content).Result;
            string msg = resp.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"{msg} >>>{resp.StatusCode}<<<");
            Thread.Sleep(5000);
        }
    }
}
