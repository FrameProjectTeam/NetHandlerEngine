using System;
using System.Threading.Tasks;

namespace HandlerEngine
{
	public delegate Task HandlerDelegateAsync(OperationCode operationCode, ref ReadOnlySpan<byte> data);
	public delegate Task RequestResponseDelegateAsync(OperationCode operationCode, short requestNumber, ref ReadOnlySpan<byte> data);
	public delegate void HandlerDelegate(OperationCode operationCode, ref ReadOnlySpan<byte> data);
	public delegate void RequestResponseDelegate(OperationCode operationCode, short requestNumber, ref ReadOnlySpan<byte> data);
}
