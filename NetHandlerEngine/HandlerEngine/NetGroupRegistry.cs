using System.Collections.Generic;

using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	public sealed class NetGroupRegistry<TKey> : INetGroupRegistry<TKey>
		where TKey : unmanaged
	{
		private readonly INetGroupProvider _groupProvider;

		private readonly Dictionary<TKey, INetGroup> _groups;

		public NetGroupRegistry(INetGroupProvider groupProvider)
		{
			_groupProvider = groupProvider;
			_groups = new Dictionary<TKey, INetGroup>();
		}

#region INetGroupRegistry<TKey> Implementation

		public bool GetGroup(TKey key, out INetGroup group)
		{
			if(_groups.TryGetValue(key, out group))
			{
				return false;
			}

			group = _groupProvider.CreateGroup();
			_groups.Add(key, group);

			return true;
		}

		public bool HasGroup(TKey key)
		{
			return _groups.ContainsKey(key);
		}

#endregion
	}
}