using  Microsoft.AspNetCore.Mvc; 
using Konce.Models; 
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Collections.Generic; 
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly KonceContext _dbContext;
    private readonly HttpClient _httpClient;
    public UsersController (KonceContext dbContext, HttpClient httpClient)
    {
        this._dbContext = dbContext;
        this._httpClient = httpClient;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] string userName)
    {
        var user = await this._dbContext.NiceUsers.Where(u=>u.UserName == userName).FirstAsync();
        Console.WriteLine(user.UserName);
        if (user!=null)
        {
            return Ok(user.UserId);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NiceUserDto>> Get(int id)
    {
        try
        {
            var findingUser = await _dbContext.NiceUsers.FindAsync(id);
            if (findingUser == null)
            {
                return NotFound(new {message = "Not found User"});
            }
            else
            {
                var UserDto = new NiceUserDto
                {
                    UserId = findingUser.UserId,
                    UserName = findingUser.UserName,
                    Email = findingUser.Email,
                    PhoneNumber = findingUser.PhoneNumber,
                    Chronotype = findingUser.Chronotype,
                    BufferTime = findingUser.BufferTime
                };
                return Ok(UserDto);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"LOI: {e}");
            return NotFound(new {message = "Hello "});
        }
    }


    [HttpGet("chronotypequiz")]
    public async Task<ActionResult> GetChronotypeQuiz()
    {
        Console.WriteLine("Hello from Chronotypequiz");
        string js_content = "";
        using (StreamReader sr = new StreamReader("KonceProperties/Chronotypequiz.json"))
        {
            js_content = await sr.ReadToEndAsync();
        }
        return  Ok(new {
            message = "Here is the quiz",
            Chronotypequiz = js_content
        });
    }
    // [HttpPost("chronotype/{id}")]
    [HttpPut("chronotype/{id}")]
    public async Task<ActionResult> UpdateChronotype(int id, [FromBody] List<Chronotypequiz> UserAnswer)
    { 
        if (UserAnswer == null || !UserAnswer.Any())
        {
            return BadRequest(new { message = "UserAnswer is required and cannot be empty." });
        }
        var apiUrl = "http://127.0.0.1:5050/classify_chronotype"; 
        var content = JsonContent.Create(UserAnswer);
        HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();   
        responseBody = responseBody.Substring(1,responseBody.Length - 3); 
        responseBody = responseBody.Replace("\\",""); 
        Console.WriteLine(responseBody); 
        ChronotypeFromAgent? chronotypeFromAgent = JsonConvert.DeserializeObject<ChronotypeFromAgent>(responseBody);
        string Chronotype = "";
        if (chronotypeFromAgent != null)
        {
            Chronotype = chronotypeFromAgent.Chronotype;
            Console.WriteLine("Current chronotype: " + chronotypeFromAgent.Chronotype);
        } 
        var currentUser = await this._dbContext.NiceUsers.FindAsync(id);
        if (currentUser == null) return NotFound(new {message = "Not Found User "});
        else 
        {
          currentUser.Chronotype = Chronotype;
          await this._dbContext.SaveChangesAsync();
        }

        return Ok(new {message = "Updated successfully"});
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<NiceUser>> Put(int id, [FromBody] NiceUser niceUser)
    { 
        var existingUser = await this._dbContext.NiceUsers.FindAsync(id); 
        if (existingUser == null)
        {
            return NotFound(new {message = "Not found User"});
        }
        else 
        {
            niceUser.UserName = niceUser.UserName == "Default Name" ? existingUser.Email : niceUser.UserName;
            niceUser.Email = niceUser.Email == "Default Email" ? existingUser.UserName : niceUser.Email;
            
            existingUser.UserName = niceUser.UserName;
            existingUser.Email = niceUser.Email;
            existingUser.PhoneNumber = niceUser.PhoneNumber;
            existingUser.BufferTime = niceUser.BufferTime;
            await this._dbContext.SaveChangesAsync();

            return Ok(new {message = "Updated successfully"});
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<NiceUser>> Delete(int id)
    {
        var findUser = await this._dbContext.NiceUsers.FindAsync(id);
        if (findUser != null)
        {
            this._dbContext.NiceUsers.Remove(findUser);
            await this._dbContext.SaveChangesAsync();
            return Ok(new {message = "Deleted successfully"});
        }
        else
        {
            return NotFound(new {message = "Not found User"});
        }
    }

}
