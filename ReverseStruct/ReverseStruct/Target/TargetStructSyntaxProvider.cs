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
			TargetFullName = namedTypeSymbol.ToString(), TargetShortName = namedTypeSymbol.Name, Fields = []
		};

		foreach ( var typeMember in namedTypeSymbol.GetMembers() )
		{
			if ( typeMember is not IFieldSymbol fieldSymbol )
				continue;

			// Make sure it's a value type
			if ( !fieldSymbol.Type.IsValueType )
				continue;

			// Make sure it doesn't have the "NotReversible" attribute
			if ( fieldSymbol.GetAttributes()
			    .Any( v => v.AttributeClass?.ToString() == NotReversibleAttributeDefinition.FullName ) )
				continue;

			// This field either needs to be supported by .ReverseEndianness or needs to be another reversible struct
			if ( BinaryPrimitivesUtil.ReverseEndiannessSupportedTypes.Contains( fieldSymbol.Type.MetadataName ) )
			{
				// Supported by ReverseEndianness
				result.Fields.Add( (fieldSymbol.Name, ReversalMethod.BinaryPrimitives) );
				continue;
			}

			if ( fieldSymbol.Type.GetAttributes()
			    .Any( v => v.AttributeClass?.ToString() == ReversibleAttributeDefinition.FullName ) )
			{
				// Another reversible struct
				result.Fields.Add( (fieldSymbol.Name, ReversalMethod.ExistingExtension) );
				continue;
			}
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
