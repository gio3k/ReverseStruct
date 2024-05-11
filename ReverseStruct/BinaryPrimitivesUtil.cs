using System.Linq;
using Microsoft.CodeAnalysis;

namespace ReverseStruct;

public static class BinaryPrimitivesUtil
{
	private static readonly string[] ReverseEndiannessSupportedTypes =
	[
		"UIntPtr", "Int64", "UInt32", "UInt128", "SByte", "IntPtr", "UInt64", "Int32", "Int16", "Int128", "Byte"
	];

	public static bool IsTypeSupportedByReverseEndianness( ITypeSymbol typeSymbol ) =>
		ReverseEndiannessSupportedTypes.Contains( typeSymbol.MetadataName );
}
