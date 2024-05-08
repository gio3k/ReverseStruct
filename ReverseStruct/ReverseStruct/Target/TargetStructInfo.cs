using System.Collections.Generic;

namespace ReverseStruct.Target;

public record struct TargetStructInfo
{
	public string TargetFullName;
	public string TargetShortName;
	public List<string> FieldNames;
}
