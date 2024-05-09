using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.Target.Partial;

public static class TargetPartialGenerator
{
	public const string FilePrefix = "RevPartial";

	private static string GetDeclarationTypeName( TargetDeclarationType declarationType )
	{
		return declarationType switch
		{
			TargetDeclarationType.Class => "class",
			TargetDeclarationType.Record => "record",
			TargetDeclarationType.Struct => "struct",
			TargetDeclarationType.Unknown => throw new InvalidOperationException( "Unknown TargetDeclarationType" ),
			_ => throw new InvalidOperationException( "Invalid TargetDeclarationType" )
		};
	}

	/// <summary>
	/// Generate additional source for the partial static extension class
	/// </summary>
	/// <param name="targetInfo">TargetInfo</param>
	/// <returns>Source</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateSource( TargetInfo targetInfo )
	{
		// We need to skip the namespace declaration if the target is in the global namespace
		var namespacePrefix = targetInfo.ContainingNamespace == null ? "// " : "";
		var namespaceFullName = targetInfo.ContainingNamespace ?? "<global>";

		return $@"{FileCrumbs.Header}
using System;
using System.Buffers.Binary;
using {LibraryConstants.GeneratedNamespace};
{namespacePrefix}namespace {namespaceFullName} {{
    public partial {GetDeclarationTypeName( targetInfo.DeclarationType )} {targetInfo.ShortName} {{
		/* Generated extension code for {targetInfo.ShortName} */
		public void ReverseEndianness() {{
			{ReverseMethodBodyGenerator.GenerateMethodBody( targetInfo, "this" )}
		}}
	}}
{namespacePrefix}}}
";
	}

	/// <summary>
	/// Add another part of the extension class for reversal of the target struct
	/// </summary>
	/// <param name="ctx">SourceProductionContext</param>
	/// <param name="v">TargetInfo</param>
	public static void GeneratePartialForTarget( SourceProductionContext ctx, TargetInfo? v )
	{
		if ( v is not { } targetInfo )
			return;

		ctx.AddSource( $"{FilePrefix}.{targetInfo.FullName}.g.cs",
			GenerateSource( targetInfo ) );
	}

	public static void Register( IncrementalGeneratorInitializationContext ctx )
	{
		ctx.RegisterSourceOutput( TargetPartialSyntaxProvider.Create( ctx ),
			GeneratePartialForTarget );
	}
}
