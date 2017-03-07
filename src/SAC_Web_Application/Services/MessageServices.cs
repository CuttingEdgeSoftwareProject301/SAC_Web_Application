using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SAC_Web_Application.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(email);
            myMessage.From = new System.Net.Mail.MailAddress("paull1068@gmail.com", "Paul Little");
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            var credentials = new System.Net.NetworkCredential(
                Options.SendGridUser,
                Options.SendGridKey);
            // Create a Web transport for sending email.
            var transportWeb = new SendGrid.Web(credentials);
            return transportWeb.DeliverAsync(myMessage);
        }

        public Task ContactUsEmailAsync(string emailFrom, string emailTo, string message
            ,string senderName, string subject)
        {
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(emailTo);
            myMessage.From = new System.Net.Mail.MailAddress(emailFrom, senderName);
            myMessage.Subject = subject;
            myMessage.Text = message;
            myMessage.Html = message;
            var credentials = new System.Net.NetworkCredential(
                Options.SendGridUser,
                Options.SendGridKey);
            // Create a Web transport for sending email.
            var transportWeb = new SendGrid.Web(credentials);
            return transportWeb.DeliverAsync(myMessage);
        }

        public Task SendSmsAsync(string number, string message)
        {
            //    //using (var client = new HttpClient())
            //    //{
            //    //    var byteArray = Encoding.ASCII.GetBytes($"{"AC646d4b74d05892b215050a732fda8ad9"}:{"7f92439797e6d442d6abd2644d0edb44"}");
            //    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            //    //    var content = new FormUrlEncodedContent(new[]
            //    //        {
            //    //             new KeyValuePair<string, string>("To",number),
            //    //             new KeyValuePair<string, string>("From", "+353861802160"),
            //    //             new KeyValuePair<string, string>("Body", message)
            //    //         });

            //    // await client.PostAsync("https://api.twilio.com/2010-04-01/Accounts/AC646d4b74d05892b215050a732fda8ad9/Messages.json", content);             
            return Task.FromResult(0);
        }
        }
    }
