using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
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

            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseForOneModel<GetCarModel> { ErrorMesssage = " User Not Added data  . " };
            var car =await _car.GetByIdAsync(id);

            if (car is null)
                return new ResponseForOneModel<GetCarModel> { ErrorMesssage = "Car Not Found." };

            var result = new ResponseForOneModel<GetCarModel>
            {
                Data = new GetCarModel
                {
                    Id = car.CarID,
                    Type = car.Type,
                    Model = car.Model,
                    Brand = car.Brand,
                    CarPlate = car.CarPlate,
                    SetsOfCar = car.SetsOfCar,
                    CarImg = car.CarImg,
                    LicImgCarFront = car.LicCarImgFront,
                    LicImgCarBack = car.LicCarImgBack,
                    EndDataOfCarLic = car.EndDataOfCarLic
                },
                IsSuccess = true
            };

            return result;
    }
       
        public async Task<GenResponseModel<GetCarModel>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetCarModel> { ErrorMesssage=" User Not Authorize . "};

            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new GenResponseModel<GetCarModel> { ErrorMesssage = " User Not Added data  . "};

            var allCars = await _car.GetAllAsync();
            var userCars = allCars.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new GenResponseModel<GetCarModel>();

            foreach (var car in userCars)
            {
                result.Data?.Add(new GetCarModel
                {
                    Id = car.CarID,
                    Type = car.Type,
                    Model = car.Model,
                    Brand = car.Brand,
                    CarPlate = car.CarPlate,
                    SetsOfCar = car.SetsOfCar,
                    CarImg = car.CarImg,
                    LicImgCarFront = car.LicCarImgFront,
                    LicImgCarBack = car.LicCarImgBack,
                    EndDataOfCarLic = car.EndDataOfCarLic
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
            var userData = await _user.FindAsync(e => e.User_Id==userId);

            if (userData is null)
                return new ResponseModel { Messsage = "user is not found " };

         

            var LecFront = await ProcessImageFile("Car", model.LecImgFront);
            var LecBack = await ProcessImageFile("Car", model.LecImgBack);

            var carImg = await ProcessImageFile("Car", model.CarImg);

   

            Car car = new Car
                {
                        CarID = Guid.NewGuid().ToString(),
                        Type = model.TypeOfCar,
                        Model = model.ModelOfCar,
                        Brand = model.BrandOfCar,
                        CarPlate = model.PlateOfCar,
                        SetsOfCar = model.CarSetNum,

                        LicCarImgFront = LecFront,
                        LicCarImgBack = LecBack,
                        EndDataOfCarLic=model.EndDataOfCarLic,

                        CarImg = carImg,

                        User_DataId = userData.UserDataID

                };

            await _car.AddAsync(car);

            return new ResponseModel { Messsage = "Car add successfully ", IsSuccess = true };
        }

        public async Task<ResponseModel> UpdateAsync(string id, CarModel model)
        { 
              var car = await _car.GetByIdAsync(id);
              if (car == null) new ResponseModel { Messsage = "Car not found . " };


                car.Type = model.TypeOfCar;
                car.Model = model.ModelOfCar;
                car.Brand = model.BrandOfCar;
                car.CarPlate = model.PlateOfCar;
                car.SetsOfCar = model.CarSetNum;
                car.EndDataOfCarLic=model.EndDataOfCarLic;

            // updata the image 
            await RemoveImageFile(car.CarImg);
              car.CarImg = await ProcessImageFile("Car", model.CarImg);

              await RemoveImageFile(car.LicCarImgBack);
              car.LicCarImgBack = await ProcessImageFile("Car", model.LecImgBack);

              await RemoveImageFile(car.LicCarImgFront);
              car.LicCarImgFront = await ProcessImageFile("Car", model.LecImgFront);


            await _car.UpdateAsync(car);

            return new ResponseModel { Messsage = "Car updated successfully" , IsSuccess=true };

        }

        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var car = await _car.FindAsync(e=>e.CarID==id);
            int res=await _car.DeleteAsync(car);
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

