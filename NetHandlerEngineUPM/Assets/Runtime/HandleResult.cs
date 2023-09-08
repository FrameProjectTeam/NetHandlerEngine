using System;

namespace HandlerEngine
{
	[Flags]
	public enum HandleResult : byte
	{
		Success				= 1 << 0,
		Procedure			= 1 << 1,
		Request				= 1 << 2,
		Response			= 1 << 3,
		
		InvalidOpCode		= 1 << 4,
		
		ServiceNotFound		= 1 << 5,
		ClientNotFound		= 1 << 6
	}
}