using System;

namespace HandlerEngine.Serialization
{
	public static class HandlerEngineSerializer
	{
		public static IDeserializer Deserializer { get; private set; }
		public static ISerializer Serializer { get; private set; }

		public static bool Initialized { get; private set; }

		public static void InitWrapper(IDeserializer deserializer, ISerializer serializer)
		{
			if(Initialized)
			{
				throw new InvalidOperationException("Already initialized");
			}
			
			Deserializer = deserializer;
			Serializer = serializer;
			
			Initialized = true;
		}

#if UNITY_EDITOR
		
		[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void OnLoadReset()
		{
			Initialized = false;
			Serializer = null;
			Deserializer = null;
		}
		
#endif
	}
}