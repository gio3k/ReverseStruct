using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ReverseStruct.Diagnostics;

public static class LibraryDiagnosticDescriptors
{
	public static readonly DiagnosticDescriptor WarningUnsupportedField = new("RVST0010",
		"Irreversible field",
		"Type {0} isn't reversible and isn't marked with the [NotReversible] attribute. It will be ignored.",
		"ReverseStructFields",
		DiagnosticSeverity.Warning,
		true);

	public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =
		ImmutableArray.Create( WarningUnsupportedField );
}
