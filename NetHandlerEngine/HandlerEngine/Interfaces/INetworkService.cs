using System;
using System.Threading.Tasks;

namespace HandlerEngine.Interfaces
{
	public interface INetworkService : IReceiverServiceUnit, IDisposable
	{
		bool IsDisposed { get; }

		Task HandleRpcAsync(
			byte actionIdx,
			OperationCode operationCode,
			ref ReadOnlySpan<byte> data);

		Task HandleRequestAsync(
			byte actionIdx,
			OperationCode operationCode,
			short requestNumber,
			ref ReadOnlySpan<byte> data);
	}
}