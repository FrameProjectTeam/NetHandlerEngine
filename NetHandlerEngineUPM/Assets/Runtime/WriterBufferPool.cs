using System;

namespace HandlerEngine
{
	public static class WriterBufferPool
	{
		[ThreadStatic]
		private static NetworkBufferWriter s_buffer;

		public static NetworkBufferWriter Buffer
		{
			get
			{
				if(s_buffer == null)
				{
					s_buffer = NetworkBufferWriter.Create(512);
				}
				else
				{
					s_buffer.Reset();
				}

				return s_buffer;
			}
		}
	}
}