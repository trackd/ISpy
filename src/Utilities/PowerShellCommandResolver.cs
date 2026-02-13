namespace ISpy.Utilities;

internal static class PowerShellCommandResolver {
    public static CommandInfo? GetCommand(PSCmdlet cmdlet, string commandName, CommandTypes commandTypes = CommandTypes.All)
        => string.IsNullOrWhiteSpace(commandName) ? null : cmdlet.InvokeCommand.GetCommand(commandName, commandTypes, null);

    // Resolve alias chains so callers see the actual command target.
    public static CommandInfo ResolveAlias(CommandInfo command) {
        while (command is AliasInfo alias && alias.ResolvedCommand is not null)
            command = alias.ResolvedCommand;

        return command;
    }

    // Try to extract CommandInfo from common pipeline input wrappers.
    public static bool TryGetCommandInfo(object? input, out CommandInfo? command) {
        if (input is null) {
            command = null;
            return false;
        }

        if (input is PSObject psObject && psObject.BaseObject is not null)
            input = psObject.BaseObject;

        command = input as CommandInfo;
        return command is not null;
    }

    public static bool TryGetCommandInfo(PSCmdlet cmdlet, object? input, out CommandInfo? command) {
        if (input is null) {
            command = null;
            return false;
        }

        if (input is PSObject psObject && psObject.BaseObject is not null)
            input = psObject.BaseObject;

        if (input is string commandName) {
            command = GetCommand(cmdlet, commandName);
            return command is not null;
        }

        command = input as CommandInfo;
        return command is not null;
    }

    // Resolve PowerShell-defined commands to script text for direct output.
    public static bool TryGetScriptText(object? input, out string? scriptText, out string? commandName) {
        scriptText = null;
        commandName = null;

        if (!TryGetCommandInfo(input, out CommandInfo? command) || command is null)
            return false;

        command = ResolveAlias(command);

        if (command is FunctionInfo functionInfo)
            scriptText = functionInfo.ScriptBlock.Ast.Extent.Text;
        else if (command is FilterInfo filterInfo)
            scriptText = filterInfo.ScriptBlock.Ast.Extent.Text;
        else if (command is ScriptInfo scriptInfo)
            scriptText = scriptInfo.ScriptBlock.Ast.Extent.Text;
        else
            return false;

        commandName = command.Name;
        return !string.IsNullOrEmpty(scriptText);
    }

    public static bool TryGetScriptText(PSCmdlet cmdlet, object? input, out string? scriptText, out string? commandName) {
        scriptText = null;
        commandName = null;

        if (!TryGetCommandInfo(cmdlet, input, out CommandInfo? command) || command is null)
            return false;

        command = ResolveAlias(command);

        if (command is FunctionInfo functionInfo)
            scriptText = functionInfo.ScriptBlock.Ast.Extent.Text;
        else if (command is FilterInfo filterInfo)
            scriptText = filterInfo.ScriptBlock.Ast.Extent.Text;
        else if (command is ScriptInfo scriptInfo)
            scriptText = scriptInfo.ScriptBlock.Ast.Extent.Text;
        else
            return false;

        commandName = command.Name;
        return !string.IsNullOrEmpty(scriptText);
    }
}
