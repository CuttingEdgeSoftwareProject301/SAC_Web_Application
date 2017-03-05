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
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using SAC_Web_Application.Models;

namespace SAC_Web_Application.Controllers
{
    public class SubscriptionsController : Controller
    {
        private ClubContext _context;
        private UserManager<ApplicationUser> _userManager;

        public SubscriptionsController(ClubContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> SubscriptionSuccessfull()
        {
            if (Request.Query["tx"] != "")
            {
                string TransactionID = Request.Query["tx"];
                string amount = Request.Query["amt"];

                UpdatePaymentTable(TransactionID, amount);
                UpdateMemberPaymentsTable(TransactionID);

                //AddToMemberRole();
                await AddUserToMemberRole();

                ViewData["Message"] = string.Format("Paypal Reference {0}", TransactionID);
                ViewData["Message2"] = string.Format("Amount Paid ${0:c}", amount);
            }
            //return RedirectToAction("Index", "Members");
            return View();
        }        

        private void UpdateMemberPaymentsTable(string TransactionID)
        {
            //CREATE A LIST TO STORE THE ATHLETE DETAILS
            List<Members> memberList = new List<Members>();
            var str = HttpContext.Session.GetString("memberList");
            if (str != null)
            {
                var obj = JsonConvert.DeserializeObject<List<Members>>(str);
                memberList = (obj);
            }
            if(User.IsInRole("RegisteredUser"))
            {
                // Retrieve members that are attached to this user account 
                memberList = GetAssociatedMembers();
            }

            MemberPayment memPay = new MemberPayment();
            foreach (Members member in memberList)
            {
                memPay.MemberID = member.MemberID;
                memPay.PaymentID = TransactionID;
                //MEMBERSHIP PAID COLUMN UPDATED TO TRUE USING A TRIGGER ON MEMBERPAYMENTS TABLE
                _context.Add(memPay);
                _context.SaveChanges();
            }
        }

        private List<Members> GetAssociatedMembers()
        {
            List<Members> memberList;
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            Members thisMember = _context.Members.Where(m => m.Email == userEmail).First();
            MemberPayment memPayment = _context.MemberPayments.Where(mp => mp.MemberID == thisMember.MemberID).First();
            List<MemberPayment> memPayments = _context.MemberPayments.Where(mp => mp.PaymentID == memPayment.PaymentID).ToList();
            memberList = new List<Members>();
            foreach (var item in memPayments)
            {
                Members member = _context.Members.Where(m => m.MemberID == item.MemberID).First();
                memberList.Add(member);
            }

            return memberList;
        }

        private void UpdatePaymentTable(string TransactionID, string amount)
        {
            Payment payment = new Payment();

            payment.PaymentID = TransactionID;
            payment.Amount = amount;
            payment.CreateTime = DateTime.Now;

            _context.Add(payment);
            _context.SaveChanges();
        }

        private async Task AddUserToMemberRole()
        {
            // GETS THE EMAIL ADDRESS OF THE USER THAT IS CURRENTLY LOGGED IN
            var userEmail = User.FindFirstValue(ClaimTypes.Name);            
            ////if(User.IsInRole("RegisteredUser"))
            if (userEmail != null)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);
                await _userManager.AddToRolesAsync(user, new string[] { "Member" });
            }
        }

        // GET: Subscriptions
        public IActionResult PayNow()
        {
            return View();
        }
    }
}
