using Microsoft.CodeAnalysis;

namespace ReverseStruct.Target.TypeSupport;

/// <summary>
/// Field type referring to one we've already generated (or will generate)
/// </summary>
/// <param name="symbol">Symbol linked to this field type</param>
public readonly struct GeneratedFriendFieldTypeInfo( ISymbol symbol ) : IFieldTypeInfo
{
	public string TypeName { get; } = symbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );
}
