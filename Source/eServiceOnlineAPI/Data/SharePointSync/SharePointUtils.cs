using System;

using System.Web;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using System.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Identity.Client;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;


namespace eServiceOnline.WebAPI.Data.SharePointSync
{
    public class SharePointUtils
    {

        static string _SharePointAccessToken = "";
        static DateTime _SharePointAccessTokenTs = DateTime.Now;
        static double _RefreshPeriodHours = 0.025;

        static public string GetToken(string siteUrl)
        {
            if (TokenNeedsRefreshing())
            {
                GetAccessTokenAsync(new Uri(siteUrl).GetLeftPart(UriPartial.Authority)).Wait();
                _SharePointAccessTokenTs = DateTime.Now;
            }

            return _SharePointAccessToken;
        }

        static public bool TokenNeedsRefreshing()
        {
            TimeSpan diff = DateTime.Now - _SharePointAccessTokenTs;
            double hours = diff.TotalHours;

            return (String.IsNullOrEmpty(_SharePointAccessToken) || hours > _RefreshPeriodHours);
        }

        static async Task<string> GetAccessTokenAsync(string tokenEndpoint)
        {
            var clientId = "880bc45f-cc28-4a4a-b24c-117eabf6f74b";
            var tenantId = "add47526-280c-414e-9031-8f2ccf9d1850";

            using var certificate = GetAppOnlyCertificate("1C5623E239CDF35E9605333085D7F8916374280A");

            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithCertificate(certificate)
                .Build();

            var token = await confidentialClient
                .AcquireTokenForClient(new[] { $"{tokenEndpoint.TrimEnd('/')}/.default" })
                .ExecuteAsync();

            _SharePointAccessToken = token.AccessToken;
            return token.AccessToken;
        }

        private static X509Certificate2 GetAppOnlyCertificate(string thumbPrint)
        {
            X509Certificate2 appOnlyCertificate = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                certStore.Close();
                return appOnlyCertificate;
            }
        }

        public static string GetConnectionString(string pServerName, string pInitialCatalog)
        {
            string connectionString = "";

            switch (pServerName.ToLower())
            {
                case @"sanjel27\dw":
                    connectionString = ConfigurationManager.ConnectionStrings["DataWarehouse"].ConnectionString;
                    break;
                case @"sanjel25\app":
                    connectionString = ConfigurationManager.ConnectionStrings["SanjelData"].ConnectionString;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = pInitialCatalog };
                connectionString = conn.ConnectionString;
            }

            return connectionString;
        }
    }

    /*
        public class SharePointUtils
        {
            static string _SharePointAccessToken = "";

            static public string GetToken(string siteUrl)
            {
                if (String.IsNullOrEmpty(_SharePointAccessToken))
                {
                    //InitializeToken().Wait();
                    //GetAccessTokenAsync().Wait();
                    GetAccessTokenAsync(new Uri(siteUrl).GetLeftPart(UriPartial.Authority)).Wait();
                }

                return _SharePointAccessToken;
            }
            static public string GetToken()
            {
                if (String.IsNullOrEmpty(_SharePointAccessToken))
                {
                    InitializeToken().Wait();
                    //GetAccessTokenAsync().Wait();
                    //GetAccessTokenAsync(new Uri(siteUrl).GetLeftPart(UriPartial.Authority)).Wait();
                }

                return _SharePointAccessToken;
            }
            static async Task InitializeToken()
            {
                //string spUser = @"powerAppsAdmin@sanjel.com";
                //string spPwd = "Suz09312";
                string spUser = @"AppNotification@sanjel.com";
                string spPwd = "Sum25389";
                string spUrl = "https://1961531albertaltd.sharepoint.com";
                string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/token";
                var clientId = "880bc45f-cc28-4a4a-b24c-117eabf6f74b"; //Application (client) ID
                var httpClient = new HttpClient();

                SecureString securePassword = new SecureString();
                foreach (char c in spPwd) securePassword.AppendChar(c);

                var pwd = new System.Net.NetworkCredential(string.Empty, securePassword).Password;

                var body = $"resource={spUrl}&client_id={clientId}&grant_type=password&username={HttpUtility.UrlEncode(spUser)}&password={HttpUtility.UrlEncode(pwd)}";
                using (var stringContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"))
                {
                    var result = await httpClient.PostAsync(tokenEndpoint, stringContent).ContinueWith((response) =>
                    {
                        return response.Result.Content.ReadAsStringAsync().Result;
                    });

                    var tokenResult = JsonSerializer.Deserialize<JsonElement>(result);
                    _SharePointAccessToken = tokenResult.GetProperty("access_token").GetString();
                }
            }
            static async Task<string> GetAccessTokenAsync(string tokenEndpoint)
            {
                var clientId = "880bc45f-cc28-4a4a-b24c-117eabf6f74b";
                var tenantId = "add47526-280c-414e-9031-8f2ccf9d1850";
                //string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/token";

                //using var certificate = GetCertificate(Path.Combine(Environment.CurrentDirectory, "MyAppCertificate.pfx"), "3FWuFdZ^Nl1k");
                using var certificate = GetAppOnlyCertificate("1C5623E239CDF35E9605333085D7F8916374280A");

                var confidentialClient = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithTenantId(tenantId)
                    .WithCertificate(certificate)
                    .Build();

                var token = await confidentialClient
                    .AcquireTokenForClient(new[] { $"{tokenEndpoint.TrimEnd('/')}/.default" })
                    .ExecuteAsync();

                _SharePointAccessToken = token.AccessToken;
                return token.AccessToken;
            }

            private static X509Certificate2 GetCertificate(string path, string password)
            {
                return new X509Certificate2(path, password, X509KeyStorageFlags.MachineKeySet);
            }

            private static X509Certificate2 GetAppOnlyCertificate(string thumbPrint)
            {
                X509Certificate2 appOnlyCertificate = null;
                using (X509Store certStore = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine))
                {
                    certStore.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                    if (certCollection.Count > 0)
                    {
                        appOnlyCertificate = certCollection[0];
                    }
                    certStore.Close();
                    return appOnlyCertificate;
                }
            }
        }
    */
}
