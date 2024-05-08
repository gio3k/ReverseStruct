using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target;

public class TargetStructSyntaxProvider
{
	private static bool IsSyntaxTarget( SyntaxNode node, CancellationToken cancellationToken ) =>
		node is StructDeclarationSyntax;

	private static TargetStructInfo? GetSemanticTarget( GeneratorAttributeSyntaxContext ctx,
		CancellationToken cancellationToken )
	{
		if ( ctx.SemanticModel.GetDeclaredSymbol( ctx.TargetNode ) is not INamedTypeSymbol namedTypeSymbol )
			return null;

		var result = new TargetStructInfo
		{
			TargetFullName = namedTypeSymbol.ToString(), TargetShortName = namedTypeSymbol.Name, FieldNames = []
		};

		foreach ( var typeMember in namedTypeSymbol.GetMembers() )
		{
			if ( typeMember is not IFieldSymbol fieldSymbol )
				continue;

			// Make sure it's a value type
			if ( !fieldSymbol.Type.IsValueType )
				continue;

			// Make sure .ReverseEndianness supports this type
			if ( !BinaryPrimitivesUtil.ReverseEndiannessSupportedTypes.Contains( fieldSymbol.Type.MetadataName ) )
				continue;

			// Make sure it doesn't have the "NotReversible" attribute
			if ( fieldSymbol.GetAttributes()
			    .Any( v => v.AttributeClass?.ToString() == NotReversibleAttributeDefinition.FullName ) )
				continue;

			result.FieldNames.Add( fieldSymbol.Name );
		}

		return result;
	}

	public static IncrementalValuesProvider<TargetStructInfo?> Create(
		IncrementalGeneratorInitializationContext context )
	{
		return context.SyntaxProvider.ForAttributeWithMetadataName( ReversibleAttributeDefinition.FullName,
			predicate: IsSyntaxTarget,
			transform: GetSemanticTarget );
	}
}
