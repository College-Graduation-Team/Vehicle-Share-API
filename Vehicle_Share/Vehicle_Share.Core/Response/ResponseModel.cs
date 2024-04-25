namespace Vehicle_Share.Core.Response
{
    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public short code { get; set; }
        public string? message { get; set; }
        public string? Id { get; set; }
    }
}
