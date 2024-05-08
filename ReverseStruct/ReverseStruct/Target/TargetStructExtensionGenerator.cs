using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.Target;

public static class TargetStructExtensionGenerator
{
	public static readonly string ExtensionClassName = "StructExtensions";

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateReverseExtRefMethodBody( TargetStructInfo targetStructInfo )
	{
		var result = $"public static void Reverse(this ref {targetStructInfo.TargetFullName} x) {{";
		foreach ( var name in targetStructInfo.FieldNames )
			result += $"\n\t\t\tx.{name} = BinaryPrimitives.ReverseEndianness(x.{name});";

		result += "\n\t\t}";
		return result;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateSource( TargetStructInfo targetStructInfo )
	{
		return $@"{FileCrumbs.Header}
using System;
using System.Buffers.Binary;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
    public static partial class {ExtensionClassName} {{
		/* Generated extension code for {targetStructInfo.TargetShortName} */
		{GenerateReverseExtRefMethodBody( targetStructInfo )}
	}}
}}
";
	}

	public static void GenerateExtensionPartialForTargetStruct( SourceProductionContext ctx, TargetStructInfo? v )
	{
		if ( v is not { } targetStructInfo )
			return;

		ctx.AddSource( $"{ExtensionClassName}.{targetStructInfo.TargetFullName}.g.cs",
			GenerateSource( targetStructInfo ) );
	}
}
