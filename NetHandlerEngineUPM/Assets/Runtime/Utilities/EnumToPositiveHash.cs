using System;
using System.Collections.Generic;

namespace HandlerEngine.Utilities
{
	internal static class EnumToPositiveHash<TEnum>
		where TEnum : Enum
	{
		// ReSharper disable once StaticMemberInGenericType
		private static readonly (TEnum value, string key, int hash)[] _hashArray;

		static EnumToPositiveHash()
		{
			Array values = Enum.GetValues(typeof(TEnum));
			_hashArray = new (TEnum, string, int)[values.Length];

			var index = 0;
			foreach(TEnum item in values)
			{
				_hashArray[index] = (item, item.ToString(), HashUtility.PositiveHash(item.ToString()));
				index++;
			}
		}

		public static (TEnum, string, int) GetValue(int index)
		{
			return _hashArray[index];
		}

		public static (TEnum, string, int) GetValue(TEnum value)
		{
			for(var i = 0; i < _hashArray.Length; i++)
			{
				if(EqualityComparer<TEnum>.Default.Equals(_hashArray[i].value, value))
				{
					return _hashArray[i];
				}
			}

			return default;
		}

		public static void GetValue(TEnum value, out string key, out int hash)
		{
			for(var j = 0; j < _hashArray.Length; j++)
			{
				if(EqualityComparer<TEnum>.Default.Equals(_hashArray[j].value, value))
				{
					key = _hashArray[j].key;
					hash = _hashArray[j].hash;
					return;
				}
			}

			key = default;
			hash = default;
		}
	}
}