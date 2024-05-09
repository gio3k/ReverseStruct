using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using ReverseStruct.StaticCode.Crumbs;

namespace ReverseStruct.Target.Extension;

public static class TargetExtensionGenerator
{
	public const string ExtensionClassName = "StructExtensions";

	/// <summary>
	/// Generate additional source for the partial static extension class
	/// </summary>
	/// <param name="targetInfo">TargetStructInfo</param>
	/// <returns>Source</returns>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	private static string GenerateSource( TargetInfo targetInfo )
	{
		var paramTypePrefix = targetInfo.DeclarationType == TargetDeclarationType.Struct ? "ref " : "";

		return $@"{FileCrumbs.Header}
using System;
using System.Buffers.Binary;
{NamespaceCrumbs.PublicNamespaceStatementStarter} {{
    public static partial class {ExtensionClassName} {{
		/* Generated extension code for {targetInfo.ShortName} */
		public static void ReverseEndianness(this {paramTypePrefix}{targetInfo.FullName} x) {{
			{ReverseMethodBodyGenerator.GenerateMethodBody( targetInfo, "x" )}
		}}
	}}
}}
";
	}

	/// <summary>
	/// Add another part of the extension class for reversal of the target
	/// </summary>
	/// <param name="ctx">SourceProductionContext</param>
	/// <param name="v">TargetInfo</param>
	public static void GenerateExtensionPartialForTarget( SourceProductionContext ctx, TargetInfo? v )
	{
		if ( v is not { } targetInfo )
			return;
		
		ctx.AddSource( $"{ExtensionClassName}.{targetInfo.FullName}.g.cs",
			GenerateSource( targetInfo ) );
	}

	public static void Register( IncrementalGeneratorInitializationContext ctx )
	{
		ctx.RegisterSourceOutput( TargetExtensionSyntaxProvider.Create( ctx ),
			GenerateExtensionPartialForTarget );
	}
}
