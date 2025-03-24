using RestSharp;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace LinkBankAcc
{
    public class Api
    {
        Common cmm = new Common();
        public string GetToken()
        {
            string access_token = string.Empty;
            string sso = ConfigurationSettings.AppSettings["apiurl"] + "openapi/oauth2/token";
            string clientId = ConfigurationSettings.AppSettings["client_id"];
            string clientSecret = ConfigurationSettings.AppSettings["client_secret"];
            string grant_type = "client_credentials";
            string scope = "read";

            RestClient client = new RestClient(sso);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"client_id={clientId}&client_secret={clientSecret}&grant_type={grant_type}&scope={scope}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.ErrorMessage);
                cmm.Log_event("","");
            }
            else
            {
                var content = response.Content;
                AccessToken data = JsonConvert.DeserializeObject<AccessToken>(response.Content);
                access_token = data.access_token;
                Console.WriteLine(data.ToString());
            }
            return access_token;
        }

        public void GetAllAccBal(string accesstoken)
        {
            string url = ConfigurationSettings.AppSettings["apiurl"] + "open-banking/secu-app/v1/allacctbal";

            // Header
            string Channel = "YANGON";
            string X_Client_Certificate = "MIIDxjCCAq6gAwIBAgIEMWgLVzANBgkqhkiG9w0BAQsFADB8MQswCQYDVQQGEwJWTjETMBEGA1UEAwwKQ0VSVF9IT0FQVjELMAkGA1UECAwCSE4xDDAKBgNVBAcMA0hCVDENMAsGA1UECgwEQklEVjEPMA0GA1UECwwGQklEVlZOMR0wGwYJKoZIhvcNAQkBFg5ob2FwdkB0ZXN0LmNvbTAeFw0yNDEyMTAxMDA2MjBaFw0yNjEyMTAxMDA2MjBaMHwxCzAJBgNVBAYTAlZOMRMwEQYDVQQDDApDRVJUX0hPQVBWMQswCQYDVQQIDAJITjEMMAoGA1UEBwwDSEJUMQ0wCwYDVQQKDARCSURWMQ8wDQYDVQQLDAZCSURWVk4xHTAbBgkqhkiG9w0BCQEWDmhvYXB2QHRlc3QuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5OAsz";
            string X_API_Interaction_ID = "B";
            string Timestamp = System.DateTime.Now.ToString();
            string X_Customer_IP_Address = "A";
            string Authorization = "Bearer " + accesstoken;
            string X_JWS_Signature = "eyJhbGciOiJSUzI1NiJ9..DDmlDX2AFWEN8l6skHfL8oPRlVU3VB8rgCdqKch_c1QY4c8IRgJGAOLq7X-IoFmm8xSXnL0UZ68QU00PrurZQ5Fee99Ugefyo3EVAw5ZRKdIuvS45jJZsP3z_o_-0kd_rRUoaDvydU7Rw7d0Ae3EQ6CxhjbQ_Iy_UiBhekeoEdPEmcvySOWJdqoQJhZhcJqHfi11mEku1pTtn1nNHBPf7gWEqKyZoEwC-qgMHFaiXzfft5TcKtTDSVyAOexfvvaxXT38Wbnx34LUGPRSwuO1JWXLwlV3sqE5WhKb-X2NcMfFWPRIXzWRIR3ZIICFqy6F1li_njgFjXMGiM2uQxVVUg";
            string User_Agent = "A";
        }
    }

    [JsonObject]
    public class AccessToken
    {
        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("access_token")]
        public string access_token { get; set; }

        [JsonProperty("scope")]
        public string scope { get; set; }

        [JsonProperty("expires_in")]
        public string expires_in { get; set; }

        [JsonProperty("consented_on")]
        public string consented_on { get; set; }
    }
}
