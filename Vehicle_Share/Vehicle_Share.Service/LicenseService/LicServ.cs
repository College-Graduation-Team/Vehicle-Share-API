

using Microsoft.AspNetCore.Http;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Models.LicModels;
using System.Security.Claims;
using Vehicle_Share.Core.Response;
using Microsoft.Extensions.Localization;
using Vehicle_Share.Core.Resources;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.UserData;

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
        public async Task<ResponseModel> GetLicenseAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };


            var Lic = await _Lic.FindAsync(e => e.UserDataId == userData.Id);
            if (Lic is null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoLicense] , code = ResponseCode.NoLicense };
            }
            var result = new ResponseDataModel<GetLicModel>()
            {
                data = new GetLicModel
                {
                    Id = Lic.Id,
                    UserDataId=Lic.UserDataId,
                    ImageFront = Lic.ImageFront,
                    ImageBack = Lic.ImageBack,
                    Expiration = Lic.Expiration,
                    Status = Lic.Status,
                    Message = Lic.Message,
                },
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> AddAndUpdateAsync(LicModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };
          
            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var lic = await _Lic.FindAsync(e => e.UserDataId == userData.Id);
            if(lic is null)
            {
                if (isAdmin)
                    return new ResponseModel { message = "Admin can't add user data" };

                var LicFront = await ProcessImageFile("License", model.ImageFront, userData.Name);
                var LicBack = await ProcessImageFile("License", model.ImageBack, userData.Name);

                License license = new License
                {
                    Id = Guid.NewGuid().ToString(),
                    ImageFront = LicFront,
                    ImageBack = LicBack,
                    Expiration = model.Expiration,
                    UserDataId = userData.Id,
                    CreatedOn = DateTime.UtcNow
                };

                await _Lic.AddAsync(license);
                var result = new ResponseDataModel<GetImageModel>
                {

                    IsSuccess = true,
                    message = _LocaLizer[SharedResourcesKey.Created],
                    data = new GetImageModel
                    {
                        Id = license.Id,
                        ImageFront = license.ImageFront,
                        ImageBack = license.ImageBack
                    }
                };
                return result;
            }
            else
            {

                lic.Expiration = model.Expiration != null ? model.Expiration : lic.Expiration;

                // updata the image 

                if (model.ImageBack != null)
                {
                    await RemoveImageFile(lic.ImageBack);
                    lic.ImageBack = await ProcessImageFile("License", model.ImageBack, userData.Name);
                }
                if (model.ImageFront != null)
                {
                    await RemoveImageFile(lic.ImageFront);
                    lic.ImageFront = await ProcessImageFile("License", model.ImageFront, userData.Name);
                }

                await _Lic.UpdateAsync(lic);

                var result = new ResponseDataModel<GetImageModel>
                {

                    IsSuccess = true,
                    message = _LocaLizer[SharedResourcesKey.Updated],
                    data = new GetImageModel
                    {
                        Id = lic.Id,
                        ImageFront = lic.ImageFront,
                        ImageBack = lic.ImageBack
                    }
                };
                return result;
            }
            // return new ResponseModel { Id = license.Id, message = _LocaLizer[SharedResourcesKey.Created], IsSuccess = true };
        }
        public async Task<ResponseModel> DeleteAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };
            var userData = await _user.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var lic = await _Lic.FindAsync(e => e.UserDataId == userData.Id);
            if (lic is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoLicense], code = ResponseCode.NoLicense };
           
            int res = await _Lic.DeleteAsync(lic);
            return res > 0 ? new ResponseModel { message = _LocaLizer[SharedResourcesKey.Deleted] ,IsSuccess=true }
            : new ResponseModel { message = _LocaLizer[SharedResourcesKey.Error] };
        }

        public async Task<ResponseModel> seedAsync(LicSeedModel model)
        {

            License license = new License
            {
                Id = Guid.NewGuid().ToString(),
                ImageFront = model.ImageFront,
                ImageBack = model.ImageBack,
                Expiration = model.Expiration,
                UserDataId = model.UserDataId,
                CreatedOn = DateTime.UtcNow
            };

            await _Lic.AddAsync(license);
            return new ResponseModel { message = "ssssssss", IsSuccess = true };
        }


        #region For Admin
        public async Task<ResponseModel> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            bool isAdmin = roleClaim != null && roleClaim.Value == "Admin";

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };


            var allLicense = await _Lic.GetAllAsync();
            var result = new ResponseDataModel<List<GetLicModel>>();
            result.data = new List<GetLicModel>();
            foreach (var Lic in allLicense)
            {
                result.data?.Add(new GetLicModel
                {
                    Id = Lic.Id,
                    UserDataId = Lic.UserDataId,
                    ImageFront = Lic.ImageFront,
                    ImageBack = Lic.ImageBack,
                    Expiration = Lic.Expiration,
                    Status = Lic.Status,
                    Message = Lic.Message,
                });
                    
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

            if (!isAdmin)
                return new ResponseModel { message = "this route for admin" };

            var Lic = await _Lic.GetByIdAsync(id);
            if (Lic is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var result = new ResponseDataModel<GetLicModel>()
            {
                data = new GetLicModel
                {
                    Id = Lic.Id,
                    UserDataId = Lic.UserDataId,
                    ImageFront = Lic.ImageFront,
                    ImageBack = Lic.ImageBack,
                    Expiration = Lic.Expiration,
                    Status = Lic.Status,
                    Message = Lic.Message,
                },
            
                IsSuccess = true
            };

            return result;
        }
        public async Task<ResponseModel> UpdateAsync(string id, LicModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var lic = await _Lic.GetByIdAsync(id);
            lic.Expiration = model.Expiration != null ? model.Expiration : lic.Expiration;

            var user = await _user.GetByIdAsync(lic.UserDataId);

            // updata the image 

            if (model.ImageBack != null)
            {
                await RemoveImageFile(lic.ImageBack);
                lic.ImageBack = await ProcessImageFile("License", model.ImageBack , user.Name);
            }
            if (model.ImageFront != null)
            {
                await RemoveImageFile(lic.ImageFront);
                lic.ImageFront = await ProcessImageFile("License", model.ImageFront, user.Name);
            }

            await _Lic.UpdateAsync(lic);
            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated],IsSuccess=true };

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

            var car = await _Lic.GetByIdAsync(id);
            if (car == null) new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoCar], code = ResponseCode.NoCar };

            car.Status = model.Status;

            if (model.Status == Core.Helper.StatusContainer.Status.Refused)
            {
                car.Message = model.Message;
            }
            await _Lic.UpdateAsync(car);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Updated], IsSuccess = true };

        }

        #endregion

        #region  ProcessImageFile
        private async Task<string> ProcessImageFile(string folder, IFormFile? file, string SubFolder)
        {
            if (file == null) return string.Empty;

            var req = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = req.Scheme + "://" + req.Host;

            var Image = await _Lic.UploadImageAsync(folder, file, SubFolder);
            return baseUrl + Image;
        }
        private async Task RemoveImageFile(string? file)
        {
            if (file == null) return;

            Uri uri = new(file);
            string relativeUrl = uri.PathAndQuery;
            await _Lic.RemoveImageAsync(relativeUrl);
        }
        #endregion
    }
}
