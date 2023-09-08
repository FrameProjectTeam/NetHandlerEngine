using System;

#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
using System.Runtime.CompilerServices;
#endif

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
#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
			return Unsafe.As<TEnum, TUnderlying>(ref enumValue);
#else
			return (TUnderlying)(object)enumValue;
#endif
		}

		public static TUnderlying To(ref TEnum enumValue)
		{
#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
			return Unsafe.As<TEnum, TUnderlying>(ref enumValue);
#else
			return (TUnderlying)(object)enumValue;
#endif
		}

		public static TEnum From(TUnderlying underlyingValue)
		{
#if NET5_0_OR_GREATER || HE_ENABLE_UNSAFE
			return Unsafe.As<TUnderlying, TEnum>(ref underlyingValue);
#else
			return (TEnum)(object)underlyingValue;
#endif
		}
	}
}