using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SlotSync.DTOs;
using SlotSync.Exceptions;
using SlotSync.Models;

namespace SlotSync.Services
{
    public class BookingService : IBookingService
    {
        private readonly SlotSyncDbContext _context;
        private readonly ILogger<BookingService> _logger;
        public BookingService(SlotSyncDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            return booking;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(Booking booking)
        {
            // 1. Check whether the requested slot exists
            var slot = await _context.Slots.FindAsync(booking.SlotId);

            if (slot == null)
            {
                throw new NotFoundException("Slot not found.");
            }

            // 2. Validate slot time
            if (slot.StartTime >= slot.EndTime)
            {
                throw new BadRequestException(
                    "Slot start time must be earlier than end time."
                );
            }

            // 3. Booking date cannot be in the past
            if (booking.BookingDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new BadRequestException(
                    "Booking date cannot be in the past."
                );
            }

            // 4. Check whether the user exists
            var user = await _context.Users.FindAsync(booking.UserId);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // 5. Check whether this slot is already booked for this date
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.SlotId == booking.SlotId &&
                    b.BookingDate == booking.BookingDate);

            if (existingBooking != null)
            {
                throw new ConflictException(
                    "This slot is already booked for this date."
                );
            }

            // 6. Server decides the cost
            booking.FinalCost = slot.Cost;

            // 7. New booking starts as Pending
            booking.Status = "Pending";

            _context.Bookings.Add(booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException(
                    "This slot has already been booked for this date."
                );
            }

            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                SlotId = booking.SlotId,
                UserId = booking.UserId,
                BookingDate = booking.BookingDate,
                FinalCost = booking.FinalCost,
                Status = booking.Status
            };
        }

        public async Task<Booking> ConfirmBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            if (booking.Status != "Pending")
            {
                throw new BadRequestException(
                    "Only pending bookings can be confirmed."
                );
            }

            booking.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return booking;
        }

        public async Task<Booking> CancelBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            if (booking.Status != "Pending")
            {
                throw new BadRequestException(
                    "Only pending bookings can be cancelled."
                );
            }

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return booking;
        }


        public async Task<Booking> CompleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            if (booking.Status != "Confirmed")
            {
                throw new BadRequestException(
                    "Only confirmed bookings can be completed."
                );
            }

            booking.Status = "Completed";

            await _context.SaveChangesAsync();

            return booking;
        }
    }
}