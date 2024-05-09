using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;
using ReverseStruct.Target.Partial;

namespace ReverseStruct.Target.Extension;

public static class TargetStructExtensionGenerator
{
	public const string ExtensionClassName = "StructExtensions";

	/// <summary>
	/// Generate source for the "Reverse(this ref *struct*)" method body
	/// </summary>
	/// <param name="targetStructInfo">TargetStructInfo</param>
	/// <returns>Source</returns>
	/// <exception cref="ArgumentOutOfRangeException">Field has invalid reversal method</exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateReverseExtRefMethodBody( TargetStructInfo targetStructInfo )
	{
		var result = $"// {targetStructInfo.Fields.Count} field(s)";
		foreach ( var (name, reversalMethod) in targetStructInfo.Fields )
		{
			result += reversalMethod switch
			{
				ReversalMethod.BinaryPrimitives => $"""
				                                    
				                                    			x.{name} = BinaryPrimitives.ReverseEndianness(x.{name});
				                                    """,
				ReversalMethod.ExistingExtension => $"""
				                                     
				                                     			x.{name}.ReverseEndianness();
				                                     """,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		return result;
	}

	/// <summary>
	/// Generate additional source for the partial static extension class
	/// </summary>
	/// <param name="targetStructInfo">TargetStructInfo</param>
	/// <returns>Source</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateSource( TargetStructInfo targetStructInfo ) =>
		$@"{FileCrumbs.Header}
using System;
using System.Buffers.Binary;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
    public static partial class {ExtensionClassName} {{
		/* Generated extension code for {targetStructInfo.ShortName} */
		public static void ReverseEndianness(this ref {targetStructInfo.FullName} x) {{
			{GenerateReverseExtRefMethodBody( targetStructInfo )}
		}}
	}}
}}
";

	/// <summary>
	/// Add another part of the extension class for reversal of the target struct
	/// </summary>
	/// <param name="ctx">SourceProductionContext</param>
	/// <param name="v">TargetStructInfo</param>
	public static void GenerateExtensionPartialForTargetStruct( SourceProductionContext ctx, TargetStructInfo? v )
	{
		if ( v is not { } targetStructInfo )
			return;

		ctx.AddSource( $"{ExtensionClassName}.{targetStructInfo.FullName}.g.cs",
			GenerateSource( targetStructInfo ) );
	}

	public static void Register( IncrementalGeneratorInitializationContext ctx )
	{
		ctx.RegisterSourceOutput( TargetStructPartialSyntaxProvider.Create( ctx ),
			GenerateExtensionPartialForTargetStruct );
	}
}
