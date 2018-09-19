using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Inkton.Nester.Cloud;
using Inkton.Nester.ViewModels;
using Websock.Model;

namespace WebsockMon
{
    public class MessageModelView : ViewModel
    {
        private Session _editSession; // currently editing

        private ObservableCollection<Session> _sessions;
        private ObservableCollection<Message> _messages;

        public MessageModelView()
        {
        }

        public Session EditSession
        {
            get
            {
                return _editSession;
            }
            set
            {
                _editSession = value;
            }
        }

        public ObservableCollection<Session> Sessions
        {
            get
            {
                return _sessions;
            }
            set
            {
                _sessions = value;
                OnPropertyChanged("Sessions");
            }
        }

        public ObservableCollection<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged("Messages");
            }
        }

        async public Task<ResultMultiple<Session>> GetSessionsAsync()
        {
            Session sessionFilter = new Session();

            ResultMultiple<Session> result = await ResultMultiple<Session>.WaitForObjectAsync(
                NesterControl.Backend, true, sessionFilter);

            if (result.Code >= 0)
            {
                Sessions = result.Data.Payload;
            }

            return result;
        }

        async public Task<ResultMultiple<Message>> GetSessionMessagesAsync(Session session = null)
        {
            Session theSession = session == null ? _editSession : session;

            Message messageFilter = new Message();
            messageFilter.OwnedBy = theSession;

            ResultMultiple<Message> result = await ResultMultiple<Message>.WaitForObjectAsync(
                NesterControl.Backend, true, messageFilter);

            if (result.Code == 0)
            {
                _messages = result.Data.Payload;
            }

            return result;
        }

        async public Task<ResultSingle<Session>> UpdateSessionAsync(Session session = null)
        {
            Session theSession = session == null ? _editSession : session;

            ResultSingle<Session> result = await ResultSingle<Session>.WaitForObjectAsync(
                true, theSession, new CachedHttpRequest<Session, ResultSingle<Session>>(
                    NesterControl.Backend.UpdateAsync), false);

            if (result.Code == 0)
            {
                _editSession = result.Data.Payload;
            }

            return result;
        }

        async public Task<ResultSingle<Session>> DeleteSessionAsync(Session session = null)
        {
            Session theSession = session == null ? _editSession : session;

            ResultSingle<Session> result = await ResultSingle<Session>.WaitForObjectAsync(
                true, theSession, new CachedHttpRequest<Session, ResultSingle<Session>>(
                    NesterControl.Backend.RemoveAsync), false);

            if (result.Code == 0)
            {
                _editSession = result.Data.Payload;
            }

            return result;
        }
    }
}
