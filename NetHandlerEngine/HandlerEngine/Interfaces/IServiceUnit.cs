namespace HandlerEngine.Interfaces
{
	public interface IServiceUnit
	{
		string ServiceName { get; }

		byte ServiceId { get; }

		INetRecipient Recipient { get; }

		void Bind(INetRecipient recipient, byte serviceId);
	}
}