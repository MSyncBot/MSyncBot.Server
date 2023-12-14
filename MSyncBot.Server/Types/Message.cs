using MSyncBot.Server.Types.Enums;

namespace MSyncBot.Server.Types;

public class Message(string senderName, int senderId, SenderType senderType, string content, User user)
{
    public string SenderName { get; set; } = senderName;
    public int SenderId { get; set; } = senderId;
    public SenderType SenderType { get; set; } = senderType;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int MessageId { get; set; } = GenerateMessageId();
    public string Content { get; set; } = content;
    public User User { get; set; } = user;
    public List<MediaFile> MediaFiles = new();
    private static int messageIdCounter;
    private static int GenerateMessageId() => messageIdCounter++;
}