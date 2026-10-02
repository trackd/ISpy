#if NETSTANDARD2_0
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace ISpy.Utilities;

public sealed class ModuleAssemblyInitializer : IModuleAssemblyInitializer, IModuleAssemblyCleanup {
    private static readonly ResolveEventHandler s_resolveHandler = Resolve;

    public void OnImport() => AppDomain.CurrentDomain.AssemblyResolve += s_resolveHandler;

    public void OnRemove(PSModuleInfo psModuleInfo) => AppDomain.CurrentDomain.AssemblyResolve -= s_resolveHandler;

    private static readonly string s_moduleAssemblyDir =
        Path.GetDirectoryName(typeof(ModuleAssemblyInitializer).Assembly.Location) ?? string.Empty;

    private static Assembly? Resolve(object? sender, ResolveEventArgs args) {
        AssemblyName assemblyName = new(args.Name);
        if (assemblyName.Name is null) {
            return null;
        }

        // Only serve requests from our own assemblies so other modules keep their own binding.
        if (args.RequestingAssembly is { } requesting && !IsModuleAssembly(requesting)) {
            return null;
        }

        // Prefer bundled copies; older ones loaded by other modules cause type identity mismatches.
        string candidatePath = Path.Combine(s_moduleAssemblyDir, $"{assemblyName.Name}.dll");
        if (File.Exists(candidatePath)) {
            return Assembly.LoadFrom(candidatePath);
        }

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(existing => AssemblyName.ReferenceMatchesDefinition(assemblyName, existing.GetName()))
            .OrderByDescending(existing => existing.GetName().Version)
            .FirstOrDefault();
    }

    private static bool IsModuleAssembly(Assembly assembly) {
        if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location)) {
            return false;
        }

        return string.Equals(
            Path.GetDirectoryName(assembly.Location),
            s_moduleAssemblyDir,
            StringComparison.OrdinalIgnoreCase);
    }
}
#endif
