using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotSync.Models;

namespace SlotSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly SlotSyncDbContext _context;

        public SlotController(SlotSyncDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getSlots()
        {
            var slots = await _context.Slots.ToListAsync();
            return Ok(slots);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> getSlot(int id)
        {
            var existingSlot = await _context.Slots.FindAsync(id);

            if (existingSlot == null)
            {
                return NotFound();
            }

            return Ok(existingSlot);

        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(Slot slot)
        {
            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(getSlot),
                new { id = slot.SlotId },
                slot
                );

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSlot(int id, Slot slot)
        {
            var existingSlot = await _context.Slots.FindAsync(id);

            if (existingSlot == null)
            {
                return NotFound();
            }

            existingSlot.VenueId = slot.VenueId;
            existingSlot.Cost = slot.Cost;
            existingSlot.StartTime = slot.StartTime;
            existingSlot.EndTime = slot.EndTime;

            await _context.SaveChangesAsync();

            return Ok(existingSlot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var existingSlot = await _context.Slots.FindAsync(id);

            if (existingSlot == null)
            {
                return NotFound();
            }

            _context.Slots.Remove(existingSlot);
            await _context.SaveChangesAsync();

            return Ok(existingSlot);
        }

    }
}
