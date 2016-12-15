using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SOF40934218_Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                DirectHandler dh = new DirectHandler();
                object myObj = "message";
                var resp = await dh.SendToPlayer(myObj);
                Console.WriteLine("Received: " + resp);
                System.Threading.Thread.Sleep(500);
                resp = await dh.SendToPlayer(myObj);
                Console.WriteLine("Received: " + resp);

                System.Threading.Thread.Sleep(5000);
            }).Wait();
        }
    }

    public class DirectHandler
    {
        HttpClient httpClient;

        public async Task<string> SendToPlayer(object message)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://127.0.0.1:18080/");
            }

            HttpResponseMessage response = await httpClient.PostAsJsonAsync("", message).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return System.Text.Encoding.Default.GetString(result);
            }
            else
            {
                throw new HttpRequestException("Error code " + response.StatusCode + ", reason: " + response.ReasonPhrase);
            }
        }
    }
}
