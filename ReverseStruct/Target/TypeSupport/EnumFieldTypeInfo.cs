using Microsoft.CodeAnalysis;

namespace ReverseStruct.Target.TypeSupport;

/// <summary>
/// Info about an enum field type
/// </summary>
/// <param name="symbol">Symbol linked to this field type</param>
/// <param name="underlyingType">Symbol linked to the enum's underlying type</param>
public readonly struct EnumFieldTypeInfo( ISymbol symbol, ISymbol underlyingType ) : IFieldTypeInfo
{
	public string TypeName { get; } = symbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );

	public readonly string UnderlyingTypeName =
		underlyingType.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
}
