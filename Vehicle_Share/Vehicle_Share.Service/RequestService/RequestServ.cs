
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

        public async Task<GenResponseModel<GetReqModel>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = " User Not Authorize . " };

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new GenResponseModel<GetReqModel> { ErrorMesssage = " User Not Added data  . " };

            var allRequests = await _request.GetAllAsync();
            var userRequests = allRequests.Where(t => t.User_DataId == userData.UserDataID).ToList();
            var result = new GenResponseModel<GetReqModel>();
            foreach (var request in userRequests)
            {
                result.Data.Add(new GetReqModel
                {
                    Id = request.RequestID,
                    Status=request.RequestStatus.ToString(),
                    tripId=request.Trip_Id,
                    UserdataId=request.User_DataId
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

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return new ResponseModel { Messsage = "User not found" };

            // Find the trip associated with the request
            var trip = await _trip.FindAsync(e => e.TripID == model.TripId);
            if ( trip is null || trip.IsFinish )
                return new ResponseModel { Messsage = "Trip not found or finished" };

            // Check if the user who added the trip is trying to make a request for the same trip
            if (trip.User_DataId == userData.UserDataID)
            {
                return new ResponseModel { Messsage = "You cannot make a request for your own trip." };
            }
            // Add the request as usual
            Request request = new()
            {
                RequestID = Guid.NewGuid().ToString(),
                RequestStatus = Status.Pending,
                Trip_Id =model.TripId,
                User_DataId = userData.UserDataID
            };
            if (request.User_DataId== userData.UserDataID)
            {
                return new ResponseModel { Messsage = "You already send request before ." };
            }

            await _request.AddAsync(request);
            return new ResponseModel { Messsage = "Request added successfully", IsSuccess = true };

        }
        public async Task<ResponseModel> AcceptRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return new ResponseModel { Messsage = "Request not found" };
            }

            // Mark the request as accepted
            request.RequestStatus = Status.Accepted;

            // Update the request in the database
            await _request.UpdateAsync(request);

            return new ResponseModel { Messsage = "Request accepted successfully" , IsSuccess=true };
        }
        public async Task<ResponseModel> DenyRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return new ResponseModel { Messsage = "Request not found" };
            }
            // Mark the request as denied
            request.RequestStatus =Status.Refused;
            // Update the request in the database
            await _request.UpdateAsync(request);

            return new ResponseModel { Messsage = "Request denied successfully" , IsSuccess=true };
        }
        public async Task<int> DeleteRequestAsync(string requestId)
        {
            if (requestId is null)
                return 0;
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return 0;
            }
            // Delete the request from the database
            var result =await _request.DeleteAsync(request);

            return result;
        }

    }
}
