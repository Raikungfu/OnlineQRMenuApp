using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;
using OnlineQRMenuApp.Service;
using System.Threading.Tasks;
namespace OnlineQRMenuApp.Hubs
{
    public class AppHub<T> : Hub
    {
        private readonly ConnectionMappingService _connectionService;

        public AppHub(ConnectionMappingService connectionService)
        {
            _connectionService = connectionService;
        }

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
            var deviceId = Context.GetHttpContext().Request.Query["deviceId"].ToString();
            var role = Context.GetHttpContext().Request.Query.ContainsKey("role") ? Context.GetHttpContext().Request.Query["role"].ToString() : string.Empty;
            int id = int.TryParse(Context.GetHttpContext().Request.Query["id"], out var parsedId) ? parsedId : 0;

            var connectionInfo = new ConnectInfo
            {
                DeviceId = deviceId,
                Role = role,
                Id = id
            };

            _connectionService.AddConnection(Context.ConnectionId, connectionInfo);

            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectionService.RemoveConnection(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
