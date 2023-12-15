using MSyncBot.Server.Types.Enums;

namespace MSyncBot.Server.Types;

public class Message(string senderName, int senderId, SenderType senderType, MessageType messageType, User user)
{
    public string SenderName { get; set; } = senderName;
    public int SenderId { get; set; } = senderId;
    public SenderType SenderType { get; set; } = senderType;
    public MessageType MessageType { get; set; } = messageType;
    public int MessageId { get; set; } = GenerateMessageId();
    public string? Content { get; set; }
    public User User { get; set; } = user;
    public List<MediaFile> MediaFiles { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    private static int messageIdCounter;
    private static int GenerateMessageId() => messageIdCounter++;
}