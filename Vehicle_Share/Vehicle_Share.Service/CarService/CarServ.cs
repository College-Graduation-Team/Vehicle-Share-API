using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.CarService
{
    public class CarServ : ICarServ
    {
        private readonly IBaseRepo<Car> _car;
        private readonly IBaseRepo<UserData> _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CarServ(IBaseRepo<Car> car, IHttpContextAccessor httpContextAccessor, IBaseRepo<UserData> user)
        {
            _car = car;
            _httpContextAccessor = httpContextAccessor;
            _user = user;
        }

        public async Task<ResponseForOneModel<GetCarModel>> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetCarModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetCarModel> { ErrorMesssage = " User Not Added data  . " };
            var car = await _car.GetByIdAsync(id);

            if (car is null)
                return new ResponseForOneModel<GetCarModel> { ErrorMesssage = "Car Not Found." };

            var result = new ResponseForOneModel<GetCarModel>
            {
                Data = new GetCarModel
                {
                    Id = car.Id,
                    Type = car.Type,
                    Model = car.Model,
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

        public async Task<GenResponseModel<GetCarModel>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetCarModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetCarModel> { ErrorMesssage = " User Not Added data  . " };

            var allCars = await _car.GetAllAsync();
            var userCars = allCars.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new GenResponseModel<GetCarModel>();

            foreach (var car in userCars)
            {
                result.Data?.Add(new GetCarModel
                {
                    Id = car.Id,
                    Type = car.Type,
                    Model = car.Model,
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
                return new ResponseModel { Messsage = "user not Autherize" };
            var userData = await _user.FindAsync(e => e.UserId == userId);

            if (userData is null)
                return new ResponseModel { Messsage = "user is not found " };



            var LecFront = await ProcessImageFile("Car", model.LicenseImageFront);
            var LecBack = await ProcessImageFile("Car", model.LicenseImageBack);

            var carImg = await ProcessImageFile("Car", model.Image);



            Car car = new Car
            {
                Id = Guid.NewGuid().ToString(),
                Type = model.Type,
                Model = model.ModelYear,
                Brand = model.Brand,
                Plate = model.Plate,
                Seats = model.Seats,

                LicenseImageFront = LecFront,
                LicenseImagBack = LecBack,
                LicenseExpiration = model.LicenseExpiration,

                Image = carImg,

                UserDataId = userData.Id

            };

            await _car.AddAsync(car);

            return new ResponseModel { Messsage = "Car add successfully ", IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, CarModel model)
        {
            var car = await _car.GetByIdAsync(id);
            if (car == null) new ResponseModel { Messsage = "Car not found . " };


            car.Type = model.Type;
            car.Model = model.ModelYear;
            car.Brand = model.Brand;
            car.Plate = model.Plate;
            car.Seats = model.Seats;
            car.LicenseExpiration = model.LicenseExpiration;

            // updata the image 
            await RemoveImageFile(car.Image);
            car.Image = await ProcessImageFile("Car", model.Image);

            await RemoveImageFile(car.LicenseImagBack);
            car.LicenseImagBack = await ProcessImageFile("Car", model.LicenseImageBack);

            await RemoveImageFile(car.LicenseImageFront);
            car.LicenseImageFront = await ProcessImageFile("Car", model.LicenseImageFront);


            await _car.UpdateAsync(car);

            return new ResponseModel { Messsage = "Car updated successfully", IsSuccess = true };

        }

        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var car = await _car.FindAsync(e => e.Id == id);
            int res = await _car.DeleteAsync(car);
            return res;
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

