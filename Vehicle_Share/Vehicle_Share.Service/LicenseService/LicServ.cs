

using Microsoft.AspNetCore.Http;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Models.LicModels;
using System.Security.Claims;
using Vehicle_Share.Core.Response;
using Microsoft.Extensions.Localization;
using Vehicle_Share.Core.Resources;

namespace Vehicle_Share.Service.LicenseService
{
    public class LicServ : ILicServ
    {
        private readonly IBaseRepo<License> _Lic;
        private readonly IBaseRepo<UserData> _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;


        public LicServ(IBaseRepo<License> Lic, IHttpContextAccessor httpContextAccessor, IBaseRepo<UserData> user, IStringLocalizer<SharedResources> locaLizer = null)
        {
            _Lic = Lic;
            _httpContextAccessor = httpContextAccessor;
            _user = user;
            _LocaLizer = locaLizer;
        }
        public async Task<ResponseForOneModel<GetLicModel>> GetAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseForOneModel<GetLicModel> { message = _LocaLizer[SharedResourcesKey.NoAuth] };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseForOneModel<GetLicModel> { message = _LocaLizer[SharedResourcesKey.NoUserData] };


            var Lic = await _Lic.FindAsync(e => e.UserDataId == userData.Id);
            var result = new ResponseForOneModel<GetLicModel>()
            {
                data = new GetLicModel
                {
                    Id = Lic.Id,
                    ImageFront = Lic.ImageFront,
                    ImageBack = Lic.ImageBack,
                    Expiration = Lic.Expiration
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> AddAsync(LicModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] };
            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] };


            var LicFront = await ProcessImageFile("License", model.ImageFront);
            var LicBack = await ProcessImageFile("License", model.ImageBack);

            License user = new License
            {
                Id = Guid.NewGuid().ToString(),
                ImageFront = LicFront,
                ImageBack = LicBack,
                Expiration = model.Expiration,
                UserDataId = userData.Id,
                CreatedOn=DateTime.UtcNow
            };

            await _Lic.AddAsync(user);
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }
        public async Task<ResponseModel> UpdateAsync(string id, LicModel model)
        {
            var lic = await _Lic.GetByIdAsync(id);
            if (lic == null) return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoLicense] };



            lic.Expiration = model.Expiration;

            // updata the image 


            await RemoveImageFile(lic.ImageBack);
            lic.ImageBack = await ProcessImageFile("License", model.ImageBack);

            await RemoveImageFile(lic.ImageFront);
            lic.ImageFront = await ProcessImageFile("License", model.ImageFront);


            await _Lic.UpdateAsync(lic);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };
        }
        public async Task<int> DeleteAsync(string id)
        {
            if (id is null)
                return 0;
            var car = await _Lic.FindAsync(e => e.Id == id);
            int res = await _Lic.DeleteAsync(car);
            return res;
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
