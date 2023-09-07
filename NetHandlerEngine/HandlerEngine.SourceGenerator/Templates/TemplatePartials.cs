using HandlerEngine.SourceGenerator.CollectableData;

namespace HandlerEngine.SourceGenerator.Templates;

public partial class ServiceClientInterfaceTemplate : ISourceFileTemplate
{
	public ServiceInfo Info { get; set; }

#region ISourceFileTemplate Implementation

	public string FileName => $"I{Info.ServiceName}Client.g.cs";

#endregion
}

public partial class ServiceRpcClientInterfaceTemplate : ISourceFileTemplate
{
	public ServiceInfo Info { get; set; }

#region ISourceFileTemplate Implementation

	public string FileName => $"I{Info.ServiceName}RpcClient.g.cs";

#endregion
}

public partial class ServiceRpcClientTemplate : ISourceFileTemplate
{
	public ServiceInfo Info { get; set; }

#region ISourceFileTemplate Implementation

	public string FileName => $"{Info.ServiceName}RpcClient.g.cs";

#endregion
}

public partial class ServiceClientTemplate : ISourceFileTemplate
{
	public ServiceInfo Info { get; set; }

#region ISourceFileTemplate Implementation

	public string FileName => $"{Info.ServiceName}Client.g.cs";

#endregion
}

public partial class ServiceMediatorTemplate : ISourceFileTemplate
{
	public ServiceInfo Info { get; set; }

#region ISourceFileTemplate Implementation

	public string FileName => $"{Info.ServiceName}Mediator.g.cs";

#endregion
}