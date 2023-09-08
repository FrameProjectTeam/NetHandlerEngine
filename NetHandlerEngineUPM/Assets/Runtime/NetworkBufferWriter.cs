using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace HandlerEngine
{
	public interface INetworkBuffer : IBufferWriter<byte>
	{
		int WrittenLength { get; }
		bool IsEmpty { get; }

		ReadOnlySpan<byte> WrittenSpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		ReadOnlyMemory<byte> WrittenMemory
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		ArraySegment<byte> RawWrittenSegment
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		int FreeCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		void Reset(bool clear = false);
		void EnsureCapacity(int appendLength);
	}
	
	public sealed class NetworkBufferWriter : INetworkBuffer, IDisposable
	{
		internal const int MinLen = 16;
		internal const int MaxLen = ushort.MaxValue;

		private byte[] _buffer;

		private NetworkBufferWriter(int initialCapacity)
		{
			_buffer = new byte[initialCapacity];
		}

		public int WrittenLength { get; private set; }

		public bool IsEmpty => WrittenLength == 0;

		public bool IsDisposed => WrittenLength == -1;

		public ReadOnlySpan<byte> WrittenSpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				CheckDisposed();
				return new Span<byte>(_buffer, 0, WrittenLength);
			}
		}

		public ReadOnlyMemory<byte> WrittenMemory
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				CheckDisposed();
				return _buffer.AsMemory(0, WrittenLength);
			}
		}

		public ArraySegment<byte> RawWrittenSegment
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				CheckDisposed();
				return new ArraySegment<byte>(_buffer, 0, WrittenLength);
			}
		}

		public int FreeCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				CheckDisposed();
				return _buffer.Length - checked((int)(uint)WrittenLength);
			}
		}

#region IBufferWriter<byte> Implementation

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Advance(int count)
		{
			CheckDisposed();
			WrittenLength += count;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Memory<byte> GetMemory(int sizeHint = 0)
		{
			EnsureCapacity(sizeHint);
			return _buffer.AsMemory(WrittenLength);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<byte> GetSpan(int sizeHint = 0)
		{
			EnsureCapacity(sizeHint);
			return _buffer.AsSpan(WrittenLength);
		}

#endregion

#region IDisposable Implementation

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ArraySegment<byte> WrittenSegment(int offset)
		{
			CheckDisposed();

			return new ArraySegment<byte>(_buffer, offset, WrittenLength - offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset(bool clear = false)
		{
			WrittenLength = 0;
			if(clear)
			{
				Array.Clear(_buffer, 0, _buffer.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NetworkBufferWriter Create(int capacityHint = 0)
		{
			var buffer = new NetworkBufferWriter(capacityHint)
			{
				WrittenLength = 0
			};

			if(capacityHint > buffer.FreeCapacity)
			{
				buffer.EnsureCapacity(capacityHint);
			}

			return buffer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureCapacity(int appendLength)
		{
			// we will get a huge number when _offset == -1 and resize path checks for disposed
			uint newLength = unchecked((uint)WrittenLength + (uint)appendLength);
			var current = (uint)_buffer.Length;
			if(newLength > current)
			{
				EnsureCapacityResize(appendLength);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Dispose(bool disposing)
		{
			WrittenLength = -1;

			if(disposing)
			{
				if(_buffer.Length > MinLen)
				{
					_buffer = default!;
				}

				_buffer = null!;
			}
			else
			{
				_buffer = default!;
			}
		}

		private uint NextPowerOf2(uint v)
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
			return v;
		}

		private void EnsureCapacityResize(int appendLength)
		{
			CheckDisposed();

			var newLength = (int)NextPowerOf2((uint)(WrittenLength + appendLength));
			if(newLength < MinLen)
			{
				newLength = MinLen;
			}

			if(newLength > MaxLen)
			{
				if(WrittenLength + appendLength > MaxLen)
				{
					throw new NotSupportedException("_offset + appendLength > MaxLen");
				}

				newLength = MaxLen;
			}

			// Instead of providing options here we have ways
			// to customize behavior of the temp pool.
			Array.Resize(ref _buffer, newLength);
		}

		private void CheckDisposed()
		{
			if(WrittenLength < 0)
			{
				throw new ObjectDisposedException("Buffer writer disposed");
			}
		}

		// at any point the number of active writers should be ~CPU count
		~NetworkBufferWriter()
		{
			Dispose(false);
		}
	}
}