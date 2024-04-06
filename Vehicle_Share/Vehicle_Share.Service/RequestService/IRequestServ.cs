using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.EF.Models;

using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Response;

namespace Vehicle_Share.Service.RequestService
{
    public interface IRequestServ
    {
        Task<GenResponseModel<GetReqModel>> GetAllTripRequestedAsync(string tripId);
        Task<GenResponseModel<GetReqModel>> GetAllMyRequestAsync();
        Task<ResponseModel> SendReqestAsync(ReqModel model);
        Task<ResponseModel> DenyRequestAsync(string requestId);
        Task<ResponseModel> AcceptRequestAsync(string requestId);
        Task<int> DeleteRequestAsync(string requestId);
    }
}