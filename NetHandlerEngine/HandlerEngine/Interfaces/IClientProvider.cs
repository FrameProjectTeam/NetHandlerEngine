namespace HandlerEngine.Interfaces
{
	public interface IClientProvider
	{
		T CreateClient<T>(INetUser user, byte serviceId)
			where T : IServiceClient, new();

		void BindClient<T>(INetUser user, byte serviceId, T instance)
			where T : IServiceClient;
	}
}