using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chat.Core.Entities;
using Chat.Core.Persistence;
using Microsoft.AspNetCore.Http;

namespace Chat.API.Middlewares
{
    public class ChatWebSocketMiddleware
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ChatWebSocketMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var user = context.Request.Query["user"];
            if (string.IsNullOrWhiteSpace(user))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Usuário não fornecido");
                return;
            }

            var ct = context.RequestAborted;
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            if (!string.IsNullOrWhiteSpace(user.ToString()))
            {
                _sockets.TryAdd(user.ToString(), socket);
            }

            await BroadcastMessageAsync("Sistema", $"{user} entrou na sala", DateTime.UtcNow, ct);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var receivedMessage = await ReceiveStringAsync(socket, ct);

                    if (string.IsNullOrWhiteSpace(receivedMessage))
                        continue;

                    var messageData = JsonSerializer.Deserialize<JsonElement>(receivedMessage);

                    if (messageData.TryGetProperty("content", out var content))
                    {
                        var messageContent = content.GetString();
                        if (!string.IsNullOrEmpty(messageContent))
                        {
                            await BroadcastMessageAsync(
                                user.FirstOrDefault() ?? "Unknown User",
                                messageContent,
                                DateTime.UtcNow,
                                ct
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro WebSocket: {ex.Message}");
            }
            finally
            {
                await DisconnectUserAsync(user.ToString(), socket, ct);
            }
        }

        private async Task BroadcastMessageAsync(
            string user,
            string content,
            DateTime timestamp,
            CancellationToken ct
        )
        {
            var messageJson = JsonSerializer.Serialize(
                new
                {
                    user,
                    content,
                    timestamp = timestamp.ToString("o"),
                }
            );

            foreach (var socket in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await SendStringAsync(socket, messageJson, ct);
                }
            }

            if (user != "Sistema")
            {
                await SaveMessageToDatabaseAsync(user, content, timestamp);
            }
        }

        private async Task SaveMessageToDatabaseAsync(
            string user,
            string content,
            DateTime timestamp
        )
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            dbContext.Messages.Add(
                new Message
                {
                    User = user,
                    Content = content,
                    Timestamp = timestamp,
                }
            );

            await dbContext.SaveChangesAsync();
        }

        private async Task DisconnectUserAsync(string user, WebSocket socket, CancellationToken ct)
        {
            _sockets.TryRemove(user, out _);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexão encerrada", ct);
            await BroadcastMessageAsync("Sistema", $"{user} saiu da sala", DateTime.UtcNow, ct);
        }

        private static Task SendStringAsync(WebSocket socket, string message, CancellationToken ct)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            return socket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                ct
            );
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await socket.ReceiveAsync(buffer, ct);
                ms.Write(buffer.Array!, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
