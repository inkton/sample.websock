using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Inkton.Nest.Model;
using Inkton.Nester;
using Inkton.Nester.Queue;
using Websock.Database;
using Websock.Model;
using Websock.WebSocketManager;

namespace Websock
{
    public class MessageHandler : WebSocketHandler
    {
        protected override int BufferSize { get => 1024 * 4; }

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly Runtime _runtime;

        public MessageHandler(
            IServiceScopeFactory scopeFactory,
            ILogger<MessageConnection> logger,
            Runtime runtime)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _runtime = runtime;
            _runtime.QueueSendType = "Message";
        }

        public override async Task<WebSocketConnection> OnConnected(HttpContext context)
        {
            var name = context.Request.Query["Name"];
            if (!string.IsNullOrEmpty(name))
            {
                var connection = Connections.FirstOrDefault(m => ((MessageConnection)m).NickName == name);

                if (connection == null)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var session = new Session { User = name, LoginTime = DateTime.Now };

                    using (var serviceScope = _scopeFactory.CreateScope())
                    {
                        var services = serviceScope.ServiceProvider;

                        try
                        {
                            var repo = services.GetRequiredService<IMessageRepository>();
                            await repo.AddSessionAsync(session);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred.");
                        }
                    }

                    connection = new MessageConnection(this)
                        {
                            NickName = name,
                            WebSocket = webSocket,
                            Session = session
                        };

                    Connections.Add(connection);
                }

                return connection;
            }

            return null;
        }

        public List<Message> GetArchivedMessages()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var repo = services.GetRequiredService<IMessageRepository>();
                    return repo.ListAllMessages();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred.");
                }
            }

            return null;
        }

        public async Task ArchiveMessageAsync(Session session, string text)
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    string status = string.Format("Message received by {0}.{1} at {2}",  
                        _runtime.NestTag,   
                        _runtime.CushionIndex, 
                    DateTime.Now.ToString("t"));
                    _logger.LogInformation(status);

                    var repo = services.GetRequiredService<IMessageRepository>();

                    Message message = new Message();
                    message.SessionId = session.Id;
                    message.Text = text;
                    message.Status = status;
            
                    await repo.AddMessageAsync(message);

                    _runtime.SendToNest(
                        JsonConvert.SerializeObject(message), 
                        "jobproc");                
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred.");
                }
            }
        }
    }
}
