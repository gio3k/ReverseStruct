using Microsoft.CodeAnalysis;

namespace ReverseStruct.Target.TypeSupport;

/// <summary>
/// Info about a primitive field type
/// </summary>
/// <param name="symbol">Symbol linked to this field type</param>
public readonly struct PrimitiveFieldTypeInfo( ISymbol symbol ) : IFieldTypeInfo
{
	public string TypeName { get; } = symbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
}
