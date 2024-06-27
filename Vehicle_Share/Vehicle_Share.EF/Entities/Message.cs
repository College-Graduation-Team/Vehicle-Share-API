using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Entities
{
    public class Message
    {
        [Key]
        public string Id { get; set; }
        public string GroupName  { get; set; } // trip id 
        public string Sender  { get; set; } // userdata id
        public string Content  { get; set; } //message
        public DateTime CreatedOn { get; set; } // data 
    }
}
