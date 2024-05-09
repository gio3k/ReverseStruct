using System.Linq;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

// ReSharper disable InconsistentNaming

namespace ReverseStruct.StaticCode;

/// <summary>
/// Data used to generate "IReversibleAttribute"
/// </summary>
public class IReversibleDefinition
{
	public const string Name = "IReversible";
	public const string FullName = $"{LibraryConstants.GeneratedNamespace}.{Name}";

	public const string Source = $@"{FileCrumbs.Header}
using System;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
	/// <summary>
	/// This field will not be affected by endian reversal
	/// </summary>
    {LanguageCrumbs.GeneratedCodeAttribute}
    public interface {Name} {{
		public void ReverseEndianness();
	}}
}}";
}

public static class IReversibleInterfaceExtensions
{
	public static bool HasReversibleInterface( this ITypeSymbol typeSymbol )
	{
		return Enumerable.Any( typeSymbol.AllInterfaces,
			interfaceNamedTypeSymbol => interfaceNamedTypeSymbol.ToString() == IReversibleDefinition.FullName );
	}
}
