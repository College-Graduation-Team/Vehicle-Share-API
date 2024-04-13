using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.SharedResources;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.UserDataService
{
    public class UserDataServ : IUserDataServ
    {
        private readonly IBaseRepo<UserData> _userData;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;

        public UserDataServ(IBaseRepo<UserData> userData, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer)
        {
            _userData = userData;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
        }
        public async Task<ResponseForOneModel<GetUserModel>> GetUserDataAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = " User Data Not Found . " };

            if (userData.Birthdate == null)
                userData.Birthdate = DateTime.UtcNow;//DateTime.Parse ("2013-10-01 13:45:01");
            var age = CalculateAge(userData.Birthdate, DateTime.UtcNow);

            var result = new ResponseForOneModel<GetUserModel>()
            {
                Data = new GetUserModel
                {
                    Id = userData.Id,
                    Name = userData.Name,
                    NationailId = userData.NationailId,
                    Age = age,
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    NationalCardImageFront = userData.NationalCardImageFront,
                    NationalCardImageBack = userData.NationalCardImageBack,
                    ProfileImage = userData.ProfileImage,
                    Type = userData.Type

                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> AddAsync(UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");

            if (userId is null)
                return new ResponseModel { Messsage = "user not Autherize" };

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is not null)
                return new ResponseModel { Messsage = " you added data before . " };

            var NationalcardImgFront = await ProcessImageFile("User", model.NationalCardImageFront);
            var NationalcardImgBack = await ProcessImageFile("User", model.NationalCardImageBack);
            var ProfileImg = await ProcessImageFile("User", model.ProfileImage);

            var IsNationlIdExist = await _userData.FindAsync(e => e.NationailId == model.NationailId);

            if (IsNationlIdExist is not null)
            {
                return new ResponseModel { Messsage = " National ID already exists . " };
            }

            UserData user = new UserData
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                NationailId = model.NationailId,
                Address = model.Address,
                Nationality = model.Nationality,
                NationalCardImageFront = NationalcardImgFront,
                NationalCardImageBack = NationalcardImgBack,
                ProfileImage = ProfileImg,
                UserId = userId,
                Type = model.Type


            };

            await _userData.AddAsync(user);
            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }
        public async Task<ResponseModel> UpdateAsync(string id, UserDataModel model)
        {
            var user = await _userData.GetByIdAsync(id);
            if (user == null) return new ResponseModel { Messsage = "User not found . " };

            user.Name = model.Name;
            user.Nationality = model.Nationality;
            user.NationailId = model.NationailId;
            user.Address = model.Address;
            user.Type = model.Type;

            // updata the image 
            await RemoveImageFile(user.NationalCardImageFront);
            user.NationalCardImageFront = await ProcessImageFile("User", model.NationalCardImageFront);

            await RemoveImageFile(user.NationalCardImageBack);
            user.NationalCardImageBack = await ProcessImageFile("User", model.NationalCardImageBack);

            await RemoveImageFile(user.ProfileImage);
            user.ProfileImage = await ProcessImageFile("User", model.ProfileImage);


            await _userData.UpdateAsync(user);

            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }
        private async Task<string> ProcessImageFile(string folder, IFormFile file)
        {
            var req = _httpContextAccessor.HttpContext.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _userData.UploadImageAsync(folder, file);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string file)
        {
            Uri uri = new Uri(file);
            string relativeUrl = uri.PathAndQuery;
            await _userData.RemoveImageAsync(relativeUrl);
        }
        private int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
    }
}
