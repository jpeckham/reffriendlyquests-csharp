namespace RefFriendlyQuests;

using SPTarkov.Server.Core.Models.Spt.Mod;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.acidphantasm.reffriendlyquests";
    public string Name { get; init; } = "Ref Friendly Quests";
    public string Author { get; init; } = "acidphantasm";
    public List<string>? Contributors { get; init; } = ["jpeckham (local SPT 5.0 port)"];
    public SemanticVersioning.Version Version { get; init; } = new("2.1.1");
    public SemanticVersioning.Range SptVersion { get; init; } = new(">=5.0.0-0 <5.1.0-0");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/jpeckham/reffriendlyquests-csharp";
    public bool HasPrepatcher { get; init; }
    public string License { get; init; } = "MIT";
}
