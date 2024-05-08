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
