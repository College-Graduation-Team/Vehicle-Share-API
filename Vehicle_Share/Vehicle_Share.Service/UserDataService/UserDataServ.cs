using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
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
        
        public async Task<GetUserModel> GetByIdAsync(string id)
        {
            var userData = await _userData.GetByIdAsync(id);
            if (userData == null)
                return null;
            if (userData.BirthData == null)
                userData.BirthData = DateTime.UtcNow;//DateTime.Parse ("2013-10-01 13:45:01");
            var age = CalculateAge(userData.BirthData, DateTime.UtcNow);

                GetUserModel user = new GetUserModel
                {
                    
                    FullName=userData.FullName,
                    NationailID=userData.NationailID,
                    Age=age,
                    Gender = userData.Gender,
                    Nationality = userData.Nationality,
                    Address = userData.Address,
                    NationalcardImgFront = userData.NationalcardImgFront,
                    NationalcardImgBack = userData.NationalcardImgBack,
                    ProfileImg = userData.ProfileImg,
                    typeOfUser=userData.typeOfUser
                    
                };
            return user;
        }
        
        public async Task<string> AddAsync(UserDataModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");

            var NationalcardImgFront = await ProcessImageFile("User",model.NationalcardImgFront);
            var NationalcardImgBack = await ProcessImageFile("User",model.NationalcardImgBack);
            var ProfileImg = await ProcessImageFile("User",model.ProfileImg);


            if (userId is null)
                return "user not Autherize";
            var IsNationlIdExist =await _userData.FindAsync(e=>e.NationailID==model.NationailID);

            if (IsNationlIdExist is not null )
            {
                return " National ID already exists . ";
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
            return "UserData add successfully ";
        }
        
        public async Task<string> UpdateAsync(string id ,UserDataModel model)
        {  //b4a6c2c6-08ad-4ba8-b934-6532c5908baf
            var user = await _userData.GetByIdAsync(id);
            if (user == null) return "User not found . ";

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

            return "User data updated successfully";

        }
        
        public Task DeleteAsync(UserData userData)
        {
            throw new NotImplementedException();
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

        public int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                age--;

            return age;
        }
    }
}
