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
		var result = $"\t// {targetStructInfo.Fields.Count} field(s)";
		foreach ( var (name, reversalMethod) in targetStructInfo.Fields )
		{
			result += reversalMethod switch
			{
				ReversalMethod.BinaryPrimitives => $"""
				                                    
				                                    			x.{name} = BinaryPrimitives.ReverseEndianness(x.{name});
				                                    """,
				ReversalMethod.ExistingExtension => $"""
				                                    
				                                    			x.{name}.Reverse();
				                                    """,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
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
		public static void Reverse(this ref {targetStructInfo.TargetFullName} x) {{
		{GenerateReverseExtRefMethodBody( targetStructInfo )}
		}}
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
