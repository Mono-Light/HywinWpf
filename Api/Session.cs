using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Api
{
    public class Session
    {
        public static Encryption.Keypair ClientKeys { get; private set;  }
        public static string ServerKey { get; private set;  }

        public static void SetKeys(string serverKey, Messager.Encryption.Keypair clientKeys)
        {
            ServerKey = serverKey;
            ClientKeys = clientKeys;
        }

    }
}
