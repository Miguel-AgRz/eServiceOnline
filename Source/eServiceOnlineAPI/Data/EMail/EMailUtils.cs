using System;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

using Microsoft.Graph;
//using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
//using Azure.Identity;
using System.Net.Http;

using Microsoft.Exchange.WebServices.Data;

using MetaShare.Common.Foundation.Logging;

namespace eServiceOnline.WebAPI.Data.EMail
{
    public static class EMailUtils
    {
        static string _From = "AppNotification@sanjel.com";
        static string signatureImagePath = @"\\Sanjel04\Application\Images\SanjelLogo_EmailSignature_Complete.jpg";
        static string signatureImageID = "SanjelLogo";
        //static string defaultJsonFileName = @"EmaiContentJson.txt";


        static public async Task<GraphServiceClient> SendGraphEmailWithAttachments(string jsonFileFullPath)
        {
            GraphServiceClient graphClient = await GetGraphClient();

            try
            {
                EmailMsgExtended msg = new EmailMsgExtended(jsonFileFullPath);  // JsonConvert.DeserializeObject<EmailMsg>(jsonText);

                try
                {
                    Message message = msg.GetGraphMessage();

                    DirectoryInfo d = new DirectoryInfo(msg.ContentPath);
                    FileInfo[] Files = d.GetFiles("*.*"); //Getting Text files

                    if (Files.GetLength(0) > 1)
                    {
                        //first create an email draft
                        var msgResult = await graphClient.Users[_From].Messages
                                                .Request()
                                                .AddAsync(message);

                        int maxFileSize = (1024 * 1024 * 3);

                        foreach (FileInfo file in Files)
                        {
                            if (file.Name != msg.JsonFileName)
                            {
                                FileInfo f = new FileInfo(file.FullName);
                                if (f.Length < maxFileSize) // 3 Mo
                                {
                                    string mimeType = MimeTypes.MimeTypeMap.GetMimeType(file.Extension);
                                    Microsoft.Graph.FileAttachment fileAttachment = new Microsoft.Graph.FileAttachment
                                    {
                                        ODataType = "#microsoft.graph.fileAttachment",
                                        ContentBytes = System.IO.File.ReadAllBytes(file.FullName),
                                        ContentType = mimeType,
                                        ContentId = f.Name,
                                        Name = f.Name
                                    };
                                    await graphClient
                                            .Users[_From]
                                            .Messages[msgResult.Id]
                                            .Attachments
                                            .Request()
                                            .AddAsync(fileAttachment);
                                }
                                else
                                {
                                    // Attachments >= 3 Mb
                                    using (var filestream = System.IO.File.Open(file.FullName, System.IO.FileMode.Open, FileAccess.Read, FileShare.None))
                                    {
                                        var attachmentItem = new AttachmentItem
                                        {
                                            AttachmentType = AttachmentType.File,
                                            Name = Path.GetFileName(filestream.Name),
                                            Size = filestream.Length
                                        };

                                        var uploadSession = await graphClient
                                                .Users[_From]
                                                .Messages[msgResult.Id]
                                                .Attachments
                                                .CreateUploadSession(attachmentItem)
                                                .Request()
                                                .PostAsync();

                                        var maxSliceSize = 320 * 1024; // 320 KB - Change this to your slice size. 5MB is the default.
                                        var largeFileUploadTask = new LargeFileUploadTask<Microsoft.Graph.FileAttachment>(uploadSession, filestream, maxSliceSize);

                                        // upload away with relevant callback
                                        IProgress<long> progressCallback = new Progress<long>(prog => { });
                                        try
                                        {
                                            var uploadResult = await largeFileUploadTask.UploadAsync(progressCallback);
                                            if (!uploadResult.UploadSucceeded)
                                            {
                                                throw new Exception(uploadResult.ToString());
                                            }
                                        }
                                        catch (ServiceException e)
                                        {
                                            throw e;
                                        }
                                    } // using()
                                }
                            }
                        }
                        message = msgResult;
                    }
                    await SendEmailWithGraphAsync(graphClient, _From, message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Email send via Microsoft Graph thrown and exception : " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return graphClient;
        }

        static public async Task<GraphServiceClient> SendGraphEmailNoAttachments(string jsonEmailMsg)
        {
            try
            {
                EmailMsg msg = JsonConvert.DeserializeObject<EmailMsg>(jsonEmailMsg);

                try
                {
                    Message message = msg.GetGraphMessage();

                    GraphServiceClient gsc = await GetGraphClient();
                    await SendEmailWithGraphAsync(gsc, _From, message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Email send via Microsoft Graph thrown and exception : " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        static public void SendGraphEmailWithAttachmentsWait(string jsomFileFullPath)
        {
            Task<GraphServiceClient> callTask = System.Threading.Tasks.Task.Run(() => SendGraphEmailWithAttachments(jsomFileFullPath));
            callTask.Wait();
        }

        static public void SendGraphEmailNoAttachmentsWait(string jsonEmailMsg)
        {
            Task<GraphServiceClient> callTask = System.Threading.Tasks.Task.Run(() => SendGraphEmailNoAttachments(jsonEmailMsg));
            callTask.Wait();
        }

        static public void SendGraphEmailNoAttachmentsWait(string to, string subject, string body, string cc)
        {
            try
            {
                dynamic obj = new JObject();
                obj.To = to;
                obj.Subject = subject;
                obj.Body = body;
                obj.Cc = cc;

                string jsonEmailMsg = JsonConvert.SerializeObject(obj);

                Task<GraphServiceClient> callTask = System.Threading.Tasks.Task.Run(() => SendGraphEmailNoAttachments(jsonEmailMsg));
                callTask.Wait();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static async Task<GraphServiceClient> SendEmailWithGraphAsync(GraphServiceClient graphClient, string fromUserPrincipalName, Message message)
        {
            if (!string.IsNullOrEmpty(message.Id))
            {
                await graphClient
                    .Users[_From]
                    .Messages[message.Id]
                    .Send()
                    .Request()
                    .WithMaxRetry(5)
                    .PostAsync();
            }
            else
            {
                await graphClient
                    .Users[_From]
                    .SendMail(message, false)
                    .Request()
                    .WithMaxRetry(5)
                    .PostAsync();
            }

            return graphClient;
        }

        static async Task<GraphServiceClient> GetGraphClient()
        {
            string tenantId = "add47526-280c-414e-9031-8f2ccf9d1850";
            string clientId = "d0f30b27-7788-43e9-9ef3-12a2dbf1ea71";
            string clientSecret = "1u58Q~5fBqHrUT6YWqNhcP1ja9x6nKWESsWrYdxd";
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(clientId).WithTenantId(tenantId).WithClientSecret(clientSecret).Build();
            var authResult = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync().ConfigureAwait(false);

            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/" + tenantId + $"/v2.0")
                .WithClientSecret(clientSecret)
                .Build();

            GraphServiceClient graphClient = new GraphServiceClient(new DelegateAuthenticationProvider
                (async (requestMessage) =>
                {
                    // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
                    var authResult = await confidentialClient
                        .AcquireTokenForClient(scopes)
                        .ExecuteAsync();
                    // Add the access token in the Authorization header of the API request.
                    requestMessage.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                }
                )
            );
            return graphClient;
        }

        public class EmailMsg
        {
            public string To { get; set; }
            public string Cc { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string SigYesNo { get; set; }

            public Message GetGraphMessage()
            {
                MessageAttachmentsCollectionPage _attachmentsList = null;

                if (SigYesNo == "Yes")
                {
                    string sanjelSig = "</br><a href=\"sanjel.com\"><img src=\"cid:" + signatureImageID + "\" /></a><br>";

                    string htmlBody = GetBody().Replace("</html>", "").Replace("</body>", "");

                    htmlBody = htmlBody + sanjelSig;

                    if (this.Body.Contains("</body>"))
                        htmlBody = htmlBody + "</body>";

                    if (this.Body.Contains("</html>"))
                        htmlBody = htmlBody + "</html>";

                    this.Body = htmlBody;

                    _attachmentsList =
                        new MessageAttachmentsCollectionPage() {
                            new Microsoft.Graph.FileAttachment {
                                ContentType= "image/jpeg",
                                ContentBytes = System.IO.File.ReadAllBytes(signatureImagePath),
                                ContentId = signatureImageID,
                                Name= "Sanjel"
                            }
                        };
                }

                return new Message
                {
                    Subject = GetSubject(),
                    Body = new ItemBody { ContentType = Microsoft.Graph.BodyType.Html, Content = GetBody() },
                    ToRecipients = GetRecipientList(),
                    CcRecipients = GetCcList(),
                    Attachments = _attachmentsList
                };
            }

            public string GetSubject()
            {
                return string.IsNullOrEmpty(Subject) ? "" : Subject;
            }

            public string GetBody()
            {
                return string.IsNullOrEmpty(Body) ? "" : Body;
            }

            public List<string> GetRecipientEmailList()
            {
                List<string> _toList = new List<string>();

                string[] splitted;

                if (!String.IsNullOrEmpty(To))
                {
                    if (To.Contains(","))
                        splitted = To.Split(",");
                    else if (To.Contains(";"))
                        splitted = To.Split(";");
                    else if (To.Contains(" "))
                        splitted = To.Split(" ");
                    else
                        splitted = new string[] { To };

                    _toList.AddRange(splitted);
                }
                else
                {
                    _toList.Add("vmolochko@sanjel.com");
                }

                return _toList;
            }
            public List<string> GetCcEmailList()
            {
                List<string> _ccList = new List<string>();

                string[] splitted;

                if (!String.IsNullOrEmpty(Cc))
                {
                    if (Cc.Contains(","))
                        splitted = Cc.Split(",");
                    else if (Cc.Contains(";"))
                        splitted = Cc.Split(";");
                    else if (To.Contains(" "))
                        splitted = Cc.Split(" ");
                    else
                        splitted = new string[] { Cc };

                    _ccList.AddRange(splitted);
                }
                //else
                //{
                //    _ccList.Add("vmolochko@sanjel.com");
                //}

                return _ccList;
            }

            private List<Recipient> GetRecipientList()
            {
                List<Recipient> _toRecipients = new List<Recipient>();

                foreach (string email in GetRecipientEmailList())
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        _toRecipients.Add(new Recipient
                        {
                            EmailAddress = new Microsoft.Graph.EmailAddress
                            {
                                Address = email.Trim()
                            }
                        });
                    }
                }

                return _toRecipients;
            }
            private List<Recipient> GetCcList()
            {
                List<Recipient> _ccRecipients = new List<Recipient>();

                foreach (string email in GetCcEmailList())
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        _ccRecipients.Add(new Recipient
                        {
                            EmailAddress = new Microsoft.Graph.EmailAddress
                            {
                                Address = email.Trim()
                            }
                        });
                    }
                }

                return _ccRecipients;
            }
        }

        public class EmailMsgExtended : EmailMsg
        {
            public string ContentPath { get; set; }
            public string JsonFileName { get; set; }

            public EmailMsgExtended(string jsonFileFullPath)
            {

                FileInfo fi = new FileInfo(jsonFileFullPath);

                ContentPath = fi.DirectoryName;
                JsonFileName = fi.Name;

                string jsonText = System.IO.File.ReadAllText(jsonFileFullPath);
                if (!string.IsNullOrEmpty(jsonText))
                {
                    try
                    {
                        EmailMsg msg = JsonConvert.DeserializeObject<EmailMsg>(jsonText);

                        Subject = msg.Subject;
                        Body = msg.Body;
                        To = msg.To;
                        Cc = msg.Cc;
                        SigYesNo = msg.SigYesNo;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        static public void SendtestEmail()
        {
            try
            {
                //string _from = "AppNotification@sanjel.com";
                string[] _to = new string[] { "vmolochko@sanjel.com" };
                string[] _cc = new string[] { "vmolochko@sanjel.com" };

                string _subj = "Test sending SSIS email via ExchangeService - " + DateTime.Now.ToString();
                string _body = @"Just a test - <b> Testing HTML </b>  <i>is working too</i> - SSIS script ...";

                dynamic obj = new JObject();
                obj.From = "";
                obj.To = JToken.FromObject(_to);
                obj.Cc = JToken.FromObject(_cc);
                obj.Subject = _subj;
                obj.Body = _body;

                var jsonString = JsonConvert.SerializeObject(obj);

                //SendEmailViaEWS(jsonString);
                //SendGraphEmailNoAttachmentsWait(jsonString);

                var mailMessage = MailUtility.CreateMailMessage(_subj, _body, null, "OVPP Automation", "vmolochko@sanjel.com", "vmolochko@sanjel.com", null);
                MailUtility.SendMail(mailMessage);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public bool MarkAsProcessed(string jsonFileFullPath)
        {
            bool result = true;
            try
            {
                FileInfo fi = new FileInfo(jsonFileFullPath);
                string newFullName = fi.DirectoryName + @"\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + fi.Name;
                System.IO.File.Move(jsonFileFullPath, newFullName);
            }
            catch (Exception ex)
            {
                //result = false;
                throw ex;
            }

            return result;
        }





        static string _EMailAccessToken = "";
        static DateTime _EMailAccessTokenTs = DateTime.Now;
        static double _RefreshPeriodHours = 0.25;

        static public void SendEmailViaEWS(string jsonEmailMsg)
        {
            try
            {
                EmailMsg msg = JsonConvert.DeserializeObject<EmailMsg>(jsonEmailMsg);

                string[] _to = msg.GetRecipientEmailList().ToArray();

                ExchangeService ews = new ExchangeService()
                {
                    Credentials = new OAuthCredentials(GetToken()),
                    ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, _From),
                    Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
                };

                EmailMessage email = new EmailMessage(ews);

                email.From = _From;
                email.ToRecipients.AddRange(_to);

                //email.Subject = _subj;
                //email.Body = _body;
                email.Subject = msg.GetSubject();
                email.Body = msg.GetBody();

                int maxAttmptsCount = 1;
                string errorMsg = "";
                for (int i = 1; i <= maxAttmptsCount; i++)
                {
                    errorMsg = "";
                    try
                    {
                        email.Send();
                    }
                    catch (Exception ex)
                    {
                        errorMsg = "EmailMessage.Send() via ExchangeService thrown and exception : " + ex.Message;
                    }

                    if (errorMsg == "")
                        break;
                    else if (i == maxAttmptsCount)
                        throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public string GetToken()
        {
            TimeSpan diff = DateTime.Now - _EMailAccessTokenTs;
            double hours = diff.TotalHours;

            if (String.IsNullOrEmpty(_EMailAccessToken) || hours > _RefreshPeriodHours)
            {
                _EMailAccessToken = GetAccessToken();
                _EMailAccessTokenTs = DateTime.Now;
            }

            return _EMailAccessToken;
        }

        class TokenObject
        {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
        }

        static string GetAccessToken()
        {
            string client_id = "d0f30b27-7788-43e9-9ef3-12a2dbf1ea71";
            string client_secret = "1u58Q~5fBqHrUT6YWqNhcP1ja9x6nKWESsWrYdxd";
            string tenant = "add47526-280c-414e-9031-8f2ccf9d1850";

            string tokenUri = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", tenant);
            string requestData = string.Format("client_id={0}&client_secret={1}&scope=https://outlook.office365.com/.default&grant_type=client_credentials", client_id, client_secret);


            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(tokenUri);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                byte[] requestBuffer = Encoding.UTF8.GetBytes(requestData);
                requestStream.Write(requestBuffer, 0, requestBuffer.Length);
                requestStream.Close();
            }

            try
            {
                HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    TokenObject to = JsonConvert.DeserializeObject<TokenObject>(responseText);
                    return string.IsNullOrEmpty(to?.access_token) ? "" : to.access_token;
                }
            }
            catch (WebException ex)
            {
                string responseText = "";
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            responseText = reader.ReadToEnd();
                }
                throw new Exception(ex.Message + @"\n\n" + responseText);
            }
        }


    }

}
