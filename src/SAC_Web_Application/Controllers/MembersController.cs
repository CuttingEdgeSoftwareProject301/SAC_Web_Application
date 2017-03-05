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
using SAC_Web_Application.Services;

//Members Controller
namespace SAC_Web_Application.Controllers
{
    public class MembersController : Controller
    {
        private ClubContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IEmailSender _emailSender;

        public MembersController(ClubContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Members
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["LastNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewData["FirstNameSortParm"] = sortOrder == "FirstName" ? "firstName_desc" : "FirstName";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var members = from m in _context.Members
                          select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                members = members.Where(m => m.LastName.Contains(searchString)
                || m.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "lastName_desc":
                    members = members.OrderByDescending(m => m.LastName);
                    break;
                case "FirstName":
                    members = members.OrderBy(m => m.FirstName);
                    break;
                case "firstName_desc":
                    members = members.OrderByDescending(m => m.FirstName);
                    break;
                default:
                    members = members.OrderBy(m => m.LastName);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Members>.CreateAsync(members.AsNoTracking(), page ?? 1, pageSize));
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
            // retrieve data for drop down lists
            var genders = _context.Genders.Select(g => new { Id = g.GenderID, Value = g.GenderName });
            ViewData["Genders"] = new SelectList(genders, "Id", "Value");
            var counties = _context.Counties.Select(c => new { Id = c.CountyID, Value = c.CountyName });
            ViewData["Counties"] = new SelectList(counties, "Id", "Value");
            var provinces = _context.Provinces.Select(p => new { Id = p.ProvinceID, Value = p.ProvinceName });
            ViewData["Provinces"] = new SelectList(provinces, "Id", "Value");

            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind
            ("MemberID,Identifier,Address1,Address2,City,County,CountyOfBirth,DOB,DateRegistered,Email,FirstName,Gender,LastName,MembershipPaid,PhoneNumber,PostCode,Province,TeamName,Category")]
            Members members)
        {
            // Query the database with values of drop down lists to get the text
            // Get the gender name and assign to member
            int genderId = Convert.ToInt32(members.Gender);
            var gender = _context.Genders.Where(g => g.GenderID == genderId).First();
            members.Gender = gender.GenderName;
            // Get the province name and assign to member
            int provinceId = Convert.ToInt32(members.Province);
            var province = _context.Provinces.Where(p => p.ProvinceID == provinceId).First();
            members.Province = province.ProvinceName;
            // Get the county name and assign to member
            int countyId = Convert.ToInt32(members.County);
            int countyOfBirthId = Convert.ToInt32(members.CountyOfBirth);
            var county = _context.Counties.Where(c => c.CountyID == countyId).First();
            var countyOfBirth = _context.Counties.Where(c => c.CountyID == countyOfBirthId).First();
            members.County = county.CountyName;
            members.CountyOfBirth = countyOfBirth.CountyName;

            // GETS THE EMAIL ADDRESS OF THE USER THAT IS CURRENTLY LOGGED IN
            var userEmail = User.FindFirstValue(ClaimTypes.Name);

            // GET THE CHOSEN SUBSCRIPTION ID AND STORE TO A VARIABLE FOR FUTURE USE
            int subNum = members.Identifier;

            // GET THE AGE OF THE MEMBER
            int age = GetMemberAge(members);

            //ASSIGN THE MEMBER TO AN AGE CATEGORY
            AssignMemberCategory(members, age);

            //CREATE A LIST TO STORE THE ATHLETE DETAILS
            List<Members> memberList = new List<Members>();

            //ADD USER EMAIL ADDRESS TO THE 1ST MEMBER
            members.Email = userEmail;
            // ADDITIONAL COLUMNS THAT MUST BE POPULATED
            members.MembershipPaid = false;
            members.DateRegistered = DateTime.Now;

            if (ModelState.IsValid)
            {
                // SUBNUM 1,2 & 3 RELATE TO SINGLE PERSON SUBSCRIPTIONS
                if (subNum == 1 || subNum == 2 || subNum == 3)
                {
                    _context.Add(members);
                    await _context.SaveChangesAsync();
                    MemberListToSession(members, memberList);

                    return RedirectToAction("PayNow", "Subscriptions");
                }
                //IF MORE THAN ONE MEMBER BEING REGISTERED GO TO CREATE2 ACTION
                else
                {
                    MemberListToSession(members, memberList);
                    return RedirectToAction("Create2", "Members", new { subId = subNum });
                }
            }
            // If we got this far, something failed, redisplay form
            return View(members);
        }

        private static void AssignMemberCategory(Members members, int age)
        {
            if (age < 18)
                members.Category = "Junior";
            else if (age >= 18 && age < 40)
                members.Category = "Senior";
            else
                members.Category = "Master";
        }

        private static int GetMemberAge(Members members)
        {
            int age;
            DateTime now = DateTime.Now;

            age = now.Year - members.DOB.Year;
            if ((now.Month < members.DOB.Month) || (now.Month == members.DOB.Month) && (now.Day < members.DOB.Day))
                age--;
            return age;
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

            // retrieve data for drop down lists
            var genders = _context.Genders.Select(g => new { Id = g.GenderID, Value = g.GenderName });
            ViewData["Genders"] = new SelectList(genders, "Id", "Value");
            var counties = _context.Counties.Select(c => new { Id = c.CountyID, Value = c.CountyName });
            ViewData["Counties"] = new SelectList(counties, "Id", "Value");

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
            // Query the database with values of drop down lists to get the text
            // Get the gender name and assign to member
            int genderId = Convert.ToInt32(members.Gender);
            var gender = _context.Genders.Where(g => g.GenderID == genderId).First();
            members.Gender = gender.GenderName;
            // Get the county name and assign to member
            int countyOfBirthId = Convert.ToInt32(members.CountyOfBirth);
            var countyOfBirth = _context.Counties.Where(c => c.CountyID == countyOfBirthId).First();
            members.CountyOfBirth = countyOfBirth.CountyName;

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

            // GET THE AGE OF THE MEMBER
            int age = GetMemberAge(members);

            //ASSIGN THE MEMBER TO AN AGE CATEGORY
            AssignMemberCategory(members, age);

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
                // SUBNUM 4, 7 & 12 RELATE TO 2 PERSON SUBSCRIPTIONS
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
                // SUBNUM 5, 8 & 10 RELATE TO 3 PERSON SUBSCRIPTIONS
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
                // SUBNUM 6 ,9 & 11 RELATE TO 4 PERSON SUBSCRIPTIONS
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
            }
            // If we got this far, something failed, redisplay form
            return View(members);
        }

        private async Task<IActionResult> UpdateDBLoop(List<Members> memberList)
        {
            List<Members> newMemberList = new List<Members>();
            foreach (Members member in memberList)
            {
                //ADD MEMBER TO DB THEN ADD IT TO SESSION AFTER IT HAS AN ID
                _context.Add(member);
                await _context.SaveChangesAsync();                
                MemberListToSession(member, newMemberList);                
            }
            return RedirectToAction("PayNow", "Subscriptions");
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
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index", "Manage");
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

        public IActionResult AnnualResetConfirm()
        {
            return View();
        }

        public async Task<IActionResult> AnnualReset()
        {
            List<Members> members = _context.Members.ToList();
            foreach (var member in members)
            {
                string message = string.Format("Your annual subscription has expired.\nPlease visit sligoathleticsclub.com to renew your membership");
                string memberEmail = member.Email;
                member.MembershipPaid = false;
                _context.SaveChanges();

                await RemoveFromMemberRole(memberEmail);
                await AddUserToRegisteredUserRole(memberEmail);

                //await _emailSender.SendEmailAsync(member.Email, "Annual Subscription", message);
            }
            return View();
        }

        private async Task RemoveFromMemberRole(string memberEmail)
        {
            if (memberEmail != null)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(memberEmail);
                if (user != null)
                await _userManager.RemoveFromRoleAsync(user, "Member");
            }
        }
        private async Task AddUserToRegisteredUserRole(string memberEmail)
        {
            if (memberEmail != null)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(memberEmail);
                if(user != null)
                    await _userManager.AddToRolesAsync(user, new string[] {"RegisteredUser"});
            }
        }
    }
}
