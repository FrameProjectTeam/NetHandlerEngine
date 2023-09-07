using System;

using HandlerEngine.Interfaces;
using HandlerEngine.Utilities;

namespace HandlerEngine
{
	public static class HandlerEngineExtensions
	{
		public static TServiceClient GetClient<TClient, TServiceClient, TEnum>(INetworkEngine engine, TClient client, TEnum serviceId)
			where TServiceClient : IServiceClient, new()
			where TEnum : Enum
			where TClient : INetUser
		{
			return engine.CreateClient<TServiceClient>(client, EnumConverter<TEnum, byte>.To(serviceId));
		}
	}
}