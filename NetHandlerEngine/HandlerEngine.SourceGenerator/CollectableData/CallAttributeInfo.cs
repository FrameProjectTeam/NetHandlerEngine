namespace HandlerEngine.SourceGenerator.CollectableData;

public readonly struct CallAttributeInfo
{
	public readonly int CallId;
	public readonly byte ChannelType;
	public readonly byte ChannelId;

	public CallAttributeInfo(int callId, byte channelType, byte channelId)
	{
		CallId = callId;
		ChannelType = channelType;
		ChannelId = channelId;
	}
}