namespace ReverseStruct.Target;

public struct TargetFieldInfo( string fieldName, ReversalMethod reversalMethod )
{
	public readonly string FieldName = fieldName;
	public readonly ReversalMethod ReversalMethod = reversalMethod;
	public bool IsArrayType = false;
}
