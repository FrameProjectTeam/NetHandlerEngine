using System;

namespace HandlerEngine.Interfaces
{
	public static class RecipientGroupHandlers
	{
		public delegate void ClearEventHandler(INetGroup group, ArraySegment<INetUser> recipients);

		public delegate void RecipientEventHandler(INetGroup group, INetUser user);
	}
}