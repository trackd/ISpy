$sb = {
    param($s)
    Push-Location $s
    # & .\build.ps1 -Task 'BuildAndTest'
    $RepoParent = Split-Path $s -Parent
    Import-Module (Join-Path $RepoParent 'PwshSpectreConsole' 'output' 'PwshSpectreConsole.psd1')
    Import-Module (Join-Path $RepoParent 'PSTextMate' 'output' 'PSTextMate.psd1')
    Import-Module ./output/ISpy.psd1
    $up = Get-Command Get-SpyFramework | Show-SpyType -Verbose
    "------ Get-SpyFramework ($($up.PSChildName)) ------"
    $up | Show-TextMate -Verbose
    $m = [math]::Truncate | Show-SpyType -Verbose
    "------ Math.Truncate ($($m.PSChildName)) ------"
    $m | Show-TextMate -Verbose
    # $math -split '\r?\n' | Select-Object -Skip 10 -first 15
    function helloworld {
        <#
        .DESCRIPTION
        A simple function that returns "Hello, World!" and a question.
        #>
        param($noop)
        "Hello, World!"
        'does this work?'
    }
    $s = Show-SpyType 'scb' -
    "------ scb (alias $($s.PSChildName)) ------"
    $s
    # $s | Show-TextMate -Verbose #-Language 'csharp'
    $f = Show-SpyType 'helloworld' -Verbose
    "------ helloworld (locally defined function $($f.PSChildName)) ------"
    $f | Show-TextMate -Verbose
    # Get-Command -Module ISpy
}
pwsh -NoProfile -Command $sb -Args $PSScriptRoot
