using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
			Unsafe.SkipInit(out ServiceId);
			Unsafe.SkipInit(out PackageId);
			OpCode = opCode;
		}

		public OperationCode(byte serviceId, byte packageId)
		{
			Unsafe.SkipInit(out OpCode);
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