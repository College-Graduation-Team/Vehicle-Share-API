namespace Vehicle_Share.Core.Response
{
    public class GenResponseModel<T>
    {
        public string? message { get; set; }
        public bool IsSuccess { get; set; }
        public List<T>? data { get; set; } = new List<T>();
    }
}
