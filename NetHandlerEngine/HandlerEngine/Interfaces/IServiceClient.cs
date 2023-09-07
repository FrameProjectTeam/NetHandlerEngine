using System;

namespace HandlerEngine.Interfaces
{
	public interface IServiceClient : IReceiverServiceUnit
	{
		void HandleResponseAsync(
			byte actionIdx,
			OperationCode operationCode,
			short requestNumber,
			ref ReadOnlySpan<byte> data);
	}
}