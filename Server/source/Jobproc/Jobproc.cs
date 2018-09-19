using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Inkton.Nester;
using Inkton.Nester.Models;
using Inkton.Nester.Queue;
using Microsoft.EntityFrameworkCore;
using Websock.Models;

namespace Websock.Work
{
    class Jobproc
    {
        static object Parse(IDictionary<string, object> headers, string message)
        {
            if (headers != null && headers.ContainsKey("Type"))
            {
                switch(Encoding.Default.GetString(headers["Type"] as byte[]))
                {
                    case "Message":
                        return JsonConvert.DeserializeObject<Message>(message);
                }
            }

            return null;
        } 
        
        static bool SaveMessage(Runtime runtime, Message message)
        {
            int changes = 0;

            using (var db = MessageContextFactory.Create(runtime))
            {
                var messageToUpdate = db.Messages.Find(message.ID);
                messageToUpdate.Status = message.Status;
                changes = db.SaveChanges();
            }

            return changes > 0;
        }
        
        static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole(LogLevel.Debug)
                .AddNesterLog(LogLevel.Debug);
            ILogger logger = loggerFactory.CreateLogger<Jobproc>();
            Runtime runtime = new Runtime(QueueMode.Client);

            try
            {
                bool isActive = true;
                Runtime.ReceiveParser parser = new Runtime.ReceiveParser(Parse);

                while (isActive) 
                {
                    Object queuedObject = runtime.Receive(parser);

                    if (queuedObject != null) 
                    {
                        logger.LogInformation(string.Format(
                            "The message {0} received by jobproc {1}.{2}.",
                            JsonConvert.SerializeObject(queuedObject), DateTime.Now.ToString("t"),
                            runtime.NestTag, runtime.CushionIndex
                        ));

                        if (queuedObject is Message)
                        {
                            Message message = queuedObject as Message;
                            message.Status += string.Format(
                                ", processed by jobproc {0}.{1} at {2}.", 
                                runtime.NestTag, runtime.CushionIndex, 
                                DateTime.Now.ToString("t")
                            );

                            if (SaveMessage(runtime, message))
                            {
                                logger.LogInformation("The message saved to database");
                            }
                            else
                            {
                                logger.LogWarning("The message could not be saved");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(e.Message);
            }
        }
    }
}
