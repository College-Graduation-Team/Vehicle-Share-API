using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IBaseRepo<Car> _car;
        private readonly ICarServ _carservice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public DashboardServ(IBaseRepo<UserData> userData, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer, IBaseRepo<Car> car, ICarServ carservice)
        {
            _userData = userData;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
            _car = car;
            _carservice = carservice;
        }


        #region Userdata

        public Task<ResponseModel> GetAllUserDataAsync()
        {
            throw new NotImplementedException();
        }


        public Task<ResponseModel> GetUserDataByIdAsync(string id)
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
            var car = await _car.GetByIdAsync(id);

            if (car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            var result = new ResponseDataModel<GetCarModel>
            {
                data = new GetCarModel
                {
                    Id = car.Id,
                    Type = car.Type,
                    ModelYear = car.ModelYear,
                    Brand = car.Brand,
                    Plate = car.Plate,
                    Seats = car.Seats,
                    Image = car.Image,
                    LicenseImageFront = car.LicenseImageFront,
                    LicenseImageBack = car.LicenseImagBack,
                    LicenseExpiration = car.LicenseExpiration
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task <ResponseModel> UpdateCar(string id, UpdateCarModel model)
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
