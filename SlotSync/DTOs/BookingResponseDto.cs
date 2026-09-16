namespace SlotSync.DTOs
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public int SlotId { get; set; }
        public int UserId { get; set; }
        public DateOnly? BookingDate { get; set; }
        public int FinalCost { get; set; }
        public string Status { get; set; } = null!;
    }
}