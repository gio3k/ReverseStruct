using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReverseStruct.Target;

public enum TargetDeclarationType
{
	Unknown,
	Record,
	StructRecord,
	Class,
	Struct
}

public static partial class SyntaxNodeExtensions
{
	public static TargetDeclarationType GetTargetDeclarationType( this SyntaxNode? node )
	{
		if ( node == null )
			return TargetDeclarationType.Unknown;

		if ( node is RecordDeclarationSyntax { ClassOrStructKeyword: var syntaxToken } )
		{
			return syntaxToken.IsKind( SyntaxKind.StructKeyword )
				? TargetDeclarationType.StructRecord
				: TargetDeclarationType.Record;
		}

		if ( node is ClassDeclarationSyntax )
			return TargetDeclarationType.Class;

		if ( node is StructDeclarationSyntax )
			return TargetDeclarationType.Struct;

		return TargetDeclarationType.Unknown;
	}
}

public static partial class TargetDeclarationTypeExtensions
{
	public static string GetText( this TargetDeclarationType targetDeclarationType )
	{
		return targetDeclarationType switch
		{
			TargetDeclarationType.Unknown => "",
			TargetDeclarationType.Record => "record",
			TargetDeclarationType.StructRecord => "record struct",
			TargetDeclarationType.Class => "class",
			TargetDeclarationType.Struct => "struct",
			_ => throw new ArgumentOutOfRangeException( nameof(targetDeclarationType), targetDeclarationType, null )
		};
	}
}
