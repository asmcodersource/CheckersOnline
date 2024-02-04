using CheckersOnlineSPA.Services.Chat.ChatClient;

namespace CheckersOnlineSPA.Services.Chat.ChatRoomAcceptRules
{
    public interface IChatRoomAcceptRule
    {
        public IChatClient AcceptClient(GenericWebSocket webSocket);
    }
}
