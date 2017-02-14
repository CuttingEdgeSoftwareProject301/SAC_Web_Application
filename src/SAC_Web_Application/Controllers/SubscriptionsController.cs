using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAC_Web_Application.Models.ClubModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SAC_Web_Application.Controllers
{
    public class SubscriptionsController : Controller
    {
        private ClubContext _context;

        public SubscriptionsController(ClubContext context)
        {
            _context = context;    
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subscriptions.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.SingleOrDefaultAsync(m => m.SubID == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Subscriptions/Select Subscription
        public IActionResult SelectSubscription(int? id)
        {
            return RedirectToAction("Create", "Members", new { subId = id });            
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubID,Cost,Item")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.SingleOrDefaultAsync(m => m.SubID == id);
            if (subscription == null)
            {
                return NotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubID,Cost,Item")] Subscription subscription)
        {
            if (id != subscription.SubID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.SingleOrDefaultAsync(m => m.SubID == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscription = await _context.Subscriptions.SingleOrDefaultAsync(m => m.SubID == id);
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.SubID == id);
        }

        // GET: SubscriptionSuccessfull
        public IActionResult SubscriptionSuccessfull()
        {
            
            if (Request.Query["tx"] != "")
            {
                string TransactionID = Request.Query["tx"];
                string amount = Request.Query["amt"];

                Payment payment = new Payment();              

                payment.PaymentID = TransactionID;
                payment.Amount = amount;
                payment.CreateTime = DateTime.Now;

                _context.Add(payment);

                //CREATE A LIST TO STORE THE ATHLETE DETAILS
                List<Members> memberList = new List<Members>();

                var str = HttpContext.Session.GetString("memberList");
                if (str != null)
                {
                    var obj = JsonConvert.DeserializeObject<List<Members>>(str);
                    memberList = (obj);
                }

                MemberPayment memPay = new MemberPayment();

                foreach (Members member in memberList)
                {
                    memPay.MemberID = member.MemberID;
                    memPay.PaymentID = TransactionID;
                    //member.MembershipPaid = true; **TRY THIS WITH A TRIGGER
                    _context.Add(memPay);
                    
                }               

                ViewData["Message"] = string.Format("Paypal Reference", TransactionID);
                ViewData["Message2"] = string.Format("Amount Paid {0:c}", amount);
            }
            return View();
        }

        //[HttpPost, ActionName("SubscriptionSuccessfull")]
        //[ValidateAntiForgeryToken]

        //public void SubscriptionSuccessfull()
        //{
        //    //var loaded = Views.Subscriptions.SubscriptionSuccessfull.cshtml;
        //    //if (!loaded.IsPostBack) && 
        //    if(Request.Query["tx"] != "")
        //    {
        //        string TransactionID = Request.Query["tx"];
        //        Console.Write(TransactionID);
        //    }

        // }
    }
}
