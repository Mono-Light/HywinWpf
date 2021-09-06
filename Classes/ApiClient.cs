using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Classes
{
    public class ApiClient
    {
        public static ApiClient CurrentClient { private set; get; }

        private HttpClient client;

        public void UseClient(ApiClient client)
        {
            CurrentClient = client;
        }
       
        public ApiClient(bool generateKeys = true)
        {
            client = new HttpClient();
            if (generateKeys)
            {
                Encryption enc = new Encryption();
                
            }
        }

        public async void RetrievePublicKey()
        {
            string key = await _RetrivePublicKey();
        }

        public async Task<string> _RetrivePublicKey()
        {
            var values = new Dictionary<string, string>
            {
                // { "username", username },
                // { "password", password }
                { "public", Encryption.GetPublicKey() }
            };

            HttpContent content = new FormUrlEncodedContent(values);

            HttpClient client = new HttpClient();
            var response = await client.PostAsync("http://localhost:3500/login", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // LoginResponse res = LoginResponse.FromJson(responseString);
            return responseString;
        }

    }
}
