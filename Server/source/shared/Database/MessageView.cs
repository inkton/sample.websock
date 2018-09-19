using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Websock.Model;

namespace Websock.Database
{
    public class MessageView
    {
        public Message Edit { get; set; }
        
        public IEnumerable<Message> List { get; set; }
    }
}