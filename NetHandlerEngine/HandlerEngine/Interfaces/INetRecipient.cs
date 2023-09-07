using System;

namespace HandlerEngine.Interfaces
{
	public interface INetRecipient
	{
		void Send(ref ReadOnlySpan<byte> data, ChannelType channelType, byte channelId);
	}
}