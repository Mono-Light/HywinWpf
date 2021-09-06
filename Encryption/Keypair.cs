using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Encryption
{
    public class Keypair
    {
        public RSAParameters privateKey;
        public RSAParameters publicKey;

        public Keypair(RSAParameters privateKey, RSAParameters publicKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

    }
}
