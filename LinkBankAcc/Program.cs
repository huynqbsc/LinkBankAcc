using Jose;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LinkBankAcc
{
    class Program
    {
        static void Main(string[] args)
        {
            Api api = new Api();
            string accesstoken = api.GetToken();
            if (!string.IsNullOrEmpty(accesstoken))
            {
                EncryptBody encryptBody = new EncryptBody();
                encryptBody.EncryptRequestBody();
            }
            
        }
    }
}
