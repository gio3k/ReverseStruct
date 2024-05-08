## ReverseStruct

### Usage
```csharp
using ReverseStruct;

[Reversible]
struct TestStruct {
	int Foo;
}

// then somewhere else...
var test = new TestStruct();
test.Reverse();
```
