using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Talktif.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(string user, string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(user, message);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).BroadcastMessage($"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).BroadcastMessage($"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}