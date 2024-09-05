using Konce.Models;
public  class EventDtoForPost
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int? UserId { get; set; }

    public bool? Attendees { get; set; }

    public string? EisenhowerLevel { get; set; }

    public bool? Completed { get; set; }

    public string? HangoutLink { get; set; }

    // public virtual List<string> EventGuests { get; set; } = new List<string>();
    public virtual ICollection<EventGuest> EventGuests { get; set; } = new List<EventGuest>();

    public bool isRecurring {get; set;}
    public string RecurrValue {get; set;}
    public DateTime RepeatUntil {get; set;}

    public EventDtoForPost()
    {
        DateTime currentTime = DateTime.Now;
        this.Id = Guid.NewGuid().ToString();
        this.CreatedDate = currentTime;
    }
}