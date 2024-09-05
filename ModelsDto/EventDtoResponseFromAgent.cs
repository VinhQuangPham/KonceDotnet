public class _EventDtoResponseFromAgent
{
    public string Id { get; set; } = null!;
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    // public DateTime? UpdatedDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime Endtime { get; set; }

    public string EisenhowerLevel { get; set; }
    public string HangoutLink { get; set; }
    public _EventDtoResponseFromAgent()
    {
        this.CreatedDate = DateTime.Now;
    }
}

public class EventDtoResponseFromAgent
{
    public List<string> explanation { get; set; }
    public List<_EventDtoResponseFromAgent> schedule  { get; set; }
}