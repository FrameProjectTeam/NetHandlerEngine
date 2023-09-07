namespace HandlerEngine.SourceGenerator.CollectableData;

public readonly struct MethodInfo
{
	public readonly string Name;
	public readonly string ReturnType;

	public readonly bool IsTask;

	public readonly CallAttributeInfo CallAttributeInfo;

	public readonly MethodArgumentInfo[] Parameters;

	public MethodInfo(
		string name,
		string returnType,
		CallAttributeInfo callAttributeInfo,
		MethodArgumentInfo[] parameters,
		bool isTask)
	{
		Name = name;
		ReturnType = returnType;
		Parameters = parameters;
		IsTask = isTask;
		CallAttributeInfo = callAttributeInfo;
	}
}