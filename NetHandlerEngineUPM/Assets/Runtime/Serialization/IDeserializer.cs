using System;

namespace HandlerEngine.Serialization
{
	public interface IDeserializer
	{
		T Read<T>(ref ReadOnlySpan<byte> data);
	}
}