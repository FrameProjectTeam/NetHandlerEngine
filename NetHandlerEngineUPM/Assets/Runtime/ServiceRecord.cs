using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	public class ServiceRecord
	{
		public byte ServiceId { get; private set; }

		public RecordFlags Flags { get; private set; }
		
		public INetworkService Service { get; private set; }
		public byte ServiceActionIdx { get; private set; }

		public IServiceClient Client{ get; private set; }
		public byte ClientActionIdx { get; private set; }

		public ServiceRecord(byte serviceId, RecordFlags flags)
		{
			ServiceId = serviceId;
			Flags = flags;
		}

		internal void SetClient(IServiceClient client, byte clientActionIdx)
		{
			Client = client;
			ClientActionIdx = clientActionIdx;
			if(client != null)
			{
				Flags |= RecordFlags.HasClient;
			}
			else
			{
				Flags &= ~RecordFlags.HasClient;
			}
		}

		internal void SetService(INetworkService service, byte serviceActionIdx)
		{
			Service = service;
			ServiceActionIdx = serviceActionIdx;
			if(service != null)
			{
				Flags |= RecordFlags.HasService;
			}
			else
			{
				Flags &= ~RecordFlags.HasService;
			}
		}
	}
}