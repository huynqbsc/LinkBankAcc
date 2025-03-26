using System;
using System.Collections;
using System.Collections.Generic;
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
                // Ma hoa body
                EncryptBody encryptBody = new EncryptBody();
                string JWE_Body = encryptBody.EncryptRequestBody();
                if (!string.IsNullOrEmpty(JWE_Body))
                {
                    // Ky so
                    string JWS_Signature = encryptBody.SignRequestBody(JWE_Body);
                    string X_Client_Certificate = encryptBody.CertRequestBody();

                    // Call api get all tk
                    api.GetAllAccBal(accesstoken, X_Client_Certificate, JWS_Signature, JWE_Body);
                }
                else
                {
                    // update log err
                }
            }
            else
            {
                // update log err
            }
        }
    }
}
