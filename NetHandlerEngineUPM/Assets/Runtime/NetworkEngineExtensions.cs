using System;

using HandlerEngine.Interfaces;
using HandlerEngine.Utilities;

namespace HandlerEngine
{
	public static class NetworkEngineExtensions
	{
		public static TService CreateService<TService, TEnum>(
			this IServicesProvider networkEngine,
			INetUser client,
			TEnum serviceId)
			where TService : INetworkService, new()
			where TEnum : Enum
		{
			return networkEngine.CreateService<TService>(client, EnumConverter<TEnum, byte>.To(serviceId));
		}

		public static void BindService<TEnum>(
			this IServicesProvider networkEngine,
			INetUser client,
			INetworkService instance,
			TEnum serviceId)
			where TEnum : Enum
		{
			networkEngine.BindService(client, EnumConverter<TEnum, byte>.To(serviceId), instance);
		}

		public static TClientInterface CreateClient<TClientInterface, TEnum>(this IClientProvider networkEngine, INetUser user, TEnum serviceId)
			where TClientInterface : IServiceClient, new()
			where TEnum : Enum
		{
			return networkEngine.CreateClient<TClientInterface>(user, EnumConverter<TEnum, byte>.To(serviceId));
		}
	}
}