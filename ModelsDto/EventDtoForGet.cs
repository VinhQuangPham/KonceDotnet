using Konce.Models;
public  class EventDtoForGet
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

    public virtual List<string> EventGuests { get; set; } = new List<string>();
}