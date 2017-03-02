using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAC_Web_Application.Models.GroupEmailModel;
using SAC_Web_Application.Services;
using SAC_Web_Application.Models.ClubModel;

namespace SAC_Web_Application.Controllers
{
    public class GroupEmailController : Controller
    {
        private ClubContext _context;
        private IEmailSender _emailSender;

        public GroupEmailController(IEmailSender emailSender, ClubContext context)
        {
            _emailSender = emailSender;
            _context = context;
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
            List<Members> memToMail = new List<Members>();
            string category = EmailTo;
            if (category == "All Members")
            memToMail = _context.Members.ToList();
            else
            memToMail = _context.Members.Where(m => m.Category == category).ToList();

            if (ModelState.IsValid)
            {
                foreach (var item in memToMail)
                {
                    _emailSender.SendEmailAsync(item.Email, groupEmail.EmailTitle, groupEmail.EmailContent);             
                }
                ModelState.AddModelError(string.Empty, "Your message has been sent sucessfully");
                return View("GroupEmail", groupEmail);
            }
            return View(groupEmail);
        }
    }
}