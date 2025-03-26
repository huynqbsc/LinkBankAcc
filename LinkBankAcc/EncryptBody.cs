using Jose;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LinkBankAcc
{
    public class EncryptBody
    {
        Common cmm = new Common();
        public string EncryptRequestBody()
        {
            string jweString = string.Empty;
            // Khóa đối xứng (SymmetricKey), cần phải giữ bí mật
            string bscKey = ConfigurationSettings.AppSettings["bsc_symmetric_key"];

            byte[] syKey = Enumerable.Range(0, bscKey.Length / 2)
                                     .Select(x => Convert.ToByte(bscKey.Substring(x * 2, 2), 16))
                                     .ToArray();
            string symmetricKey = Encoding.UTF8.GetString(syKey);

            // Dữ liệu cần mã hóa (body)
            var payload = "{\"ref\":\"123456\",\"scid\":30,\"page\":1\"}";
            string jsonPayload = JsonConvert.SerializeObject(payload);

            JweObj objWE = new JweObj();
            try
            {
                // Mã hóa body theo chuẩn JWT
                string jwt = JWT.Encode(jsonPayload, Encoding.UTF8.GetBytes(symmetricKey), JweAlgorithm.A256KW, JweEncryption.A128GCM);
                if (!string.IsNullOrEmpty(jwt))
                {
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

                    objWE.Recipients = recipients;
                    objWE.Protected = protected_.ToString();
                    objWE.Ciphertext = ciphertext;
                    objWE.Iv = iv;
                    objWE.Tag = tag;

                    jweString = JsonConvert.SerializeObject(objWE);

                    Console.WriteLine("Serialized Encrypted JWE: " + jweString);
                }
            }
            catch (Exception ex)
            {
                cmm.UPDATE_FILE_LOG("ERR", "JWT Encode", "EncryptRequestBody()", ex.Message, "error", "Lien ket tai khoan");
            }

            return jweString;
        }

        public string CertRequestBody()
        {
            string certPath = Environment.CurrentDirectory + "\\cert\\certificate.crt";
            string certPem = File.ReadAllText(certPath).Replace("-----BEGIN CERTIFICATE-----", "").Replace("-----END CERTIFICATE-----", "").Replace("\n", "");
            return certPem;
        }

        public string SignRequestBody(string encrypyBody)
        {
            // Sign Cert
            // Đọc khóa riêng từ tệp
            // Đảm bảo tệp khóa nằm tại đây
            string privateKeyPath = Environment.CurrentDirectory + "\\cert\\private.key";
            string privateKeyPEM = File.ReadAllText(privateKeyPath);
            var rsa = RsaKeyAsPerContent(privateKeyPEM);
            //byte[] signature = rsa.SignHash(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(encrypyBody);
            byte[] signature = rsa.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }

        private static RSA RsaKeyAsPerContent(string privatekey)
        {
            RSA rSA = RSA.Create();
            rSA.ImportParameters(ImportPrivateKey(privatekey));
            return rSA;
        }

        public static RSAParameters ImportPrivateKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            RsaPrivateCrtKeyParameters privKey = (RsaPrivateCrtKeyParameters)pr.ReadObject();
            RSAParameters rp = new RSAParameters();
            rp.Modulus = privKey.Modulus.ToByteArrayUnsigned();
            rp.Exponent = privKey.PublicExponent.ToByteArrayUnsigned();
            rp.P = privKey.P.ToByteArrayUnsigned();
            rp.Q = privKey.Q.ToByteArrayUnsigned();
            rp.D = ConvertRSAParametersField(privKey.Exponent, rp.Modulus.Length);
            rp.DP = ConvertRSAParametersField(privKey.DP, rp.P.Length);
            rp.DQ = ConvertRSAParametersField(privKey.DQ, rp.Q.Length);
            rp.InverseQ = ConvertRSAParametersField(privKey.QInv, rp.Q.Length);

            return rp;
        }

        private static byte[] ConvertRSAParametersField(BigInteger n, int size)
        {
            byte[] bs = n.ToByteArrayUnsigned();
            if (bs.Length == size)
                return bs;
            if (bs.Length > size)
                throw new ArgumentException("Specified size too small", "size");
            byte[] padded = new byte[size];
            Array.Copy(bs, 0, padded, size - bs.Length, bs.Length);
            return padded;
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
    }

    public class Header
    {

    }
}
