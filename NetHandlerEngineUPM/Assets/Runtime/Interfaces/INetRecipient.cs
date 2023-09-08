namespace HandlerEngine.Interfaces
{
	public interface INetRecipient
	{
		void Send(INetworkBuffer buffer, ChannelType channelType, byte channelId);
	}
}