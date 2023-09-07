using System;
using System.Threading.Tasks;

namespace HandlerEngine.Interfaces
{
	public interface IUserNetScope : IDisposable
	{
		bool IsDisposed { get; }

		INetUser User { get; }

		Task HandleAsync(ref ReadOnlySpan<byte> data, out OperationCode operationCode, out HandleResult handleResult);
	}
}