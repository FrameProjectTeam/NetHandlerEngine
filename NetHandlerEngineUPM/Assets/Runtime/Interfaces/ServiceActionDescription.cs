namespace HandlerEngine.Interfaces
{
	public readonly struct ServiceActionDescription
	{
		public readonly byte PackageId;
		public readonly byte ActionIdx;

		public readonly bool IsRequestResponse;

		public ServiceActionDescription(byte packageId, byte actionIdx, bool isRequestResponse)
		{
			PackageId = packageId;

			ActionIdx = actionIdx;
			IsRequestResponse = isRequestResponse;
		}
	}
}