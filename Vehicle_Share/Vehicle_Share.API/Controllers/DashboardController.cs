using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Response;
using Vehicle_Share.Service.CarService;
using Vehicle_Share.Service.DashboardService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardServ _service;

        public DashboardController(IDashboardServ service)
        {
            _service = service;
        }
        [HttpGet("Car/{id?}")]
        public async Task<IActionResult> GetCarAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                var result = await _service.GetAllCarAsync();
                if (result is ResponseDataModel<List<GetCarModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetCarByIdAsync(id);
                if (result is ResponseDataModel<GetCarModel> res)
                    return Ok(new { res.data });
                return BadRequest(new { result.code, result.message });
            }
        }
    }
}
