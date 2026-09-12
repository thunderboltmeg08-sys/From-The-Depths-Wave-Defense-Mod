param(
    [string]$GameRoot = 'D:\Program Files (x86)\Steam\steamapps\common\From The Depths'
)

$ErrorActionPreference = 'Stop'

$worlds = Join-Path $GameRoot 'From_The_Depths_Data\StreamingAssets\Worlds'
$sourceName = 'Ashes of the Empire'
$targetName = 'Hold Your Ground'
$targetPrefix = Join-Path $worlds $targetName

if (-not (Test-Path -LiteralPath $worlds -PathType Container)) {
    throw "Worlds directory not found: $worlds"
}

$sourceFiles = Get-ChildItem -LiteralPath $worlds -File |
    Where-Object {
        $_.Name.StartsWith($sourceName + '.', [StringComparison]::OrdinalIgnoreCase) -and
        $_.Name -notmatch '_backup$' -and
        $_.Extension -ne '.hash'
    }

if (-not $sourceFiles) {
    throw "No Ashes of the Empire world files found in $worlds"
}

Get-ChildItem -LiteralPath $worlds -File |
    Where-Object { $_.Name.StartsWith($targetName + '..', [StringComparison]::OrdinalIgnoreCase) } |
    Remove-Item -Force

Remove-Item -LiteralPath (Join-Path $worlds ($targetName + '.hash')) -Force -ErrorAction SilentlyContinue

foreach ($sourceFile in $sourceFiles) {
    $extension = $sourceFile.Name.Substring($sourceName.Length)
    Copy-Item -LiteralPath $sourceFile.FullName -Destination ($targetPrefix + $extension) -Force
}

$planetPath = $targetPrefix + '.planet'
$planet = Get-Content -LiteralPath $planetPath -Raw | ConvertFrom-Json
$planet.Name = 'Hold Your Ground'
$planet.PlanetIdentifier = [Guid]::NewGuid().ToString()
$planet.VersionIdentifier = [Guid]::NewGuid().ToString()
$planet.Summary = 'A fixed-position wave defense mode based on the Ashes of the Empire world. Stand your ground against relentless enemy waves.'
$planet.CampaignMainMenuPanelText = 'Defend your foothold against escalating waves of enemy forces. Survive as long as you can.'
$planet.AllowAdventures = $false
$planet.AllowStories = $false
$planet | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $planetPath -Encoding utf8

$campaignPath = $targetPrefix + '.campaign'
$campaign = Get-Content -LiteralPath $campaignPath -Raw | ConvertFrom-Json
foreach ($instance in $campaign.Instances) {
    $instance.Header.Id.Id = Get-Random -Minimum 100000000 -Maximum 2000000000
    $instance.Header.Guid = [Guid]::NewGuid().ToString()
    $instance.Header.Name = 'Hold Your Ground'
    $instance.Header.Summary = "Hold Your Ground - Wave Defense`n`nYou are stationed at a remote foothold on the ash-covered plains. Enemy forces will assault your position in escalating waves. Fortify your base, manage your resources, and survive the onslaught."
    $instance.Header.CampaignHeader.Hash = ''
    $instance.Header.CommonSettings.AvatarAutoSpawn = 1
    $instance.Header.CommonSettings.BlueprintSpawningOptions = 3
    $instance.Header.CommonSettings.AccessToMap = 1
    $instance.Header.CommonSettings.FogOfWarType = 0
    $instance.Header.CommonSettings.DiplomacyMode = 0
    $instance.Header.CommonSettings.DiplomacyMeetingTime = 0
    $instance.Header.CommonSettings.DisplayRelationshipMatrix = $false
    $instance.Header.CommonSettings.DisplayReinforcementsInMenu = $false
    $instance.Header.CommonSettings.DisplayEnemiesInMenu = $false
    $instance.Header.CommonSettings.DisplayFailureConditions = $false

    # Make every non-player faction hostile to the player for wave combat.
    if ($instance.Factions -and $instance.Factions.Relationships -and $instance.Factions.Relationships.R) {
        $relationshipMatrix = $instance.Factions.Relationships.R
        for ($factionIndex = 1; $factionIndex -lt $relationshipMatrix.Count; $factionIndex++) {
            $relationshipMatrix[0][$factionIndex] = -100.0
            $relationshipMatrix[$factionIndex][0] = -100.0
        }
    }
    
    # Clear out the normal campaign victory conditions that auto-trigger when enemies/territories are neutralized
    if ($instance.VictoryConditions) {
        $instance.VictoryConditions.GroupConditions = @()
        $instance.VictoryConditions.FullConditions = @()
    }
    
    # Clear out the Ashes story events (like the "Junk Trader" lore popup)
    if ($instance.EventSystem) {
        $instance.EventSystem.Events = @()
    }

    # Keep only the player's foothold fleet. Enemy faction designs remain available as
    # wave templates, but their campaign fleets must not exist or FtD will offer battles.
    if ($instance.Factions -and $instance.Factions.Factions.Count -gt 0) {
        $playerFaction = $instance.Factions.Factions[0]
        if ($playerFaction.Fleets -and $playerFaction.Fleets.Fleets) {
            $mainFleets = @($playerFaction.Fleets.Fleets | Where-Object { $_.Name -eq 'The Watering Hole' })
            foreach ($fleet in $mainFleets) {
                if ($fleet.Forces) {
                    # Filter out Land harvester units from player forces
                    $fleet.Forces = @($fleet.Forces | Where-Object { $_.Name -ne 'Land harvester' })
                }
            }
            $playerFaction.Fleets.Fleets = $mainFleets
        }

        for ($factionIndex = 1; $factionIndex -lt $instance.Factions.Factions.Count; $factionIndex++) {
            $instance.Factions.Factions[$factionIndex].Fleets.Fleets = @()
        }
    }

    # Remove all remote resource rings / material regeneration zones, keeping only the single Foothold Depot
    if ($instance.ResourceZones -and $instance.ResourceZones.Zones.Count -gt 0) {
        $footholdZone = $instance.ResourceZones.Zones[0]
        $footholdZone.Name = "Foothold Stockpile"
        $footholdZone.Description = "The primary resource depot supporting your defensive foothold."
        $footholdZone.Material.Growth = 0.0 # Disable passive map-wide regeneration rings
        $instance.ResourceZones.Zones = @($footholdZone)
    }
}
$campaign | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $campaignPath -Encoding utf8

Write-Output "Installed separate campaign: $targetName"
Write-Output "Files copied: $($sourceFiles.Count)"
Write-Output "Campaign file: $campaignPath"

# Also sync and install mod plugin files
$modDir = Join-Path $GameRoot 'From_The_Depths_Data\StreamingAssets\Mods\WaveDefense'
if (-not (Test-Path -LiteralPath $modDir)) {
    New-Item -ItemType Directory -Path $modDir -Force | Out-Null
}
$repoRoot = Split-Path -Parent $PSScriptRoot
if (Test-Path -LiteralPath (Join-Path $repoRoot 'header.header')) {
    Copy-Item -LiteralPath (Join-Path $repoRoot 'header.header') -Destination (Join-Path $modDir 'header.header') -Force
}
if (Test-Path -LiteralPath (Join-Path $repoRoot 'plugin.json')) {
    Copy-Item -LiteralPath (Join-Path $repoRoot 'plugin.json') -Destination (Join-Path $modDir 'plugin.json') -Force
}
if (Test-Path -LiteralPath (Join-Path $repoRoot 'bin\Debug\WaveDefense.dll')) {
    Copy-Item -LiteralPath (Join-Path $repoRoot 'bin\Debug\WaveDefense.dll') -Destination (Join-Path $modDir 'WaveDefense.dll') -Force
}
Write-Output "Installed mod plugin to: $modDir"