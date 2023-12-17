using System.Text.Json.Serialization;
using MSyncBot.API.Types.Enums;

namespace MSyncBot.API.Types;

public class Message(string senderName, Guid senderId, SenderType senderType, MessageType messageType, User user)
{
    [JsonPropertyName("SenderName")]
    public string SenderName { get; set; } = senderName;
    
    [JsonPropertyName("SenderId")]
    public Guid SenderId { get; set; } = senderId;
    
    [JsonPropertyName("SenderType")]
    public SenderType SenderType { get; set; } = senderType;
    
    [JsonPropertyName("MessageType")]
    public MessageType MessageType { get; set; } = messageType;
    
    [JsonPropertyName("MessageId")]
    public int MessageId { get; set; } = GenerateMessageId();
    
    [JsonPropertyName("Content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("User")]
    public User User { get; set; } = user;
    
    [JsonPropertyName("MediaFiles")]
    public List<MediaFile> MediaFiles { get; set; } = new();
    
    [JsonPropertyName("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    private static int messageIdCounter;
    private static int GenerateMessageId() => messageIdCounter++;
}