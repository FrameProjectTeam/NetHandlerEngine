using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	public class ServiceRecord
	{
		public INetworkService Service;
		public IServiceClient Client;

		public byte ServiceId;

		public byte ServiceActionIdx;
		public byte ClientActionIdx;

		public bool IsRequestResponse;
	}
}