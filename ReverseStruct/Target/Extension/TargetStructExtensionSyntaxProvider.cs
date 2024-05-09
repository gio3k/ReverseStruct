using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target.Extension;

public class TargetStructExtensionSyntaxProvider
{
	private static bool IsSyntaxTarget( SyntaxNode node, CancellationToken cancellationToken ) =>
		node is StructDeclarationSyntax;

	private static TargetStructInfo? GetSemanticTarget( GeneratorAttributeSyntaxContext ctx,
		CancellationToken cancellationToken )
	{
		if ( ctx.SemanticModel.GetDeclaredSymbol( ctx.TargetNode ) is not INamedTypeSymbol namedTypeSymbol )
			return null;

		return TargetStructInfo.CreateFromSymbol( namedTypeSymbol );
	}

	public static IncrementalValuesProvider<TargetStructInfo?> Create(
		IncrementalGeneratorInitializationContext context )
	{
		return context.SyntaxProvider.ForAttributeWithMetadataName( ReversibleAttributeDefinition.FullName,
			predicate: IsSyntaxTarget,
			transform: GetSemanticTarget );
	}
}
