using System;
using System.Collections.Generic;

namespace SlotSync.Models;

public partial class Slot
{
    public int SlotId { get; set; }

    public int VenueId { get; set; }

    public int Cost { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Venue? Venue { get; set; }
}
