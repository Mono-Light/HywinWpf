using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messager.Api;
using Messager.Responses;

namespace Messager.Api
{
    public class User
    {
        public string username;
        public string id;
        public string id_tag;
    }

    public class LocalUser : User
    {
        public static async Task<bool> TryLogin(string username, string pw)
        {
            
            string u = Encryption.Crypto.EncryptToChunks(username, Session.ServerKey);
            string p = Encryption.Crypto.EncryptToChunks(Encryption.Crypto.SHA256Hash(pw), Session.ServerKey);

            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "username", u},
                { "password", p}
            };

            var resRaw = await WebClient.PostAsync("login", body);
            string response = Encryption.Crypto.DecryptMessage(resRaw, Encryption.Crypto.GetPrivateKey());


            LoginResponse loginData = Response.LoadFromJson<LoginResponse>(response);
            ResponseCode details = (ResponseCode)Enum.Parse(typeof(ResponseCode), loginData.details);

            switch (details)
            {
                case (ResponseCode.NO_ENCRYPTION_KEY):
                    MainWindow.ShowErrorMessage("Unkown Error. Please restart the app.");
                    return false;
                case (ResponseCode.LOGIN_SUCCESS):
                    MainWindow.ShowErrorMessage("");
                    return true;
                case (ResponseCode.INVALID_CREDENTIALS):
                    MainWindow.ShowErrorMessage("Password or username is incorrect.");
                    return false;
                case (ResponseCode.ENCRYPTION_ERROR):
                    MainWindow.ShowErrorMessage("There was an error while encrypting. Please restart the app!");
                    return false;
            }
            MainWindow.ShowErrorMessage("An unkown error occured whilst trying to login!");
            return false;
        }

        public static async Task GetEndKeys()
        {

            Encryption.Crypto enc = new Encryption.Crypto();
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "public", Encryption.Crypto.GetPublicKey() }
            };

            string resRaw = await WebClient.PostAsync("login", body);
            string serverKey = "";

            Console.WriteLine(resRaw.Length);
            foreach (string chunk in Encryption.Crypto.SplitInParts(resRaw, 344))
            {
                serverKey += enc.Decrypt(chunk);
            }
            serverKey = serverKey.Trim();

            Session.SetKeys(serverKey, Encryption.Crypto.GetKeypair());
        }

    }

}
