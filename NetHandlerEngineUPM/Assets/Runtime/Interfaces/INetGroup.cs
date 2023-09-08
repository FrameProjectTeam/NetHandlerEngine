using System.Collections.Generic;

namespace HandlerEngine.Interfaces
{
	public interface INetGroup : INetRecipient
	{
		event RecipientGroupHandlers.ClearEventHandler Cleared;
		event RecipientGroupHandlers.RecipientEventHandler Added;
		event RecipientGroupHandlers.RecipientEventHandler Removed;
		int Id { get; }

		IReadOnlyList<INetUser> Recipients { get; }

		void Add(INetUser client);
		bool Remove(INetUser client);

		bool Has(INetUser client);

		void Clear();
	}
}