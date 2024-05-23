using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Models.GeneralModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Vehicle_Share.Service.UserDataService
{
    public class UserDataServ : IUserDataServ
    {
        private readonly IBaseRepo<User> _user;
        private readonly IBaseRepo<UserData> _userData;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserDataServ(IBaseRepo<UserData> userData, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer, IWebHostEnvironment webHostEnvironment, IBaseRepo<User> user)
        {
            _userData = userData;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
            _webHostEnvironment = webHostEnvironment;
            _user = user;
        }

       
        public async Task<ResponseModel> GetUserDataAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] ,code=ResponseCode.NoAuth};

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";
            if(isAdmin)
                return new ResponseModel { message = " Admin not have user data . " };

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var result = new ResponseDataModel<GetUserModel>()
            {
                data = new GetUserModel
                {
                    Id = userData.Id,
                    Name = userData.Name,
                    NationalId = userData.NationalId,
                    Birthdate = userData.Birthdate.ToString("yyyy-MM-dd"),
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    ProfileImage = userData.ProfileImage,
                    Status = userData.Status,
                    Message = userData.Message,
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> AddAndUpdateAsync(UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");

            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code=ResponseCode.NoAuth};

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";
            if (isAdmin)
                return new ResponseModel { message = "Admin can't add user data" };


            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is null)
            {
               
                if (model.Name == null || model.NationalId == null || model.Address == null || model.Nationality == null ||
                 model.ProfileImage == null)
                {
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Required] };
                }
                // return new ResponseForOneModel<ImageModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };

                var ProfileImg = await ProcessImageFile("User", model.ProfileImage, model.Name);

                var IsNationlIdExist = await _userData.FindAsync(e => e.NationalId == model.NationalId);

                if (IsNationlIdExist is not null)
                {
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NationalId] };
                }

                UserData user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    NationalId = model.NationalId.GetValueOrDefault(),
                    Address = model.Address,
                    Nationality = model.Nationality,
                    ProfileImage = ProfileImg,
                    UserId = userId,
                    CreatedOn = DateTime.UtcNow,
                    Birthdate = (DateTime)model.Birthdate,
                    Gender = (bool)model.Gender
                };

                await _userData.AddAsync(user);

                var result = new ResponseDataModel<ProfileImageModel>
                {
                    IsSuccess = true,
                    message = _LocaLizer[SharedResourcesKey.Created],
                    data = new ProfileImageModel
                    {
                        Id = user.Id,
                        ProfileImage = ProfileImg,
                    }
                };
                return result;
            }
            else
            {
                userData.Name = model.Name ?? userData.Name;
                userData.NationalId = model.NationalId ?? userData.NationalId;
                userData.Birthdate = model.Birthdate ?? userData.Birthdate;
                userData.Gender = model.Gender ?? userData.Gender;
                userData.Nationality = model.Nationality ?? userData.Nationality;
                userData.Address = model.Address ?? userData.Address;

                // Update images
                
                if (model.ProfileImage != null)
                {
                    await RemoveImageFile(userData.ProfileImage);
                    userData.ProfileImage = await ProcessImageFile("User", model.ProfileImage,userData.Name);
                }

                await _userData.UpdateAsync(userData);
                var result = new ResponseDataModel<ProfileImageModel>
                {
                    IsSuccess = true,
                    message = _LocaLizer[SharedResourcesKey.Updated],
                    data = new ProfileImageModel
                    {
                        Id= userData.Id,
                        ProfileImage = userData.ProfileImage,
                    }
                };
                return result;
            }
        }
        public async Task<ResponseModel> AddAndUpdateNationalImageAsync(NationalImageModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");

            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
          

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is null)
                  return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoUserData };


            // Update images
            if (model.NationalCardImageFront != null)
            {
                await RemoveImageFile(userData.NationalCardImageFront);
                userData.NationalCardImageFront = await ProcessImageFile("User", model.NationalCardImageFront,userData.Name);
            }
            else
            {
                userData.NationalCardImageFront = await ProcessImageFile("User", model.NationalCardImageFront, userData.Name);
            }


            if (model.NationalCardImageBack != null)
            {
                await RemoveImageFile(userData.NationalCardImageBack);
                userData.NationalCardImageBack = await ProcessImageFile("User", model.NationalCardImageBack, userData.Name);
            }
            else
            {
                userData.NationalCardImageBack = await ProcessImageFile("User", model.NationalCardImageBack, userData.Name);

            }

            await _userData.UpdateAsync(userData);
                var result = new ResponseDataModel<ImageModel>
                {
                    IsSuccess = true,
                    message = _LocaLizer[SharedResourcesKey.Updated],
                    data = new ImageModel
                    {
                        Id = userData.Id,
                        NationalCardImageFront = userData.NationalCardImageFront,
                        NationalCardImageBack = userData.NationalCardImageBack
                    }
                };
                return result;
            
        }
        public async Task<ResponseModel> seedAsync(SeedModel model)
        {
           
            UserData user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                NationalId = model.NationalId.GetValueOrDefault(),
                Address = model.Address,
                Nationality = model.Nationality,
                NationalCardImageFront = model.NationalCardImageFront,
                NationalCardImageBack =model.NationalCardImageBack,
                ProfileImage = model.ProfileImage,
                UserId =model.userId,
                CreatedOn = DateTime.UtcNow,
                Birthdate= (DateTime)model.Birthdate,
                Gender=(bool)model.Gender

            };
            var path = _webHostEnvironment.WebRootPath + "/" + "User" + "/" + model.Name + "/";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
                await _userData.AddAsync(user);
            return new ResponseModel {message="ssssssss" ,IsSuccess=true};
        }

        #region For Admin

        public async Task<ResponseModel> GetAllUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";
            if (!isAdmin)
                return new ResponseModel { message = " this rout for admin . " };

            var users = await _user.GetAllAsync(u => u.UserName != "Admin");

            var result = new ResponseDataModel<List<GetAllUsersModel>>();
            result.data = new List<GetAllUsersModel>();
            foreach (var user in users)
            {
                result.data?.Add(new GetAllUsersModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Phone = user.PhoneNumber,
                }
                    );
            }
            result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> GetUserByIdAsyc(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var user = await _user.GetByIdAsync(id);
            if (user is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUser], code = ResponseCode.NoUserData };

            var result = new ResponseDataModel<GetAllUsersModel>()
            {
                data = new GetAllUsersModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Phone = user.PhoneNumber
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> GetUserDataByUserIdAsync(string id) 
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var userData = await _userData.FindAsync(i => i.UserId == id);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUser], code = ResponseCode.NoUserData };

            var result = new ResponseDataModel<GetUserDataModel>()
            {
                data = new GetUserDataModel
                {
                    Id = userData.Id,
                    Name = userData.Name,
                    NationalId = userData.NationalId,
                    Birthdate = userData.Birthdate.ToString("yyyy-MM-dd"),
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    NationalCardImageFront = userData.NationalCardImageFront,
                    NationalCardImageBack = userData.NationalCardImageBack,
                    ProfileImage = userData.ProfileImage,
                    Status = userData.Status,
                    Message = userData.Message,
                },
                IsSuccess = true
            };

            return result;
        }

        public async Task<ResponseModel> GetUserDataAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };


            var allUser = await _userData.GetAllAsync();
            var result = new ResponseDataModel<List<GetUserDataModel>>();
            result.data = new List<GetUserDataModel>();
            foreach (var userData in allUser)
            {
                result.data?.Add(new GetUserDataModel
                {

                    Id = userData.Id,
                    Name = userData.Name,
                    NationalId = userData.NationalId,
                    Birthdate = userData.Birthdate.ToString("yyyy-MM-dd"),
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    NationalCardImageFront = userData.NationalCardImageFront,
                    NationalCardImageBack = userData.NationalCardImageBack,
                    ProfileImage = userData.ProfileImage,
                    Status = userData.Status,
                    Message = userData.Message,
                }
                    );
            }
                result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> GetUserDataByIdAsyc(string id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            var userData = await _userData.GetByIdAsync(id);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUser], code = ResponseCode.NoUserData };

            if (isAdmin)
            {
                var result = new ResponseDataModel<GetUserDataModel>()
                {
                    data = new GetUserDataModel
                    {
                        Id = userData.Id,
                        Name = userData.Name,
                        NationalId = userData.NationalId,
                        Birthdate = userData.Birthdate.ToString("yyyy-MM-dd"),
                        Gender = userData.Gender,
                        Nationality = userData.Nationality,
                        Address = userData.Address,
                        NationalCardImageFront = userData.NationalCardImageFront,
                        NationalCardImageBack = userData.NationalCardImageBack,
                        ProfileImage = userData.ProfileImage,
                        Status = userData.Status,
                        Message = userData.Message,
                    },
                    IsSuccess = true
                };

                return result;
            }
            else
            {
                var result = new ResponseDataModel<GetUserModel>()
                {
                    data = new GetUserModel
                    {
                        Id = userData.Id,
                        Name = userData.Name,
                        NationalId = userData.NationalId,
                        Birthdate = userData.Birthdate.ToString("yyyy-MM-dd"),
                        Gender = userData.Gender,
                        Nationality = userData.Nationality,
                        Address = userData.Address,
                        ProfileImage = userData.ProfileImage,
                        Status = userData.Status,
                        Message = userData.Message,
                    },
                    IsSuccess = true
                };

                return result;
            }
        }
        public async Task<ResponseModel> UpdateAsync(string id, UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";
            if(!isAdmin)
                return new ResponseModel { message = "the route for admin" };

            var userData = await _userData.GetByIdAsync(id);

            userData.Name = model.Name ?? userData.Name;
            userData.NationalId = model.NationalId ?? userData.NationalId;
            userData.Birthdate = model.Birthdate ?? userData.Birthdate;
            userData.Gender = model.Gender ?? userData.Gender;
            userData.Nationality = model.Nationality ?? userData.Nationality;
            userData.Address = model.Address ?? userData.Address;

            // Update images

            if (model.ProfileImage != null)
            {
                await RemoveImageFile(userData.ProfileImage);
                userData.ProfileImage = await ProcessImageFile("User", model.ProfileImage, userData.Id);
            }

            await _userData.UpdateAsync(userData);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
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

            var car = await _userData.GetByIdAsync(id);
            if (car == null) new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            car.Status = model.Status;

            if (model.Status == Core.Helper.StatusContainer.Status.Refused)
            {
                car.Message = model.Message;
            }
            await _userData.UpdateAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }

        #endregion


        #region  ProcessImageFile
        private async Task<string> ProcessImageFile(string folder, IFormFile? file,string SubFolder)
        {
            if (file == null) return string.Empty;

            var req = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _userData.UploadImageAsync(folder, file, SubFolder);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string? file)
        {
            if (file == null) return;

            Uri uri = new(file);
            string relativeUrl = uri.PathAndQuery;
            await _userData.RemoveImageAsync(relativeUrl);
        }
        #endregion

        /* private static int CalculateAge(DateTime birthDate, DateTime now)
         {
             int age = now.Year - birthDate.Year;

             if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                 age--;

             return age;
         }*/
    }
}
