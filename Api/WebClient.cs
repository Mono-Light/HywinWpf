using Messager.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Api
{
    public class WebClient
    {

        private static HttpClient client;
        private static string url;

        public static void Initialize(string url)
        {
            WebClient.url = url;
            client = new HttpClient();
        }

        public static async Task<string> PostAsync(string route, Dictionary<string, string> body)
        {

            HttpContent content = new FormUrlEncodedContent(body);

            var response = await client.PostAsync($"{url}/{route}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

    }
}
