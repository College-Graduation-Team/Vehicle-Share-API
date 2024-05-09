using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.RequestService
{
    public interface IRequestServ
    {
        Task<ResponseModel> GetAllMyRequestAsync();
        Task<ResponseModel> GetReceiveRequestDriverAsync();
        Task<ResponseModel> GetSendRequestDriverAsync();

        Task<ResponseModel> GetReceiveRequestPassengerAsync();
        Task<ResponseModel> GetSendRequestPassengerAsync();


        Task<ResponseModel> GetAllTripRequestedAsync(string tripId);
        Task<ResponseModel> SendReqestAsync(ReqModel model);
        Task<ResponseModel> DenyRequestAsync(string requestId);
        Task<ResponseModel> AcceptRequestAsync(string requestId);
        Task<int> DeleteRequestAsync(string requestId);
    }
}