namespace Vehicle_Share.Core.Response
{
    public class ResponseDataModel<T>: ResponseModel
    {
        public T? data { get; set; }
    }
}
