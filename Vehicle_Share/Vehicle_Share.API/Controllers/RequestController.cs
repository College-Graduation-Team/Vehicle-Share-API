using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Service.RequestService;
using Vehicle_Share.Service.TripService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestServ _repo;

        public RequestController(IRequestServ repo)
        {
            _repo = repo;
        }

        [HttpGet("Read-AllRequest")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _repo.GetAllAsync();
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpPost("Send-Request")]
        public async Task<IActionResult> SendReqestAsync([FromBody] ReqModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.SendReqestAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
       
        [HttpPost("Accept-Request")]
        public async Task<IActionResult> AcceptReqestAsync(string requestId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AcceptRequestAsync(requestId);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
       
        [HttpPost("Deny-Request")]
        public async Task<IActionResult> DenyReqestAsync(string requestId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.DenyRequestAsync(requestId);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
        
        [HttpPost("Delete-Request")]
        public async Task<IActionResult> DeleteReqestAsync(string requestId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.DeleteRequestAsync(requestId);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

       
    }
}
