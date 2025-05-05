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
        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Index action hit"); // Will show in terminal/console
            var bookings = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();

            return View(bookings);
        }

     

        [HttpGet]
        public IActionResult Create()
        {
            var booking = new Booking
            {
                Event = new Event(), // Initialize required member 'Event'
                Venue = new Venue
                {
                    VenueName = "Default Venue Name", // Set required member 'VenueName'
                    Location = "Default Location",   // Set required member 'Location'
                    ImageUrl = "DefaultImageUrl.jpg", // Set required member 'ImageUrl'
                   
                },
                Events = _context.Event.Select(e => new SelectListItem
                {
                    Value = e.EventId.ToString(),
                    Text = e.EventName
                }).ToList(),
                Venues = _context.Venue.Select(v => new SelectListItem
                {
                    Value = v.VenueId.ToString(),
                    Text = v.VenueName
                }).ToList()
            };

            return View(booking); // Ensure this maps to Views/Booking/Create.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check for double booking
                var isDoubleBooked = _context.Booking.Any(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate == booking.BookingDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "The selected venue is already booked for the specified date and time.");
                }
                else
                {

                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return Redirect("/Booking/Index"); // Ensure this redirects to the Index action
                }
            }

            // Reload dropdown data in case of validation failure
            booking.Events = _context.Event.Select(e => new SelectListItem
            {
                Value = e.EventId.ToString(),
                Text = e.EventName
            }).ToList();

            booking.Venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

            return View(booking);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var booking = _context.Booking
                .Include(b => b.Event)
        .Include(b => b.Venue)
        .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }


            booking.Events = _context.Event.Select(e => new SelectListItem
            {
                Value = e.EventId.ToString(),
                Text = e.EventName
            }).ToList();

            booking.Venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();


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

            // Reload dropdown data in case of validation failure
            booking.Events = _context.Event.Select(e => new SelectListItem
            {
                Value = e.EventId.ToString(),
                Text = e.EventName
            }).ToList();

            booking.Venues = _context.Venue.Select(v => new SelectListItem
            {
                Value = v.VenueId.ToString(),
                Text = v.VenueName
            }).ToList();

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
            if (booking == null)
            {
                return NotFound(); // Handle the case where the booking is null
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
