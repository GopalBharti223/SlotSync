using SlotSync.DTOs;
using SlotSync.Models;

namespace SlotSync.Services
{
    public interface IBookingService
    {
        Task<Booking?> GetBookingByIdAsync(int id);

        Task<BookingResponseDto> CreateBookingAsync(Booking booking);

        Task<Booking> ConfirmBookingAsync(int id);
        Task<Booking> CancelBookingAsync(int id);

        Task<Booking> CompleteBookingAsync(int id);
    }
}   