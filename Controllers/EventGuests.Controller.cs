using  Microsoft.AspNetCore.Mvc; 
using Konce.Models;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]

public class EventGuestsController: ControllerBase
{
    private readonly KonceContext _dbContext;
    public EventGuestsController(KonceContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<IEnumerable<EventGuestDto>>> Get(string eventId)
    {
        List<EventGuest> eventGuests = new List<EventGuest>();
        List<EventGuestDto> eventGuestDtos = new List<EventGuestDto>();

        eventGuests = await this._dbContext.EventGuests
                                                .Where(eg => eg.EventId == eventId)
                                                .ToListAsync();
        eventGuests.ForEach(eg=>{
            eventGuestDtos.Add(
                new EventGuestDto {
                    EventGuestId = eg.EventGuestId,
                    GuestId = eg.GuestId,
                    EventId = eg.EventId,
                    ResponseStatus = eg.ResponseStatus
                }
            );
        }); 
        return Ok(eventGuestDtos);
    }
    //This function is used for posting from EventsController
    public async Task<ActionResult<EventGuest>> PostFromEventsController([FromBody] EventGuest eventGuest)
    {
        try
        {
            await this._dbContext.EventGuests.AddAsync(eventGuest);
            return CreatedAtAction(nameof(Get), new { id = eventGuest.EventGuestId }, eventGuest);
        }
        catch (Exception e)
        {
            Console.WriteLine($"LOIIIIII: {e}");
        }
        Console.WriteLine("Posted attendees of the event successfully");
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<EventGuest>> Post([FromBody] EventGuest eventGuest)
    {
        try
        {
            await this._dbContext.EventGuests.AddAsync(eventGuest);
            await this._dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = eventGuest.EventGuestId }, eventGuest);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e}");
        }
            
        Console.WriteLine("Posted attendee of the event successfully");
        return Ok();
    }
    [HttpDelete("{EventGuestID}")]
    public async Task<ActionResult<Event>> Delete(int EventGuestID)
    {
        var deleteGuest = await this._dbContext.EventGuests.FindAsync(EventGuestID);
        if (deleteGuest == null) {return NotFound("Can not found guest");}
        else
        {
            this._dbContext.EventGuests.Remove(deleteGuest);
            await this._dbContext.SaveChangesAsync();
            return Ok("Deleted this guest successfully");
        } 
    }
}