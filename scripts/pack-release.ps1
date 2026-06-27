param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$PublishDir = Join-Path $ProjectRoot "publish"
$ReleaseDir = Join-Path $ProjectRoot "releases"

Push-Location $ProjectRoot
try {
    dotnet publish GestionCommerciale.csproj `
        -c Release `
        --self-contained `
        -r win-x64 `
        -o $PublishDir `
        /p:Version=$Version

    dnx vpk@1.2.0 pack `
        --packId Sonlighting.GestionCommerciale `
        --packTitle "Solighting" `
        --packVersion $Version `
        --packDir $PublishDir `
        --mainExe GestionCommerciale.exe `
        --outputDir $ReleaseDir
}
finally {
    Pop-Location
}

Write-Host "Release artifacts written to $ReleaseDir"
