using System.Linq;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.StaticCode;

/// <summary>
/// Data used to generate "ReversibleAttribute"
/// </summary>
public class ReversibleAttributeDefinition
{
	public const string Name = "ReversibleAttribute";
	public const string FullName = $"{LibraryConstants.GeneratedNamespace}.{Name}";

	public const string Source = $@"{FileCrumbs.Header}
using System;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
	/// <summary>
	/// Enables endian reversal with .ReverseEndianness()
	/// </summary>
    {LanguageCrumbs.GeneratedCodeAttribute}
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class {Name} : System.Attribute {{
	}}
}}";
}

public static class ReversibleExtensions
{
	public static bool HasReversibleAttribute( this ISymbol fieldSymbol )
	{
		return fieldSymbol.GetAttributes()
			.Any( v => v.AttributeClass?.ToString() == ReversibleAttributeDefinition.FullName );
	}
}
