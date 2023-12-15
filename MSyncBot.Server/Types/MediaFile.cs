namespace MSyncBot.Server.Types;

public class MediaFile(string name, string extension)
{
    public string Name { get; set; } = name;
    public string Extension { get; set; } = extension;
}