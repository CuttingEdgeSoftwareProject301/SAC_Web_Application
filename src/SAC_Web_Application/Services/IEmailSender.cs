using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task ContactUsEmailAsync(string emailFrom, string emailTo, string message
           ,string senderName, string subject);
    }
}
