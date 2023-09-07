namespace HandlerEngine.Interfaces
{
	public interface INetEngine : INetGroupProvider
	{
		T CreateBroadcastClient<T>(INetRecipient target, byte serviceId)
			where T : IBroadcastServiceClient, new();
	}
}