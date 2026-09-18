namespace SlotSync.DTOs
{
    public class VenueWithSlotsResponseDto
    {
        public int VenueId { get; set; }
        public string Name { get; set; } = null!;
        public string VenueType { get; set; } = null!;
        public int Capacity { get; set; }

        public List<SlotResponseDto> Slots { get; set; } = new();
    }

    public class SlotResponseDto
    {
        public int SlotId { get; set; }
        public int Cost { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}