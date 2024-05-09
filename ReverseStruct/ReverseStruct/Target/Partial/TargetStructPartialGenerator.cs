using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.Target.Partial;

public static class TargetStructPartialGenerator
{
	public const string FileSuffixName = "RevPartial";

	/// <summary>
	/// Generate source for the "ReverseEndianness()" method body
	/// </summary>
	/// <param name="targetStructInfo">TargetStructInfo</param>
	/// <returns>Source</returns>
	/// <exception cref="ArgumentOutOfRangeException">Field has invalid reversal method</exception>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateMethodBody( TargetStructInfo targetStructInfo )
	{
		var result = $"// {targetStructInfo.Fields.Count} field(s)";
		foreach ( var (name, reversalMethod) in targetStructInfo.Fields )
		{
			result += reversalMethod switch
			{
				ReversalMethod.BinaryPrimitives => $"""
				                                    
				                                    			this.{name} = BinaryPrimitives.ReverseEndianness(this.{name});
				                                    """,
				ReversalMethod.ExistingExtension => $"""
				                                     
				                                     			this.{name}.Reverse();
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
namespace {targetStructInfo.ContainingNamespace} {{
    public partial struct {targetStructInfo.ShortName} {{
		/* Generated extension code for {targetStructInfo.ShortName} */
		public void ReverseEndianness() {{
			{GenerateMethodBody( targetStructInfo )}
		}}
	}}
}}
";

	/// <summary>
	/// Add another part of the extension class for reversal of the target struct
	/// </summary>
	/// <param name="ctx">SourceProductionContext</param>
	/// <param name="v">TargetStructInfo</param>
	public static void GeneratePartialForTargetStruct( SourceProductionContext ctx, TargetStructInfo? v )
	{
		if ( v is not { } targetStructInfo )
			return;

		ctx.AddSource( $"{FileSuffixName}.{targetStructInfo.FullName}.g.cs",
			GenerateSource( targetStructInfo ) );
	}

	public static void Register( IncrementalGeneratorInitializationContext ctx )
	{
		ctx.RegisterSourceOutput( TargetStructPartialSyntaxProvider.Create( ctx ),
			GeneratePartialForTargetStruct );
	}
}
