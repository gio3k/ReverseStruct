using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReverseStruct.StaticCode;
using ReverseStruct.Target;

namespace ReverseStruct.Diagnostics;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public class LibraryUserCodeAnalyzer : DiagnosticAnalyzer
{
	public override void Initialize( AnalysisContext ctx )
	{
		ctx.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze );
		ctx.EnableConcurrentExecution();

		ctx.RegisterSymbolAction( HandleNamedTypeSymbolAction, SymbolKind.NamedType );
	}

	private static void HandleNamedTypeSymbolAction( SymbolAnalysisContext ctx )
	{
		var namedTypeSymbol = (INamedTypeSymbol)ctx.Symbol;
		if ( !namedTypeSymbol.HasReversibleInterface() && !namedTypeSymbol.HasReversibleAttribute() )
			return;

		var targetInfo = TargetInfo.Create( namedTypeSymbol, null );
		foreach ( var diagnostic in targetInfo.Diagnostics ) ctx.ReportDiagnostic( diagnostic );
	}

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		LibraryDiagnosticDescriptors.SupportedDiagnostics;
}
