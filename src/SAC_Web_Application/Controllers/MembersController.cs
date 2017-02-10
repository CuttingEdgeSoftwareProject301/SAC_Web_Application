using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAC_Web_Application.Models.ClubModel;
using System.Security.Claims;
using SAC_Web_Application.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using SAC_Web_Application.Models.MembersViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

//Members Controller
namespace SAC_Web_Application.Controllers
{
    public class MembersController : Controller
    {
        private ClubContext _context;
        
        public MembersController(ClubContext context)
        {
            _context = context;    
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var members = await _context.Members.SingleOrDefaultAsync(m => m.MemberID == id);
            if (members == null)
            {
                return NotFound();
            }

            return View(members);
        }

        // GET: Members/Create
        public IActionResult Create(int? subId)
        {
            if (subId != null)
            {
                var sub = _context.Subscriptions.Where(s => s.SubID == subId).First();
                ViewData["SubName"] = sub.Item;
                ViewData["SubID"] = sub.SubID;
            }

            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind
            ("MemberID,Identifier,memberList,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName")]
            Members members
            /*, IServiceProvider serviceProvider*/) //for adding to member role
        {
            List<Members> memberList = new List<Members>();
            var str = HttpContext.Session.GetString("memberList");            
            if (str != null)
            {
                var obj = JsonConvert.DeserializeObject<List<Members>>(str);
                memberList = (obj);
            }

            // GETS THE EMAIL ADDRESS OF THE USER THAT IS CURRENTLY LOGGED IN
            var userEmail = User.FindFirstValue(ClaimTypes.Name);

            members.Email = userEmail;
            // addional columns that must be added
            members.MembershipPaid = false;
            members.DateRegistered = DateTime.Now;

            int subNum = members.Identifier;
            //int viewCount;

            //for adding to member role
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (ModelState.IsValid)
            {
                if (subNum == 1 || subNum == 2 || subNum == 3)
                {
                    members.Email = userEmail;
                    // addional columns that must be added
                    members.MembershipPaid = false;
                    members.DateRegistered = DateTime.Now;

                    _context.Add(members);
                }
                if (subNum == 4 || subNum == 7 || subNum == 12)
                {
                    memberList.Add(members);                    
                    var memList = JsonConvert.SerializeObject (memberList);                    
                    HttpContext.Session.SetString("memberList", memList);

                    if (memberList.Count == 2)
                    {
                        foreach (Members member in memberList)
                        {
                            _context.Add(member);
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    else
                        return RedirectToAction("Create", "Members", new { subId = subNum});
                }

                    //for adding to member role
                    /*ApplicationUser user1 = await userManager.FindByEmailAsync(userEmail);
                    if (user1 != null)
                    {
                        await userManager.AddToRolesAsync(user1, new string[] { "Member" });
                    }*/

                //await _context.SaveChangesAsync();
               // return RedirectToAction("Index");

            }

            // If we got this far, something failed, redisplay form
            return View(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult _Create([Bind
            ("MemberID,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName")]
            Members member1
            /*, IServiceProvider serviceProvider*/)
        {
            return PartialView(new Members()
            {
                Address1 = member1.Address1,
                Address2 = member1.Address2,
                PostCode = member1.PostCode,
                County = member1.County,
                City = member1.City,
                Province = member1.Province
            });
        }        

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var members = await _context.Members.SingleOrDefaultAsync(m => m.MemberID == id);
            if (members == null)
            {
                return NotFound();
            }
            return View(members);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberID,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName")] Members members)
        {
            if (id != members.MemberID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(members);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembersExists(members.MemberID))
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
            return View(members);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var members = await _context.Members.SingleOrDefaultAsync(m => m.MemberID == id);
            if (members == null)
            {
                return NotFound();
            }

            return View(members);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var members = await _context.Members.SingleOrDefaultAsync(m => m.MemberID == id);
            _context.Members.Remove(members);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool MembersExists(int id)
        {
            return _context.Members.Any(e => e.MemberID == id);
        }

        // GET: Members/Subscriptions
        public IActionResult Subscriptions()
        {
            return View();
        }
    }
}
