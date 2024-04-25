namespace Vehicle_Share.EF.ImpRepo.SendOTPImplement
{
    public class BaseRequestDto<TRequest>
    {
        public BaseRequestDto()
        {

        }
        public BaseRequestDto(TRequest data)
        {
            this.data = data;
        }
        public TRequest data { get; set; }
    }
   
}

