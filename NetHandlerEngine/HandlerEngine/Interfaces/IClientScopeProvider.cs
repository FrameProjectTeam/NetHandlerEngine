namespace HandlerEngine.Interfaces
{
	public interface IClientScopeProvider
	{
		IUserNetScope CreateScope(INetUser user);
		bool DisposeScope(INetUser user, out IUserNetScope scope);
	}
}