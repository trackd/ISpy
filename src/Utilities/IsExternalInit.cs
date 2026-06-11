#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides compiler support for init-only setters and records on frameworks
/// that do not define IsExternalInit.
/// </summary>
internal static class IsExternalInit;
#endif
