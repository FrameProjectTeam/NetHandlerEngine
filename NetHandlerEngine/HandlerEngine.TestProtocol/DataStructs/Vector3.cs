using MemoryPack;

namespace HandlerEngine.TestProtocol.DataStructs
{
	[MemoryPackable]
	public partial struct Vector3
	{
		public float x;
		public float y;
		public float z;
	}
	
	[MemoryPackable]
	public partial struct Vector4
	{
		public float x;
		public float y;
		public float z;
		public float w;
	}
	
		
	[MemoryPackable]
	public partial struct SomeStruct
	{
		public Vector3 Point { get; set; }
		public bool IsTrue { get; set; }
		public string Text { get; set; }
	}

	[MemoryPackable]
	public partial class SomeData
	{
		public List<Vector4> Points { get; set; }
	}
}