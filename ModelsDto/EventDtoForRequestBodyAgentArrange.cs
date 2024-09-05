public class EventDtoForRequestBodyAgentArrange
{
    public string Id { get; set; } = null!;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int Duration { get; set; }
    public string? EisenhowerLevel { get; set; }
    public string? HangoutLink { get; set; }
    public EventDtoForRequestBodyAgentArrange()
    {
        this.Id = Guid.NewGuid().ToString();
    }
}