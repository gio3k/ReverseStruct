using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target.Partial;

public class TargetStructPartialSyntaxProvider
{
	private static bool IsSyntaxTarget( SyntaxNode node, CancellationToken cancellationToken ) =>
		node is StructDeclarationSyntax;

	private static TargetStructInfo? GetSemanticTarget( GeneratorSyntaxContext ctx,
		CancellationToken cancellationToken )
	{
		if ( ctx.SemanticModel.GetDeclaredSymbol( ctx.Node ) is not INamedTypeSymbol namedTypeSymbol )
			return null;

		if ( !namedTypeSymbol.HasReversibleInterface() )
			return null;

		return TargetStructInfo.CreateFromSymbol( namedTypeSymbol );
	}

	public static IncrementalValuesProvider<TargetStructInfo?> Create(
		IncrementalGeneratorInitializationContext context )
	{
		return context.SyntaxProvider.CreateSyntaxProvider(
			predicate: IsSyntaxTarget,
			transform: GetSemanticTarget );
	}
}
