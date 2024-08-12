using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using OnlineQRMenuApp.Models;
using System.Threading.Tasks;

namespace OnlineQRMenuApp.Hubs
{
    public class AppHub<T> : Hub
    {
        public async Task SendMessage(User user, T message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(User user, T message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToGroup(string group, User user, T message)
        {
            return Clients.Group(group).SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendMessageUpdate(T post)
        {
            await Clients.All.SendAsync("ReceivePostUpdate", post);
        }

        public Task SendMessageToUser(User user, T message)
        {
            return Clients.User(user.UserId.ToString()).SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnDisconnectedAsync(exception);
        }
    }
}