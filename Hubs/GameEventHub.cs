using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Mappers;
using PickUpGames.Models;

namespace PickUpGames.Hubs
{
    public class GameEventHub : Hub
    {
        private readonly PickupGamesDBContext _dBContext;

        public GameEventHub(PickupGamesDBContext dBContext) {
             _dBContext = dBContext;
        } 

        public async Task SendEventMessages(string message)
        {
            var events = await _dBContext.Events.ToListAsync<Event>();
            await Clients.All.SendAsync("ReceiveEventMessages", EventMapper.MapEvents(events));
        }
    }
}
