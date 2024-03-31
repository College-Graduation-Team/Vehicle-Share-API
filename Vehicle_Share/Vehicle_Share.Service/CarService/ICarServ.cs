using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.CarService
{
    public interface ICarServ
    {
        Task<List<GetCarModel>> GetAllAsync();
        Task<string> AddAsync(CarModel model);
        Task<string> UpdateAsync(string id, CarModel model);
        Task<int> DeleteAsync(string id);
        Task<List<string>> GetCarBrandsForUser();
    }
}
