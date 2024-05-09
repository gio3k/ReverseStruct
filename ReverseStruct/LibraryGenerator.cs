using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ReverseStruct.StaticCode;
using ReverseStruct.Target;
using ReverseStruct.Target.Extension;
using ReverseStruct.Target.Partial;

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

			postInitCtx.AddSource( $"{IReversibleDefinition.Name}.g.cs",
				SourceText.From( IReversibleDefinition.Source, Encoding.UTF8 ) );
		} );

		TargetPartialGenerator.Register( ctx );
		TargetExtensionGenerator.Register( ctx );
	}
}
