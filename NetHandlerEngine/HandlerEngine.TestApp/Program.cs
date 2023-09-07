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

		void INetRecipient.Send(ref ReadOnlySpan<byte> data, ChannelType channelType, byte channelId)
		{
			var deliveryMethod = (DeliveryMethod)channelType;
			_netPeer.Send(data, channelId, deliveryMethod);
		}
	}
	
	private static void Server()
	{
		var listener = new EventBasedNetListener();
		var server = new NetManager(listener);
		server.Start(9050 /* port */);

		var networkEngine = new NetworkEngine();
		
		listener.ConnectionRequestEvent += request =>
		{
			if(server.ConnectedPeersCount < 10 /* max connections */)
			{
				request.AcceptIfKey("SomeConnectionKey");
			}
			else
			{
				request.Reject();
			}
		};

		listener.PeerConnectedEvent += peer =>
		{
			var hPeer = new HUser(peer);
			IUserNetScope scope = networkEngine.CreateScope(hPeer);
			peer.Tag = scope;
			
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
			server.PollEvents();
			Thread.Sleep(15);
		}

		server.Stop();
	}

	private static void Client()
	{
		var listener = new EventBasedNetListener();
		var client = new NetManager(listener);
		var networkEngine = new NetworkEngine();
		
		client.Start();
		NetPeer? serverPeer = client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);

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
			var hNetPeer = new HUser(netPeer);
			serverPeer.Tag = networkEngine.CreateScope(hNetPeer);
			echoService = networkEngine.CreateClient<EchoServiceClient>(hNetPeer, 5);
        };
		
		while(!Console.KeyAvailable)
		{
			echoService?.EchoString("Some string echo.")
					   .ContinueWith(task => Log.Info("Echo response: " + task.Result));
			
			client.PollEvents();
			Thread.Sleep(15);
		}

		client.Stop();
	}
}