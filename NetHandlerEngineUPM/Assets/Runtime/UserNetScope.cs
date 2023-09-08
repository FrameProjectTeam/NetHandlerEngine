using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;

using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	internal sealed class UserNetScope : IUserNetScope
	{
		private readonly Dictionary<OperationCode, ServiceRecord> _handlers = new();

		private readonly List<INetGroup> _userGroups = new();

		private bool _isDisposing;
		
		public UserNetScope(INetUser client)
		{
			User = client;
		}

#region IDisposable Implementation

		public void Dispose()
		{
			if(IsDisposed || _isDisposing)
			{
				return;
			}

			_isDisposing = true;

			_handlers.Clear();

			RemoveFromAllGroups();

			IsDisposed = true;
			_isDisposing = false;
		}

#endregion

#region IUserNetScope Implementation

		public bool IsDisposed { get; private set; }

		public INetUser User { get; }

		public Task HandleAsync(ref ReadOnlySpan<byte> data, out OperationCode operationCode, out HandleResult handleResult)
		{
			ushort opCode = BinaryPrimitives.ReadUInt16LittleEndian(data);
			data = data[sizeof(ushort)..];

			operationCode = new OperationCode(opCode);
			if(!_handlers.TryGetValue(operationCode, out ServiceRecord record))
			{
				handleResult = HandleResult.InvalidOpCode;
				return Task.CompletedTask;
			}

			if(record.Flags.IsRpc())
			{
				handleResult = HandleResult.Procedure | HandleResult.Success;
				return record.Service.HandleRpcAsync(record.ServiceActionIdx, operationCode, ref data);
			}

			short requestNumber = BinaryPrimitives.ReadInt16LittleEndian(data);
			data = data[sizeof(ushort)..];

			if(requestNumber >= 0)
			{
				if(record.Service == null)
				{
					handleResult = HandleResult.ServiceNotFound | HandleResult.Request;
					return Task.CompletedTask;
				}

				handleResult = HandleResult.Request | HandleResult.Success;
				return record.Service.HandleRequestAsync(record.ServiceActionIdx, operationCode, requestNumber, ref data);
			}

			requestNumber = (short)(~requestNumber + 1);

			if(record.Client == null)
			{
				handleResult = HandleResult.ClientNotFound | HandleResult.Response;
				return Task.CompletedTask;
			}

			handleResult = HandleResult.Response | HandleResult.Success;
			record.Client.HandleResponseAsync(record.ClientActionIdx, operationCode, requestNumber, ref data);
			return Task.CompletedTask;
		}

#endregion

		public void AddGroup(INetGroup group)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			_userGroups.Add(group);
		}

		public bool RemoveGroup(INetGroup group)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			return _userGroups.Remove(group);
		}

		public void RegisterServiceHandler(OperationCode operationCode, INetworkService service, byte actionIdx, bool isRequest)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			if(!_handlers.TryGetValue(operationCode, out ServiceRecord record))
			{
				var nr = new ServiceRecord(service.ServiceId, isRequest ? RecordFlags.AsyncRequest : RecordFlags.AsyncProcedure);
				nr.SetService(service, actionIdx);
				_handlers.Add(operationCode, nr);
			}
			else
			{
				if(record.Service == null)
				{
					record.SetService(service, actionIdx);
				}
				else
				{
					throw new InvalidOperationException($"Service handler already registered for {operationCode}");
				}
			}
		}

		public bool RemoveServiceHandler(OperationCode operationCode, out INetworkService serviceRecord)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			if(!_handlers.TryGetValue(operationCode, out ServiceRecord record))
			{
				serviceRecord = null;
				return false;
			}

			if(record.Service == null)
			{
				serviceRecord = null;
				return false;
			}

			serviceRecord = record.Service;

			record.SetService(null, 0);

			if(record.Client == null)
			{
				_handlers.Remove(operationCode);
			}

			return true;
		}

		public void RegisterClientHandler(OperationCode operationCode, IServiceClient clientService, byte actionIdx)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			if(!_handlers.TryGetValue(operationCode, out ServiceRecord record))
			{
				var nr = new ServiceRecord(operationCode.ServiceId, RecordFlags.AsyncRequest);
				nr.SetClient(clientService, actionIdx);
				
				_handlers.Add(operationCode, nr);
			}
			else
			{
				if(record.Client == null)
				{
					if(!record.Flags.IsRequest())
					{
						throw new InvalidOperationException("Service handler already registered for non request response operation code.");
					}

					record.SetClient(clientService, actionIdx);
				}
				else
				{
					throw new InvalidOperationException($"Client handler already registered for {operationCode}");
				}
			}
		}

		public bool RemoveClientHandler(OperationCode operationCode, out IServiceClient serviceRecord)
		{
			if(IsDisposed)
			{
				throw new ObjectDisposedException(nameof(UserNetScope));
			}

			if(!_handlers.TryGetValue(operationCode, out ServiceRecord record))
			{
				serviceRecord = null;
				return false;
			}

			if(record.Client == null)
			{
				serviceRecord = null;
				return false;
			}

			serviceRecord = record.Client;

			record.SetClient(null, 0);

			if(record.Service == null)
			{
				_handlers.Remove(operationCode);
			}

			return true;
		}

		private void RemoveFromAllGroups()
		{
			for(int i = _userGroups.Count - 1; i >= 0; i--)
			{
				_userGroups[i].Remove(User);
			}

			_userGroups.Clear();
		}
	}
}