using Airgap.Constant;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Airgap.WebApi.Helper
{
    public class ApplePushNotificationService
    {
        readonly string _certificatePwd = "pg20111321"; //"pg20111321";
        SslStream _sslStream;
        private readonly AppSetting _appSettings;

        public ApplePushNotificationService(AppSetting appSettings)
        {
            _appSettings = appSettings;
        }

        public async System.Threading.Tasks.Task<bool> ConnectToApnsAsync()
        {
            X509Certificate2Collection certs = new X509Certificate2Collection();
            string filePath = @"./PEM/Certificates.p12";
            FileInfo file = new FileInfo(Path.Combine(filePath));
            var certificatePath = file.FullName;

            X509Certificate2 clientCertificate = new X509Certificate2(File.ReadAllBytes(certificatePath), _certificatePwd, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            certs = new X509Certificate2Collection(clientCertificate);
            //----------------------------------------


            // Apple development server address
            string apsHost = _appSettings.AppleHost;

            // Create a TCP socket connection to the Apple server on port 2195
            //TcpClient tcpClient = new TcpClient(apsHost, 2195);
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(apsHost, 2195);


            // Create a new SSL stream over the connection
            _sslStream = new SslStream(tcpClient.GetStream(), false);

            // Authenticate using the Apple cert
            await _sslStream.AuthenticateAsClientAsync(apsHost, certs, System.Security.Authentication.SslProtocols.Tls, false);

            //PushMessage();

            return true;
        }

        private byte[] HexToData(string hexString)
        {
            if (hexString == null)
                return null;

            if (hexString.Length % 2 == 1)
                hexString = '0' + hexString; // Up to you whether to pad the first or last byte

            byte[] data = new byte[hexString.Length / 2];

            for (int i = 0; i < data.Length; i++)
                data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return data;
        }

        public async System.Threading.Tasks.Task<bool> PushMessage(string Mess, string DeviceToken, int Badge, string Custom_Field)
        {
            await ConnectToApnsAsync();
            List<string> Key_Value_Custom_Field = new List<string>();
            String cToken = DeviceToken;
            String cAlert = Mess;
            int iBadge = Badge;

            // Ready to create the push notification
            byte[] buf = new byte[256];
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(new byte[] { 0, 0, 32 });

            byte[] deviceToken = HexToData(cToken);
            bw.Write(deviceToken);

            bw.Write((byte)0);

            // Create the APNS payload - new.caf is an audio file saved in the application bundle on the device
            string msg = "";
            msg = "{\"aps\":{\"alert\":\"" + cAlert + "\",\"badge\":\"" + iBadge.ToString() + "\",\"sound\":\"noti.aiff\"}";

            String PayloadMess = "";
            if (string.IsNullOrWhiteSpace(Custom_Field) == false)
            {
                List<string> list_Custom_Field = Custom_Field.Split(';').ToList();

                if (list_Custom_Field.Count > 0)
                {
                    for (int indx = 0; indx < list_Custom_Field.Count; indx++)
                    {
                        Key_Value_Custom_Field = list_Custom_Field[indx].Split('=').ToList();
                        if (Key_Value_Custom_Field.Count > 1)
                        {
                            if (PayloadMess != "") PayloadMess += ", ";
                            PayloadMess += "\"" + Key_Value_Custom_Field[0].ToString() + "\":\"" + Key_Value_Custom_Field[1].ToString() + "\"";
                        }
                    }
                }
            }

            if (PayloadMess != "")
            {
                msg += ", " + PayloadMess;
            }
            msg += "}";

            // Write the data out to the stream
            bw.Write((byte)msg.Length);
            bw.Write(msg.ToCharArray());
            bw.Flush();

            if (_sslStream != null)
            {
                _sslStream.Write(ms.ToArray());
                return true;
            }

            return false;
        }
    }
}
