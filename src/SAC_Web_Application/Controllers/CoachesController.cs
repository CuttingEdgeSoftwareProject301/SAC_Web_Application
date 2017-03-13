using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAC_Web_Application.Models.ClubModel;

namespace SAC_Web_Application.Controllers
{
    public class CoachesController : Controller
    {
        private readonly ClubContext _context;

        public CoachesController(ClubContext context)
        {
            _context = context;    
        }

        // GET: Coaches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coaches.ToListAsync());
        }

        // GET: Coaches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coaches = await _context.Coaches.SingleOrDefaultAsync(m => m.CoachID == id);
            if (coaches == null)
            {
                return NotFound();
            }

            // retrieve qualifications for this coach
            var qualifications =
                from coach in _context.Coaches
                join coachQual in _context.CoachQualifications
                on coach.CoachID equals coachQual.CoachID
                join qual in _context.Qualifications
                on coachQual.QualID equals qual.QualID
                where coachQual.CoachID == id
                select new Qualifications
                {
                    QualID = qual.QualID,
                    QualName = qual.QualName
                };

            List<Qualifications> coachQualifications = qualifications.ToList();

            // retrieve additional qualifications that coach doesn't have
            var additionalQual =
                from qual in _context.Qualifications
                where !coachQualifications
                .Select(q => q.QualID)
                .Contains(qual.QualID)
                select new Qualifications
                {
                    QualID = qual.QualID,
                    QualName = qual.QualName
                };

            List<Qualifications> additionalQualifications = additionalQual.ToList();


            ViewData["CoachID"] = id;
            ViewData["CoachName"] = coaches.FirstName;
            ViewData["CoachQual"] = coachQualifications;
            ViewData["AdditionalQual"] = additionalQualifications;
            var coachName = coaches.FirstName;
            ViewData["coachName"] = coachName;

            return View(coaches);
        }

        // GET: Coaches/Create
        public IActionResult Create()
        {
            // retrieve data for drop down lists
            var days = _context.Days.Select(d => new { Id = d.DayID, Value = d.DayName });
            ViewData["Days"] = new SelectList(days, "Id", "Value");

            return View();
        }

        // POST: Coaches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoachID,Availability,ContactNumber,FirstName,GardaVetExpDate,LastName")] Coaches coaches)
        {
            if (ModelState.IsValid)
            {
                // Query the database with values of drop down lists to get the text
                // Get the day name and assign to coach
                int dayId = Convert.ToInt32(coaches.Availability);
                var day = _context.Days.Where(d => d.DayID == dayId).First();
                coaches.Availability = day.DayName;
                _context.Add(coaches);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coaches);
        }

        // GET: Coaches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coaches = await _context.Coaches.SingleOrDefaultAsync(m => m.CoachID == id);
            if (coaches == null)
            {
                return NotFound();
            }

            // retrieve data for drop down lists
            var days = _context.Days.Select(d => new { Id = d.DayID, Value = d.DayName });
            ViewData["Days"] = new SelectList(days, "Id", "Value");

            var coachName = coaches.FirstName;
            ViewData["coachName"] = coachName;
            return View(coaches);
        }

        // POST: Coaches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CoachID,Availability,ContactNumber,FirstName,GardaVetExpDate,LastName")] Coaches coaches)
        {
            if (id != coaches.CoachID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Query the database with values of drop down lists to get the text
                    // Get the day name and assign to coach
                    int dayId = Convert.ToInt32(coaches.Availability);
                    var day = _context.Days.Where(d => d.DayID == dayId).First();
                    coaches.Availability = day.DayName;

                    _context.Update(coaches);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoachesExists(coaches.CoachID))
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
            return View(coaches);
        }

        // GET: Coaches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coaches = await _context.Coaches.SingleOrDefaultAsync(m => m.CoachID == id);
            if (coaches == null)
            {
                return NotFound();
            }

            return View(coaches);
        }

        // POST: Coaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coaches = await _context.Coaches.SingleOrDefaultAsync(m => m.CoachID == id);
            _context.Coaches.Remove(coaches);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CoachesExists(int id)
        {
            return _context.Coaches.Any(e => e.CoachID == id);
        }

        // Assign a coach to a particular qualification
        public async Task<IActionResult> CoachAssignQual(int coachID, int qualID)
        {
            // assign qualification to coach
            CoachQualification coachQual = new CoachQualification();
            coachQual.CoachID = coachID;
            coachQual.QualID = qualID;
            _context.CoachQualifications.Add(coachQual);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = coachID });
        }

        public async Task<IActionResult> CoachRemoveQual(int coachID, int qualID)
        {
            // remove member from event
            CoachQualification coachQual = _context.CoachQualifications
                .Where(cq => cq.CoachID == coachID && cq.QualID == qualID).First();
            _context.CoachQualifications.Remove(coachQual);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = coachID });
        }
    }
}
