using Microsoft.CodeAnalysis;

namespace ReverseStruct.Target.TypeSupport;

/// <summary>
/// Info about an array field type
/// </summary>
/// <param name="symbol">Symbol linked to this field type</param>
/// <param name="elementType">Symbol linked to the array's element type</param>
public readonly struct ArrayFieldTypeInfo( ISymbol symbol, IFieldTypeInfo elementType ) : IFieldTypeInfo
{
	/// <summary>
	/// Arrays have element types
	/// </summary>
	public readonly IFieldTypeInfo ElementType = elementType;

	public string TypeName { get; } = symbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
}
