User request details:

- Fix `Show-Type` pipeline behavior: `Get-Command Get-Uptime | Show-SpyType -Verbose` should resolve the command/type before decompiling.
- Re-order logic so command/type resolution happens earlier when pipeline input is `CommandInfo` or `Type`.
- Accept pipeline input from `Type` and `CommandInfo` objects.
- When non-pipeline input is a string, resolve it as a command/alias (e.g., `Show-Type ls` -> `Get-ChildItem` cmdlet) and decompile the cmdlet.

Action plan:

1. Inspect current command/type resolution flow.

- Task: Review ShowTypeCmdlet input parameter handling for pipeline vs non-pipeline usage.
- Task: Review resolution helpers (e.g., PowerShellCommandResolver) to see available APIs.

1. Adjust resolution ordering and input handling.

- Task: Update ShowTypeCmdlet to resolve `CommandInfo` and `Type` inputs before decompilation.
- Task: Ensure string input resolves command/alias to `CommandInfo` before trying type lookup.

1. Validate behavior with expected scenarios.

- Task: Check logic paths for `Get-Command ... | Show-SpyType` and `Show-Type ls`.
- Task: Run targeted tests or build if applicable.

Status:

- Completed: Reviewed ShowTypeCmdlet input handling and resolution helpers.
- Completed: Reordered Show-Type resolution to resolve command/type before script and method handling.
- Completed: Build succeeded to validate changes.

Summary:

- Reordered Show-Type resolution to prioritize command/type inputs before script and method fallback.
- Updated handling to resolve command/alias inputs early for pipeline and string scenarios.
- Build completed successfully.
