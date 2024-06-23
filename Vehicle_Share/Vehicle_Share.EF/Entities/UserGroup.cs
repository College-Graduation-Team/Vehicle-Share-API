using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Entities
{
    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }
        public string? GroupName { get; set; } //trip_id
        public string? SenderId { get; set; } //userdataid
        public string? Content { get; set; } //massage 
        public DateTime Timestamp { get; set; }
        public bool Delivered { get; set; }
    }
    public class UserConnection
    {
        [Key]
        public int Id { get; set; }
        public string? GroupName { get; set; } //trip_id
        public string? SenderId { get; set; } //userdataid
        public string? ConnectionId { get; set; }
    }
}

