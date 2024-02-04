namespace CheckersOnlineSPA.Services.Chat
{
    public class ChatRoomsController
    {
        private int i = 0;
        public List<ChatRoom> ChatRooms { get; protected set; } = new List<ChatRoom>();
        protected Dictionary<int, ChatRoom> RoomsDictionary = new Dictionary<int, ChatRoom>();
        
        public ChatRoom CreateChatRoom()
        {
            int creatingRoomId = ClaimNewRoomId();
            ChatRoom chatRoom = new ChatRoom(creatingRoomId);
            RoomsDictionary.Add(creatingRoomId, chatRoom);
            ChatRooms.Add(chatRoom);
            return chatRoom;
        }

        public void RemoveChatRoom(ChatRoom chatRoom)
        {
            RoomsDictionary.Remove(chatRoom.Id);
            ChatRooms.Remove(chatRoom);
        }

        protected int ClaimNewRoomId()
        {
            // TODO: realize something better
            return i++;
        }
    }
}
