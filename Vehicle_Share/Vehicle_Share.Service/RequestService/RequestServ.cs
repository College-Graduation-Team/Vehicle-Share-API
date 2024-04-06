
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Vehicle_Share.Core.Response;
using static Vehicle_Share.EF.Helper.StatusContainer;

namespace Vehicle_Share.Service.RequestService
{
    public class RequestServ : IRequestServ
    { 
        private readonly IBaseRepo<UserData> _userdata;
        private readonly IBaseRepo<Trip> _trip;
        private readonly IBaseRepo<Request> _request;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestServ(IBaseRepo<UserData> userdata, IBaseRepo<Trip> trip, IBaseRepo<Request> request, IHttpContextAccessor httpContextAccessor)
        {
            _userdata = userdata;
            _trip = trip;
            _request = request;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<GenResponseModel<GetReqModel>> GetAllTripRequestedAsync(string tripId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = "User Not Authorized." };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = "User Data Not Found." };

            var allRequests = await _request.GetAllAsync();
            var userRequests = allRequests.Where(r => r.TripId == tripId).ToList();

            var result = new GenResponseModel<GetReqModel>();
            foreach (var request in userRequests)
            {
                result.Data?.Add(new GetReqModel
                {
                    Id = request.Id,
                    Status = request.Status.ToString(),
                    tripId = request.TripId,
                    UserDataId = request.UserDataId
                });
            }

            result.IsSuccess = true;
            return result;
        }

        public async Task<GenResponseModel<GetReqModel>> GetAllMyRequestAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = " User Not Added data  . " };

            var allRequests = await _request.GetAllAsync();
            var userRequests = allRequests.Where(t => t.UserDataId == userData.Id).ToList();
            var result = new GenResponseModel<GetReqModel>();
            foreach (var request in userRequests)
            {
                result.Data?.Add(new GetReqModel
                {
                    Id = request.Id,
                    Status=request.Status.ToString(),
                    tripId=request.TripId,
                    UserDataId=request.UserDataId
                }

                );
            };
            result.IsSuccess=true;
            return result;
        }
        public async Task<ResponseModel> SendReqestAsync(ReqModel model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = "User not authorized" };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            // Find the trip associated with the request
            var trip = await _trip.FindAsync(e => e.Id == model.TripId);
            if ( trip is null || trip.IsFinished )
                return new ResponseModel { Messsage = "Trip not found or finished" };

            // Check if the user who added the trip is trying to make a request for the same trip
            if (trip.UserDataId == userData.Id)
            {
                return new ResponseModel { Messsage = "You cannot make a request for your own trip." };
            }
            Request request = new();
            if (userData.Type is true) //driver
                request.Seats = 0;
            else
            {  if(model.NumSeats>trip.AvailableSeats)                      //Passenger
                    return new ResponseModel { Messsage = "You enter invalid value . " };
                request.Seats = model.NumSeats;
            }
              
            if (request.UserDataId== userData.Id)
            {
                return new ResponseModel { Messsage = "You already send request before ." };
            }
            request.Id = Guid.NewGuid().ToString();
            request.Status = Status.Pending;
            request.TripId = model.TripId;
            request.UserDataId = userData.Id;
            await _request.AddAsync(request);
            return new ResponseModel { Messsage = "Request added successfully", IsSuccess = true };

        }
        public async Task<ResponseModel> AcceptRequestAsync(string requestId)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new ResponseModel { Messsage = "User not authorized" };

            var userData = await _userdata.FindAsync(e => e.UserId == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            var request = await _request.FindAsync(r => r.Id == requestId);
            if (request == null)
            {
                return new ResponseModel { Messsage = "Request not found" };
            }

            var trip = await _trip.FindAsync(e => e.Id ==request.TripId);
            if (trip is null || trip.IsFinished)
                return new ResponseModel { Messsage = "Trip not found or finished" };

            if (userData.Type is true) //driver
            {
                request.Status = Status.Accepted;

                if (trip.AvailableSeats.HasValue)
                {
                    var availbleseats = trip.AvailableSeats.Value - request.Seats;
                    trip.AvailableSeats = availbleseats;
                    await _trip.UpdateAsync(trip);
                }
                await _request.UpdateAsync(request);

            }

            return new ResponseModel { Messsage = "Request accepted successfully" , IsSuccess=true };
        }
        public async Task<ResponseModel> DenyRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.Id == requestId);
            if (request == null)
            {
                return new ResponseModel { Messsage = "Request not found" };
            }
            // Mark the request as denied
            request.Status =Status.Refused;
            // Update the request in the database
            await _request.UpdateAsync(request);

            return new ResponseModel { Messsage = "Request denied successfully" , IsSuccess=true };
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
            var result =await _request.DeleteAsync(request);

            return result;
        }

    }
}
