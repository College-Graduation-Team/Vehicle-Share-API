using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;

namespace Vehicle_Share.Service.UserDataService
{
    public class UserDataServ : IUserDataServ
    {
        private readonly IBaseRepo<UserData> _userData;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserDataServ(IBaseRepo<UserData> userData, IHttpContextAccessor httpContextAccessor)
        {
            _userData = userData;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseForOneModel<GetUserModel>> GetUserDataAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userData.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseForOneModel<GetUserModel> { ErrorMesssage = " User Data Not Found . " };

            if (userData.BirthData == null)
                userData.BirthData = DateTime.UtcNow;//DateTime.Parse ("2013-10-01 13:45:01");
            var age = CalculateAge(userData.BirthData, DateTime.UtcNow);

            var result = new ResponseForOneModel<GetUserModel>()
            {
                Data = new GetUserModel
                {
                    Id = userData.UserDataID,
                    FullName = userData.FullName,
                    NationailID = userData.NationailID,
                    Age = age,
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    NationalcardImgFront = userData.NationalcardImgFront,
                    NationalcardImgBack = userData.NationalcardImgBack,
                    ProfileImg = userData.ProfileImg,
                    typeOfUser = userData.typeOfUser

                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> AddAsync(UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");

            if (userId is null)
                return new ResponseModel {Messsage= "user not Autherize" };

            var NationalcardImgFront = await ProcessImageFile("User",model.NationalcardImgFront);
            var NationalcardImgBack = await ProcessImageFile("User",model.NationalcardImgBack);
            var ProfileImg = await ProcessImageFile("User",model.ProfileImg);

            var IsNationlIdExist =await _userData.FindAsync(e=>e.NationailID==model.NationailID);

            if (IsNationlIdExist is not null )
            {
                return new ResponseModel { Messsage = " National ID already exists . " };
            }

            UserData user = new UserData
            {
                UserDataID= Guid.NewGuid().ToString(),
                FullName=model.FullName ,
                NationailID =model.NationailID,
                Address=model.Address,
                Nationality=model.Nationality,
                NationalcardImgFront = NationalcardImgFront,
                NationalcardImgBack = NationalcardImgBack,
                ProfileImg= ProfileImg,
                User_Id= userId,
                typeOfUser=model.typeOfUser
                

            };

            await _userData.AddAsync(user);
            return new ResponseModel { Messsage = "UserData add successfully ", IsSuccess = true };
        }
        public async Task<ResponseModel> UpdateAsync(string id ,UserDataModel model)
        {  
            var user = await _userData.GetByIdAsync(id);
            if (user == null) return new ResponseModel { Messsage = "User not found . " };

                 user.FullName = model.FullName;
                 user.Nationality = model.Nationality;
                 user.NationailID = model.NationailID;
                 user.Address = model.Address;
                 user.typeOfUser = model.typeOfUser;

                // updata the image 
                 await RemoveImageFile(user.NationalcardImgFront);
                 user.NationalcardImgFront = await ProcessImageFile("User", model.NationalcardImgFront);
            
                 await RemoveImageFile(user.NationalcardImgBack);
                 user.NationalcardImgBack = await ProcessImageFile("User", model.NationalcardImgBack);
          
                 await RemoveImageFile(user.ProfileImg);
                 user.ProfileImg = await ProcessImageFile("User", model.ProfileImg);
           

            await _userData.UpdateAsync(user);

            return new ResponseModel { Messsage = "User data updated successfully", IsSuccess = true };

        }
        
        private async Task< string> ProcessImageFile(string folder,IFormFile file)
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
