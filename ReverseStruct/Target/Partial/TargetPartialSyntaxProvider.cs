using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target.Partial;

public class TargetPartialSyntaxProvider
{
	private static bool IsSyntaxTarget( SyntaxNode node, CancellationToken cancellationToken ) =>
		node is StructDeclarationSyntax or ClassDeclarationSyntax or RecordDeclarationSyntax;

	private static TargetInfo? GetSemanticTarget( GeneratorSyntaxContext ctx,
		CancellationToken cancellationToken )
	{
		if ( ctx.SemanticModel.GetDeclaredSymbol( ctx.Node ) is not INamedTypeSymbol namedTypeSymbol )
			return null;

		if ( !namedTypeSymbol.HasReversibleInterface() )
			return null;

		return TargetInfo.Create( namedTypeSymbol, ctx.Node );
	}

	public static IncrementalValuesProvider<TargetInfo?> Create(
		IncrementalGeneratorInitializationContext context )
	{
		return context.SyntaxProvider.CreateSyntaxProvider(
			predicate: IsSyntaxTarget,
			transform: GetSemanticTarget );
	}
}
