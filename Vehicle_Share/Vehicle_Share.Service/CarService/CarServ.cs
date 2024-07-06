using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.LicModels;
using static Vehicle_Share.Core.Helper.StatusContainer;

namespace Vehicle_Share.Service.CarService
{
    public class CarServ : ICarServ
    {
        private readonly IBaseRepo<Car> _car;
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public CarServ(IBaseRepo<Car> car, IHttpContextAccessor httpContextAccessor
            , IBaseRepo<UserData> userdata,
            IStringLocalizer<SharedResources> locaLizer = null,
            IBaseRepo<Trip> trip = null)
        {
            _car = car;
            _httpContextAccessor = httpContextAccessor;
            _userdata = userdata;
            _LocaLizer = locaLizer;
            _trip = trip;
        }

        public async Task<ResponseModel> GetByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.UserId == userId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }

            var car = await _car.GetByIdAsync(id);

            if (car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            var result = new ResponseDataModel<GetCarModel>
            {
                data = new GetCarModel
                {
                    Id = car.Id,
                    UserDataId = car.UserDataId,
                    Type = car.Type,
                    ModelYear = car.ModelYear,
                    Brand = car.Brand,
                    Plate = car.Plate,
                    Seats = car.Seats,
                    Image = car.Image,
                    LicenseImageFront = car.LicenseImageFront,
                    LicenseImageBack = car.LicenseImagBack,
                    LicenseExpiration = car.LicenseExpiration,
                    Status = car.Status,
                    Message = car.Message
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.UserId == userId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }


            var allCars = await _car.GetAllAsync();
            var userCars = isAdmin ? allCars.ToList() : allCars.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new ResponseDataModel<List<GetCarModel>>();
            result.data = new List<GetCarModel>();
            foreach (var car in userCars)
            {
                result.data?.Add(new GetCarModel
                {
                    Id = car.Id,
                    UserDataId = car.UserDataId,
                    Type = car.Type,
                    ModelYear = car.ModelYear,
                    Brand = car.Brand,
                    Plate = car.Plate,
                    Seats = car.Seats,
                    Image = car.Image,
                    LicenseImageFront = car.LicenseImageFront,
                    LicenseImageBack = car.LicenseImagBack,
                    LicenseExpiration = car.LicenseExpiration,
                    Status = car.Status,
                    Message = car.Message
                });
            }
            result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> AddAsync(CarModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };
            var userData = await _userdata.FindAsync(e => e.UserId == userId);

            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };


            var LecFront = await ProcessImageFile("Car", model.LicenseImageFront, userData.Name);
            var LecBack = await ProcessImageFile("Car", model.LicenseImageBack, userData.Name);
            var carImg = await ProcessImageFile("Car", model.Image, userData.Name);

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
                CreatedOn = DateTime.UtcNow

            };
            await _car.AddAsync(car);
            var result = new ResponseDataModel<GetImageCarModel>
            {

                IsSuccess = true,
                message = _LocaLizer[SharedResourcesKey.Created],
                data = new GetImageCarModel
                {
                    Id = car.Id,
                    Image = car.Image,
                    LicenseImageFront = car.LicenseImageFront,
                    LicenseImageBack = car.LicenseImagBack
                }
            };
            return result;
            //  return new ResponseModel { Id = car.Id, message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }
        public async Task<ResponseModel> UpdateAsync(string id, UpdateCarModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.UserId == userId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }

            var car = await _car.GetByIdAsync(id);
            if (car is null) return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            var user = await _userdata.GetByIdAsync(car.UserDataId);
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
                car.Image = await ProcessImageFile("Car", model.Image, user.Id);
            }
            if (model.LicenseImageBack != null)
            {
                await RemoveImageFile(car.LicenseImagBack);
                car.LicenseImagBack = await ProcessImageFile("Car", model.LicenseImageBack, user.Id);
            }
            if (model.LicenseImageFront != null)
            {
                await RemoveImageFile(car.LicenseImageFront);
                car.LicenseImageFront = await ProcessImageFile("Car", model.LicenseImageFront, user.Id);
            }

            await _car.UpdateAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }
        public async Task<ResponseModel> DeleteAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";
            var car = await _car.FindAsync(e => e.Id == id);
            if (car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.Id == car.UserDataId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }

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

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Deleted], IsSuccess = true };
        }

        public async Task<ResponseModel> GetStatusAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            UserData userData = null;
            if (!isAdmin)
            {
                userData = await _userdata.FindAsync(e => e.UserId == userId);
                if (userData is null)
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };
            }

            var car = await _car.FindAsync(e => e.Id == id);
            if (car is null) return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            //  return new ResponseDataModel<StatusResponseModel> { data = new() { Status = (int)Lic.Status, ErrorMessage = ErrorMessage(Lic) } };
            var result = new ResponseDataModel<StatusResponseModel>();
            result.data = new() {
                Status = (int)car.Status,
                ErrorMessage = ErrorMessage(car)
            };
            result.IsSuccess = true;
            return result;
        }


        #region Admin

        public async Task<ResponseModel> GetByUserDataIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var car = await _car.FindAsync(e => e.UserDataId == id);

            if (car is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            var result = new ResponseDataModel<GetCarModel>
            {
                data = new GetCarModel
                {
                    Id = car.Id,
                    UserDataId = car.UserDataId,
                    Type = car.Type,
                    ModelYear = car.ModelYear,
                    Brand = car.Brand,
                    Plate = car.Plate,
                    Seats = car.Seats,
                    Image = car.Image,
                    LicenseImageFront = car.LicenseImageFront,
                    LicenseImageBack = car.LicenseImagBack,
                    LicenseExpiration = car.LicenseExpiration,
                    Status = car.Status,
                    Message = car.Message
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> UpdateStatusRequestAsync(string id, UpdateStatusRequestModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var car = await _car.GetByIdAsync(id);
            if (car is null) return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            car.Status = model.Status;

            if (model.Status == Core.Helper.StatusContainer.Status.Refused)
            {
                car.Message = model.Message;
            }
            await _car.UpdateAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }


        #endregion

        #region  ProcessImageFile
        private async Task<string> ProcessImageFile(string folder, IFormFile? file, string SubFolder)
        {
            if (file == null) return string.Empty;

            var req = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _car.UploadImageAsync(folder, file, SubFolder);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string? file)
        {
            if (file == null) return;

            Uri uri = new(file);
            string relativeUrl = uri.PathAndQuery;
            await _car.RemoveImageAsync(relativeUrl);
        }
        #endregion

        private string ErrorMessage(Car car)
        {
            var MSG = car.Message;
            int code;
            string message = "";
            if (car.Status == Status.Refused)
            {
                var values = MSG.Split(",");

                foreach (var item in values)
                {
                    code = int.Parse(item);
                    if (code == 100)
                        message += _LocaLizer[SharedResourcesKey.CarProblem100] + " ";

                    else if (code == 101)
                        message += _LocaLizer[SharedResourcesKey.CarProblem101] + " ";

                    else if (code == 102)
                        message += _LocaLizer[SharedResourcesKey.CarProblem102] + " ";

                    else if (code == 103)
                        message += _LocaLizer[SharedResourcesKey.CarProblem103] + " ";
                }
            }
            else
            {
                car.Message = null;
                message = car.Message;
                _car.UpdateAsync(car);
            }

            return message;
        }



    }
}

