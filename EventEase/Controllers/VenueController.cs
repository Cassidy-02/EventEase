using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace EventEase.Controllers
{
    
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public VenueController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Display()
        {
            var venues = await _context.Venue
                .Select(v => new Venue
                {
                    VenueId = v.VenueId,
                    VenueName = v.VenueName ?? "Unknown", // Default value for null
                    Location = v.Location ?? "Not specified", // Default value for null
                    Capacity = v.Capacity,
                    ImageUrl = v.ImageUrl ?? string.Empty // Default value for null
                }).ToListAsync(); // Retrieve all venues asynchronously
            return View(venues);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Return the view for creating a new venue
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId, VenueName, Location, Capacity")] Venue venue, IFormFile? imageFile)
        {
            

            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                venue.ImageUrl = await _blobService.UploadFileAsync(stream, imageFile.FileName);
            }

            if (ModelState.IsValid)
            {
                return View(venue); // Redisplay the form if validation fails
            }

            _context.Venue.Add(venue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Display));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var venue = await _context.Venue.FindAsync(id); // Retrieve the venue with the specified id asynchronously
            if (venue == null)
            {
                return NotFound(); // Return a 404 Not Found error if the venue is not found
            }
            return View(venue); // Return the view for editing the venue
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile? imageFile)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return View(venue);
            }

            var existingVenue = await _context.Venue.FindAsync(id);
            if (existingVenue == null)
            {
                return NotFound();
            }

            // Update fields
            existingVenue.VenueName = venue.VenueName;
            existingVenue.Location = venue.Location;
            existingVenue.Capacity = venue.Capacity;

            // Upload new image if one is provided
            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                existingVenue.ImageUrl = await _blobService.UploadFileAsync(stream, imageFile.FileName);
            }

            _context.Update(existingVenue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Display));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return 404 if id is null
            }

            var venue = await _context.Venue.FindAsync(id); // Await the asynchronous database call
            if (venue == null)
            {
                return NotFound(); // Return 404 if venue is not found
            }

            return View(venue); // Pass the venue model to the Delete view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id); // Retrieve the venue by ID asynchronously
            if (venue != null)
            {
                _context.Venue.Remove(venue); // Remove the venue from the database
                await _context.SaveChangesAsync(); // Save changes asynchronously
            }
            return RedirectToAction(nameof(Display)); // Redirect to the Display action
        }

      
    }
}
