using System;

using HandlerEngine.Interfaces;
using HandlerEngine.Utilities;

namespace HandlerEngine
{
	public static class BroadcastEngineExtensions
	{
		public static TBroadcastServiceClient GetBroadcastClient<TBroadcastServiceClient, TEnum>(
			this INetEngine netEngine,
			INetRecipient group,
			TEnum serviceId)
			where TBroadcastServiceClient : IBroadcastServiceClient, new()
			where TEnum : Enum
		{
			return netEngine.CreateBroadcastClient<TBroadcastServiceClient>(group, EnumConverter<TEnum, byte>.To(serviceId));
		}
	}
}