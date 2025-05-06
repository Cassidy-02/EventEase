using EventEase.Data;
using EventEase.Models;
using EventEase.ViewModels;
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
                 Event = new Event(),
                 Venue = new Venue(),

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

             return View(booking);
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
                    return RedirectToAction("Index"); // Ensure this redirects to the Index action
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
        public async Task<IActionResult> Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Booking.Any(b => b.BookingId == booking.BookingId))
                        return NotFound();
                    else
                        throw;
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

        //Booking Management
       
        public IActionResult Manage(string? search, string sortOrder, int page = 1)
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                bookings = bookings.Where(b => b.BookingId.ToString().Contains(search) ||
                                               (b.Event != null && b.Event.EventName.Contains(search)) ||
                                               (b.Venue != null && b.Venue.VenueName.Contains(search)) ||
                                               (b.Venue != null && b.Venue.Location.Contains(search)));



            }

            // Sorting logic 
            switch (sortOrder)
            {
                case "EventName":
                    bookings = bookings.OrderBy(b => b.Event != null ? b.Event.EventName : string.Empty);
                    break;
                case "Date":
                    bookings = bookings.OrderBy(b => b.BookingDate);
                    break;
                case "VenueName":
                    bookings = bookings.OrderBy(b => b.Venue != null ? b.Venue.VenueName : string.Empty);
                    break;
                case "Location":
                    bookings = bookings.OrderBy(b => b.Venue != null ? b.Venue.Location : string.Empty);
                    break;
                default:
                    bookings = bookings.OrderBy(b => b.BookingId);
                    break;
            }

            // Pagination logic
            int pageSize = 10;
            var paginatedBookings = bookings.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ManageBookingsViewModel
            {
                Search = search,
                Bookings = paginatedBookings.Select(b => new Booking
                {
                    BookingId = b.BookingId,
                    Event = b.Event,
                    BookingDate = b.BookingDate,
                    Venue = b.Venue,
                    



                }).ToList()

            };

            ViewBag.TotalPages = (int)Math.Ceiling(bookings.Count() / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSort = sortOrder;

            return View(viewModel);
        }
    }
}
