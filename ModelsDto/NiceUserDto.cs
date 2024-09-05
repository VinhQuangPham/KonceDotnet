public class NiceUserDto
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Chronotype { get; set; }
    public int? BufferTime { get; set; }

}