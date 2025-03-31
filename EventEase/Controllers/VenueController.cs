using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EventEase.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var venues = _context.Venues.ToList(); // Retrieve all venues asynchronously
            return View(venues);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Return the view for creating a new venue
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId, VenueName, Location, Capacity, ImageUrl")] Venue venue, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                venue.ImageUrl = await HandleImageUploadAsync(ImageFile); // Handle image upload

                _context.Venues.Add(venue); // Add the venue to the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
                return RedirectToAction("Index"); // Redirect to the Index action
            }
            return View(venue); // Redisplay the form if validation fails
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _context.Venues.FindAsync(id); // Retrieve the venue with the specified id asynchronously
            if (venue == null)
            {
                return NotFound(); // Return a 404 Not Found error if the venue is not found
            }
            return View(venue); // Return the view for editing the venue
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId, VenueName, Location, Capacity, ImageUrl")] Venue venue, IFormFile ImageFile)
        {
            if (id != venue.VenueId)
            {
                return BadRequest(); // Return 400 Bad Request if the id does not match
            }

            if (ModelState.IsValid)
            {
                try
                {
                    venue.ImageUrl = await HandleImageUploadAsync(ImageFile); // Handle image upload

                    _context.Update(venue); // Update the venue in the database
                    await _context.SaveChangesAsync(); // Save changes asynchronously
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                    {
                        return NotFound(); // Return 404 Not Found if the venue does not exist
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the Index action
            }
            return View(venue); // Redisplay the form if validation fails
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest(); // Return 400 Bad Request if id is null
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id); // Retrieve the venue by ID asynchronously
            if (venue == null)
            {
                return NotFound(); // Return 404 error if not found
            }

            return View(venue); // Return the delete confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id); // Retrieve the venue by ID asynchronously
            if (venue != null)
            {
                _context.Venues.Remove(venue); // Remove the venue from the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
            }
            return RedirectToAction(nameof(Index)); // Redirect to the Index action
        }

        private async Task<string> HandleImageUploadAsync(IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                string filename = Path.GetFileName(ImageFile.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Venues", filename);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream); // Copy the file asynchronously
                }
                return "/Images/Venues/" + filename; // Return the relative path
            }
            return null;
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
