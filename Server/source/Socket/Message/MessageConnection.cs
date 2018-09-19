using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Inkton.Nest;
using Websock.WebSocketManager;
using Websock.Model;

namespace Websock
{
    public class MessageConnection : WebSocketConnection
    {
        public string NickName { get; set; }
        public Session Session { get; set; }

        public MessageConnection(MessageHandler handler) : base(handler)
        {
        }

        public override async Task ReceiveAsync(string message)
        {
            await (Handler as MessageHandler).ArchiveMessageAsync(Session, message);
            var receiveMessage = JsonConvert.DeserializeObject<ReceiveMessage>(message);

            var receiver = Handler.Connections.FirstOrDefault(m => ((MessageConnection)m).NickName == receiveMessage.Receiver);

            if (receiver != null)
            {
                var sendMessage = JsonConvert.SerializeObject(new SendMessage
                {
                    Sender = NickName,
                    Message = receiveMessage.Message,
                    Archive = (Handler as MessageHandler).GetArchivedMessages()
                });

                await receiver.SendMessageAsync(sendMessage);
            }
            else
            {
                var sendMessage = JsonConvert.SerializeObject(new SendMessage
                {
                    Sender = NickName,
                    Message = "Can not seed to " + receiveMessage.Receiver
                });

                await SendMessageAsync(sendMessage);
            }
        }
        
        private class ReceiveMessage
        {
            public string Receiver { get; set; }

            public string Message { get; set; }
        }

        private class SendMessage
        {
            public string Sender { get; set; }

            public string Message { get; set; }

            public List<Message> Archive { get; set; }
        }
    }
}