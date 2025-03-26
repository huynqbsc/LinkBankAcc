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
        static string apiurl = ConfigurationSettings.AppSettings["apiurl"];
        static string sso_url = apiurl + ConfigurationSettings.AppSettings["sso"];
        static string allacctbal_url = apiurl + ConfigurationSettings.AppSettings["allacctbal"];

        public string GetToken()
        {
            string access_token = string.Empty;
            string clientId = ConfigurationSettings.AppSettings["client_id"];
            string clientSecret = ConfigurationSettings.AppSettings["client_secret"];
            string grant_type = "client_credentials";
            string scope = "read";

            RestClient client = new RestClient(sso_url);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"client_id={clientId}&client_secret={clientSecret}&grant_type={grant_type}&scope={scope}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.ErrorMessage);
                cmm.UPDATE_FILE_LOG("ERR", "Get Access Token", "GetToken()", response.ErrorMessage, "error", "Lien ket tai khoan");
            }
            else
            {
                var content = response.Content;
                AccessToken data = JsonConvert.DeserializeObject<AccessToken>(response.Content);
                access_token = data.access_token;
                cmm.UPDATE_FILE_LOG("LOG", "Get Access Token", "GetToken()", "Get access token success: " + access_token, "info", "Lien ket tai khoan");
                Console.WriteLine(data.ToString());
            }
            return access_token;
        }

        public void GetAllAccBal(string accesstoken, string x_client_crtificate, string x_jws_signature, string body)
        {
            string Channel = "YANGON";
            string X_Client_Certificate = x_client_crtificate;
            string X_API_Interaction_ID = "B";
            string Timestamp = DateTime.Now.ToString();
            string X_Customer_IP_Address = "A";
            string Authorization = "Bearer " + accesstoken;
            string X_JWS_Signature = x_jws_signature;
            string User_Agent = "A";

            RestClient client = new RestClient(allacctbal_url);
            RestRequest request = new RestRequest(Method.POST);

            // Header
            request.AddHeader("Channel", Channel);
            request.AddHeader("X_Client_Certificate", x_client_crtificate);
            request.AddHeader("X_API_Interaction_ID", X_API_Interaction_ID);
            request.AddHeader("Timestamp", Timestamp);
            request.AddHeader("X_Customer_IP_Address", X_Customer_IP_Address);
            request.AddHeader("Authorization", Authorization);
            request.AddHeader("X_JWS_Signature", X_JWS_Signature);
            request.AddHeader("User_Agent", User_Agent);
            request.AddHeader("content-type", "application/json");

            // Body
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(response.ErrorMessage);
                cmm.UPDATE_FILE_LOG("ERR", "Get All Account Balance", "GetAllAccBal()", response.ErrorMessage, "error", "Lien ket tai khoan");
            }
            else
            {
                var content = response.Content;
            }
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

    [JsonObject]
    public class ResponseStatus
    {
        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("errorSource")]
        public string errorSource { get; set; }

        [JsonProperty("errorCode")]
        public string errorCode { get; set; }

        [JsonProperty("errorDesc")]
        public string errorDesc { get; set; }
    }

    [JsonObject]
    public class ResponseBody
    {
        [JsonProperty("totalpage")]
        public string totalpage { get; set; }

        [JsonProperty("pagesize")]
        public string pagesize { get; set; }

        [JsonProperty("allacctbaldetail")]
        public allacctbaldetail[] allacctbaldetail { get; set; }
    }

    [JsonObject]
    public class allacctbaldetail
    {
        [JsonProperty("account")]
        public string account { get; set; }

        [JsonProperty("availbal")]
        public string availbal { get; set; }

        [JsonProperty("holdbal")]
        public string holdbal { get; set; }

        [JsonProperty("currentbal")]
        public string currentbal { get; set; }

        [JsonProperty("accountname")]
        public string accountname { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }
}
