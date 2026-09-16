using System;
using System.Collections.Generic;

namespace SlotSync.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int SlotId { get; set; }

    public int UserId { get; set; }

    public DateOnly? BookingDate { get; set; }

    public int FinalCost { get; set; }

    public string Status { get; set; } = null!;

    public virtual Slot Slot { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
