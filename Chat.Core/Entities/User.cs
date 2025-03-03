using System.Net.WebSockets;

namespace Chat.Core.Entities
{
    public class User
    {
        public required string Username { get; set; }
        public required WebSocket WebSocket { get; set; }
    }
}
