using ReverseStruct.Target.TypeSupport;

namespace ReverseStruct.Target;

public struct TargetFieldInfo( string fieldName, IFieldTypeInfo fieldType )
{
	public readonly string FieldName = fieldName;
	public readonly IFieldTypeInfo FieldType = fieldType;
}
