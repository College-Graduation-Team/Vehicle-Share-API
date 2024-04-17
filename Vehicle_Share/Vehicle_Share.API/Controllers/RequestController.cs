﻿using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.RequestModels;
using Vehicle_Share.Service.RequestService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestServ _service;

        public RequestController(IRequestServ service)
        {
            _service = service;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetAllTripRequestedAsync([FromRoute] string? id)
        {
            if (id == null) {
                var result = await _service.GetAllMyRequestAsync();
                if (result.IsSuccess)
                    return Ok(new { result.data });

                return BadRequest(new { result.message });
            } else {
                var result = await _service.GetAllTripRequestedAsync(id);
                if (result.IsSuccess)
                    return Ok(new { result.data });

                return BadRequest(new { result.message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendReqestAsync([FromBody] ReqModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.SendReqestAsync(model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AcceptRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
        }

        [HttpPost("decline/{id}")]
        public async Task<IActionResult> DenyReqestAsync([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.DenyRequestAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.message });
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
