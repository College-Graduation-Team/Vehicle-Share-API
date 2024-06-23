using Microsoft.AspNetCore.SignalR;
using Vehicle_Share.EF.Data;

namespace Chat
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessageToGroup(string group, string name, string message)// tripid , msg
        {
          await  Clients.Group(group).SendAsync("ReceiveGroupMessage", group, name, message);
        }

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
