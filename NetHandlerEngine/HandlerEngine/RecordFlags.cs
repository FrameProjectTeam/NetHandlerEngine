using System;

namespace HandlerEngine
{
	[Flags]
	public enum RecordFlags : byte
	{
		Request = 1 << 0,
		Async = 1 << 1,
		
		HasClient = 1 << 2,
		HasService = 1 << 3,
		
		SyncProcedure = 0,
		AsyncProcedure = Async,
		SyncRequest = Request,
		AsyncRequest = Request | Async,
	}
}