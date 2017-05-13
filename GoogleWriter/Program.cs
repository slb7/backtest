using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleWriter
{
    class Program
    {
        static string[] writeScope = { SheetsService.Scope.Drive, SheetsService.Scope.Spreadsheets };
        static string dir = Environment.GetEnvironmentVariable("BackTestDir");
        static void PostToGoogle(string s)
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
            HttpContent clearContent = new StringContent("{\"ranges\": [\"Sheet2!A1:Z99999\"]}");
            HttpResponseMessage clResp = cl.PostAsync("https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values:batchClear", clearContent).Result;
            string url = "https://sheets.googleapis.com/v4/spreadsheets/14CoUxe6E12rGMzn6pic7ZmFPRZr1Tqvo7voSOhk7MDg/values/sheet2!a1:e255?valueInputOption=USER_ENTERED";
            HttpContent content = new ByteArrayContent(ASCIIEncoding.ASCII.GetBytes(s));
            //File.WriteAllText(@"c:\src\foo.json", s);
            HttpResponseMessage resp = cl.PutAsync(url, content).Result;
        }
        static void Main(string[] args)
        {

            string udpate = File.ReadAllText(@"c:\googres\foo.txt");
            PostToGoogle(udpate);
            return;
        }
    }
}
