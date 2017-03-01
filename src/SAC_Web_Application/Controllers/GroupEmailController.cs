using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAC_Web_Application.Models.GroupEmailModel;
using SAC_Web_Application.Services;

namespace SAC_Web_Application.Controllers
{
    public class GroupEmailController : Controller
    {
        private IEmailSender _emailSender;

        public GroupEmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;

        }
        // GET: GroupEmail
        public ActionResult GroupEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendGroupEmail(GroupEmailViewModel groupEmail, string EmailTo)
        {
            string v = EmailTo;
            if (ModelState.IsValid)
            {
                _emailSender.SendEmailAsync("S00147728@mail.itsligo.ie", groupEmail.EmailTitle, groupEmail.EmailContent);
                ModelState.AddModelError(string.Empty, "Your message has been sent sucessfully");
                return View("GroupEmail", groupEmail);

            }
            return View(groupEmail);
        }
    }
}