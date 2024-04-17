using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Core.Resources;
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
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = _LocaLizer[SharedResourcesKey.NoUserData] };

            // ===== DateTime is non-nullable struct can never be null =====
            // if (userData.Birthdate == null) 
            //     userData.Birthdate = DateTime.UtcNow;//DateTime.Parse ("2013-10-01 13:45:01");
            var age = CalculateAge(userData.Birthdate, DateTime.UtcNow);

            var result = new ResponseForOneModel<GetUserModel>()
            {
                Data = new GetUserModel
                {
                    Id = userData.Id,
                    Name = userData.Name,
                    NationalId = userData.NationalId,
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
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _userData.FindAsync(e => e.UserId == userId);
            if (userData is not null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.UserData] };

            var NationalcardImgFront = await ProcessImageFile("User", model.NationalCardImageFront);
            var NationalcardImgBack = await ProcessImageFile("User", model.NationalCardImageBack);
            var ProfileImg = await ProcessImageFile("User", model.ProfileImage);

            var IsNationlIdExist = await _userData.FindAsync(e => e.NationalId == model.NationalId);

            if (IsNationlIdExist is not null)
            {
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NationalId] };
            }

            UserData user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                NationalId = model.NationalId.GetValueOrDefault(),
                Address = model.Address,
                Nationality = model.Nationality,
                NationalCardImageFront = NationalcardImgFront,
                NationalCardImageBack = NationalcardImgBack,
                ProfileImage = ProfileImg,
                UserId = userId,
                Type = model.Type.GetValueOrDefault()
            };

            await _userData.AddAsync(user);
            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }
        public async Task<ResponseModel> UpdateAsync(UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");

            if (userId is null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.NoAuth] };

            var user = await _userData.FindAsync(e => e.UserId == userId);
            if (user == null)
                return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.UserData] };

            if (model.Name != null) user.Name = model.Name;
            if (model.NationalId.HasValue) user.NationalId = model.NationalId.Value;
            if (model.Birthdate.HasValue) user.Birthdate = model.Birthdate.Value;
            if (model.Gender.HasValue) user.Gender = model.Gender.Value;
            if (model.Nationality != null) user.Nationality = model.Nationality;
            if (model.Address != null) user.Address = model.Address;
            if (model.Type.HasValue) user.Type = model.Type.Value;

            // Update images
            if (model.NationalCardImageFront != null) {
                await RemoveImageFile(user.NationalCardImageFront);
                user.NationalCardImageFront = await ProcessImageFile("User", model.NationalCardImageFront);
            }
            
            if (model.NationalCardImageBack != null) {
                await RemoveImageFile(user.NationalCardImageBack);
                user.NationalCardImageBack = await ProcessImageFile("User", model.NationalCardImageBack);
            }
            
            if (model.ProfileImage != null) {
                await RemoveImageFile(user.ProfileImage);
                user.ProfileImage = await ProcessImageFile("User", model.ProfileImage);
            }

            await _userData.UpdateAsync(user);

            return new ResponseModel { Messsage = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }
        private async Task<string> ProcessImageFile(string folder, IFormFile? file)
        {
            if (file == null) return string.Empty;

            var req = _httpContextAccessor.HttpContext.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _userData.UploadImageAsync(folder, file);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string? file)
        {
            if (file == null) return;

            Uri uri = new(file);
            string relativeUrl = uri.PathAndQuery;
            await _userData.RemoveImageAsync(relativeUrl);
        }

        private static int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
    }
}
