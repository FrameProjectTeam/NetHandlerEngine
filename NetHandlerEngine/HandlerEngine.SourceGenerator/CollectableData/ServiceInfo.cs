namespace HandlerEngine.SourceGenerator.CollectableData;

public readonly struct ServiceInfo
{
	public readonly string ServiceName;
	public readonly string Namespace;

	public ServiceInfo(string name, MethodInfo[] methodInfos, string @namespace)
	{
		ServiceName = name;
		MethodInfos = methodInfos;
		Namespace = @namespace;
	}

	public bool HasNamespace => !string.IsNullOrEmpty(Namespace);

	public MethodInfo[] MethodInfos { get; }
}