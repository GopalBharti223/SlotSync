using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotSync.Models;

namespace SlotSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly SlotSyncDbContext _context;

        public VenuesController(SlotSyncDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _context.Venues.ToListAsync();

            return Ok(venues);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getVeenue(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return Ok(venue);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateVenue(Venue venue)
        {

            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            //return Ok(venue);
            return CreatedAtAction(nameof(getVeenue), new { id = venue.VenueId }, venue);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(int id, Venue venue)
        {

            var existingVenue = await _context.Venues.FindAsync(id);
            if (existingVenue == null)
            {
                return NotFound();
            }

            existingVenue.Name = venue.Name;
            existingVenue.VenueType = venue.VenueType;
            existingVenue.Capacity = venue.Capacity;
            await _context.SaveChangesAsync();
            return Ok(existingVenue);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var existingVenue = await _context.Venues.FindAsync(id);

            if (existingVenue == null)
            {
                return NotFound();
            }

            _context.Venues.Remove(existingVenue);

            await _context.SaveChangesAsync();

            return Ok(existingVenue);
        }

        

    }
}