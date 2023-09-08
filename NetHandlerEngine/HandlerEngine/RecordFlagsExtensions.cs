using System.Runtime.CompilerServices;

namespace HandlerEngine
{
	public static class RecordFlagsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRequest(this RecordFlags flags)
		{
			return (flags & RecordFlags.Request) != 0;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRpc(this RecordFlags flags)
		{
			return !IsRequest(flags);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsync(this RecordFlags flags)
		{
			return (flags & RecordFlags.Async) != 0;
		}
	}
}