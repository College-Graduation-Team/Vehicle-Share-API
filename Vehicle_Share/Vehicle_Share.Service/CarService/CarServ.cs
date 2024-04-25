using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;
using Twilio.TwiML.Messaging;
using System.ComponentModel;

namespace Vehicle_Share.Service.CarService
{
    public class CarServ : ICarServ
    {
        private readonly IBaseRepo<Car> _car;
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public CarServ(IBaseRepo<Car> car, IHttpContextAccessor httpContextAccessor, IBaseRepo<UserData> user, IStringLocalizer<SharedResources> locaLizer = null, IBaseRepo<Trip> trip = null)
        {
            _car = car;
            _httpContextAccessor = httpContextAccessor;
            _user = user;
            _LocaLizer = locaLizer;
            _trip = trip;
        }

        public async Task<ResponseModel> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };
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

        public async Task<ResponseModel> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var allCars = await _car.GetAllAsync();
            var userCars = allCars.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new ResponseDataModel<List<GetCarModel>>();
            result.data = new List<GetCarModel>();
            foreach (var car in userCars)
            {
                result.data?.Add(new GetCarModel
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
                });
            }
            result.IsSuccess = true;
            return result;
        }

        public async Task<ResponseModel> AddAsync(CarModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };
            var userData = await _user.FindAsync(e => e.UserId == userId);

            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };



            var LecFront = await ProcessImageFile("Car", model.LicenseImageFront);
            var LecBack = await ProcessImageFile("Car", model.LicenseImageBack);

            var carImg = await ProcessImageFile("Car", model.Image);



            Car car = new Car
            {
                Id = Guid.NewGuid().ToString(),
                Type = model.Type,
                ModelYear = model.ModelYear,
                Brand = model.Brand,
                Plate = model.Plate,
                Seats = model.Seats,

                LicenseImageFront = LecFront,
                LicenseImagBack = LecBack,
                LicenseExpiration = model.LicenseExpiration,

                Image = carImg,

                UserDataId = userData.Id,
                CreatedOn= DateTime.UtcNow

            };
            await _car.AddAsync(car);

            return new ResponseModel { Id = car.Id, message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, UpdateCarModel model)   
        {
            var car = await _car.GetByIdAsync(id);
            if (car == null) new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar] , code = ResponseCode.NoCar };

            // userData.Name = model.Name ?? userData.Name;
            car.Type = model.Type ?? car.Type;
            car.ModelYear = model.ModelYear == 0 ? car.ModelYear : model.ModelYear;
            car.Brand = model.Brand ?? car.Brand;
            car.Plate = model.Plate ?? car.Plate;
            car.Seats = model.Seats == 0 ? car.Seats : model.Seats;
            car.LicenseExpiration = model.LicenseExpiration != null ? model.LicenseExpiration : car.LicenseExpiration;

            // updata the image 
            if (model.Image != null)
            {
                await RemoveImageFile(car.Image);
                car.Image = await ProcessImageFile("Car", model.Image);
            }
            if (model.LicenseImageBack != null)
            {
                await RemoveImageFile(car.LicenseImagBack);
                car.LicenseImagBack = await ProcessImageFile("Car", model.LicenseImageBack);
            }
            if (model.LicenseImageFront != null)
            {
                await RemoveImageFile(car.LicenseImageFront);
                car.LicenseImageFront = await ProcessImageFile("Car", model.LicenseImageFront);
            }

            await _car.UpdateAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }

        public async Task<ResponseModel> DeleteAsync(string id)
        {
            var car = await _car.FindAsync(e => e.Id == id);
            if (car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar] , code = ResponseCode.NoCar };

            var trips = await _trip.GetAllAsync(e => e.CarId == id);
            var unfinishedTrips = trips.Where(t => !t.IsFinished).ToList();

            if (unfinishedTrips.Any())
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoDelete] };
            }

            foreach (var trip in trips)
            {
                trip.CarId = null;
                await _trip.UpdateAsync(trip);
            }

            await _car.DeleteAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Deleted] ,IsSuccess=true};
        }

        private async Task<string> ProcessImageFile(string folder, IFormFile file)
        {
            var req = _httpContextAccessor.HttpContext.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _car.UploadImageAsync(folder, file);
            return baseUrl + Image;
        }

        private async Task RemoveImageFile(string file)
        {
            Uri uri = new Uri(file);
            string relativeUrl = uri.PathAndQuery;
            await _car.RemoveImageAsync(relativeUrl);
        }

    }
}

