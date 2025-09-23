using Microsoft.AspNetCore.SignalR;
using BusTracker.Models;
using System.Threading.Tasks;

namespace BusTracker.Hubs
{
    public class PositionsHub : Hub
    {
        // Método opcional para que cliente se suscriba a una ruta
        public Task SubscribeRoute(string route)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, route);
        }

        public Task UnsubscribeRoute(string route)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, route);
        }

        // No necesita más métodos; el servidor enviará actualizaciones con Clients.Group/All
    }
}
