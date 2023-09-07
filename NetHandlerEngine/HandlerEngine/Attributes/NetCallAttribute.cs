using System;

namespace HandlerEngine.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class NetCallAttribute : Attribute
	{
		public NetCallAttribute(byte packageId)
		{
			PackageId = packageId;
		}

		public byte PackageId { get; }

		public ChannelType ChannelType { get; set; }
		public byte ChannelId { get; set; }
	}
}