using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Service.RequestService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestServ _service;

        public RequestController(IRequestServ service)
        {
            _service = service;
        }



        #region Get requests 


        [HttpGet("{id?}")]
        public async Task<IActionResult> GetAllTripRequestedAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                var result = await _service.GetAllMyRequestAsync();
                if (result is ResponseDataModel<List<GetReqModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });

            }
            else
            {
                var result = await _service.GetAllTripRequestedAsync(id);
                if (result is ResponseDataModel<List<GetReqModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
        }

        [HttpGet("Send/driver")]
        public async Task<IActionResult> GetSendRequestDriverAsync()
        {
              var result = await _service.GetSendRequestDriverAsync();
                if (result is ResponseDataModel<List<GetReqModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
        }
        [HttpGet("Receive/driver")]
        public async Task<IActionResult> GetReceiveRequestDriverAsync()
        {
            var result = await _service.GetReceiveRequestDriverAsync();
            if (result is ResponseDataModel<List<GetReqModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpGet("Send/passenger")]
        public async Task<IActionResult> GetSendRequestPassengerAsync()
        {
            var result = await _service.GetSendRequestPassengerAsync();
            if (result is ResponseDataModel<List<GetReqModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }
        [HttpGet("Receive/passenger")]
        public async Task<IActionResult> GetReceiveRequestPassengerAsync()
        {
            var result = await _service.GetReceiveRequestPassengerAsync();
            if (result is ResponseDataModel<List<GetReqModel>> res)
                return Ok(new { res.data });

            return BadRequest(new { result.code, result.message });
        }


        #endregion



        [HttpPost]
        public async Task<IActionResult> SendReqestAsync([FromBody] ReqModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.SendReqestAsync(model);
            if (result is ResponseDataModel<IdResponseModel> res)
                return Ok(new { res.message , res.data});

            return BadRequest(new { result.code, result.message });
        }

        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AcceptRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPost("decline/{id}")]
        public async Task<IActionResult> DenyReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.DenyRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.DeleteRequestAsync(id);
            if (result > 0)
                return Ok(result);

            return BadRequest(result);
        }

    }
}
