using System;

namespace HandlerEngine.Attributes
{
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class NetServiceAttribute : Attribute
	{
		public NetServiceAttribute(string uniqueId, uint version)
		{ }
	}
}