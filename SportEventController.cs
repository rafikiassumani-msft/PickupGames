using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickUpGames.Dtos;
using PickUpGames.Models;

namespace PickUpGames;

[ApiController]
public class SportEventController : ControllerBase  {

  private readonly PickupGamesDBContext _dbContext;

public SportEventController(PickupGamesDBContext dBContext) {
    _dbContext = dBContext;
}

  [HttpGet("/sportEvents/{id}")]
  public async Task<ActionResult<List<Event>>> GetEvents(int id) {
      
   var searchedEvent = await _dbContext.Events
                                       .Include(ev => ev.User)
                                       .Include(ev => ev.Participants)
                                       .FirstOrDefaultAsync(ev => ev.EventId == id);
     return Ok(searchedEvent);
     
   }

}