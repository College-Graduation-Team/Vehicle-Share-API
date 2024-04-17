namespace Vehicle_Share.Core.Response
{
    public class ResponseForOneModel<T>
    {
        public bool IsSuccess { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }
    }
}
