using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAC_Web_Application.Models.ClubModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;

namespace SAC_Web_Application.Controllers
{
    public class EventsController : Controller
    {
        private readonly ClubContext _context;

        public EventsController(ClubContext context)
        {
            _context = context;    
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }



        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //bool IsMembersAttending = false;

            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.SingleOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }
            
            // Retrieve members that are attached to this user account 
            List<Members> userMembers = _context.Members.Where(m => m.Email == User.FindFirstValue(ClaimTypes.Name)).ToList();

            // Retrieve members that are attending this event
            var memsAttend =
                 from member in userMembers
                 join memberEvents in _context.MemberEvents
                 on member.MemberID equals memberEvents.MemberID
                 where memberEvents.EventID == id
                 && memberEvents.MemberID == member.MemberID
                 select new Members
                 {
                     MemberID = member.MemberID,
                     FirstName = member.FirstName,
                     LastName = member.LastName,
                     TeamName = member.TeamName
                 };

            List<Members> membersAttending = memsAttend.ToList();

            // retrieve members who are not attending this event
            var memsNotAttend =
                from member in userMembers
                where !membersAttending
                .Select(m => m.MemberID)
                .Contains(member.MemberID)
                select new Members
                {
                    MemberID = member.MemberID,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    TeamName = member.TeamName
                };

            List<Members> membersNotAttending = memsNotAttend.ToList();

            // Retrieve all members that are attending this event
            var allMemsAttend =
                 from member in _context.Members
                 join memberEvents in _context.MemberEvents
                 on member.MemberID equals memberEvents.MemberID
                 where memberEvents.EventID == id
                 && memberEvents.MemberID == member.MemberID
                 select new Members
                 {
                     MemberID = member.MemberID,
                     FirstName = member.FirstName,
                     LastName = member.LastName,
                     TeamName = member.TeamName
                 };

            List<Members> allMembersAttending = allMemsAttend.ToList();

            ViewData["MembersAttending"] = membersAttending;
            ViewData["MembersNotAttending"] = membersNotAttending;
            ViewData["AllMembersAttending"] = allMembersAttending;
            ViewData["EventID"] = id;

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> AllDetails(int? id)
        {
            //bool IsMembersAttending = false;

            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.SingleOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }
            // Retrieve all members that are attending this event
            var allMemsAttend =
                 from member in _context.Members
                 join memberEvents in _context.MemberEvents
                 on member.MemberID equals memberEvents.MemberID
                 where memberEvents.EventID == id
                 && memberEvents.MemberID == member.MemberID
                 select new Members
                 {
                     MemberID = member.MemberID,
                     FirstName = member.FirstName,
                     LastName = member.LastName,
                     TeamName = member.TeamName
                 };

            List<Members> allMembersAttending = allMemsAttend.ToList();
            
            ViewData["AllMembersAttending"] = allMembersAttending;
            ViewData["EventID"] = id;

            //return RedirectToAction("AllDetails");
            return View(events);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,Category,Date,EventTitle,Location")] Events events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.SingleOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }
            return View(events);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Category,Date,EventTitle,Location")] Events events)
        {
            if (id != events.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsExists(events.EventID))
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
            return View(events);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.SingleOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var events = await _context.Events.SingleOrDefaultAsync(m => m.EventID == id);
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }

        // Assign a member to a particular event
        public async Task<IActionResult> MemberAttend(int memberID, int eventID)
        {        
            // assign member to event
            MemberEvent memberEvent = new MemberEvent();
            memberEvent.MemberID = memberID;
            memberEvent.EventID = eventID;
            _context.MemberEvents.Add(memberEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventID });
        }

        // Remove a member to a particular event
        public async Task<IActionResult> MemberLeave(int memberID, int eventID)
        {
            // remove member from event
            MemberEvent memberEvent = _context.MemberEvents
                .Where(me => me.MemberID == memberID && me.EventID == eventID).First();
            _context.MemberEvents.Remove(memberEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventID });
        }

        #region Partials
        /*public PartialViewResult _MemberEvents(int? id)
        {
            ViewBag.clubId = id;
            var qry = db.ClubEvents.Where(ce => ce.ClubId == id).ToList();
            return PartialView(qry);
        }*/
        #endregion
    }
}
