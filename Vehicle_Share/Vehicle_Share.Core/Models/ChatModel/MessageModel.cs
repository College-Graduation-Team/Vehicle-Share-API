using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.Core.Models.ChatModel
{
    public class MessageModel
    {
        public string GroupName { get; set; } // trip id 
        public string Sender { get; set; } // userdata id
        public string Content { get; set; } //message
        public DateTime CreatedOn { get; set; } // data 
    }
}
