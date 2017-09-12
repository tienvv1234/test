using Airgap.Constant;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Airgap.WebApi.Helper
{
    public class AndroidGcmPushNotification
    {

        private readonly AppSetting _appSettings;

        public AndroidGcmPushNotification(AppSetting appSettings)
        {
            _appSettings = appSettings;
        }

        public void InitPushNotification(string deviceId,string message)
        {
            //string deviceId = "dLiNjWr5gB0:APA91bGR63QdBhBbCHRifFes_1cnRINn4yYLns4CScWqs5qytSjPkmJqk-1vv2mH76M0bGiEAOPP4PqVfrWtJLBu51xbqj0xiMyxWTRRRVVE3R7-0rAZxPf2wdPLo8X8_JxeUV3X2k1Z";
            //string tickerText = "example test GCM";
            //string contentTitle = "content title GCM";
            string data =
            "{ \"registration_ids\": [ \"" + deviceId + "\" ], " +
              "\"data\": {\"tickerText\":\"" + Configuration.TickerText + "\", " +
                         "\"contentTitle\":\"" + Configuration.ContentTitle + "\", " +
                         "\"message\": \"" + message + "\", " +
                         "\"userid\":\"" + string.Empty + "\", " +
                         "\"type\":\"" + string.Empty + "\", " +
                         "\"objectId\":\"" + string.Empty + "\"" +
                         "}}";


            SendGcmNotification(_appSettings.APIOfGCM, data).Wait();
        }

        public async Task<string> SendGcmNotification(string apiKey, string postData, string postDataContentType = "application/json")
        {
            //
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(_appSettings.GCMPushNotification);
            Request.Method = "POST";
            //Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            //Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.Headers["Authorization"] = "key="+ apiKey + "";
            //Request.Headers["Sender"] = "id=411981033294";
            Request.Headers["ContentLength"] = byteArray.Length.ToString();
            //Request.ContentLength = byteArray.Length;

            Stream dataStream = await Request.GetRequestStreamAsync();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Dispose();

            //
            //  SEND MESSAGE
            try
            {
                WebResponse Response = await Request.GetResponseAsync();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    var text = "Unauthorized - need new token";
                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    var text = "Response from web service isn't OK";
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = await Reader.ReadToEndAsync();
                Reader.Dispose();

                return responseLine;
            }
            catch (Exception e)
            {
            }
            return "error";
        }
    }
}
