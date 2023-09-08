using System;
using System.Runtime.InteropServices;

#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
using System.Runtime.CompilerServices;
#endif

namespace HandlerEngine
{
	[StructLayout(LayoutKind.Explicit, Size = 2, Pack = 2)]
	public readonly struct OperationCode : IEquatable<OperationCode>
	{
		[FieldOffset(0)]
		public readonly byte ServiceId;

		[FieldOffset(1)]
		public readonly byte PackageId;

		[FieldOffset(0)]
		public readonly ushort OpCode;

		public OperationCode(ushort opCode)
		{
#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
			Unsafe.SkipInit(out ServiceId);
			Unsafe.SkipInit(out PackageId);
#else
			ServiceId = 0;
			PackageId = 0;
#endif
			OpCode = opCode;
		}

		public OperationCode(byte serviceId, byte packageId)
		{
#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
			Unsafe.SkipInit(out OpCode);
#else
			OpCode = 0;
#endif
			ServiceId = serviceId;
			PackageId = packageId;
		}

#region IEquatable<OperationCode> Implementation

		public bool Equals(OperationCode other)
		{
			return OpCode == other.OpCode;
		}

#endregion

		public override bool Equals(object obj)
		{
			return obj is OperationCode other && Equals(other);
		}

		public override int GetHashCode()
		{
			return OpCode.GetHashCode();
		}

		public static bool operator ==(OperationCode left, OperationCode right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(OperationCode left, OperationCode right)
		{
			return !left.Equals(right);
		}
	}
}