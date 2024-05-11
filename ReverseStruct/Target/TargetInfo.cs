using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReverseStruct.StaticCode;
using ReverseStruct.Target.TypeSupport;

namespace ReverseStruct.Target;

/// <summary>
/// Information about a struct with the [Reversible] attribute
/// <see cref="ReversibleAttributeDefinition"/>
/// </summary>
public record struct TargetInfo
{
	public string FullName;
	public string ShortName;
	public string? ContainingNamespace;
	public TargetDeclarationType DeclarationType;
	public List<TargetFieldInfo> Fields;
	public List<Diagnostic> Diagnostics;

	internal void ReportDiagnostic( Diagnostic diagnostic ) => Diagnostics.Add( diagnostic );

	private static TargetFieldInfo? TryCreateFieldInfo( TargetInfo targetInfo, IFieldSymbol fieldSymbol )
	{
		// Check for NotReversible attribute
		if ( fieldSymbol.HasNotReversibleAttribute() )
			return null;

		// Create field type info
		if ( FieldTypeInfoCreator.TryCreateFieldTypeInfo( targetInfo, fieldSymbol.Type, fieldSymbol )
		    is not { } fieldTypeInfo )
			return null;

		return new TargetFieldInfo( fieldSymbol.Name, fieldTypeInfo );
	}

	public static TargetInfo Create( INamedTypeSymbol namedTypeSymbol, SyntaxNode? syntaxNode )
	{
		var targetInfo = new TargetInfo
		{
			FullName = namedTypeSymbol.ToString(),
			ShortName = namedTypeSymbol.Name,
			ContainingNamespace =
				namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
					? null
					: namedTypeSymbol.ContainingNamespace.ToString(),
			Fields = [],
			Diagnostics = [],
			DeclarationType = syntaxNode switch
			{
				ClassDeclarationSyntax => TargetDeclarationType.Class,
				StructDeclarationSyntax => TargetDeclarationType.Struct,
				RecordDeclarationSyntax => TargetDeclarationType.Record,
				_ => TargetDeclarationType.Unknown
			}
		};

		foreach ( var typeMemberSymbol in namedTypeSymbol.GetMembers() )
		{
			if ( typeMemberSymbol is not IFieldSymbol fieldSymbol )
				continue;

			if ( TryCreateFieldInfo( targetInfo, fieldSymbol ) is { } fieldInfo )
				targetInfo.Fields.Add( fieldInfo );
		}

		return targetInfo;
	}
}
