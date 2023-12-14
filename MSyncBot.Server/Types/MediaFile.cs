namespace MSyncBot.Server.Types;

public class MediaFile(string name, string extension, byte[] data)
{
    public string Name { get; set; } = name;
    public string Extension { get; set; } = extension;
    public byte[] Data { get; set; } = data;
}