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
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
            var members = from m in _context.Members
                          select m;
            switch (sortOrder)
            {
                case "lastName_desc":
                    members = members.OrderByDescending(m => m.LastName);
                    break;
                case "firstName_desc":
                    members = members.OrderByDescending(m => m.FirstName);
                    break;
                default:
                    members = members.OrderBy(m => m.LastName);
                    break;
            }
            return View(await members.AsNoTracking().ToListAsync());


            //return View(await _context.Members.ToListAsync());
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
            ("MemberID,Identifier,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName")]
            Members members
            /*, IServiceProvider serviceProvider*/) //for adding to member role
        {
            // GETS THE EMAIL ADDRESS OF THE USER THAT IS CURRENTLY LOGGED IN
            var userEmail = User.FindFirstValue(ClaimTypes.Name);

            // GET THE CHOSEN SUBSCRIPTION ID AND STORE TO A VARIABLE FOR FUTURE USE
            int subNum = members.Identifier;

            //CREATE A LIST TO STORE THE ATHLETE DETAILS
            List<Members> memberList = new List<Members>();

            //ADD USER EMAIL ADDRESS TO THE 1ST MEMBER
            members.Email = userEmail;
            // ADDITIONAL COLUMNS THAT MUST BE POPULATED
            members.MembershipPaid = false;
            members.DateRegistered = DateTime.Now;

            //for adding to member role
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (ModelState.IsValid)
            {
                if (subNum == 1 || subNum == 2 || subNum == 3)
                {
                    _context.Add(members);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    MemberListToSession(members, memberList);
                    return RedirectToAction("Create2", "Members", new { subId = subNum });
                }
                //for adding to member role
                /*ApplicationUser user1 = await userManager.FindByEmailAsync(userEmail);
                if (user1 != null)
                {
                    await userManager.AddToRolesAsync(user1, new string[] { "Member" });
                }*/
            }
            // If we got this far, something failed, redisplay form
            return View(members);
        }

        // GET: Members/Create2
        public IActionResult Create2(int? subId)
        {
            if (subId != null)
            {
                var sub = _context.Subscriptions.Where(s => s.SubID == subId).First();
                ViewData["SubName"] = sub.Item;
                ViewData["SubID"] = sub.SubID;
            }
            return View();
        }

        // POST: Members/Create2
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2([Bind
            ("MemberID,Identifier,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName")]
            Members members
            /*, IServiceProvider serviceProvider*/) //for adding to member role
        {
            // GET THE CHOSEN SUBSCRIPTION ID AND STORE TO A VARIABLE FOR FUTURE USE
            int subNum = members.Identifier;

            //CREATE A LIST TO STORE THE ATHLETE DETAILS
            List<Members> memberList = new List<Members>();

            // RETRIEVE THE LIST OF MEMBERS FROM SESSION IF IT CONTAINS DATA
            //CONVERT FROM JSON AND CREATE A LIST OF MEMBERS WITH IT
            var str = HttpContext.Session.GetString("memberList");
            if (str != null)
            {
                var obj = JsonConvert.DeserializeObject<List<Members>>(str);
                memberList = (obj);
            }

            // THESE COLUMNS POPULATED VIA THE USER DETAILS (ATHELETE 1)
            members.Address1 = memberList.ElementAt(0).Address1;
            members.Address2 = memberList.ElementAt(0).Address2;
            members.City = memberList.ElementAt(0).City;
            members.County = memberList.ElementAt(0).County;
            members.PostCode = memberList.ElementAt(0).PostCode;
            members.Province = memberList.ElementAt(0).Province;            
            members.MembershipPaid = false;
            members.DateRegistered = DateTime.Now;
            
            //for adding to member role
            //var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (ModelState.IsValid)
            {
                if (subNum == 4 || subNum == 7 || subNum == 12)
                {
                    MemberListToSession(members, memberList);

                    if (memberList.Count == 2)
                    {
                        return await UpdateDBLoop(memberList);
                    }
                    else
                        return RedirectToAction("Create", "Members", new { subId = subNum });
                }
                if (subNum == 5 || subNum == 8 || subNum == 10)
                {
                    MemberListToSession(members, memberList);

                    if (memberList.Count == 3)
                    {
                        return await UpdateDBLoop(memberList);
                    }
                    else
                        return RedirectToAction("Create2", "Members", new { subId = subNum });
                }

                if (subNum == 6 || subNum == 9 || subNum == 11)
                {
                    MemberListToSession(members, memberList);

                    if (memberList.Count == 4)
                    {
                        return await UpdateDBLoop(memberList);
                    }
                    else
                        return RedirectToAction("Create2", "Members", new { subId = subNum });
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

        private async Task<IActionResult> UpdateDBLoop(List<Members> memberList)
        {
            foreach (Members member in memberList)
            {
                _context.Add(member);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void MemberListToSession(Members members, List<Members> memberList)
        {
            memberList.Add(members);
            var memList = JsonConvert.SerializeObject(memberList);
            HttpContext.Session.SetString("memberList", memList);
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
