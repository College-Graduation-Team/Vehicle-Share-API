
using Vehicle_Share.EF.Models;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Repository.GenericRepo;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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

        public async Task<List<Request>> GetAllAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid");
            if (userId is null)
                return null;

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return null;

            var allRequests = await _request.GetAllAsync();
            var userRequests = allRequests.Where(t => t.User_DataId == userData.UserDataID).ToList();

            return userRequests;
        }

        public async Task<string> SendReqestAsync(ReqModel model)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("uid");
            if (userId is null)
                return "User not authorized";

            var userData = await _userdata.FindAsync(e => e.User_Id == userId);
            if (userData is null)
                return "User not found";

            // Find the trip associated with the request
            var trip = await _trip.FindAsync(e => e.TripID == model.TripId);
            if (trip is null)
                return "Trip not found";

            // Check if the user who added the trip is trying to make a request for the same trip
            if (trip.User_DataId == userData.UserDataID)
            {
                return "You cannot make a request for your own trip.";
            }

            // Additional validation or processing logic for the request can be added here

            // Add the request as usual
            Request request = new()
            {
                RequestID = Guid.NewGuid().ToString(),

                IsAccept = false, 
                Trip_Id=model.TripId,
                User_DataId = userData.UserDataID
            };

            await _request.AddAsync(request);

            return "Request added successfully";

        }

        public async Task<string> AcceptRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return "Request not found";
            }

            // Mark the request as accepted
            request.IsAccept = true;

            // Update the request in the database
            await _request.UpdateAsync(request);

            return "Request accepted successfully";
        }

        public async Task<string> DenyRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return "Request not found";
            }

            // Mark the request as denied
            request.IsAccept = false;

            // Update the request in the database
            await _request.UpdateAsync(request);

            return "Request denied successfully";
        }

        public async Task<string> DeleteRequestAsync(string requestId)
        {
            var request = await _request.FindAsync(r => r.RequestID == requestId);
            if (request == null)
            {
                return "Request not found";
            }

            // Delete the request from the database
            await _request.DeleteAsync(request);

            return "Request deleted successfully";
        }

    }
}
