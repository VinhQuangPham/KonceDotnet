using System;
using System.Collections.Generic;

namespace Konce.Models;

public partial class NiceUser
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Chronotype { get; set; }
    public string? UserCredentials { get; set; }
    public int? BufferTime { get; set; }
    public virtual ICollection<EventGuest> EventGuests { get; set; } = new List<EventGuest>();
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public NiceUser()
    {
        this.UserName = "Default Name";
        this.Email = "Default Email";
    } 

}
