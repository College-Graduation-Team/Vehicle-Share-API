using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Share.EF.Models
{
    public class Trip
    {
        [Key]
        public string TripID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public float price { get; set; }
        public int AllSets { get; set; }
        public int AvilableSets { get; set; }
        public bool IsFinish { get; set; }

        // relations 
        // user and car  car can not .
        public String User_DataId { get; set; }
        [ForeignKey("User_DataId")]
        public UserData UserData { get; set; }

		public String Car_Id { get; set; }
		[ForeignKey("Car_Id")]
		public Car Car { get; set; }

		public String Request_Id { get; set; }
		[ForeignKey("Request_Id")]
		public Request Request { get; set; }
	}
}
