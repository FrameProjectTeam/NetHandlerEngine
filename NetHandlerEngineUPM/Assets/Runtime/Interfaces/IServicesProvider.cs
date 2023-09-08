namespace HandlerEngine.Interfaces
{
	public interface IServicesProvider
	{
		TService CreateService<TService>(INetUser client, byte serviceId)
			where TService : INetworkService, new();

		void BindService(INetUser client, byte serviceId, INetworkService instance);

		bool UnbindService(INetUser client, INetworkService service);
	}
}