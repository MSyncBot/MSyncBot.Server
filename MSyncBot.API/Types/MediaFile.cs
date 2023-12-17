using System.Text.Json.Serialization;

namespace MSyncBot.API.Types;

public class MediaFile(string name, string extension)
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = name;
    
    [JsonPropertyName("Extension")]
    public string Extension { get; set; } = extension;
}