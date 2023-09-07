using Microsoft.CodeAnalysis;

namespace HandlerEngine.SourceGenerator.CollectableData;

public readonly struct MethodArgumentInfo
{
	public readonly RefKind Specifier;
	public readonly string Type;
	public readonly string Name;

	public MethodArgumentInfo(RefKind specifier, string type, string name)
	{
		Name = name;
		Type = type;
		Specifier = specifier;
	}
}