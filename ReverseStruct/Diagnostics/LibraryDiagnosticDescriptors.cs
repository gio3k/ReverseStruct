using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ReverseStruct.Diagnostics;

public static class LibraryDiagnosticDescriptors
{
	public static readonly DiagnosticDescriptor WarningUnsupportedField = new(
		"RVST0010",
		"Irreversible field",
		"Irreversible field '{0}'",
		"ReverseStructFields",
		DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description:
		"Field type is unsupported and isn't marked with the [NotReversible] attribute. It will be ignored.");

	public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =
		ImmutableArray.Create( WarningUnsupportedField );
}
