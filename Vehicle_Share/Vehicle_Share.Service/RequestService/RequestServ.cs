
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vehicle_Share.Core.Response;
using static Vehicle_Share.EF.Helper.StatusContainer;
using Microsoft.Extensions.Localization;
using Vehicle_Share.Core.Resources;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Vehicle_Share.Service.RequestService
{
    public class RequestServ : IRequestServ
    {
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<Request> _request;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResources> _LocaLizer;


        public RequestServ(IBaseRepo<UserData> userdata, IBaseRepo<Trip> trip, IBaseRepo<Request> request, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResources> locaLizer = null)
        {
            _userdata = userdata;
            _trip = trip;
            _request = request;
            _httpContextAccessor = httpContextAccessor;
            _LocaLizer = locaLizer;
        }
        public async Task<ResponseModel> GetAllTripRequestedAsync(string tripId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            var trip = await _trip.GetByIdAsync(tripId);
            if (trip is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };

            var userRequests = await _request.GetAllAsync(r => r.TripId == tripId);

            var result = new ResponseDataModel<List<GetReqModel>>();
            result.data = new List<GetReqModel>();
            foreach (var request in userRequests)
            {
                result.data?.Add(new GetReqModel
                {
                    Id = request.Id,
                    Status = request.Status.ToString(),
                    CreatedOn = request.CreatedOn,
                    TripId = request.TripId,
                    UserDataId = request.UserDataId
                });
            }

            result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> GetAllMyRequestAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var allRequests = await _request.GetAllAsync();
            var userRequests = allRequests.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new ResponseDataModel<List<GetReqModel>>();
            result.data = new List<GetReqModel>();
            foreach (var request in userRequests)
            {
                result.data?.Add(new GetReqModel
                {
                    Id = request.Id,
                    Status = request.Status.ToString(),
                    CreatedOn = request.CreatedOn,
                    TripId = request.TripId,
                    UserDataId = request.UserDataId
                }

                );
            };
            result.IsSuccess = true;
            return result;
        }
        public async Task<ResponseModel> SendReqestAsync(ReqModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth], code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData], code = ResponseCode.NoUserData };

            // Find the trip associated with the request
            var trip = await _trip.FindAsync(e => e.Id == model.TripId);
            if (trip is null || trip.IsFinished)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip], code = ResponseCode.NoTrip };

            // Check if the user who added the trip is trying to make a request for the same trip
            if (trip.UserDataId == userData.Id)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoSendRequest] };
            }
            Request request = new();
            if (model.Type is true) //driver
                request.Seats = 0;
            else
            {
                if (model.Seats > trip.AvailableSeats)                      //Passenger
                    return new ResponseModel { message = _LocaLizer[SharedResourcesKey.InvalidRequestedSeats] };
                request.Seats = model.Seats;
            }

            if (request.UserDataId == userData.Id)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.SendRequestBefore] };
            }
            request.Id = Guid.NewGuid().ToString();
            request.Status = Status.Pending;
            request.TripId = model.TripId;
            request.UserDataId = userData.Id;
            request.CreatedOn = DateTime.UtcNow;
            await _request.AddAsync(request);
            var result = new ResponseDataModel<IdResponseModel>
            {
                message = _LocaLizer[SharedResourcesKey.Created],
                IsSuccess = true,
                data = new IdResponseModel { Id = request.Id }
            };
            return result;
        }

    
        
        
        public async Task<ResponseModel> AcceptRequestAsync(string requestId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoAuth] , code = ResponseCode.NoAuth };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoUserData] , code = ResponseCode.NoUserData };

            var request = await _request.FindAsync(r => r.Id == requestId);
            if (request == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoRequest] };
            }

            var trip = await _trip.FindAsync(e => e.Id == request.TripId);
            if (trip is null || trip.IsFinished)
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoTrip] , code = ResponseCode.NoTrip  };

            if (request.Seats >0 ) //driver
            {
                request.Status = Status.Accepted;

                if (trip.AvailableSeats.HasValue)
                {
                    short availbleseats = (short)(trip.AvailableSeats.Value - request.Seats);
                    trip.AvailableSeats = availbleseats;
                    await _trip.UpdateAsync(trip);
                }
                await _request.UpdateAsync(request);

            }

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.Success], IsSuccess = true };
        }
        public async Task<ResponseModel> DenyRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.Id == requestId);
            if (request == null)
            {
                return new ResponseModel { message = _LocaLizer[SharedResourcesKey.NoRequest] };
            }
            // Mark the request as denied
            request.Status = Status.Refused;
            // Update the request in the database
            await _request.UpdateAsync(request);

            return new ResponseModel { message = _LocaLizer[SharedResourcesKey.DenyRequest], IsSuccess = true };
        }
        public async Task<int> DeleteRequestAsync(string requestId)
        {
            if (requestId is null)
                return 0;
            var request = await _request.FindAsync(r => r.Id == requestId);
            if (request == null)
            {
                return 0;
            }
            var result = await _request.DeleteAsync(request);

            return result;
        }

    }
}
