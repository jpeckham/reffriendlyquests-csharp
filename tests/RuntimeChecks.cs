using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Utils;

namespace RefFriendlyRuntimeChecks;

public static class Snapshot
{
    public static void Write(string phase, TemplateTable templates, TradersTable traders, LocaleTable locales, JsonUtil json)
    {
        var quests = templates.Quests.Where(pair => pair.Value.TraderId == Traders.REF).ToDictionary(pair => pair.Key.ToString(), pair => pair.Value);
        var locale = locales.Global["en"].Value!;
        var labels = quests.Values.SelectMany(q => q.Conditions.AvailableForFinish ?? [])
            .Where(c => !c.Id.IsEmpty).Select(c => c.Id.ToString()).Distinct()
            .ToDictionary(id => id, id => locale.GetValueOrDefault(id));
        Directory.CreateDirectory("user/logs");
        File.WriteAllText($"user/logs/ref-friendly-{phase}.json", json.Serialize(new
        {
            quests,
            labels,
            loyalty = traders.GetTrader(Traders.REF)!.Base.LoyaltyLevels,
        }, true));
        Console.WriteLine($"[RefFriendly Runtime Checks] {phase}: captured {quests.Count} Ref quests");
    }
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 13)]
public class Before(TemplateTable templates, TradersTable traders, LocaleTable locales, JsonUtil json) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken token) { Snapshot.Write("before", templates, traders, locales, json); return Task.CompletedTask; }
}

[Injectable(TypePriority = OnLoadOrder.PostLoad + 15)]
public class After(TemplateTable templates, TradersTable traders, LocaleTable locales, JsonUtil json) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken token) { Snapshot.Write("after", templates, traders, locales, json); return Task.CompletedTask; }
}

public record Metadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.jpeckham.reffriendly.runtimechecks";
    public string Name { get; init; } = "Ref Friendly Runtime Checks";
    public string Author { get; init; } = "jpeckham";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new(">=5.0.0-0 <5.1.0-0");
    public bool HasPrepatcher { get; init; }
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new() { ["com.acidphantasm.reffriendlyquests"] = new("~2.1.1") };
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
}
