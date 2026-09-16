using System;
using System.Collections.Generic;

namespace SlotSync.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public string Name { get; set; } = null!;

    public string VenueType { get; set; } = null!;

    public int Capacity { get; set; }

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}
