using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;


namespace Messager.Encryption
{
    public class Crypto
    {
        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private string _publicKey;
        private static RSAParameters privateParam;
        private static RSAParameters publicParam;
        private static string publicKey;
        private static string privateKey;
        
        public Crypto()
        {
            RSA rsa = RSA.Create(2048);
            _publicKey = rsa.ToXmlString(false);
            privateKey = rsa.ToXmlString(true);
            publicParam = rsa.ExportParameters(false);
            privateParam = rsa.ExportParameters(true);
            publicKey = ExportPublicKey(publicParam);
        }


        public static string SHA256Hash(string message)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
             }
        } 


        public static string DecryptMessage(string message, string privateKey, bool xml = true)
        {
            RSA rsa = RSA.Create(2048);
            int _;

            if (xml) rsa.FromXmlString(privateKey);
            else
            {
                var binaryEncoding = Convert.FromBase64String(string.Join(null, privateKey));
                rsa.ImportPkcs8PrivateKey(binaryEncoding, out _);
            }


            string decrypted = "";
            foreach (string chunk in SplitInParts(message, 344))
            {
                decrypted += Decrypt(chunk, rsa.ExportParameters(true));
            }

            return decrypted;
        }

        public string PublicKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _publicKey);

            return sw.ToString();
        }


        public string Encrypt(string text)
        {
            csp = new RSACryptoServiceProvider(2048);
            csp.PersistKeyInCsp = false;
            csp.FromXmlString(_publicKey);

            UnicodeEncoding converter = new UnicodeEncoding();
            byte[] data = converter.GetBytes(text);
            byte[] cypher = csp.Encrypt(data, false);

            return Convert.ToBase64String(cypher);
        }

        public static string Encrypt(string text, RSAParameters publicKey)
        {
            csp = new RSACryptoServiceProvider();
            csp.PersistKeyInCsp = false;
            csp.ImportParameters(publicKey);


            UnicodeEncoding converter = new UnicodeEncoding();
            byte[] data = converter.GetBytes(text);
            byte[] cypher = csp.Encrypt(data, false);

            return Convert.ToBase64String(cypher);
        }


        public static string EncryptToChunks(string s, string publicKey)
        {

            RSA rsa = RSA.Create(2048);
            int _;

            publicKey = publicKey.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");

            var binaryEncoding = Convert.FromBase64String(string.Join(null, publicKey));
            rsa.ImportRSAPublicKey(binaryEncoding, out _);


            string encrypted = "";
            foreach (string chunk in SplitInParts(s, 64))
            {
                encrypted += Encrypt(chunk, rsa.ExportParameters(false));
            }   
            
            return encrypted;
        }


        public static string GetPublicKey()
        {
            return publicKey;
        }

        public static string GetPrivateKey() 
        { 
            return privateKey;
        }

        public static Keypair GetKeypair()
        {
            return new Keypair(privateParam, publicParam);
        }

        public string Decrypt(string hash)
        {
            var bytes = Convert.FromBase64String(hash);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(privateParam);
            var data = rsa.Decrypt(bytes, false);
            return Encoding.Unicode.GetString(data);
        }

        private static string Decrypt(string hash, RSAParameters privateKey)
        {
            var bytes = Convert.FromBase64String(hash);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(privateKey);

            var data = rsa.Decrypt(bytes, false);
            return Encoding.Unicode.GetString(data);
        }

        public static IEnumerable<string> SplitInParts(string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }


        public static string ExportPublicKey(RSAParameters parameters)
        {
            StringWriter outputStream = new StringWriter();

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                // WriteLine terminates with \r\n, we want only \n
                outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                    outputStream.Write("\n");
                }
                outputStream.Write("-----END PUBLIC KEY-----");
            }

            return outputStream.ToString();
        }



        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="forceUnsigned"></param>
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }


    }
}
