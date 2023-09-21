using YamlDotNet.Serialization;

namespace GrantBot.Models;

public class RankConfig
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }
    
    [YamlMember(Alias = "id")]
    public ulong Id { get; set; }
    
    [YamlMember(Alias = "priority")]
    public uint Priority { get; set; }
    
    [YamlMember(Alias = "image")]
    public string? Image { get; set; }
}