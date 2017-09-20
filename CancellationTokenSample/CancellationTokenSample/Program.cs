using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenSample
{
    class Program
    {
        // http://www.jaylee.org/post/2013/04/16/Cancellation-with-Async-Fsharp-Csharp-and-the-Reactive-Extensions.aspx
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Task.Run(async () => await Test(cts.Token), cts.Token);
            Console.ReadLine();
            cts.Cancel();
            Console.WriteLine("Cancel...");
            Console.ReadLine();
        }

        private static async Task Test(CancellationToken token)
        {
            var url = "https://1.2.3.4";
            var wr = HttpWebRequest.Create(url);
            Console.WriteLine($"Try connect {url}");

            token.Register(() =>
            {
                Console.WriteLine("Query cancelled");
                wr.Abort();
            });

            var r = await Task<WebResponse>.Factory.FromAsync(wr.BeginGetResponse, wr.EndGetResponse, null);

            if (token.IsCancellationRequested)
            {
                return;
            }

            Console.WriteLine("Got a result");
        }
    }
}