using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ReverseStruct.StaticCode;
using ReverseStruct.Target;

namespace ReverseStruct;

[Generator]
public class LibraryGenerator : IIncrementalGenerator
{
	public void Initialize( IncrementalGeneratorInitializationContext ctx )
	{
		ctx.RegisterPostInitializationOutput( postInitCtx =>
		{
			postInitCtx.AddSource( $"{ReversibleAttributeDefinition.Name}.g.cs",
				SourceText.From( ReversibleAttributeDefinition.Source, Encoding.UTF8 ) );

			postInitCtx.AddSource( $"{NotReversibleAttributeDefinition.Name}.g.cs",
				SourceText.From( NotReversibleAttributeDefinition.Source, Encoding.UTF8 ) );
		} );

		ctx.RegisterSourceOutput( TargetStructSyntaxProvider.Create( ctx ),
			TargetStructExtensionGenerator.GenerateExtensionPartialForTargetStruct );
	}
}
