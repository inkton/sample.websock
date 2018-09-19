using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Websock.Model;

namespace Websock.Database
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageContext _dbContext;

        public MessageRepository (MessageContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSessionAsync(Session session)
        {
            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync();
        }

        public Session GetSession(int sessionId)
        {
            Session existingSession = _dbContext.Sessions
                .FirstOrDefault(session => session.Id == sessionId);
            return existingSession;
        }

        public async Task<bool> UpdateSessionAsync(Session session)
        {
            int updates = await _dbContext.SaveChangesAsync();
            return updates > 0;
        }

        public List<Session> ListAllSessions(bool activeOnly = true)
        {
            IQueryable<Session> sessions;

            if (activeOnly)
            {
                sessions = _dbContext.Sessions.Where(
                    session => session.Active == true);
            }
            else
            {
                sessions = _dbContext.Sessions;            
            }

            return sessions.ToList();
        }

        public async Task AddMessageAsync(Message message)
        {
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        public List<Message> ListAllMessages()
        {
            return _dbContext.Messages.ToList();
        }

        public List<Message> ListMessagesBySession(int sessionId)
        {
            var messages = _dbContext.Messages.Where(
                message => message.SessionId == sessionId);
            return messages.ToList();
        }
    }
}
