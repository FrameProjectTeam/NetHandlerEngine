using HandlerEngine.Attributes;
using HandlerEngine.TestProtocol.DataStructs;

namespace HandlerEngine.TestProtocol
{
	[NetService("featured", 0)]
	public interface IFeaturedService
	{
		[NetCall(0x00)]
		void TestRpc1(uint someId, Vector3 someStruct);

		[NetCall(0x01)]
		Task<bool> TestRequest1(uint someId);

		[NetCall(0x02)]
		bool TestRequest2(uint someId, out byte someSimpleOutData, out Vector3 someStructOutData);
		
		[NetCall(0xFF)]
		Task<Vector3> TestRequest3(uint someId, out byte someSimpleOutData, out Vector3 someStructOutData);

		[NetCall(0x03)]
		Task<int[]> TestRequest4(float[] someId, List<Vector3> points, out byte someSimpleOutData, out Vector3 someStructOutData);
		
		[NetCall(0xF1, ChannelType = ChannelType.Unreliable, ChannelId = 2)]
		void UnreliableRpc1(uint someData);
		
		[NetCall(0xF2, ChannelType = ChannelType.Sequenced, ChannelId = 1)]
		void SequencedRpc1(uint someData);
		
		[NetCall(0xF3, ChannelType = ChannelType.ReliableOrdered, ChannelId = 2)]
		void ReliableOrderedRpc3(uint someData);
		
		[NetCall(0xF4, ChannelType = ChannelType.ReliableSequenced, ChannelId = 1)]
		void ReliableSequencedRpc1(uint someData);
        
		[NetCall(0xF5, ChannelType = ChannelType.ReliableUnordered, ChannelId = 1)]
		void ReliableUnorderedRpc1(uint someData);
	}
}