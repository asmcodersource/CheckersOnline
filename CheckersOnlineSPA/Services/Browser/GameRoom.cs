using System.Security.Claims;

namespace CheckersOnlineSPA.Services.Browser
{
    public record GameRoom
    {
        public int Id { get; set; }
        public GenericWebSocket CreatorSocket;
        public ClaimsPrincipal ClientCreator { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public GameRoom(int id, ClaimsPrincipal clientCreator, GenericWebSocket creatorSocket, string title, string description)
        {
            Id = id;
            ClientCreator = clientCreator;
            CreatorSocket = creatorSocket;
            Title = title;
            Description = description;
        }

        public GameRoomDTO CreateDTO()
        {
            Claim claimId = ClientCreator.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var clientId = claimId.Value;
            return new GameRoomDTO() { id = Id.ToString(), creatorId = clientId, title = Title, description = Description };
        }
    }

    public record GameRoomDTO
    {
        public string id { get; set; }
        public string creatorId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
