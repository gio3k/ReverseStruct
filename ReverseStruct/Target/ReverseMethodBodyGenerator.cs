using System;
using System.Runtime.CompilerServices;
using ReverseStruct.Target.TypeSupport;

namespace ReverseStruct.Target;

public static class ReverseMethodBodyGenerator
{
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string MakeTab( int depth ) => new('\t', depth);

	private static void GenerateArrayFieldReversalCode( string fieldName, string fieldIdentifier,
		ref ArrayFieldTypeInfo arrayFieldTypeInfo, ref string output, int depth )
	{
		// depth (0)
		var t0 = MakeTab( depth );

		// new identifier: reference to the array field
		var idArr = $"{fieldName}_{depth}";
		var idArrRef = $"ref_{idArr}";

		// gen: Name helper comment
		output += $"\n{t0}// Array [name = {idArr}, identifier = {fieldIdentifier}, depth = {depth}]";

		// gen: Create ID variable (id0)
		output += $"\n{t0}var {idArrRef} = {fieldIdentifier};";

		// gen: Array null check
		output += $"\n{t0}if ({idArrRef} != null) {{";

		// depth up (+1): Null check body
		var t1 = MakeTab( depth + 1 );

		// new identifier: loop iterator
		var idLoopIter = $"it_{depth}";

		// new identifier: array length
		var idArrLen = $"len_{fieldName}_{depth}";

		// gen: Create array length variable
		output += $"\n{t1}var {idArrLen} = {idArrRef}.Length;";

		// gen: Create for loop statement starter
		output += $"\n{t1}for (var {idLoopIter} = 0; {idLoopIter} < {idArrLen}; {idLoopIter}++) {{";

		// depth up (+2): Loop body
		// gen: Reversal of the current value represented by the iterator
		GenerateFieldReversalCode( idArr, $"{idArrRef}[{idLoopIter}]", arrayFieldTypeInfo.ElementType, ref output,
			depth + 2 );

		// depth down (+1): Loop end
		output += $"\n{t1}}};";

		// depth down (0): Null check end
		output += $"\n{t0}}};";
	}

	private static void GenerateEnumFieldReversalCode( string fieldName, string fieldIdentifier,
		ref EnumFieldTypeInfo enumFieldTypeInfo, ref string output, int depth )
	{
		var t0 = MakeTab( depth );

		// gen: Create assignment for field with enum type
		output +=
			$"\n{t0}{fieldIdentifier} = ({enumFieldTypeInfo.TypeName})BinaryPrimitives.ReverseEndianness(({enumFieldTypeInfo.UnderlyingTypeName}) {fieldIdentifier});";
	}

	private static void GenerateGeneratedFriendFieldReversalCode( string fieldName, string fieldIdentifier,
		ref GeneratedFriendFieldTypeInfo generatedFriendFieldTypeInfo, ref string output, int depth )
	{
		var t0 = MakeTab( depth );

		// gen: Create assignment for field we will generate code for (or have generated code for)
		output +=
			$"\n{t0}{fieldIdentifier}.ReverseEndianness();";
	}

	private static void GeneratePrimitiveFieldReversalCode( string fieldName, string fieldIdentifier,
		ref PrimitiveFieldTypeInfo primitiveFieldTypeInfo, ref string output, int depth )
	{
		var t0 = MakeTab( depth );

		// gen: Create assignment for field we will generate code for (or have generated code for)
		output +=
			$"\n{t0}{fieldIdentifier} = BinaryPrimitives.ReverseEndianness({fieldIdentifier});";
	}

	/// <summary>
	/// Generate code to reverse a field
	/// </summary>
	/// <param name="fieldName">Field name - just something we can refer to the field by (eg: "testField")</param>
	/// <param name="fieldIdentifier">Field identifier - the way we interact with the field (eg: "this.testField")</param>
	/// <param name="fieldTypeInfo">Field type we're generating code for</param>
	/// <param name="output">Output string reference</param>
	/// <param name="depth">Current indent / scope depth</param>
	/// <exception cref="InvalidOperationException"></exception>
	private static void GenerateFieldReversalCode( string fieldName, string fieldIdentifier,
		IFieldTypeInfo fieldTypeInfo, ref string output, int depth )
	{
		switch ( fieldTypeInfo )
		{
			case ArrayFieldTypeInfo arrayFieldTypeInfo:
				GenerateArrayFieldReversalCode( fieldName, fieldIdentifier,
					ref arrayFieldTypeInfo,
					ref output, depth );
				break;
			case EnumFieldTypeInfo enumFieldTypeInfo:
				GenerateEnumFieldReversalCode( fieldName, fieldIdentifier, ref enumFieldTypeInfo,
					ref output, depth );
				break;
			case GeneratedFriendFieldTypeInfo generatedFriendFieldTypeInfo:
				GenerateGeneratedFriendFieldReversalCode( fieldName, fieldIdentifier, ref generatedFriendFieldTypeInfo,
					ref output, depth );
				break;
			case PrimitiveFieldTypeInfo primitiveFieldTypeInfo:
				GeneratePrimitiveFieldReversalCode( fieldName, fieldIdentifier, ref primitiveFieldTypeInfo,
					ref output, depth );
				break;
			default:
				throw new InvalidOperationException( $"Unknown field type info {fieldTypeInfo.GetType().Name}" );
		}
	}

	public static string GenerateMethodBody( TargetInfo targetInfo, string targetIdentifier, int depth )
	{
		var output = $"// {targetInfo.Fields.Count} field(s)";

		foreach ( var targetFieldInfo in targetInfo.Fields )
			GenerateFieldReversalCode( targetFieldInfo.FieldName, $"{targetIdentifier}.{targetFieldInfo.FieldName}",
				targetFieldInfo.FieldType, ref output, depth );

		return output;
	}
}
