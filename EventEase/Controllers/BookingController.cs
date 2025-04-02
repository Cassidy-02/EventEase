using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToList(); 
            return View(bookings);
        }

        public IActionResult Create()
        {
           ViewBag.VenueList = _context.Venue.ToList();
            ViewBag.EventList = _context.Event.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.Event);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var booking = _context.Booking.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewBag.EventList = _context.Event.ToList();
            ViewBag.VenueList = _context.Venue.ToList();

            return View(booking);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(booking).State = EntityState.Modified; 
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventList = _context.Event.ToList();
            ViewBag.VenueList = _context.Venue.ToList();

            return View(booking);
        }


        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Booking.Include(b => b.Event).Include(b => b.Venue).FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
