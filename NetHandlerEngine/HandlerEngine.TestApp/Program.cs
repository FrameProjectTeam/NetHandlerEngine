using System.Buffers;

using HandlerEngine.Interfaces;
using HandlerEngine.Serialization;

using LiteNetLib;
using HandlerEngine.TestProtocol;

using MemoryPack;

using NLog;

namespace HandlerEngine.TestApp;

public static class Program
{
	public static ILogger Log => LogManager.GetCurrentClassLogger();
	
	private sealed class MemPackSerializer : ISerializer
	{
		public void Write<T, TBuffer>(ref TBuffer buffer, T value)
			where TBuffer : class, IBufferWriter<byte>
		{
			MemoryPackSerializer.Serialize(in buffer, value);
		}
	}

	private sealed class MemPackDeserializer : IDeserializer
	{
		public T? Read<T>(ref ReadOnlySpan<byte> data)
		{
			T? value = default;
			int consumed = MemoryPackSerializer.Deserialize(data, ref value);
			data = data[consumed..];
			return value;
		}
	}
	
	public static void Main(string[] args)
	{
		HandlerEngineSerializer.InitWrapper(new MemPackDeserializer(), new MemPackSerializer());
		
		if(args.Contains("--server"))
		{
			Log.Info("Starting server.");
			Server();	
		}
		else
		{
			Log.Info("Starting client.");
			Client();	
		}
	}

	private sealed class EchoService : EchoServiceMediator
	{
		public override string EchoString(string message)
		{
			return message;
		}
	}

	private sealed class HUser : INetUser
	{
		private readonly NetPeer _netPeer;

		public int Id => _netPeer.Id;

		public HUser(NetPeer netPeer)
		{
			_netPeer = netPeer;
		}

		void INetRecipient.Send(INetworkBuffer buffer, ChannelType channelType, byte channelId)
		{
			var deliveryMethod = (DeliveryMethod)channelType;
			_netPeer.Send(buffer.WrittenSpan, channelId, deliveryMethod);
		}
	}
	
	private static void Server()
	{
		var listener = new EventBasedNetListener();
		var server = new NetManager(listener);
		server.Start(9050);

		var networkEngine = new NetworkEngine();
		EchoServiceClient? echoService = null;

		listener.ConnectionRequestEvent += request =>
		{
			request.AcceptIfKey("SomeConnectionKey");
		};

		listener.PeerConnectedEvent += peer =>
		{
			var hPeer = new HUser(peer);
			IUserNetScope scope = networkEngine.CreateScope(hPeer);
			peer.Tag = scope;
			echoService = networkEngine.CreateClient<EchoServiceClient>(hPeer, 5);
			networkEngine.CreateService<EchoService>(hPeer, 5);
		};

		listener.PeerDisconnectedEvent += (peer, info) =>
		{
			var scope = (IUserNetScope)peer.Tag;
			scope.Dispose();
		};

		listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
		{
			var scope = (IUserNetScope)peer.Tag;
			ReadOnlySpan<byte> data = reader.GetRemainingBytesSegment();
			scope.HandleAsync(ref data, out _, out HandleResult result);
			if(result.IsFailure())
			{
				Log.Error($"Handle failure [{result:F}].");
			}
			reader.Recycle();
		};
		
		while(!Console.KeyAvailable)
		{
			echoService?.EchoString("Echo from server.")
					   .ContinueWith(task => Log.Info("Echo response: " + task.Result));

			server.PollEvents();
			Thread.Sleep(15);
		}

		server.Stop();
	}

	private static void Client()
	{
		var listener = new EventBasedNetListener();
		var client = new NetManager(listener);
		
		client.Start();
		NetPeer? serverPeer = client.Connect("localhost", 9050, "SomeConnectionKey");

		var networkEngine = new NetworkEngine();
		EchoServiceClient? echoService = null;
		
		listener.NetworkReceiveEvent += (fromPeer, reader, channel, deliveryMethod) =>
		{
			var scope = (IUserNetScope)serverPeer.Tag;
			ReadOnlySpan<byte> data = reader.GetRemainingBytesSegment();
			scope.HandleAsync(ref data, out _, out HandleResult result);
			if(result.IsFailure())
			{
				Log.Error($"Handle failure [{result:F}].");
			}
			reader.Recycle();
		};

		listener.PeerConnectedEvent += netPeer =>
		{
			var hPeer = new HUser(netPeer);
			serverPeer.Tag = networkEngine.CreateScope(hPeer);
			echoService = networkEngine.CreateClient<EchoServiceClient>(hPeer, 5);
			networkEngine.CreateService<EchoService>(hPeer, 5);
        };
		
		while(!Console.KeyAvailable)
		{
			echoService?.EchoString("Echo from client.")
					   .ContinueWith(task => Log.Info("Echo response: " + task.Result));
			
			client.PollEvents();
			Thread.Sleep(15);
		}

		client.Stop();
	}
}