using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Share.Core.Models.AuthModels;
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
        public async Task<IActionResult> GetUserDataAsync([FromRoute] string? id)
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
       
        [HttpPost("generate-fake-userData")]
        public async Task<IActionResult> GenerateFakeUsers(int count)
        {
            var users = await _user.GetAllAsync();
            var faker = new Faker<SeedModel>()
                .RuleFor(u => u.Name, f => f.Internet.UserName())
                .RuleFor(u => u.NationalId, f => f.Random.Long(10000000000000, 99999999999999))
                .RuleFor(u => u.Birthdate, f => f.Date.Past())
                .RuleFor(u => u.Gender, f => f.PickRandom(true, false))
                .RuleFor(u => u.Nationality, f => f.Address.Country())
                .RuleFor(u => u.Address, f => f.Address.FullAddress())
                .RuleFor(u => u.NationalCardImageFront, f => "https://localhost:44305/User/3f6f18a6-470c-4beb-85c7-9a2621d010c2.1.jpeg") // You may customize this rule as needed
                .RuleFor(u => u.NationalCardImageBack, f => "https://localhost:44305/User/3f6f18a6-470c-4beb-85c7-9a2621d010c2.1.jpeg")  // You may customize this rule as needed
                .RuleFor(u => u.ProfileImage, f => "https://localhost:44305/User/3f6f18a6-470c-4beb-85c7-9a2621d010c2.1.jpeg")          // You may customize this rule as needed                                                                                                                        //.RuleFor(u => u.userId, f => f.PickRandom(users).Id); // Get a random user ID
                .RuleFor(u => u.userId, f => users[(f.UniqueIndex + 1) % (users.Count - 1)].Id);
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

