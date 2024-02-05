using CheckersOnlineSPA.Services.Chat.ChatRoom;

namespace CheckersOnlineSPA.Services.Chat
{
    public class ChatRoomsController
    {
        private int i = 0;
        public List<IChatRoom> ChatRooms { get; protected set; } = new List<IChatRoom>();
        protected Dictionary<int, IChatRoom> RoomsDictionary = new Dictionary<int, IChatRoom>();

        public IChatRoom? GetRoomById(int id)
        {
            if( RoomsDictionary.ContainsKey(id))
                return RoomsDictionary[i];
            return null;
        }

        public IChatRoom? CreateChatRoom(ChatRoomType chatRoomType)
        {
            int creatingRoomId = ClaimNewRoomId();
            IChatRoom? chatRoom = null;
            switch (chatRoomType) {
                case ChatRoomType.PublicChatRoom:
                    chatRoom = new PublicGameChatRoom(creatingRoomId);
                    break;
            };
            if (chatRoom == null)
                return null;
            RoomsDictionary.Add(creatingRoomId, chatRoom);
            ChatRooms.Add(chatRoom);
            return chatRoom;
        }

        public void RemoveChatRoom(PublicGameChatRoom chatRoom)
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
