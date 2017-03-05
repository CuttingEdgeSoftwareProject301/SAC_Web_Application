using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SAC_Web_Application.Models.ClubModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using SAC_Web_Application.Services;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using SAC_Web_Application.Models.GroupSMSModel;

namespace SAC_Web_Application.Controllers
{
    public class GroupSMSController : Controller
    {
        private IOptions<ApplicationSettings> _settings;
        private ClubContext _context;
        public GroupSMSController(IOptions<ApplicationSettings> settings, ClubContext context)
        {
            _settings = settings;
            _context = context;
        }
        public IActionResult GroupSMS()
        {
            var categories = _context.Categories.Select(c => new { Id = c.CatID, Value = c.CatName });
            ViewData["Categories"] = new SelectList(categories, "Id", "Value");
            return View();
        }

        [HttpGet("SendSms", Name = "SendSms")]
        public async Task<IActionResult> SendSms(GroupSMSViewModel model,string SMSTo, string SMSContent)
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{"AC646d4b74d05892b215050a732fda8ad9"}:{"7f92439797e6d442d6abd2644d0edb44"}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = new FormUrlEncodedContent(new[]
                {
             new KeyValuePair<string, string>("To",SMSTo),
             new KeyValuePair<string, string>("From", "+353861802160"),
             new KeyValuePair<string, string>("Body", SMSContent)
        });

                await client.PostAsync("https://api.twilio.com/2010-04-01/Accounts/AC646d4b74d05892b215050a732fda8ad9/Messages.json", content);
            }
            return RedirectToAction("GroupSMSSuccess", "GroupSMS");
        }
        public ActionResult GroupSMSSuccess()
        {
            return View();
        }
    }

}
