using  Microsoft.AspNetCore.Mvc; 
using Konce.Models;
using Microsoft.EntityFrameworkCore; 
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

[ApiController]
[Route("api/[controller]")]

public class EventsController: ControllerBase
{
    private readonly KonceContext _dbContext;
        private readonly EventGuestsController _eventGuestsController;
        private readonly HttpClient _httpClient;
        public EventsController(KonceContext dbContext, EventGuestsController eventGuestsController, HttpClient httpClient)
        {
            this._dbContext = dbContext;
            this._eventGuestsController = eventGuestsController;
            this._httpClient = httpClient;
        }

    [HttpGet("{UserId}")]
    public async Task<ActionResult<IEnumerable<EventDtoForGet>>> Get(int UserId)
    { 
        var events =  await this._dbContext.Events
                                .Where(e => e.UserId == UserId) 
                                .ToListAsync();
        
        try{
            var eventDtos = new List<EventDtoForGet>();
            events.ForEach(e =>{
                eventDtos.Add(new EventDtoForGet{
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    UserId = e.UserId,
                    Attendees = e.Attendees,
                    EisenhowerLevel = e.EisenhowerLevel,
                    Completed = e.Completed,
                    HangoutLink = e.HangoutLink,
                    EventGuests = (from u in this._dbContext.NiceUsers
                                join eg in this._dbContext.EventGuests
                                on u.UserId equals eg.GuestId
                                where eg.EventId == e.Id
                                select u.UserName).ToList()
                });
            });
        
            return Ok(eventDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine($"EXCEPTION OCCURRED: {e}");
            return NotFound(new {message = $"EXCEPTION OCCURRED: {e}"});
        }
    }

    public async Task<ActionResult<Event>> SubPost(int UserId, [FromBody] Event newevent)
    {
        if (newevent == null)
        {
            return BadRequest(new { message = "Invalid event data." });
        }
        Console.WriteLine("HTTP POST Manualy 1 Task");
        try
        {
            newevent.UserId = UserId;
            await this._dbContext.Events.AddAsync(newevent);
            var EventGuests = newevent.EventGuests;
            foreach(var eg in EventGuests)
            {
                eg.EventGuestId = 0;
                eg.EventId = newevent.Id;
                await this._eventGuestsController.PostFromEventsController(eg);
            }
            await this._dbContext.SaveChangesAsync();
            return Ok(new {message = "Posted successfully"});
        }
        catch (Exception e)
        {
            Console.WriteLine($"Throw Exception: {e};");
            return BadRequest(new {message = $"Exception: {e}"});
        }
    }
    [HttpPost("{UserId}")]
    public async Task<ActionResult<EventDtoForPost>> Post(int UserId, [FromBody] EventDtoForPost newEvent)
    {
        Console.WriteLine("Hello from post event");
        if (newEvent == null) return BadRequest(new {message = "Invalid event data"});
        
        bool isRecurring = newEvent.isRecurring;
        string RecurrentValue = newEvent.RecurrValue;
        DateTime repeatUntil = newEvent.RepeatUntil;
        Event postEvent = new Event();
        postEvent.Id = newEvent.Id; postEvent.Title = newEvent.Title; postEvent.Description = newEvent.Description;
        postEvent.CreatedDate = newEvent.CreatedDate; postEvent.StartTime = newEvent.StartTime; postEvent.EndTime = newEvent.EndTime;
        postEvent.Attendees = newEvent.Attendees; postEvent.EisenhowerLevel = newEvent.EisenhowerLevel; 
        postEvent.Completed = newEvent.Completed; postEvent.UserId = UserId; 
        postEvent.HangoutLink = newEvent.HangoutLink; postEvent.EventGuests = newEvent.EventGuests;       
         
        if (isRecurring == true)
        {       
            await this._dbContext.AddAsync(postEvent);
            Console.WriteLine($"It is recurrent and recur by {RecurrentValue} and repeatUntil {repeatUntil}");
            switch (RecurrentValue)
            {
                case "everyday": //recur everyday
                    var res1 = this.MakeRecurrence(newEvent.StartTime,newEvent.EndTime, newEvent.RepeatUntil,1);
                    int numberOfIteration1 = res1.startTime.Count;
                    for (int i = 0; i < numberOfIteration1; i++)
                    {
                        Console.WriteLine($"POST {i} times");
                        postEvent.Id = Guid.NewGuid().ToString();
                        postEvent.StartTime = res1.startTime[i];
                        postEvent.EndTime = res1.endTime[i];
                        await this.SubPost(UserId, postEvent);    
                    }
                    break;
                case "everyweek": // recur everyweek
                    var res7 = this.MakeRecurrence(newEvent.StartTime,newEvent.EndTime, newEvent.RepeatUntil,7);
                    int numberOfIteration7 = res7.startTime.Count;
                    for (int i = 0; i < numberOfIteration7; i++)
                    {
                        Console.WriteLine($"POST {i} times");
                        postEvent.Id = Guid.NewGuid().ToString();
                        postEvent.StartTime = res7.startTime[i];
                        postEvent.EndTime = res7.endTime[i];
                        await this.SubPost(UserId, postEvent);    
                    }
                    break;
                case "everymonth":
                    Console.WriteLine("BUOC 1");
                    List<DateTime> startTimes = this.GetRecurrentMonth(newEvent.StartTime, newEvent.RepeatUntil);
                    Console.WriteLine("BUOC 2");
                    List<DateTime> endTimes = this.GetRecurrentMonth(newEvent.StartTime, newEvent.RepeatUntil);
                    int numberOfIteration = startTimes.Count;
                    for (int i = 0; i < numberOfIteration; i++)
                    {
                        Console.WriteLine($"POST {i} times");
                        postEvent.Id = Guid.NewGuid().ToString();
                        postEvent.StartTime = startTimes[i];
                        postEvent.EndTime = endTimes[i];
                        await this.SubPost(UserId, postEvent); 
                    }
                    
                    break;
                default:
                    string customizedNumString = RecurrentValue.Substring(0,2);
                    int customizedNumber = int.Parse(customizedNumString);
                    var resC = this.MakeRecurrence(newEvent.StartTime,newEvent.EndTime, newEvent.RepeatUntil,customizedNumber);
                    int numberOfIterationC = resC.startTime.Count;
                    for (int i = 0; i < numberOfIterationC; i++)
                    {
                        Console.WriteLine($"POST {i} times");
                        postEvent.Id = Guid.NewGuid().ToString();
                        postEvent.StartTime = resC.startTime[i];
                        postEvent.EndTime = resC.endTime[i];
                        await this.SubPost(UserId, postEvent);    
                    }
                    break;
                
            }
        }
        else
        {
            Console.WriteLine($"The task is not recurrent");
            await this._dbContext.AddAsync(postEvent);
            await this._dbContext.SaveChangesAsync();

        }
        return Ok("very good");   
    }

    [HttpPut("{TaskID}")]
    public async Task<ActionResult<Event>> Put(string TaskID, [FromBody] Event putEvent)
    {
        var updateEvent = await this._dbContext.Events.FindAsync(TaskID);
        updateEvent.Title = putEvent.Title; updateEvent.Description = putEvent.Description;
        updateEvent.CreatedDate = putEvent.CreatedDate; updateEvent.StartTime = putEvent.StartTime; 
        updateEvent.EndTime = putEvent.EndTime; updateEvent.Attendees = putEvent.Attendees; 
        updateEvent.UpdatedDate = DateTime.Now;
        updateEvent.EisenhowerLevel = putEvent.EisenhowerLevel; updateEvent.Completed = putEvent.Completed; 
        updateEvent.HangoutLink = putEvent.HangoutLink; updateEvent.EventGuests = putEvent.EventGuests; 
        
        await this._dbContext.SaveChangesAsync();
        return Ok("Updated successfully");
    }


    [HttpDelete("{EventID}")]
    public async Task<ActionResult<Event>> Delete(string EventID)
    {
        var deletedEvent = await this._dbContext.Events.FindAsync(EventID);
        if (deletedEvent == null) {Console.WriteLine("Task not found");}
        else{
            var attendees = await this._dbContext.EventGuests
                                    .Where(eg => eg.EventId == EventID)
                                    .ToListAsync();
            foreach(var att in attendees)
            {
                this._dbContext.EventGuests.Remove(att);
            }
            this._dbContext.Events.Remove(deletedEvent);
            await this._dbContext.SaveChangesAsync();
        }
        return Ok("Deleted successfully");
    }

    public (List<DateTime> startTime, List<DateTime> endTime) MakeRecurrence (DateTime startTime, DateTime endTime, 
                                                                                        DateTime repeatUntil, int recurrentValue)
    {
        List<DateTime> recurrentDateStartTime = new List<DateTime>();
        List<DateTime> recurrentDateEndTime = new List<DateTime>();
        DateTime  nextDateStartTime = startTime.AddDays(recurrentValue);
        DateTime  nextDateEndTime = endTime.AddDays(recurrentValue);
        while (nextDateStartTime <= repeatUntil)
        {
            recurrentDateStartTime.Add(nextDateStartTime);
            recurrentDateEndTime.Add(nextDateEndTime);
            nextDateStartTime = nextDateStartTime.AddDays(recurrentValue);
            nextDateEndTime = nextDateEndTime.AddDays(recurrentValue);
            
        }
        return (recurrentDateStartTime, recurrentDateEndTime);
        
    }
    public List<DateTime> GetRecurrentMonth(DateTime startTime, DateTime dueDate)
    {
        var dates = new List<DateTime>();
        DateTime currentTime = startTime;

        while(currentTime <= dueDate)
        {
            var dateInMonth = DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
            int date = Math.Min(dateInMonth, startTime.Day);

            dates.Add(new DateTime(currentTime.Year, currentTime.Month, date,
                                startTime.Hour, startTime.Minute, startTime.Second));
            currentTime = currentTime.AddMonths(1);
        }

        return dates;
    }

    // [HttpPost("auto_arrange_events/{UserId}")]
    // public async Task<ActionResult<EventDtoForAIArrange>> AutoArrangeEvents(int UserId,[FromBody] List<EventDtoForAIArrange> events)
    // {
    //     var User = await this._dbContext.NiceUsers.FindAsync(UserId);
    //     ToAgent_AutoArrangeEvents content = new ToAgent_AutoArrangeEvents{
    //         UserChronotype = User.Chronotype,
    //         UserBufferTime = User.BufferTime,
    //         events = events
    //     };
    //     var AgenUrltEndpoint = "http://127.0.0.1:5050/auto_arrange_events";
    //     var contentToAgent = JsonContent.Create(content);

    //     HttpResponseMessage response = await this._httpClient.PostAsync(AgenUrltEndpoint,contentToAgent);
    //     response.EnsureSuccessStatusCode();
    //     Console.WriteLine($"{response.EnsureSuccessStatusCode()}");
        
    //     return Ok(content);
       
    // }
    [HttpPost("auto_arrange_events/{UserId}")]
    public async Task<ActionResult<EventDtoForAgentInController>> AutoArrangeEvents(int UserId,[FromBody] EventDtoForAgentInController requestBody)
    {
        var User = await this._dbContext.NiceUsers.FindAsync(UserId);
        ToAgent_AutoArrangeEvents content = new ToAgent_AutoArrangeEvents{
            UserChronotype = User.Chronotype,
            UserBufferTime = User.BufferTime,
            doDate = requestBody.doDate,
            events = requestBody.events
        };
        Console.WriteLine($"Execute day: {content.doDate}");
        var AgenUrltEndpoint = "http://127.0.0.1:5050/auto_arrange_events";
        var contentToAgent = JsonContent.Create(content);

        HttpResponseMessage response;
        

        int maxRetries = 3;
        int attempt = 0;
        do
        {
            attempt++;
            response = await this._httpClient.PostAsync(AgenUrltEndpoint,contentToAgent);
            if (!response.IsSuccessStatusCode)
            {
                break;
            }
            else{Console.WriteLine($"Have problem with Agent, trying to do the process again");}
        } 
        while(!response.IsSuccessStatusCode && attempt <= maxRetries);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();   
        var jsonResponse = JsonSerializer.Deserialize<EventDtoResponseFromAgent>(responseBody);
        Console.WriteLine($"jsonResponse: {jsonResponse}");
        Event postEvent = new Event();
        if(jsonResponse!=null)
        {
            foreach (var eg in jsonResponse.schedule)
            {
                postEvent.Id = eg.Id; postEvent.Title = eg.Title; postEvent.Description = eg.Description;
                postEvent.EisenhowerLevel = eg.EisenhowerLevel; postEvent.StartTime = eg.StartTime;
                postEvent.EndTime = eg.Endtime; postEvent.HangoutLink = eg.HangoutLink; postEvent.UserId = UserId;
                postEvent.Attendees = false;

                await this.SubPost(UserId, postEvent);
            }
        } else {Console.WriteLine("Something went wrong ");}
        
        return Ok("Post all event successfully");
    }

    [HttpPost("test/{UserId}")]
    public async Task<ActionResult<EventDtoForAgentInController>> test(int UserId, [FromBody] EventDtoResponseFromAgent requestBody)
    {
        Event postEvent = new Event();
        foreach(var eg in requestBody.schedule)
        {
            postEvent.Id = eg.Id; postEvent.Title = eg.Title; postEvent.Description = eg.Description;
            postEvent.EisenhowerLevel = eg.EisenhowerLevel; postEvent.StartTime = eg.StartTime;
            postEvent.EndTime = eg.Endtime; postEvent.HangoutLink = eg.HangoutLink; postEvent.UserId = UserId;
            postEvent.Attendees = false;

            await this.SubPost(UserId, postEvent);
        }
        return Ok("Post successfully");
    }
    
}