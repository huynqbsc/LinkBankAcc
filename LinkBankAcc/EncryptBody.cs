using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkBankAcc
{
    public class EncryptBody
    {
        public void EncryptRequestBody()
        {
            // Khóa đối xứng (SymmetricKey), cần phải giữ bí mật
            string bscKey = ConfigurationSettings.AppSettings["bsc_symmetric_key"];

            byte[] syKey = Enumerable.Range(0, bscKey.Length / 2)
                                     .Select(x => Convert.ToByte(bscKey.Substring(x * 2, 2), 16))
                                     .ToArray();
            string symmetricKey = Encoding.UTF8.GetString(syKey);

            // Dữ liệu cần mã hóa (body)
            var payload = "{\"ref\":\"123456\",\"scid\":30,\"page\":1\"}";
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // Mã hóa body theo chuẩn JWT
            string jwt = JWT.Encode(jsonPayload, Encoding.UTF8.GetBytes(symmetricKey), JweAlgorithm.A256KW, JweEncryption.A128GCM);
            string[] splitjwe = jwt.Split('.');
            string protected_ = splitjwe[0];
            string encrypted_key = splitjwe[1];
            string iv = splitjwe[2];
            string ciphertext = splitjwe[3];
            string tag = splitjwe[4];

            Recipients recipient = new Recipients();
            recipient.encrypted_key = encrypted_key;
            Header _header = new Header();
            recipient.header = _header;
            Recipients[] recipients = new Recipients[1];
            recipients[0] = recipient;


            JweObj objWE = new JweObj(recipients, protected_.ToString(), ciphertext, iv, tag);
            string jweString = JsonConvert.SerializeObject(objWE);

            Console.WriteLine("encrypted_key: " + encrypted_key);
            Console.WriteLine("protected: " + protected_);
            Console.WriteLine("ciphertext: " + ciphertext);
            Console.WriteLine("iv: " + iv);
            Console.WriteLine("tag: " + tag);
            Console.WriteLine("Serialized Encrypted JWE: " + jweString);


            // Sign Cert

            // Đọc khóa riêng từ tệp
            string privateKeyPath = System.Environment.CurrentDirectory + "\\cert\\private.key";  // Đảm bảo tệp khóa của bạn nằm tại đây
            byte[] privateKeyBytes = File.ReadAllBytes(privateKeyPath);
            string privateKeyPem = File.ReadAllText(privateKeyPath).Replace("-----BEGIN PRIVATE KEY-----", "")
                                     .Replace("-----END PRIVATE KEY-----", "")
                                     .Replace("\r", "").Replace("\n", "");

            var bytesToDecrypt = Convert.FromBase64String(privateKeyPem);

            using (var reader = File.OpenText(privateKeyPath))
            {
                // file containing RSA PKCS1 privated key
                //keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();

                var pr = new PemReader(reader);
                var o = pr.ReadObject();


                var keyPair = (RsaPrivateCrtKeyParameters)o;
                var decryptEngine = new Pkcs1Encoding(new RsaEngine());
                decryptEngine.Init(false, keyPair);
                var decrypted = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length));


            }
        }
    }

    public class Recipients
    {
        [JsonProperty("header")]
        public Header header { get; set; }
        [JsonProperty("encrypted_key")]
        public string encrypted_key { get; set; }
    }

    public class JweObj
    {
        [JsonProperty("recipients")]
        public Recipients[] Recipients { get; set; }
        [JsonProperty("protected")]
        public string Protected { get; set; }
        [JsonProperty("ciphertext")]
        public string Ciphertext { get; set; }
        [JsonProperty("iv")]
        public string Iv { get; set; }
        [JsonProperty("tag")]
        public string Tag { get; set; }

        public JweObj(Recipients[] recipients, string protectedValue, string ciphertext, string iv, string tag)
        {
            Recipients = recipients;
            Protected = protectedValue;
            Ciphertext = ciphertext;
            Iv = iv;
            Tag = tag;
        }
    }

    public class Header
    {
        
    }
}
