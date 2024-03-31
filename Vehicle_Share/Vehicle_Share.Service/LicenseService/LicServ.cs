

using Microsoft.AspNetCore.Http;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Models.LicModels;
using System.Security.Claims;
using Vehicle_Share.Core.Models.CarModels;

namespace Vehicle_Share.Service.LicenseService
{
    public class LicServ : ILicServ
    {
        private readonly IBaseRepo<License> _Lic;
        private readonly IBaseRepo<UserData> _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LicServ(IBaseRepo<License> Lic, IHttpContextAccessor httpContextAccessor, IBaseRepo<UserData> user)
        {
            _Lic = Lic;
            _httpContextAccessor = httpContextAccessor;
            _user = user;
        }
        public async Task<License> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            return await _Lic.FindAsync(e=>e.User_DataId==userData.UserDataID);
        }
        public async Task<string> AddAsync(LicModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
               return "user not Autherize";
            var userData = await _user.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return "user is not found ";


            var LicFront = await ProcessImageFile("License", model.LicUserImgFront);
            var LicBack = await ProcessImageFile("License", model.LicUserImgBack);

            License user = new License
            {
               LicID=Guid.NewGuid().ToString(),          
               LicUserImgFront=LicFront,
               LicUserImgBack=LicBack,  
               EndDataOfUserLic=model.EndDataOfUserLic,
                User_DataId = userData.UserDataID
            };

            await _Lic.AddAsync(user);
            return "License added successfully ";
        }


        public async Task<string> UpdateAsync(string id, LicModel model)
        {
            var lic = await _Lic.GetByIdAsync(id);
            if (lic == null) return "license not found . ";



            lic.EndDataOfUserLic = model.EndDataOfUserLic;

            // updata the image 
         

            await RemoveImageFile(lic.LicUserImgBack);
            lic.LicUserImgBack = await ProcessImageFile("License", model.LicUserImgBack);

            await RemoveImageFile(lic.LicUserImgFront);
            lic.LicUserImgFront = await ProcessImageFile("License", model.LicUserImgFront);


            await _Lic.UpdateAsync(lic);

            return "License updated successfully";
        }

        public Task DeleteAsync(License license)
        {
            throw new NotImplementedException();
        }

        private async Task<string> ProcessImageFile(string folder, IFormFile file)
        {
            var req = _httpContextAccessor.HttpContext.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _Lic.UploadImageAsync(folder, file);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string file)
        {
            Uri uri = new Uri(file);
            string relativeUrl = uri.PathAndQuery;
            await _Lic.RemoveImageAsync(relativeUrl);
        }
    }
}
