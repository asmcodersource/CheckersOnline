using Newtonsoft.Json.Linq;
using CheckersOnlineSPA.Services.Chat.ChatClient;

namespace CheckersOnlineSPA.Services.Chat.ChatMessages
{
    [Serializable]
    public class ChatMessageWrapper
    {
        public DateTime SendTime { get; set; }
        public dynamic Message { get; set; }

        public ChatMessageWrapper(object Message)
        {
            this.Message = Message;
            SendTime = DateTime.UtcNow;
        }
    }
}
