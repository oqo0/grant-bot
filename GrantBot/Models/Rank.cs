using YamlDotNet.Serialization;

namespace GrantBot.Models;

public class Rank
{
    public string Name { get; set; }
    public ulong Id { get; set; }
    public uint Priority { get; set; }
    public string? Image { get; set; }
}