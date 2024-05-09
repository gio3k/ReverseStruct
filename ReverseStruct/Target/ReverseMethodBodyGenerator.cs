using System;

namespace ReverseStruct.Target;

public static class ReverseMethodBodyGenerator
{
	/// <summary>
	/// Generate source for the "ReverseEndianness()" method body
	/// </summary>
	/// <param name="targetInfo">TargetInfo</param>
	/// <param name="targetIdentifier"></param>
	/// <returns>Source</returns>
	/// <exception cref="ArgumentOutOfRangeException">Field has invalid reversal method</exception>
	public static string GenerateMethodBody( TargetInfo targetInfo, string targetIdentifier )
	{
		const string nl = "\n";
		const string tab = "\t";
		const string align = $"{tab}{tab}{tab}";
		var result = $"// {targetInfo.Fields.Count} field(s)";

		foreach ( var fieldInfo in targetInfo.Fields )
		{
			if ( fieldInfo.IsArrayType )
			{
				// Create array reversing source
				// Generate code to create a variable for the array
				var arrayIdentifier = $"refType_{fieldInfo.FieldName}";
				result += $"{nl}{align}var {arrayIdentifier} = {targetIdentifier}.{fieldInfo.FieldName};";

				// Generate null check
				result += $"{nl}{align}if ({arrayIdentifier} != null) {{";

				// Generate for loop code
				result += $"{nl}{align}{tab}for (var i = 0; i < {arrayIdentifier}.Length; i++) {{";

				// Generate for loop body
				result += fieldInfo.ReversalMethod switch
				{
					ReversalMethod.BinaryPrimitives =>
						$"{nl}{align}{tab}{tab}{arrayIdentifier}[i] = BinaryPrimitives.ReverseEndianness({arrayIdentifier}[i]);",
					ReversalMethod.ExistingExtension =>
						$"{nl}{align}{tab}{tab}{arrayIdentifier}[i].ReverseEndianness();",
					_ => throw new ArgumentOutOfRangeException()
				};

				// Generate for loop end
				result += $"{nl}{align}{tab}}}";

				// Generate null check end
				result += $"{nl}{align}}}";
				continue;
			}

			// Generate normal field reversal code
			result += fieldInfo.ReversalMethod switch
			{
				ReversalMethod.BinaryPrimitives =>
					$"{nl}{align}{targetIdentifier}.{fieldInfo.FieldName} = BinaryPrimitives.ReverseEndianness({targetIdentifier}.{fieldInfo.FieldName});",
				ReversalMethod.ExistingExtension =>
					$"{nl}{align}{targetIdentifier}.{fieldInfo.FieldName}.ReverseEndianness();",
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		return result;
	}
}
