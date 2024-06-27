using Microsoft.AspNetCore.SignalR;
using Vehicle_Share.EF.Data;
using Vehicle_Share.EF.Entities;

namespace Chat
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessageToGroup(string group, string sender, string message)// tripid , name , msg
        {
            var MSG = new Message() 
            { 
                Id=Guid.NewGuid().ToString(),
                GroupName=group,
                Sender=sender,
                Content=message,
                CreatedOn=DateTime.UtcNow
            };
            await _context.AddAsync(MSG);
            await _context.SaveChangesAsync();

          await  Clients.Group(group).SendAsync("ReceiveGroupMessage", group, sender, message);
        }


        // to make join to group if connection closed we need to add member(who are in group) aga in
        public async Task JoinGroup(string groupName , string name)
        {
          await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
          await Clients.OthersInGroup(groupName).SendAsync("newmebemer",name, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

    }

}
