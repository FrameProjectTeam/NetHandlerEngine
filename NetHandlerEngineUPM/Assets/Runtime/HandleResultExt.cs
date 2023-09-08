namespace HandlerEngine
{
	public static class HandleResultExt
	{
		public static bool IsSuccess(this HandleResult handleResult) => (handleResult & HandleResult.Success) != 0;
		public static bool IsFailure(this HandleResult handleResult) => (handleResult & HandleResult.Success) == 0;
		public static bool IsProcedure(this HandleResult handleResult) => (handleResult & HandleResult.Procedure) != 0;
		public static bool IsRequest(this HandleResult handleResult) => (handleResult & HandleResult.Request) != 0;
		public static bool IsResponse(this HandleResult handleResult) => (handleResult & HandleResult.Response) != 0;
	}
}