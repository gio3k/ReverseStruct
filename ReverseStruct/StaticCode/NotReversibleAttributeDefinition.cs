using System.Linq;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.StaticCode;

/// <summary>
/// Data used to generate "NotReversibleAttribute"
/// </summary>
public class NotReversibleAttributeDefinition
{
	public const string Name = "NotReversibleAttribute";
	public const string FullName = $"{LibraryConstants.GeneratedNamespace}.{Name}";

	public const string Source = $@"{FileCrumbs.Header}
using System;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
	/// <summary>
	/// This field will not be affected by endian reversal
	/// </summary>
    {LanguageCrumbs.GeneratedCodeAttribute}
    [System.AttributeUsage(AttributeTargets.Field)]
    public class {Name} : System.Attribute {{
	}}
}}";
}

public static class NotReversibleExtensions
{
	public static bool HasNotReversibleAttribute( this ISymbol fieldSymbol )
	{
		return fieldSymbol.GetAttributes()
			.Any( v => v.AttributeClass?.ToString() == NotReversibleAttributeDefinition.FullName );
	}
}
