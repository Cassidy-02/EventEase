using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EventEase.Controllers
{

    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Event
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .ToListAsync();

            return View(events);

        }
        [HttpGet]
        public IActionResult Create()
        {
            var eventModel = new Event
            {
                Venue = new Venue(),
               

                Venues = _context.Venue
                    .Select(v => new SelectListItem
                    {
                        Value = v.VenueId.ToString(),
                        Text = v.VenueName
                    })
                    .ToList(),

                EventType = new EventType(),

                EventTypes = _context.EventType
                    .Select(et => new SelectListItem
                    {
                        Value = et.EventTypeId.ToString(),
                        Text = et.EventTypeName
                    })
                    .ToList()

            };
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            if (ModelState.IsValid)
            {

               
                    _context.Add(evt);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                
               
            }
            evt.Venues = _context.Venue
                 .Select(v => new SelectListItem
                 {
                     Value = v.VenueId.ToString(),
                     Text = v.VenueName
                 }).ToList();
            evt.EventTypes = _context.EventType
                .Select(e => new SelectListItem
                {
                    Value = e.EventTypeId.ToString(),
                    Text = e.EventTypeName
                }).ToList();

            return View(evt);

        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var evt = _context.Event

               .FirstOrDefault(b => b.EventId == id);

            if (evt == null)
            {
                return NotFound();
            }

            // Corrected the dropdown data to use Venue instead of Event  
            evt.Venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event evt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evt);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Event.Any(b => b.EventId == evt.EventId))
                        return NotFound();
                    else
                        throw;
                }
            }

            // Reload dropdown data in case of validation failure  
            evt.Venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

            return View(evt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var evt = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (evt == null)
                return NotFound();

            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Event.FindAsync(id);
            if (evt != null)
            {
                _context.Event.Remove(evt);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }

        
        
    }
}
