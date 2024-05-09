using System.Linq;

namespace ReverseStruct;

public static class BinaryPrimitivesUtil
{
	private static readonly string[] ReverseEndiannessSupportedTypes =
	[
		"UIntPtr", "Int64", "UInt32", "UInt128", "SByte", "IntPtr", "UInt64", "Int32", "Int16", "Int128", "Byte"
	];

	public static bool IsTypeSupportedByReverseEndianness( string v ) => ReverseEndiannessSupportedTypes.Contains( v );
}
