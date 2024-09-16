using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;
using System.Threading.Tasks;
namespace OnlineQRMenuApp.Hubs
{
    public class AppHub<T> : Hub
    {
        private static Dictionary<string, ConnectInfo> _connections = new Dictionary<string, ConnectInfo>();

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
            int id;
            var idQuery = Context.GetHttpContext().Request.Query["id"].ToString();
            if (!int.TryParse(idQuery, out id))
            {
                id = 0;
            }

            var connectionInfo = new ConnectInfo
            {
                DeviceId = deviceId,
                Role = role,
                Id = id
            };

            _connections[Context.ConnectionId] = connectionInfo;

            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR User");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connections.Remove(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR User");

            await base.OnDisconnectedAsync(exception);
        }


        public ConnectInfo GetInfoByConnectionId(string connectionId)
        {
            if (_connections.TryGetValue(connectionId, out var info))
            {
                return info;
            }
            return null;
        }

        public List<string> GetConnectionIdsByRoleAndId(string role, int id)
        {
            var connectionIds = _connections
                .Where(c => c.Value.Role == role && c.Value.Id == id)
                .Select(c => c.Key)
                .ToList();

            return connectionIds;
        }

        public List<string> GetConnectionIdsByRoleAndDeviceId(string role, string deviceId)
        {
            var connectionIds = _connections
                .Where(c => c.Value.Role == role && c.Value.DeviceId == deviceId)
                .Select(c => c.Key)
                .ToList();

            return connectionIds;
        }
    }
}
