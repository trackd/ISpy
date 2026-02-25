#! /usr/bin/pwsh
#Requires -Version 7.4 -Module InvokeBuild
param(
    [string]$Configuration = 'Release',
    [switch]$SkipHelp,
    [switch]$SkipTests
)
Write-Host "$($PSBoundParameters.GetEnumerator())" -ForegroundColor Cyan

$modulename = [System.IO.Path]::GetFileName($PSCommandPath) -replace '\.build\.ps1$'

$script:folders = @{
    ModuleName       = $modulename
    ProjectRoot      = $PSScriptRoot
    SourcePath       = Join-Path $PSScriptRoot 'src'
    OutputPath       = Join-Path $PSScriptRoot 'output'
    DestinationPath  = Join-Path $PSScriptRoot 'output' 'lib'
    ModuleSourcePath = Join-Path $PSScriptRoot 'module'
    DocsPath         = Join-Path $PSScriptRoot 'docs' 'en-US'
    TestPath         = Join-Path $PSScriptRoot 'tests'
    CsprojPath       = Join-Path $PSScriptRoot 'src' "$modulename.csproj"
}

task Clean {
    if (Test-Path $folders.OutputPath) {
        Remove-Item -Path $folders.OutputPath -Recurse -Force -ErrorAction 'Ignore'
    }
    New-Item -Path $folders.OutputPath -ItemType Directory -Force | Out-Null
}

task Build {
    if (-not (Test-Path $folders.CsprojPath)) {
        Write-Warning 'C# project not found, skipping Build'
        return
    }
    try {
        Push-Location $folders.SourcePath
        $buildOutput = dotnet publish $folders.CsprojPath --configuration $Configuration --nologo --verbosity minimal --output $folders.DestinationPath 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed:`n$buildOutput"
        }
    }
    finally {
        Pop-Location
    }
}

task ModuleFiles {
    if (Test-Path $folders.ModuleSourcePath) {
        Get-ChildItem -Path $folders.ModuleSourcePath -File | Copy-Item -Destination $folders.OutputPath -Force
    }
    else {
        Write-Warning "Module directory not found at: $($folders.ModuleSourcePath)"
    }
}

task GenerateHelp -if (-not $SkipHelp) {
    if (-not (Test-Path $folders.DocsPath)) {
        Write-Warning "Documentation path not found at: $($folders.DocsPath)"
        return
    }
    if (-not (Get-Module -ListAvailable -Name Microsoft.PowerShell.PlatyPS)) {
        Write-Host '    Installing Microsoft.PowerShell.PlatyPS...' -ForegroundColor Yellow
        Install-Module -Name Microsoft.PowerShell.PlatyPS -Scope CurrentUser -Force -AllowClobber
    }

    Import-Module Microsoft.PowerShell.PlatyPS -ErrorAction Stop

    $modulePath = Join-Path $folders.OutputPath ($folders.ModuleName + '.psd1')
    if (-not (Test-Path $modulePath)) {
        Write-Warning "Module manifest not found at: $modulePath. Skipping help generation."
        return
    }

    Import-Module $modulePath -Force

    function Get-PipelineInputLabel {
        param(
            [Parameter(Mandatory)]
            [System.Management.Automation.ParameterMetadata]$ParameterMetadata
        )

        $byValue = $ParameterMetadata.ParameterSets.Values | Where-Object { $_.ValueFromPipeline }
        $byProperty = $ParameterMetadata.ParameterSets.Values | Where-Object { $_.ValueFromPipelineByPropertyName }

        if (-not $byValue -and -not $byProperty) {
            return 'False'
        }

        if ($byValue -and $byProperty) {
            return 'True (ByValue, ByPropertyName)'
        }

        if ($byValue) {
            return 'True (ByValue)'
        }

        return 'True (ByPropertyName)'
    }

    function Get-PositionLabel {
        param(
            [Parameter(Mandatory)]
            [System.Management.Automation.ParameterMetadata]$ParameterMetadata
        )

        $positions = $ParameterMetadata.ParameterSets.Values |
            ForEach-Object { $_.Position } |
            Where-Object { $_ -ge 0 } |
            Select-Object -Unique

        if ($positions.Count -eq 1) {
            return [string]$positions[0]
        }

        return 'Named'
    }

    function Get-ParameterHelpMessage {
        param(
            [Parameter(Mandatory)]
            [System.Management.Automation.ParameterMetadata]$ParameterMetadata,
            [Parameter(Mandatory)]
            [string]$CommandName
        )

        $helpMessage = $ParameterMetadata.Attributes |
            Where-Object { $_ -is [System.Management.Automation.ParameterAttribute] -and -not [string]::IsNullOrWhiteSpace($_.HelpMessage) } |
            Select-Object -First 1 -ExpandProperty HelpMessage

        $description = if (-not [string]::IsNullOrWhiteSpace($helpMessage)) {
            $helpMessage
        }
        elseif ($ParameterMetadata.IsDynamic) {
            "Dynamic parameter for $CommandName."
        }
        else {
            "$($ParameterMetadata.Name) parameter."
        }

        $parameterType = $ParameterMetadata.ParameterType

        if ($parameterType -eq [System.Management.Automation.SwitchParameter]) {
            return $description
        }

        if ($parameterType.IsEnum) {
            $validateSet = $ParameterMetadata.Attributes |
                Where-Object { $_ -is [System.Management.Automation.ValidateSetAttribute] } |
                Select-Object -First 1

            $values = if ($null -ne $validateSet) {
                $validateSet.ValidValues
            }
            else {
                [System.Enum]::GetNames($parameterType)
            }

            $displayValues = $values
            # if ($values.Count -gt 12) {
            #     $displayValues = $values[0..11]
            #     return "$description `nAccepted values include: $([string]::Join(', ', $displayValues))..."
            # }

            return "$description  `nAccepted values: $([string]::Join(', ', $displayValues))."
        }

        if ($parameterType -eq [int] -or $parameterType -eq [long] -or $parameterType -eq [double] -or $parameterType -eq [decimal]) {
            return "$description  `nProvide a numeric value."
        }

        if ($parameterType -eq [string]) {
            return "$description  `nProvide a string value."
        }

        return $description
    }

    function Get-ParametersMarkdown {
        param(
            [Parameter(Mandatory)]
            [string]$CommandName
        )

        $commonParameters = @(
            'Verbose',
            'Debug',
            'ErrorAction',
            'WarningAction',
            'InformationAction',
            'ProgressAction',
            'ErrorVariable',
            'WarningVariable',
            'InformationVariable',
            'OutVariable',
            'OutBuffer',
            'PipelineVariable'
        )

        $parameters = (Get-Command $CommandName).Parameters.Values |
            Where-Object { $_.Name -notin $commonParameters } |
            Sort-Object Name

        $lines = [System.Collections.Generic.List[string]]::new()

        foreach ($parameter in $parameters) {
            $required = ($parameter.ParameterSets.Values | Where-Object { $_.IsMandatory }).Count -gt 0
            $position = Get-PositionLabel -ParameterMetadata $parameter
            $pipelineInput = Get-PipelineInputLabel -ParameterMetadata $parameter
            $description = Get-ParameterHelpMessage -ParameterMetadata $parameter -CommandName $CommandName
            $lines.Add("### -$($parameter.Name)")
            $lines.Add('')
            $lines.Add($description)
            $lines.Add('')
            $lines.Add('```yaml')
            $lines.Add("Type: $($parameter.ParameterType.Name)")

            if ($parameter.Aliases.Count -gt 0) {
                $lines.Add("Aliases: $([string]::Join(', ', ($parameter.Aliases | Sort-Object)))")
            }

            $lines.Add("Required: $required")
            $lines.Add("Position: $position")
            $lines.Add("Accept pipeline input: $pipelineInput")
            $lines.Add('```')
            $lines.Add('')
        }

        return [string]::Join([Environment]::NewLine, $lines)
    }

    function Sync-CommandParameterDocs {
        param(
            [Parameter(Mandatory)]
            [string]$CommandName,
            [Parameter(Mandatory)]
            [string]$DocPath
        )

        if (-not (Test-Path $DocPath)) {
            Write-Warning "Doc file not found for $CommandName at: $DocPath"
            return
        }

        $content = Get-Content -Path $DocPath -Raw
        $parametersMarkdown = Get-ParametersMarkdown -CommandName $CommandName
        $replacement = "## PARAMETERS$([Environment]::NewLine)$([Environment]::NewLine)$parametersMarkdown$([Environment]::NewLine)## INPUTS"

        $updated = [regex]::Replace(
            $content,
            '## PARAMETERS\r?\n[\s\S]*?\r?\n## INPUTS',
            $replacement,
            [System.Text.RegularExpressions.RegexOptions]::Singleline)

        Set-Content -Path $DocPath -Value $updated -NoNewline
    }

    Sync-CommandParameterDocs -CommandName 'New-DecompilerSetting' -DocPath (Join-Path $folders.DocsPath 'New-DecompilerSetting.md')
    Sync-CommandParameterDocs -CommandName 'New-DecompilerFormattingOption' -DocPath (Join-Path $folders.DocsPath 'New-DecompilerFormattingOption.md')

    $helpOutputPath = Join-Path $folders.OutputPath 'en-US'
    New-Item -Path $helpOutputPath -ItemType Directory -Force | Out-Null

    $allCommandHelp = Get-ChildItem -Path $folders.DocsPath -Filter '*.md' -Recurse -File |
        Where-Object { $_.Name -ne "$($folders.ModuleName).md" } |
        Import-MarkdownCommandHelp

    if ($allCommandHelp.Count -gt 0) {
        $tempOutputPath = Join-Path $helpOutputPath 'temp'
        Export-MamlCommandHelp -CommandHelp $allCommandHelp -OutputFolder $tempOutputPath -Force | Out-Null

        $generatedFile = Get-ChildItem -Path $tempOutputPath -Filter '*.xml' -Recurse -File | Select-Object -First 1
        if ($generatedFile) {
            Move-Item -Path $generatedFile.FullName -Destination $helpOutputPath -Force
        }
        Remove-Item -Path $tempOutputPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

task Test -if (-not $SkipTests) {
    if (-not (Test-Path $folders.TestPath)) {
        Write-Warning "Test directory not found at: $($folders.TestPath)"
        return
    }
    $pesterConfig = New-PesterConfiguration
    # $pesterConfig.Output.Verbosity = 'Detailed'
    $pesterConfig.Run.Path = $folders.TestPath
    $pesterConfig.Run.Throw = $true
    $pesterConfig.Debug.WriteDebugMessages = $false
    Invoke-Pester -Configuration $pesterConfig
}

task CleanAfter {
    if ($script:folders.DestinationPath -and (Test-Path $script:folders.DestinationPath)) {
        Get-ChildItem -Path $script:folders.DestinationPath -File |
            Where-Object { $_.Extension -in @('.pdb', '.json') } |
            Remove-Item -Force -ErrorAction Ignore
    }
}


task All -Jobs Clean, Build, ModuleFiles, GenerateHelp, CleanAfter, Test
task BuildAndTest -Jobs Clean, Build, ModuleFiles, CleanAfter #, Test
