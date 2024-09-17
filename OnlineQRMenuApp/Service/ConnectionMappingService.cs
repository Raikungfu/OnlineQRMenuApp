using OnlineQRMenuApp.Models.ViewModel;

namespace OnlineQRMenuApp.Service
{
    public class ConnectionMappingService
    {
        private readonly Dictionary<string, ConnectInfo> _connections = new Dictionary<string, ConnectInfo>();

        public void AddConnection(string connectionId, ConnectInfo info)
        {
            _connections[connectionId] = info;
        }

        public void RemoveConnection(string connectionId)
        {
            _connections.Remove(connectionId);
        }

        public List<string> GetConnectionIdsByRoleAndId(string role, int id)
        {
            return _connections
                .Where(c => c.Value.Role == role && c.Value.Id == id)
                .Select(c => c.Key)
                .ToList();
        }

        public List<string> GetConnectionIdsByRoleAndDeviceId(string role, string deviceId)
        {
            return _connections
                .Where(c => c.Value.Role == role && c.Value.DeviceId == deviceId)
                .Select(c => c.Key)
                .ToList();
        }
    }

}
