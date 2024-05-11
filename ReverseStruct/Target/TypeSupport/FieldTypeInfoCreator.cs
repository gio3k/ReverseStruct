using System.Linq;
using Microsoft.CodeAnalysis;
using ReverseStruct.Diagnostics;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target.TypeSupport;

public static class FieldTypeInfoCreator
{
	private static IFieldTypeInfo? TryCreateFieldTypeInfoForArray( TargetInfo targetInfo, ITypeSymbol typeSymbol,
		IFieldSymbol fieldSymbol )
	{
		if ( typeSymbol is not IArrayTypeSymbol arrayTypeSymbol )
			return null;

		// Try to create a TargetFieldTypeInfo from the element type of the array
		if ( TryCreateFieldTypeInfo( targetInfo, arrayTypeSymbol.ElementType, fieldSymbol ) is not { } elementTypeInfo )
			return null;

		return new ArrayFieldTypeInfo( typeSymbol, elementTypeInfo );
	}

	/// <summary>
	/// Attempt to create an IFieldTypeInfo from a type symbol
	/// </summary>
	/// <param name="targetInfo">Target this field / field type belongs to</param>
	/// <param name="typeSymbol">Type symbol</param>
	/// <param name="fieldSymbol">Field symbol for diagnostic output</param>
	/// <returns>IFieldTypeInfo?</returns>
	public static IFieldTypeInfo? TryCreateFieldTypeInfo( TargetInfo targetInfo, ITypeSymbol typeSymbol,
		IFieldSymbol fieldSymbol )
	{
		// Reference types
		if ( typeSymbol.IsReferenceType )
		{
			// Reference type - array maybe?
			if ( TryCreateFieldTypeInfoForArray( targetInfo, typeSymbol, fieldSymbol ) is { } arrayFieldTypeInfo )
				return arrayFieldTypeInfo;

			// Reference type - maybe a class?
			if ( typeSymbol.HasReversibleAttribute() || typeSymbol.HasReversibleInterface() )
				return new GeneratedFriendFieldTypeInfo( typeSymbol );

			targetInfo.ReportDiagnostic( Diagnostic.Create( LibraryDiagnosticDescriptors.WarningUnsupportedField,
				fieldSymbol.Locations.First(), fieldSymbol.Name ) );
			return null;
		}

		// Value types
		// Value type - enum maybe?
		if ( typeSymbol is INamedTypeSymbol { EnumUnderlyingType: { } enumUnderlyingType } )
		{
			if ( BinaryPrimitivesUtil.IsTypeSupportedByReverseEndianness( enumUnderlyingType ) )
				return new EnumFieldTypeInfo( typeSymbol, enumUnderlyingType );
		}

		// Value type - anything else
		if ( BinaryPrimitivesUtil.IsTypeSupportedByReverseEndianness( typeSymbol ) )
			return new PrimitiveFieldTypeInfo( typeSymbol );

		if ( typeSymbol.HasReversibleAttribute() || typeSymbol.HasReversibleInterface() )
			return new GeneratedFriendFieldTypeInfo( typeSymbol );

		targetInfo.ReportDiagnostic( Diagnostic.Create( LibraryDiagnosticDescriptors.WarningUnsupportedField,
			fieldSymbol.Locations.First(), fieldSymbol.Name ) );
		return null;
	}
}
