# From The Depths Wave Defense Mod

A fixed-position wave defense mode for From The Depths.

## Milestone 0.2.0

This milestone defines the fixed defense scenario. It uses the built-in Neter land-campaign starter structure at `Neter/Player/starting_fortress` and reserves world position `(0, 0, 0)` as the defense position.

The game currently exposes the starter structure through campaign data rather than a public runtime spawn method. The next step is to package a custom campaign entry that uses this scenario definition.

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
