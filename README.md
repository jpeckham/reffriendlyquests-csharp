# Ref Friendly Quests — local SPT 5.0 port

Fork of [acidphantasm/reffriendlyquests-csharp](https://github.com/acidphantasm/reffriendlyquests-csharp), upstream `master` at `6635a21eacb0f25e3d7216e182a6ae6d9367cf9a`. No public 5.0 branch/fork was found; the only historical PR targeted 4.1.1. Original author and MIT license retained.

Branch `spt-5.0-eft-1.3.0.0.48484` targets our local SPT 5.0 BE / EFT 1.3.0.0.48484 installation. Version 2.1.1 is a local compatibility port, not an upstream release. This server mod replaces six Ref quest finish-condition sets with raid objectives, updates their locale strings, optionally reduces Ref standing requirements and adds Lega medal rewards. It does not auto-complete quests.

## Build and install

Requires .NET 10 and the matching SPT installation. Builds reference its server DLLs without packaging those DLLs or automatically deploying.

```powershell
dotnet build acidphantasm-reffriendlyquests.csproj -c Release '-p:SPTPath=C:\sptarkov\SPTarkov 5.0.0-BE-1.3.0.0.48484'
```

With the server/client stopped, back up profiles and any previous installation, then copy `Build/SPT_Runtime/user/mods/acidphantasm-reffriendlyquests` into the target's `SPT_Runtime/user/mods`. Keep the folder name for the web config editor. The package contains the mod DLL, `db/quests.json`, `config.json`, README and LICENSE. No client plugin is required.

The supplied config is retained: standing requirement multiplier **0.85**, add Lega medal rewards **true**, GP reward multiplier **1**. The upstream code fallback for a missing config is 0.8; include the supplied config to retain 0.85. Settings take effect after server restart. Quest objective text remains English, as in upstream.

To uninstall, stop the server/client and remove the mod folder. This restores base templates on restart but does not undo progress or rewards already saved in profiles. Restore a profile backup only if intentionally reverting that progress.

## Port validation

Compatibility changes: build against the installed SPT 5.0 assemblies, use nonnullable `LoyaltyLevel.MinStanding`, and supply required empty `AvailableForStart`/`Fail` fields in the replacement-data schema. Only finish conditions are copied into real quest templates; original prerequisites/fail conditions are preserved. The upstream auto-deploy/7-Zip build target is replaced with local staging.

Release build: zero warnings/errors. Installed-server before/after snapshots verified all six replacement objective sets, six populated weapon groups, objective locale overrides, unchanged prerequisite/failure conditions, Lega medals on all 15 targeted quests, unchanged GP rewards with multiplier 1, and the 0.85 standing multiplier. Existing profile JSON content stayed unchanged. The temporary test harness was removed after verification. In-game quest progression/reward turn-in must still be tested by the player.

To repeat the runtime check, build `tests/RuntimeChecks.csproj -c Release` with the same `SPTPath`, temporarily install its RuntimeChecks.dll in a separate `SPT_Runtime/user/mods/RefFriendlyRuntimeChecks` folder, and start the server with this mod installed. The harness captures database tables immediately before/after the mod loads; it does not edit profiles. Run:

```powershell
python tests/verify_runtime.py --runtime 'C:\sptarkov\SPTarkov 5.0.0-BE-1.3.0.0.48484\SPT_Runtime'
```

Then stop the server and remove the temporary harness. It is excluded from the production package. Snapshots are `user/logs/ref-friendly-before.json` and `ref-friendly-after.json`.
