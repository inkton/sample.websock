using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Websock.WebSocketManager
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var connection = await _webSocketHandler.OnConnected(context);
                if (connection != null)
                {
                    await _webSocketHandler.ListenConnection(connection);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
        }
    }
}
