using CheckersOnlineSPA.Services.Chat.ChatClient;
using System.Net.Sockets;
using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Chat.ChatRoomAcceptRules
{
    /// <summary>
    /// Rule for Acceptance chat clients By Email Claims
    /// Clients allowed by this rule can text in chat
    /// Clients is not allowed by this rule can read chat
    /// </summary>
    public class ChatRoomRuleABEC: IChatRoomAcceptRule
    {
        public IChatRoom Room { get; protected set; }
        protected HashSet<string> allowedEmails { get; set; }

        public ChatRoomRuleABEC(IChatRoom room)
        {
            Room = room;
            allowedEmails = new HashSet<string>();
        }

        public IChatClient AcceptClient(GenericWebSocket webSocket)
        {
            var email = webSocket.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            if (allowedEmails.Contains(email))
                return new SendReceiveChatClient(Room, webSocket);
            return new SendReceiveChatClient(Room, webSocket);
        }

        public void AddAllowedEmail(string email) => allowedEmails.Add(email);
        public void RemoveAllowedEmail(string email) => allowedEmails.Remove(email);
    }
}
