# From The Depths Wave Defense Mod

A fixed-position wave defense mode for From The Depths.

## Milestone 0.2.1

This milestone defines the fixed defense scenario using the Ashes of the Empire player land base at `Neter/Player/Foot Hold Base` and reserves world position `(0, 0, 0)` as the defense position.

The matching built-in asset is `Foot Hold Base.blueprint`. The game exposes this structure through campaign data rather than a public runtime spawn method. The next step is to package a custom campaign entry that uses this scenario definition.

## Build

From a PowerShell terminal:

```powershell
dotnet build .\WaveDefense.csproj -c Debug
```

The project defaults to this FtD installation path:

```text
D:\Program Files (x86)\Steam\steamapps\common\From The Depths
```

For a different installation path:

```powershell
dotnet build .\WaveDefense.csproj -c Debug -p:FtdGamePath="D:\path\to\From The Depths"
```

## Install for testing

Copy this repository folder into:

```text
From_The_Depths_Data\StreamingAssets\Mods\WaveDefense
```

The copied folder must contain `plugin.json` and `bin\Debug\WaveDefense.dll`.
