using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.UserDataService
{
    public interface IUserDataServ
    {
      //  Task<IEnumerable<UserData>> GetAllAsyc();
        Task<GetUserModel> GetByIdAsync(string id);
        Task<string> AddAsync(UserDataModel model);
        Task<string> UpdateAsync(string id , UserDataModel model);
        Task DeleteAsync(UserData userData);
    }
}
