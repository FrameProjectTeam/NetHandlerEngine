using HandlerEngine.Attributes;

namespace HandlerEngine.TestProtocol
{
	[NetService("echo", 0)]
	public interface IEchoService
	{
		[NetCall(0x00)]
		string EchoString(string message);
	}
}