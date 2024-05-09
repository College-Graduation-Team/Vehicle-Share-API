using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Vehicle_Share.Core.Models.CarModels;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.LicModels;
using Vehicle_Share.Core.Models.TripModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.CarService;
namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize()]
    public class CarController : ControllerBase
    {
        private readonly ICarServ _service;
        private readonly IBaseRepo<License> _Lic;
        private readonly IBaseRepo<UserData> _user;
        public CarController(ICarServ service, IBaseRepo<UserData> user, IBaseRepo<License> lic)
        {
            _service = service;
            _user = user;
            _Lic = lic;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetCarAsync([FromRoute] string? id)
        {
            if (id == null)
            {
                var result = await _service.GetAllAsync();
                if (result is ResponseDataModel<List<GetCarModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetByIdAsync(id);
                if (result is ResponseDataModel<GetCarModel> res)
                    return Ok(new { res.data });
                return BadRequest(new { result.code, result.message }); 
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCarAsync([FromForm] CarModel model)
        {
            if (!ModelState.IsValid || model == null)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(model);
            if (result is ResponseDataModel<GetImageCarModel> res)
                return Ok(new {  res.message, res.data });

            return BadRequest(new { result.code, result.message });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdataCarAsync([FromRoute] string id, [FromForm] UpdateCarModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }
        

        #region Admin

        [HttpGet("Admin/{userdataId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUserDataIdAsync([FromRoute] string userdataId)
        {
           
                var result = await _service.GetByUserDataIdAsync(userdataId);
                if (result is ResponseDataModel<GetCarModel> res)
                    return Ok(new { res.data });
                return BadRequest(new { result.code, result.message });
            
        }

        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdataCarStatusAsync([FromRoute] string id, [FromBody] UpdateStatusRequestModel model)
        {
            var result = await _service.UpdateStatusRequestAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }

        #endregion

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarAsync([FromRoute] string id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(new { result.message });

            return BadRequest(new { result.code, result.message });
        }


        [HttpPost("generate-fake-License")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateFakeUsers(int count)
        {
            var userLic = new List<UserData>();
            var users = await _user.GetAllAsync();
        //    var lic = await _Lic.GetAllAsync();
            foreach (var use in users)
            {
                var lic = await _Lic.FindAsync(e => e.UserDataId == use.Id);
                if (lic is null) continue;
                
                    var x = await _user.GetByIdAsync(lic.UserDataId);
                userLic.Add(use);
            }
            
            var nonAdminUsers = userLic.Where(u => u.Name != "Admin").ToList();
            var faker = new Faker<SeedCarModel>()
               .RuleFor(c => c.Type, f => f.Vehicle.Type())
               .RuleFor(c => c.ModelYear, f => f.Random.Number(1990, DateTime.UtcNow.Year))
               .RuleFor(c => c.Brand, f => f.Vehicle.Manufacturer())
               .RuleFor(c => c.Plate, f => "QWER 1234")
               .RuleFor(c => c.Seats, f => f.Random.Short(2, 4))
               .RuleFor(c => c.LicenseImageFront, f => $"https://localhost:44305/License/{users[(f.UniqueIndex) % (nonAdminUsers.Count)].Name}/11f83072-cc95-4633-9654-0e3b5baa97c4.98.jpeg")
               .RuleFor(c => c.LicenseImageBack, f => $"https://localhost:44305/License/{users[(f.UniqueIndex) % (nonAdminUsers.Count)].Name}/11f83072-cc95-4633-9654-0e3b5baa97c4.99.jpeg")
               .RuleFor(c => c.Image, f => $"https://localhost:44305/License/{users[(f.UniqueIndex) % (nonAdminUsers.Count)].Name}/11f83072-cc95-4633-9654-0e3b5baa97c4.97.jpeg")
               .RuleFor(c => c.userId,f=> users[(f.UniqueIndex) % (nonAdminUsers.Count)].Id)
               .RuleFor(c => c.LicenseExpiration, f => f.Date.Future());
            // You may customize this rule as needed

            var fakeUsers = faker.Generate(count);

            // Save fake users to the database or use them as needed
            foreach (var user in fakeUsers)
            {
                await _service.seedAsync(user);
            }

            return Ok(fakeUsers);
        }


    }
}
