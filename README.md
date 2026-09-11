# From The Depths Wave Defense Mod

A fixed-position wave defense mode for From The Depths.

## Milestone 0.1.0

This milestone verifies that the native plugin loads in From The Depths. The wave gameplay system is intentionally not implemented yet.

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
