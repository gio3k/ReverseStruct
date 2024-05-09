using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target;

/// <summary>
/// Information about a struct with the [Reversible] attribute
/// <see cref="ReversibleAttributeDefinition"/>
/// </summary>
public record struct TargetInfo
{
	public string FullName;
	public string ShortName;
	public string ContainingNamespace;
	public TargetDeclarationType DeclarationType;
	public List<TargetFieldInfo> Fields;

	private static ReversalMethod GetReversalMethodForType( ITypeSymbol typeSymbol, bool checkBinaryPrimitives )
	{
		if ( checkBinaryPrimitives && typeSymbol.IsValueType )
		{
			// Is it supported by BinaryPrimitives.ReverseEndianness?
			if ( BinaryPrimitivesUtil.IsTypeSupportedByReverseEndianness( typeSymbol.MetadataName ) )
				// Supported by ReverseEndianness
				return ReversalMethod.BinaryPrimitives;
		}

		// Is it supported by us? (ReversibleAttribute, extension method)
		if ( typeSymbol.GetAttributes()
		    .Any( v => v.AttributeClass?.ToString() == ReversibleAttributeDefinition.FullName ) )
			return ReversalMethod.ExistingExtension;

		// Is it supported by us? (IReversible, partial target)
		if ( typeSymbol.HasReversibleInterface() )
			return ReversalMethod.ExistingExtension;

		return ReversalMethod.Invalid;
	}

	private static TargetFieldInfo? TryCreateFieldInfoForArray( TargetInfo targetInfo, IFieldSymbol fieldSymbol )
	{
		if ( fieldSymbol.Type is not IArrayTypeSymbol arrayTypeSymbol )
			return null;

		var reversalMethod = GetReversalMethodForType( arrayTypeSymbol.ElementType, checkBinaryPrimitives: true );
		if ( reversalMethod == ReversalMethod.Invalid )
			return null;

		return new TargetFieldInfo( fieldSymbol.Name, reversalMethod )
		{
			IsArrayType = true
		};
	}

	private static TargetFieldInfo? TryCreateFieldInfo( TargetInfo targetInfo, IFieldSymbol fieldSymbol )
	{
		// Check for NotReversible attribute
		if ( fieldSymbol.HasNotReversibleAttribute() )
			return null;
		
		ReversalMethod reversalMethod;
		if ( fieldSymbol.Type.IsReferenceType )
		{
			// Reference type - maybe it's an array?
			if ( TryCreateFieldInfoForArray( targetInfo, fieldSymbol ) is { } arrayTargetFieldInfo )
				return arrayTargetFieldInfo;

			// Reference type - maybe it's a class?
			reversalMethod = GetReversalMethodForType( fieldSymbol.Type, checkBinaryPrimitives: false );
			if ( reversalMethod != ReversalMethod.Invalid )
				return new TargetFieldInfo( fieldSymbol.Name, reversalMethod );
		}

		reversalMethod = GetReversalMethodForType( fieldSymbol.Type, checkBinaryPrimitives: true );
		if ( reversalMethod != ReversalMethod.Invalid )
			return new TargetFieldInfo( fieldSymbol.Name, reversalMethod );

		return null;
	}

	public static TargetInfo Create( INamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxNode )
	{
		var targetInfo = new TargetInfo
		{
			FullName = namedTypeSymbol.ToString(),
			ShortName = namedTypeSymbol.Name,
			ContainingNamespace = namedTypeSymbol.ContainingNamespace.ToString(),
			Fields = [],
			DeclarationType = syntaxNode switch
			{
				ClassDeclarationSyntax => TargetDeclarationType.Class,
				StructDeclarationSyntax => TargetDeclarationType.Struct,
				RecordDeclarationSyntax => TargetDeclarationType.Record,
				_ => TargetDeclarationType.Unknown
			}
		};

		foreach ( var typeMemberSymbol in namedTypeSymbol.GetMembers() )
		{
			if ( typeMemberSymbol is not IFieldSymbol fieldSymbol )
				continue;

			if ( TryCreateFieldInfo( targetInfo, fieldSymbol ) is { } fieldInfo )
				targetInfo.Fields.Add( fieldInfo );
		}

		return targetInfo;
	}
}
