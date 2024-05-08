using System.Collections.Generic;
using ReverseStruct.StaticCode;

namespace ReverseStruct.Target;

/// <summary>
/// Information about a struct with the [Reversible] attribute
/// <see cref="ReversibleAttributeDefinition"/>
/// </summary>
public record struct TargetStructInfo
{
	public string TargetFullName;
	public string TargetShortName;
	public List<(string name, ReversalMethod reversalMethod)> Fields;
}
