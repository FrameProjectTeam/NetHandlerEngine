using System;
using System.Collections.Generic;

using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	internal sealed class RecipientGroup : INetGroup
	{
		private readonly Dictionary<int, int> _recipientIndex = new();
		private INetUser[] _recipients = Array.Empty<INetUser>();

		private int _count;

		public RecipientGroup(int id)
		{
			Id = id;
		}

#region INetGroup Implementation

		public event RecipientGroupHandlers.ClearEventHandler Cleared;
		public event RecipientGroupHandlers.RecipientEventHandler Added;
		public event RecipientGroupHandlers.RecipientEventHandler Removed;
		public int Id { get; }

		public IReadOnlyList<INetUser> Recipients => new ArraySegment<INetUser>(_recipients, 0, _count).ToArray();

		public void Add(INetUser client)
		{
			if(_recipients.Length == _count)
			{
				Array.Resize(ref _recipients, _recipients.Length + 1);
			}

			_recipientIndex.Add(client.Id, _count);
			_recipients[_count++] = client;

			OnAdded(this, client);
		}

		public bool Remove(INetUser client)
		{
			if(!_recipientIndex.Remove(client.Id, out int idx))
			{
				return false;
			}

			_count--;
			if(idx == _count)
			{
				_recipients[idx] = null;
			}
			else
			{
				//TODO: Perhaps it is worth leaving the order as it was and not shuffling users, or adding an optimization parameter, i.e. shuffling can be expensive

				INetUser moveUser = _recipients[_count];
				_recipientIndex[moveUser.Id] = idx;

				_recipients[idx] = moveUser;
				_recipients[_count] = default;
			}

			OnRemoved(this, client);
			return true;
		}

		public bool Has(INetUser client)
		{
			return _recipientIndex.ContainsKey(client.Id);
		}

		public void Clear()
		{
			_recipientIndex.Clear();

			int count = _count;
			INetUser[] recipients = _recipients;

			_recipients = Array.Empty<INetUser>();
			_count = 0;

			OnCleared(this, new ArraySegment<INetUser>(recipients, 0, count));
		}

#endregion

#region INetRecipient Implementation

		public void Send(INetworkBuffer buffer, ChannelType channelType, byte channelId)
		{
			for(var i = 0; i < _count; i++)
			{
				_recipients[i].Send(buffer, channelType, channelId);
			}
		}

#endregion

		private void OnAdded(INetGroup group, INetUser user)
		{
			Added?.Invoke(group, user);
		}

		private void OnRemoved(INetGroup group, INetUser user)
		{
			Removed?.Invoke(group, user);
		}

		private void OnCleared(INetGroup group, ArraySegment<INetUser> recipients)
		{
			Cleared?.Invoke(group, recipients);
		}
	}
}