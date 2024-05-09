## ReverseStruct

Simple C# source generator that adds byte order reversal to structs

https://www.nuget.org/packages/giodotblue.ReverseStruct/

### Usage
```csharp
using ReverseStruct;

namespace MyNamespace;

// create an extension class with the [Reversible] attribute...
[Reversible]
public struct StructA
{
	public int Value;
}

// or use IReversible to generate ReverseEndianness() as part of your struct... 
public partial struct StructB : IReversible
{
	// nested structs work fine!
	public StructA Foo;
	public uint Bar;
}

public static class Program
{
	public static void Main( string[] args )
	{
		var test = new StructB();
		test.ReverseEndianness();
	}
}
```
