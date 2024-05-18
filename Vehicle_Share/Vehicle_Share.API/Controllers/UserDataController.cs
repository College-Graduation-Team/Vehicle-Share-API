using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.GeneralModels;
using Vehicle_Share.Core.Models.UserData;
using Vehicle_Share.Core.Repository.GenericRepo;
using Vehicle_Share.Core.Response;
using Vehicle_Share.EF.Models;
using Vehicle_Share.Service.UserDataService;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataServ _service;
        private readonly IBaseRepo<User> _user;

        public UserDataController(IUserDataServ service, IBaseRepo<User> user)
        {
            _service = service;
            _user = user;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetMyUserDataAsync([FromRoute]string? id)
        {
            if (id is null)
            {
                var result = await _service.GetUserDataAsync();
                if (result is ResponseDataModel<GetUserModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserDataByIdAsyc(id);
                if (result is ResponseDataModel<GetUserModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> AddAndUpdateAsync([FromForm] UserDataModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateAsync(model);
            if (result is ResponseDataModel<ProfileImageModel> res)
                return Ok(new { res.message, res.data.Id });

            return BadRequest(new { result.message });

        }
       
        [HttpPost("Image")]
        public async Task<IActionResult> AddAndUpdateNationalImageAsync([FromForm] NationalImageModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _service.AddAndUpdateNationalImageAsync(model);
            if (result is ResponseDataModel<ImageModel> res)
                return Ok(new { res.message, res.data.Id });

            return BadRequest(new { result.message });

        }


        #region  Admin

        [HttpGet("Admin/user/{id?}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetAllUserAsync();
                if (result is ResponseDataModel<List<GetAllUsersModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserByIdAsyc(id);
                if (result is ResponseDataModel<GetAllUsersModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }


        [HttpGet("Admin/userdata/{id?}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetUserDataAsync([FromRoute] string? id)
        {
            if (id is null)
            {
                var result = await _service.GetUserDataAllAsync();
                if (result is ResponseDataModel<List<GetUserDataModel>> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }
            else
            {
                var result = await _service.GetUserDataByIdAsyc(id);
                if (result is ResponseDataModel<GetUserDataModel> res)
                    return Ok(new { res.data });

                return BadRequest(new { result.code, result.message });
            }

        }

       
        [HttpPut("Admin/UpdateDate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromForm] UserDataModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }

        [HttpPut("Admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatusRequestAsync([FromRoute] string id, [FromForm] UpdateStatusRequestModel model)
        {
            var result = await _service.UpdateStatusRequestAsync(id, model);
            if (result.IsSuccess)
                return Ok(new { result.message });
            return BadRequest(new { result.message });
        }

        #endregion

       /* [HttpPost("generate-fake-userData")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateFakeUsers(int count)
        {
            var users = await _user.GetAllAsync();
            var nonAdminUsers = users.Where(u => u.UserName != "Admin").ToList();
            var faker = new Faker<SeedModel>()
                .RuleFor(u => u.Name, f => f.Internet.UserName())
                .RuleFor(u => u.NationalId, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(u => u.Birthdate, f => f.Date.Past())
                .RuleFor(u => u.Gender, f => f.PickRandom(true, false))
                .RuleFor(u => u.Nationality, f => f.Address.Country())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.NationalCardImageFront, (f, u) => $"https://localhost:44305/User/{u.Name}/1435eb1a-5588-446b-9197-586390e5168c.3.jpeg") // You may customize this rule as needed
                .RuleFor(u => u.NationalCardImageBack, (f, u) => $"https://localhost:44305/User/{u.Name}/4f516309-72b5-44db-97b0-7b01c8aec159.4.jpeg")  // You may customize this rule as needed
                .RuleFor(u => u.ProfileImage, (f, u) => $"https://localhost:44305/User/{u.Name}/11f83072-cc95-4633-9654-0e3b5baa97c4.10.jpeg")          // You may customize this rule as needed                                                                                                                        //.RuleFor(u => u.userId, f => f.PickRandom(users).Id); // Get a random user ID
                .RuleFor(u => u.userId, f => users[(f.UniqueIndex) % (nonAdminUsers.Count)].Id);
            // You may customize this rule as needed

            var fakeUsers = faker.Generate(count);

            // Save fake users to the database or use them as needed
            foreach (var user in fakeUsers)
            {
                await _service.seedAsync(user);
            }

            return Ok(fakeUsers);
        }
*/
    
    }
}

