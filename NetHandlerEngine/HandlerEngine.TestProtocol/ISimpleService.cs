using HandlerEngine.Attributes;

namespace HandlerEngine.TestProtocol
{
	[NetService("sample_1", 0)]
	public interface ISimpleService
	{
		[NetCall(0x00)]
		void TestRpc1(uint someId);

		[NetCall(0x01)]
		Task TestRpc2(uint someId);
		
		[NetCall(0x03)]
		Task<bool> TestRequest1(uint someId);
		
		[NetCall(0x04)]
		Task TestRequest2(uint someId, out int someValue);
	}
}