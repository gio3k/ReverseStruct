using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target.Extension;

public class TargetExtensionSyntaxProvider
{
	private static bool IsSyntaxTarget( SyntaxNode node, CancellationToken cancellationToken ) =>
		node is StructDeclarationSyntax or ClassDeclarationSyntax or RecordDeclarationSyntax;

	private static TargetInfo? GetSemanticTarget( GeneratorAttributeSyntaxContext ctx,
		CancellationToken cancellationToken )
	{
		if ( ctx.SemanticModel.GetDeclaredSymbol( ctx.TargetNode ) is not INamedTypeSymbol namedTypeSymbol )
			return null;
		
		return TargetInfo.Create( namedTypeSymbol, ctx.TargetNode );
	}

	public static IncrementalValuesProvider<TargetInfo?> Create(
		IncrementalGeneratorInitializationContext context )
	{
		return context.SyntaxProvider.ForAttributeWithMetadataName( ReversibleAttributeDefinition.FullName,
			predicate: IsSyntaxTarget,
			transform: GetSemanticTarget );
	}
}
