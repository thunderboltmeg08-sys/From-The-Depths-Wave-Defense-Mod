# From The Depths Wave Defense Mod

A fixed-position wave defense mode for From The Depths.

## Milestone 0.2.1

This milestone defines the fixed defense scenario using the Ashes of the Empire player land base at `Neter/Player/Foot Hold Base` and the Ashes campaign default world position `(68.73599, 0, 193.3501)` as the defense position.

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

The copied folder must contain `plugin.json` and `WaveDefense.dll`.

## Install the separate campaign mode

Run the setup script from PowerShell:

```powershell
.\tools\Install-HoldYourGround.ps1
```

This creates a new `Hold Your Ground` campaign entry from the installed Ashes world data. It writes new files named `Hold Your Ground.*` and does not replace the normal Ashes of the Empire files. The generated campaign disables diplomacy, council meetings, relationship displays, and normal campaign reinforcement/enemy menus.

The installer assigns unique campaign and planet identifiers so FtD treats Hold Your Ground as a separate load target rather than an Ashes duplicate. It applies the fixed-defense settings to both campaign instances, enables map access with no fog of war, and removes the inherited territory grid while retaining faction data for future waves.

After running the script, start FtD and select **Hold Your Ground** from the campaign list. The current milestone provides the separate campaign entry and Ashes foothold setup; escalating wave spawning is the next gameplay milestone.
