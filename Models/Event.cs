using System;
using System.Collections.Generic;

namespace Konce.Models;

public partial class Event
{
    public string Id { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? UserId { get; set; }
    public bool? Attendees { get; set; }
    public string? EisenhowerLevel { get; set; }
    public bool? Completed { get; set; }
    public string? HangoutLink { get; set; }
    public virtual ICollection<EventGuest> EventGuests { get; set; } = new List<EventGuest>();
    // public virtual EventGuest? Attendee {get; set;}
    public virtual NiceUser? User { get; set; }

    public Event()
    {
        DateTime currentTime = DateTime.Now;
        this.Id = Guid.NewGuid().ToString();
        this.CreatedDate = currentTime;
    }
    public Event(string Id, string Title, string Description,DateTime CreatedDate ,DateTime StartTime,
                DateTime Endtime, bool Attendees, string EisenhowerLevel, bool Completed, string HangoutLink, ICollection<EventGuest> EventGuests)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description; 
            this.CreatedDate = CreatedDate;
            this.StartTime = StartTime;
            this.EndTime = Endtime;
            this.Attendees = Attendees;
            this.EisenhowerLevel = EisenhowerLevel;
            this.Completed = Completed;
            this.HangoutLink = HangoutLink;
            this.EventGuests = EventGuests;
        }
}


