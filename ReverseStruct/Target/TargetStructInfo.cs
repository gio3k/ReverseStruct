using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target;

/// <summary>
/// Information about a struct with the [Reversible] attribute
/// <see cref="ReversibleAttributeDefinition"/>
/// </summary>
public record struct TargetStructInfo
{
	public string FullName;
	public string ShortName;
	public string ContainingNamespace;
	public List<(string name, ReversalMethod reversalMethod)> Fields;

	public static TargetStructInfo CreateFromSymbol( INamedTypeSymbol namedTypeSymbol )
	{
		var result = new TargetStructInfo
		{
			FullName = namedTypeSymbol.ToString(),
			ShortName = namedTypeSymbol.Name,
			ContainingNamespace = namedTypeSymbol.ContainingNamespace.ToString(),
			Fields = []
		};

		foreach ( var typeMember in namedTypeSymbol.GetMembers() )
		{
			if ( typeMember is not IFieldSymbol fieldSymbol )
				continue;

			// Make sure it's a value type
			if ( !fieldSymbol.Type.IsValueType )
				continue;

			// Make sure it doesn't have the "NotReversible" attribute
			if ( fieldSymbol.HasNotReversibleAttribute() )
				continue;

			// This field either needs to be supported by .ReverseEndianness or needs to be another reversible struct
			// Is it supported by BinaryPrimitives.ReverseEndianness?
			if ( BinaryPrimitivesUtil.IsTypeSupportedByReverseEndianness( fieldSymbol.Type.MetadataName ) )
			{
				// Supported by ReverseEndianness
				result.Fields.Add( (fieldSymbol.Name, ReversalMethod.BinaryPrimitives) );
				continue;
			}

			// Is it supported by us? (ReversibleAttribute, extension method)
			if ( fieldSymbol.Type.GetAttributes()
			    .Any( v => v.AttributeClass?.ToString() == ReversibleAttributeDefinition.FullName ) )
			{
				result.Fields.Add( (fieldSymbol.Name, ReversalMethod.ExistingExtension) );
				continue;
			}

			// Is it supported by us? (IReversible, partial struct)
			if ( fieldSymbol.Type.HasReversibleInterface() )
			{
				result.Fields.Add( (fieldSymbol.Name, ReversalMethod.ExistingExtension) );
				continue;
			}
		}

		return result;
	}
}
