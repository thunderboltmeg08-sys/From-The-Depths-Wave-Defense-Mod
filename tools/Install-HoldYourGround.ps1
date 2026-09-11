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
$planet.Summary = 'A fixed-position tower defense campaign based on the Ashes of the Empire world.'
$planet.CampaignMainMenuPanelText = 'Defend the foothold. Survive escalating enemy waves.'
$planet.AllowAdventures = $false
$planet.AllowStories = $false
$planet | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $planetPath -Encoding utf8

$campaignPath = $targetPrefix + '.campaign'
$campaign = Get-Content -LiteralPath $campaignPath -Raw | ConvertFrom-Json
foreach ($instance in $campaign.Instances) {
    $instance.Header.Id.Id = Get-Random -Minimum 100000000 -Maximum 2000000000
    $instance.Header.Guid = [Guid]::NewGuid().ToString()
    $instance.Header.Name = 'Hold Your Ground'
    $instance.Header.Summary = 'Defend a fixed Ashes foothold against escalating waves.'
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
    $instance.Territory.Info = @()
}
$campaign | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $campaignPath -Encoding utf8

Write-Output "Installed separate campaign: $targetName"
Write-Output "Files copied: $($sourceFiles.Count)"
Write-Output "Campaign file: $campaignPath"