using System;
using System.Runtime.CompilerServices;

namespace HandlerEngine.Utilities
{
	public static class EnumConverter<TEnum, TUnderlying>
		where TEnum : Enum
		where TUnderlying : unmanaged
	{
		static EnumConverter()
		{
			Type enumType = typeof(TEnum);
			Type underlyingType = typeof(TUnderlying);

			if(Enum.GetUnderlyingType(enumType) != underlyingType)
			{
				throw new InvalidOperationException($"The underlying type of the enum {enumType} is not {underlyingType}.");
			}
		}

		public static TUnderlying To(TEnum enumValue)
		{
			return Unsafe.As<TEnum, TUnderlying>(ref enumValue);
		}

		public static TUnderlying To(ref TEnum enumValue)
		{
			return Unsafe.As<TEnum, TUnderlying>(ref enumValue);
		}

		public static TEnum From(TUnderlying underlyingValue)
		{
			return Unsafe.As<TUnderlying, TEnum>(ref underlyingValue);
		}
	}
}