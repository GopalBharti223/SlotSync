namespace SlotSync.DTOs
{
    public class CreateBookingRequestDto
    {
        public int SlotId { get; set; }

        public DateOnly BookingDate { get; set; }
    }
}
