//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace PrisonLabor_Tests
//{
//    public static class GistCreate
//    {
//        public static void SendReport()
//        {
//            //lock

//            var collatedData = PrepareLogData();

//            if (collatedData == null)
//            {
//                //ErrorMessage = "Failed to collect data";
//                //Status = PublisherStatus.Error;
//                return;
//            }

//            //ServicePointManager.ServerCertificateValidationCallback = (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true ;

//            var sendingTask = new Task(() =>
//            {
//                try
//                {
//                    using (var client = new WebClient())
//                    {
//                        client.Headers.Add("Authorization", "token " + GitHubAuthToken);
//                        client.Headers.Add("User-Agent", RequestUserAgent);
//                        collatedData = CleanForJSON(collatedData);
//                        var payload = String.Format(GistPayloadJson, GistDescription, OutputLogFilename, collatedData);
//                        var response = client.UploadString(GistApiUrl, payload);
//                        var status = client.ResponseHeaders.Get("Status");
//                        if (status == SuccessStatusResponse)
//                        {
//                            OnUploadComplete(response);
//                        }
//                        else
//                        {
//                            OnRequestError(status);
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (userAborted) return;
//                    OnRequestError(e.Message);
//                    HugsLibController.Logger.Warning("Exception during log publishing (gist creation): " + e);
//                }
//            });
//            sendingTask.Start();
//        }
//    }
//}
