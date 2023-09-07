namespace HandlerEngine.SourceGenerator.CollectableData;

public readonly struct ResponseMethodInfo
{
	public readonly string Name;
	public readonly string ReturnType;
	public readonly MethodArgumentInfo[] RequestArguments;
	public readonly MethodArgumentInfo[] ResponseArguments;
	public readonly CallAttributeInfo CallAttributeInfo;
	public readonly bool ReturnIsTuple;

	public ResponseMethodInfo(
		string name,
		string returnType,
		CallAttributeInfo callAttributeInfo,
		MethodArgumentInfo[] requestArguments,
		MethodArgumentInfo[] responseArguments,
		bool returnIsTuple)
	{
		Name = name;
		ReturnType = returnType;
		RequestArguments = requestArguments;
		CallAttributeInfo = callAttributeInfo;
		ReturnIsTuple = returnIsTuple;
		ResponseArguments = responseArguments;
	}
}