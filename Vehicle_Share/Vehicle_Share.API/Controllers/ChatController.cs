using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Vehicle_Share.Core.Models.AuthModels;
using Vehicle_Share.Core.Models.ChatModel;
using Vehicle_Share.EF.Data;
using Vehicle_Share.EF.Entities;

namespace Vehicle_Share.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMessage([FromRoute] string id)
        {
            var trip = await _context.Trip.FindAsync(id);
            if (trip is null)
                return BadRequest(" trip not found ");
            var messages = _context.Message
                 .Where(t => t.GroupName == trip.Id)
                 .OrderBy(d => d.CreatedOn)
                 .Select(m => new
                 {
                     m.GroupName,
                     m.Content,
                     m.Sender,
                     m.CreatedOn
                 
                 }).ToList();

            return Ok(messages);
        }
    }
}