using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.EF.Models;

using Vehicle_Share.Core.Models.RequestModels;

namespace Vehicle_Share.Service.RequestService
{
    public interface IRequestServ
    {
        Task<List<Request>> GetAllAsync();
        Task<string> SendReqestAsync(ReqModel model);
        Task<string> DenyRequestAsync(string requestId);
        Task<string> AcceptRequestAsync(string requestId);
        Task<string> DeleteRequestAsync(string requestId);
    }
}