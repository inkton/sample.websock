using System;
using Inkton.Nest.Cloud;

namespace Websock.Model
{
    public class Session : CloudObject
    {
        private int _id;
        private string _user;
        private DateTime _loginTime;
        private bool _active;
        
        public Session()
        {
            Active = true;  
        }

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }
       
        public DateTime LoginTime
        {
            get { return _loginTime; }
            set { SetProperty(ref _loginTime, value); }
        }

        public bool Active
        {
            get { return _active; }
            set { SetProperty(ref _active, value); }
        }

        public override string CloudKey
        {
            get { return Id.ToString(); }
        }
    }
}