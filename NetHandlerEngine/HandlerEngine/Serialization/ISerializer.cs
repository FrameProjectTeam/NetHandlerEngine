using System.Buffers;

namespace HandlerEngine.Serialization
{
	public interface ISerializer
	{
		void Write<T, TBuffer>(ref TBuffer buffer, T value)
			where TBuffer : class, IBufferWriter<byte>;
	}
}