using System;
using System.Collections.Generic;

namespace Konce.Models;

public partial class EventGuest
{
    public int EventGuestId { get; set; }
    public int? GuestId { get; set; }
    public string? EventId { get; set; }
    public string? ResponseStatus { get; set; }
    public virtual Event? Event { get; set; }
    public virtual NiceUser? Guest { get; set; }
    
}
