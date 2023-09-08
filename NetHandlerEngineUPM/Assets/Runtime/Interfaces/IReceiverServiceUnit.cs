using System.Collections.Generic;

namespace HandlerEngine.Interfaces
{
	public interface IReceiverServiceUnit : IServiceUnit
	{
		IEnumerable<ServiceActionDescription> Actions();
	}
}