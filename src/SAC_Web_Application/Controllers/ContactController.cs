using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAC_Web_Application.Models.ContactViewModels;
using SAC_Web_Application.Services;
using Microsoft.AspNetCore.Authorization;
using SAC_Web_Application.Models.GroupEmailModel;

namespace SAC_Web_Application.Controllers
{
    [RequireHttps]
    public class ContactController : Controller
    {
        private IEmailSender _emailSender;

        public ContactController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
            
        }

        // GET: Events/Create
        public IActionResult ContactUs()
        {
            return View();
        }

        // POST: Contact/Send Question
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendQuestion(ContactUsViewModel contactUs)
        {
            if (ModelState.IsValid)
            {
                _emailSender.ContactUsEmailAsync(contactUs.Email, "paull1068@gmail.com", contactUs.Question
                   ,contactUs.FullName, contactUs.Subject);
                ModelState.AddModelError(string.Empty, "Your message has been received, we will be in touch very soon");
                return View("ContactUs", contactUs);

            }
            return View(contactUs);
        }
    }
}