using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Repository.GenericRepo;
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
      
        public async Task<List<GetCarModel>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            var allCars = await _car.GetAllAsync();
            var userCars = allCars.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new List<GetCarModel>();

            foreach (var car in userCars)
            {
                result.Add(new GetCarModel
                {
                    id = car.CarID,
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
            return result;
        }
        
        public async Task<List<string>> GetCarBrandsForUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new List<string>();

            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new List<string>();

            var cars = await _user.FindAsync(
                u => u.UserDataID == userData.UserDataID,
                u => u.cars
            );

            // Extracting only the brand information from the cars
            var carBrands = cars?.cars.Select(car => car.Brand).ToList() ?? new List<string>();

            return carBrands;
        }

        public async Task<string> AddAsync(CarModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return "user not Autherize";
            var userData = await _user.FindAsync(e => e.User_Id==userId);

            if (userData is null)
                return "user is not found ";

         

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

            return "Car add successfully ";
        }

        public async Task<string> UpdateAsync(string id, CarModel model)
        { 
              var car = await _car.GetByIdAsync(id);
              if (car == null) return "Car not found . ";


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

            return "Car updated successfully";

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

