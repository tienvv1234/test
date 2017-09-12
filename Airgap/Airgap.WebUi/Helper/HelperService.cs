using System;
using MailKit.Net.Smtp;
using MimeKit;
using System.Security.Authentication;
using Microsoft.Extensions.Options;
using Airgap.Constant;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.IO;
using Airgap.Entity.Entities;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Reflection;
using Airgap.Data.DTOEntities;

namespace Airgap.Service.Helper
{
    public class HelperService : IHelperService
    {
        private readonly AppSetting _appSettings;

        public HelperService(IOptions<AppSetting> appSettings) {
            _appSettings = appSettings.Value;
        }

        public void ExecuteSendMail(string sendTo, string subject, string path, string url, Account account, string content)
        {
            sendEmail(sendTo, subject, path, url, account, content).Wait();
        }

        private async Task sendEmail(string sendTo, string subject, string path, string url, Account account, string content)
        {
            //try
            //{
            //    var message = new MimeMessage();
            //    message.From.Add(new MailboxAddress(string.Empty, _appSettings.From));
            //    message.To.Add(new MailboxAddress(string.Empty, sendTo));
            //    message.Subject = subject;
            //    message.Body = new TextPart("plain")
            //    {
            //        Text = @"Please clink to this link to reset your password " + content + ""
            //    };

            //    using (var client = new SmtpClient())
            //    {
            //        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
            //        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            //        client.Connect(_appSettings.MailHost, 25, false);
            //        client.SslProtocols = SslProtocols.Tls;
            //        // Note: since we don't have an OAuth2 token, disable
            //        // the XOAUTH2 authentication mechanism.
            //        client.AuthenticationMechanisms.Remove("XOAUTH2");

            //        // Note: only needed if the SMTP server requires authentication
            //        client.Authenticate(_appSettings.MailUsername, _appSettings.MailPassword);

            //        client.Send(message);
            //        client.Disconnect(true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(ex);
            //}


            try
            {
                var client = new SendGridClient(_appSettings.APIkey);
                var from = new EmailAddress(_appSettings.From);
                var to = new EmailAddress(sendTo);
                var plainTextContent = "";
                var htmlContent = createEmailBody(url, path, account, content);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private string createEmailBody(string url, string path, Account account, string content)
        {

            string body = string.Empty;
            //using streamreader for reading my htmltemplate   
            using (var stream = new FileStream(path, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{Url}", url); //replacing the required things  
            body = body.Replace("{Content}", content); //replacing the required things  
            body = body.Replace("{Name}", account.FirstName); //replacing the required things  
            body = body.Replace("{Email}", account.Email); //replacing the required things  
            return body;
        }

        public FileInfo ExportExcel(List<SerialNumberExport> serialNumbers)
        {
            string sFileName = @"export.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sFileName));
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("WorkSheet1");

                var listHeader = GetHeader<SerialNumberExport>(serialNumbers);
                int rowStart = 1;
                for (int i = 0; i < listHeader.Count; i++)
                {
                    worksheet.Cells[rowStart, i + 1].Value = listHeader[i];
                }

                if (serialNumbers.Count > 0)
                {
                    worksheet.Cells["A2"].LoadFromCollection(serialNumbers);
                }

                package.Save(); //Save the workbook.
            }
            return file;
        }

        public static List<string> GetHeader<T>(List<T> header)
        {

            List<string> listHeader = new List<string>();
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsConstructedGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                listHeader.Add(prop.Name);
            }
            return listHeader;
        }
    }
}
