$sb = {
    param($s)
    Push-Location $s
    & .\build.ps1 -Task 'BuildAndTest'
    Import-Module ./output/ISpy.psd1
    Get-Command Get-Uptime | Show-SpyType -Verbose
    '------ break ------'
    [Math]::Round | Show-SpyType -Verbose
    # $math -split '\r?\n' | Select-Object -Skip 10 -first 15
    Show-SpyType 'scb' -Verbose
    Get-Command -Module ISpy
}
pwsh -NoProfile -Command $sb -Args $PSScriptRoot
