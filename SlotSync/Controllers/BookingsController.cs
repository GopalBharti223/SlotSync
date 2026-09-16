using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlotSync.DTOs;
using SlotSync.Models;
using SlotSync.Services;
using System.Security.Claims;
namespace SlotSync.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly SlotSyncDbContext _context;
        private readonly IBookingService _bookingService;
        public BookingsController(SlotSyncDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var allBookings = await _context.Bookings.ToListAsync();
                return Ok(allBookings);
            }
            var bookings = await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
            return Ok(bookings);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin" && booking.UserId != userId)
            {
                return Forbid();
            }
            return Ok(booking);
        }
       
        [HttpPost]
        public async Task<IActionResult> CreateBooking(
    CreateBookingRequestDto request)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var booking = new Booking
            {
                SlotId = request.SlotId,
                BookingDate = request.BookingDate,
                UserId = userId
            };

            var result = await _bookingService.CreateBookingAsync(booking);

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = result.BookingId },
                result
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var result = await _bookingService.ConfirmBookingAsync(id);
            return Ok(result);
        }
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin" && booking.UserId != userId)
            {
                return Forbid();
            }
            var result = await _bookingService.CancelBookingAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteBooking(int id)
        {
            var result = await _bookingService.CompleteBookingAsync(id);
            return Ok(result);
        }
    }
}