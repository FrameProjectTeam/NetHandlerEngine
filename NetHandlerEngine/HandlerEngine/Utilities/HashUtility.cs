namespace HandlerEngine.Utilities
{
	public static class HashUtility
	{
		public static int PositiveHash(string groupName)
		{
			return groupName.GetHashCode() & 0x7FFFFFFF;
		}
	}
}