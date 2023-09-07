using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	public interface INetGroupRegistry<in TKey>
		where TKey : unmanaged
	{
		bool GetGroup(TKey key, out INetGroup group);

		bool HasGroup(TKey key);
	}
}