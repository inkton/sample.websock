using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Websock.Model;

namespace Websock.Database
{    
    public interface IMessageRepository
    {        
        Task AddSessionAsync(Session session);
        Session GetSession(int sessionId);
        Task<bool> UpdateSessionAsync(Session session);
        List<Session> ListAllSessions(bool activeOnly = true);

        Task AddMessageAsync(Message message);
        List<Message> ListAllMessages();
        List<Message> ListMessagesBySession(int sessionId);
    }
}
