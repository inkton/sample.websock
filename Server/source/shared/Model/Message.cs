using System;
using Inkton.Nest.Cloud;

namespace Websock.Model
{
    public class Message : CloudObject
    {
        private int _id;
        private int _sessionId;
        private string _text;
        private string _status;

        public Message()
        {
        }
        
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public int SessionId
        {
            get { return _sessionId; }
            set { SetProperty(ref _sessionId, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        public override string CloudKey
        {
            get { return Id.ToString(); }
        }
    }
}