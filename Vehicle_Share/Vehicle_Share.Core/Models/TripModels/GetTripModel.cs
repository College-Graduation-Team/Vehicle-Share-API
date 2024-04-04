

namespace Vehicle_Share.Core.Models.TripModels
{
    public class GetTripModel
    {
        public string ID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public float Recommendprice { get; set; }
        public int? AvilableSets { get; set; }    //driver
        public int? NumOfSetWant { get; set; }    //passenger

        public bool IsFinish { get; set; }
        public String User_DataId { get; set; }
        public String? Car_Id { get; set; }
    
    }
}
