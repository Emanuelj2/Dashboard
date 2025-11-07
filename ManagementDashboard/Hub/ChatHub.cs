
namespace ManagementDashboard.Hub
{
    using ManagementDashboard.Data;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Concurrent;

    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private static readonly ConcurrentDictionary<string, int> ConnectedUsers = new();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    ConnectedUsers[user.Email] = user.Id;

                    // NEW: Load unread messages
                    var unreadMessages = await GetUnreadMessages();
                    if (unreadMessages.Any())
                    {
                        await Clients.Caller.SendAsync("LoadUnreadMessages", unreadMessages);
                    }

                    await Clients.All.SendAsync("UpdateUserList", await GetOnlineUsers());
                }
            }
            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    ConnectedUsers.TryRemove(user.Email, out _);
                    await Clients.All.SendAsync("UpdateUserList", await GetOnlineUsers());
                }
            }
            await base.OnDisconnectedAsync(exception);
        }



        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }



        private string? GetConnectionId(int userId)
        {
            var email = ConnectedUsers.FirstOrDefault(x => x.Value == userId).Key;
            return email != null ? Context.ConnectionId : null;
        }



        private async Task<List<object>> GetOnlineUsers()
        {
            var userIds = ConnectedUsers.Values.ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Name, u.Email, u.Role })
                .ToListAsync();

            return users.Cast<object>().ToList();
        }


        //send message to all clients
        public async Task SendMessage(string message)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            //save message to database
            var msg = new Models.Message
            {
                SenderId = user.Id,
                Content = message,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            //broadcast message to all clients
            await Clients.All.SendAsync("ReceiveMessage", user.Name, message, msg.SentAt);
        }



        //send private message
        public async Task SendPrivateMessage(string recipientEmail, string message)
        {
            var senderId = GetUserId();
            if (senderId == null) return;

            var sender = await _context.Users.FindAsync(senderId);
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Email == recipientEmail);

            //if either sender or recipient is null, return
            if (sender == null || recipient == null) return;

            //save message to database
            var msg = new Models.Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = message,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            //send message to recipient if online
            if (ConnectedUsers.TryGetValue(recipient.Email, out var recipientUserId))
            {
                var recipientConnectionId = GetConnectionId(recipientUserId);
                if (recipientConnectionId != null)
                {
                    await Clients.Client(recipientConnectionId).SendAsync(
                        "ReceivePrivateMessage",
                        sender.Name,
                        message,
                        msg.SentAt);
                }
            }

            //send confirmation to sender
            await Clients.Caller.SendAsync(
                "PrivateMessageSent",
                recipient.Name,
                message,
                msg.SentAt);
        }



        //join group
        public async Task JoinGroup(string groupName)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync(
                "ReceiveMessage",
                "System",
                $"{user.Name} has joined the group {groupName}.",
                DateTime.UtcNow);
        }




        //leave group
        public async Task LeaveGroup(string groupName)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync(
                "ReceiveGroupMessage",
                groupName,
                "System",
                $"{user.Name} left the group",
                DateTime.UtcNow);
        }


        //send group message
        public async Task SendMessageToGroup(string groupName, string message)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            //save message to database
            var msg = new Models.Message
            {
                SenderId = user.Id,
                GroupName = groupName,
                Content = message,
                SentAt = DateTime.UtcNow
            };

            //add and savee the message to database
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            //broadcast message to group
            await Clients.Group(groupName).SendAsync(
                "ReceiveGroupMessage",
                groupName,
                user.Name,
                message,
                msg.SentAt);
        }

        public async Task<List<object>> GetUnreadMessages()
        {
            var userId = GetUserId();
            if (userId == null) return new List<object>();

            var unreadMessages = await _context.Messages
                .Where(m => m.RecipientId == userId && !m.IsRead)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    SenderName = m.Sender.Name,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    MessageId = m.Id
                })
                .ToListAsync();

            return unreadMessages.Cast<object>().ToList();
        }

        public async Task MarkMessageAsRead(int messageId)
        {
            var userId = GetUserId();
            if (userId == null) return;

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.RecipientId == userId);

            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }


    }
}
