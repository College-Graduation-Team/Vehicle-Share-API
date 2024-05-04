using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.CarService;

namespace Vehicle_Share.Service.DashboardService
{
    public class DashboardServ : IDashboardServ
    {
        private readonly IBaseRepo<UserData> _userData;
        private readonly ICarServ _carservice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public DashboardServ(IBaseRepo<UserData> userData,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<SharedResources> locaLizer , 
            ICarServ carservice)
        {
            _userData = userData;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
            _carservice = carservice;
        }


        #region Userdata

        public Task<ResponseModel> GetUserDataByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetAllUserDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateUserData(string id, UserDataModel model)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Car


        public async Task<ResponseModel> GetAllCarAsync()
        {
            return await _carservice.GetAllAsync();
        }

        public async Task<ResponseModel> GetCarByIdAsync(string id)
        {
            return await _carservice.GetByIdAsync(id);
        }

        public async Task<ResponseModel> UpdateCar(string id, UpdateCarModel model)
        {
            return await _carservice.UpdateAsync(id, model);
        }

        #endregion

        #region License

        public Task<ResponseModel> GetAllLicenseAsync()
        {
            throw new NotImplementedException();
        }
        public Task<ResponseModel> GetLicenseByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateLicense(string id, LicModel model)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
