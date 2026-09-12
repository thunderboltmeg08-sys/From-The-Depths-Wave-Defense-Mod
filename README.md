# From The Depths Wave Defense Mod

A fixed-position wave defense mode for From The Depths.

## Milestone 0.3.1 - In-Game HUD & Visual Countdown Timers

This milestone adds real-time visual HUD indicators rendered on screen:

- **HUD Display (Top-Center Screen)**:
  - **🛡️ Fortification Phase**: Amber header with real-time countdown (`Next Wave In: MM:SS`), upcoming wave number, target difficulty, and an interactive `[⚡ Start Wave Now]` button to skip downtime if ready.
  - **⚔️ Wave Assault Phase**: Crimson header with real-time survival countdown (`Survive For: MM:SS`), wave difficulty rating, objective reminder, and cumulative material bounty tracker.
  - **💀 Foothold Overwhelmed**: Summary screen showing total waves survived and materials earned.
- **HUD Hotkey**:
  - Press `[F8]` at any time to toggle HUD visibility on/off.
- **Persistent Unity Runner**:
  - `WaveController` and `WaveHud` are mounted onto a persistent Unity `GameObject` (`WaveDefense_Runner`) when the mod is loaded by FtD.

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
