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

        [HttpGet("Read-All-Requests-trip/{id}")]
        public async Task<IActionResult> GetAllTripRequestedAsync(string id)
        {
            var result = await _repo.GetAllTripRequestedAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.Data });

            return BadRequest(new { result.ErrorMesssage });
        }

        [HttpGet("Read-All-MyRequest")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _repo.GetAllMyRequestAsync();
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
       
        [HttpPost("Accept-Request/{id}")]
        public async Task<IActionResult> AcceptReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.AcceptRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
       
        [HttpPost("Deny-Request/{id}")]
        public async Task<IActionResult> DenyReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.DenyRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.Messsage });

            return BadRequest(new { result.Messsage });
        }
        
        [HttpPost("Delete-Request/{id}")]
        public async Task<IActionResult> DeleteReqestAsync([FromRoute]string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _repo.DeleteRequestAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

       
    }
}
