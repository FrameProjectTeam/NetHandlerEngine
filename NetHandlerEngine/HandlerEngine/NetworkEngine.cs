using System;
using System.Collections.Generic;

using HandlerEngine.Interfaces;

namespace HandlerEngine
{
	public sealed class NetworkEngine : INetworkEngine
	{
		private readonly Dictionary<int, UserNetScope> _userScopes = new();
		private readonly Dictionary<int, INetGroup> _groups = new();
		
#region IClientProvider Implementation

		public T CreateClient<T>(INetUser user, byte serviceId)
			where T : IServiceClient, new()
		{
			var clientService = new T();

			BindClient(user, serviceId, clientService);

			return clientService;
		}
		
		public void BindClient<T>(INetUser user, byte serviceId, T instance)
			where T : IServiceClient
		{
			UserNetScope scope = GetUserScopeInternal(user);

			foreach(ServiceActionDescription desc in instance.Actions())
			{
				scope.RegisterClientHandler(new OperationCode(serviceId, desc.PackageId), instance, desc.ActionIdx);
			}

			instance.Bind(user, serviceId);
		}

#endregion

#region IClientScopeProvider Implementation

		public IUserNetScope CreateScope(INetUser user)
		{
			if(_userScopes.Remove(user.Id, out UserNetScope oldScope))
			{
				if(!oldScope.IsDisposed)
				{
#if DEBUG
					throw new InvalidOperationException($"Old scope is not disposed for user {user}");
#else
					oldScope.Dispose();
#endif
				}
			}

			var scope = new UserNetScope(user);
			_userScopes.Add(user.Id, scope);

			return scope;
		}

		public bool DisposeScope(INetUser user, out IUserNetScope scope)
		{
			if(_userScopes.Remove(user.Id, out UserNetScope internalScope))
			{
				internalScope.Dispose();
				scope = internalScope;

				return true;
			}

			scope = null;
			return false;
		}

#endregion

#region INetEngine Implementation

		public T CreateBroadcastClient<T>(INetRecipient target, byte serviceId)
			where T : IBroadcastServiceClient, new()
		{
			var userService = new T();
			userService.Bind(target, serviceId);
			return userService;
		}

#endregion

#region INetGroupProvider Implementation

		public INetGroup CreateGroup()
		{
			//TODO: Remove group if needed, but now it is not needed
			var group = new RecipientGroup(_groups.Count);
			_groups.Add(group.Id, group);

			group.Added += OnGroupAdded;
			group.Removed += OnGroupRemoved;
			group.Cleared += OnGroupCleared;

			return group;
		}

#endregion

#region IServicesProvider Implementation

		public TService CreateService<TService>(INetUser user, byte serviceId)
			where TService : INetworkService, new()
		{
			UserNetScope scope = GetUserScopeInternal(user);

			var service = new TService();
			foreach(ServiceActionDescription desc in service.Actions())
			{
				scope.RegisterServiceHandler(new OperationCode(serviceId, desc.PackageId), service, desc.ActionIdx, desc.IsRequestResponse);
			}

			service.Bind(user, serviceId);
			return service;
		}

		public void BindService(INetUser user, byte serviceId, INetworkService instance)
		{
			UserNetScope scope = GetUserScopeInternal(user);

			foreach(ServiceActionDescription desc in instance.Actions())
			{
				scope.RegisterServiceHandler(new OperationCode(serviceId, desc.PackageId), instance, desc.ActionIdx, desc.IsRequestResponse);
			}

			instance.Bind(user, serviceId);
		}

		public bool UnbindService(INetUser user, INetworkService service)
		{
			UserNetScope scope = GetUserScopeInternal(user);

			byte serviceId = service.ServiceId;

			var skipped = 0;
			var dropped = 0;

			foreach(ServiceActionDescription desc in service.Actions())
			{
				//TODO: Theoretically, we can't skip any package but i'll check it.
				if(!scope.RemoveServiceHandler(new OperationCode(serviceId, desc.PackageId), out INetworkService registeredService))
				{
					skipped++;
					continue;
				}

				//Validate service
				if(!ReferenceEquals(registeredService, service))
				{
					throw new InvalidOperationException("Service in scope is not equal to service in argument");
				}

				dropped++;
			}

			bool isDropped = dropped > 0;

			if(skipped > 0 && isDropped)
			{
				throw new InvalidOperationException("Some packages were skipped and some were dropped. This is not allowed. Something went wrong.");
			}

			return isDropped;
		}

#endregion

		private void OnGroupCleared(INetGroup group, ArraySegment<INetUser> recipients)
		{
			foreach(INetUser recipient in recipients)
			{
				GetUserScopeInternal(recipient).RemoveGroup(group);
			}
		}

		private void OnGroupRemoved(INetGroup group, INetUser user)
		{
			GetUserScopeInternal(user).RemoveGroup(group);
		}

		private void OnGroupAdded(INetGroup group, INetUser user)
		{
			GetUserScopeInternal(user).AddGroup(group);
		}

		private UserNetScope GetUserScopeInternal(INetUser user)
		{
			if(_userScopes.TryGetValue(user.Id, out UserNetScope scope))
			{
				return scope;
			}

			throw new InvalidOperationException("User scope not found");
		}
	}
}