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
        $_.Name -notmatch '_backup$'
    }

if (-not $sourceFiles) {
    throw "No Ashes of the Empire world files found in $worlds"
}

Get-ChildItem -LiteralPath $worlds -File |
    Where-Object { $_.Name.StartsWith($targetName + '..', [StringComparison]::OrdinalIgnoreCase) } |
    Remove-Item -Force

foreach ($sourceFile in $sourceFiles) {
    $extension = $sourceFile.Name.Substring($sourceName.Length)
    Copy-Item -LiteralPath $sourceFile.FullName -Destination ($targetPrefix + $extension) -Force
}

$planetPath = $targetPrefix + '.planet'
$planet = Get-Content -LiteralPath $planetPath -Raw | ConvertFrom-Json
$planet.Name = 'Hold Your Ground'
$planet.Summary = 'A fixed-position tower defense campaign based on the Ashes of the Empire world.'
$planet.CampaignMainMenuPanelText = 'Defend the foothold. Survive escalating enemy waves.'
$planet.AllowAdventures = $false
$planet.AllowStories = $false
$planet | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $planetPath -Encoding utf8

$campaignPath = $targetPrefix + '.campaign'
$campaign = Get-Content -LiteralPath $campaignPath -Raw | ConvertFrom-Json
$campaign.Instances[0].Header.Name = 'Hold Your Ground'
$campaign.Instances[0].Header.Summary = 'Defend a fixed Ashes foothold against escalating waves.'
$campaign.Instances[0].Header.CampaignHeader.Hash = ''
$campaign.Instances[0].Header.CommonSettings.AvatarAutoSpawn = 1
$campaign.Instances[0].Header.CommonSettings.BlueprintSpawningOptions = 3
$campaign.Instances[0].Header.CommonSettings.DiplomacyMode = 0
$campaign.Instances[0].Header.CommonSettings.DiplomacyMeetingTime = 0
$campaign.Instances[0].Header.CommonSettings.DisplayRelationshipMatrix = $false
$campaign.Instances[0].Header.CommonSettings.DisplayReinforcementsInMenu = $false
$campaign.Instances[0].Header.CommonSettings.DisplayEnemiesInMenu = $false
$campaign | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $campaignPath -Encoding utf8

Write-Output "Installed separate campaign: $targetName"
Write-Output "Files copied: $($sourceFiles.Count)"
Write-Output "Campaign file: $campaignPath"