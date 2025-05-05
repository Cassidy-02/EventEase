using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EventEase.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventController> _logger;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
            _logger = context.GetService<ILogger<EventController>>();
        }

        public IActionResult Index()
        {
            var events = _context.Event
                .Include(e => e.Venue)
                .ToList();
            return View(events);
        }

        public IActionResult Create()
        {
            ViewBag.VenueId = GetVenuesSelectList();
            return View();
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
            ViewBag.VenueId = GetVenuesSelectList(evt.VenueId);
            return View(evt);
        }

        public IActionResult Edit(int id)
        {
            var eventModel = _context.Event.Include(e => e.Venue).FirstOrDefault(e => e.EventId == id);
            if (eventModel == null)
            {
                return NotFound();
            }
            eventModel.Venues = _context.Venue
                .Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error occurred while updating event with ID {EventId}", id);
                    if (!await EventExistsAsync(evt.EventId))
                        return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VenueId = GetVenuesSelectList(evt.VenueId);
            return View(evt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var evt = await GetEventByIdAsync(id);
            if (evt == null) return NotFound();

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

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }

        private SelectList GetVenuesSelectList(int? selectedVenueId = null)
        {
            return new SelectList(_context.Venue, "VenueId", "VenueName", selectedVenueId);
        }

        private async Task<Event?> GetEventByIdAsync(int? id)
        {
            if (id == null) return null;
            return await _context.Event
                .Include(e => e.Venue)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == id);
        }

        private async Task<bool> EventExistsAsync(int id)
        {
            return await _context.Event.AnyAsync(e => e.EventId == id);
        }
    }
   
}
