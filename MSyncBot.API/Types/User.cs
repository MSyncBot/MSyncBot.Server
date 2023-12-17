using System.Text.Json.Serialization;

namespace MSyncBot.API.Types;

public class User(string firstName, string? lastName = null, string? username = null, ulong? id = null)
{
    [JsonPropertyName("FirstName")]
    public string FirstName { get; set; } = firstName;
    
    [JsonPropertyName("LastName")]
    public string? LastName { get; set; } = lastName;
    
    [JsonPropertyName("Username")]
    public string? Username { get; set; } = username;
    
    [JsonPropertyName("Id")]
    public ulong? Id { get; set; } = id;
}