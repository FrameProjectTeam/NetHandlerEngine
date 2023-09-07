using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis;

namespace HandlerEngine.SourceGenerator.CollectableData;

public static class CollectableExtensions
{
	public const string RetValueName = "retValue";

	private static readonly StringBuilder _sb = new();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasRpc(this ServiceInfo info)
	{
		return info.MethodInfos.Any(methodInfo => methodInfo.IsRpc());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsRpc(this MethodInfo methodInfo)
	{
		return methodInfo.ReturnType == "void" && !methodInfo.Parameters.Any(m => m.Specifier is RefKind.Out);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ResponseMethodInfo ToResponse(this MethodInfo mi)
	{
		bool returnTypeIsVoid = mi.ReturnType.Equals("void");
		List<MethodArgumentInfo> outParameters = new();
		List<MethodArgumentInfo> inParameters = new();

		if(!returnTypeIsVoid)
		{
			outParameters.Add(new MethodArgumentInfo(RefKind.Out, mi.ReturnType, RetValueName));
		}

		foreach(MethodArgumentInfo parameterInfo in mi.Parameters)
		{
			if(parameterInfo.Specifier is RefKind.Out)
			{
				outParameters.Add(parameterInfo);
			}
			else
			{
				inParameters.Add(parameterInfo);
			}
		}

		string returnType = outParameters.Count > 1
			? $"({string.Join(", ", outParameters.Select(a => $"{a.Type} {a.Name}"))})"
			: $"{outParameters[0].Type}";

		return new ResponseMethodInfo(mi.Name, returnType, mi.CallAttributeInfo, inParameters.ToArray(), outParameters.ToArray(), outParameters.Count > 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Inline(this IEnumerable<MethodArgumentInfo> mai, bool withSpecifier = false, bool withType = true)
	{
		try
		{
			var count = 0;
			foreach(MethodArgumentInfo info in mai)
			{
				if(count++ > 0)
				{
					_sb.Append(", ");
				}

				if(withSpecifier && info.Specifier != RefKind.None)
				{
					_sb.Append(info.Specifier.Inline());
					_sb.Append(" ");
				}

				if(withType)
				{
					_sb.Append(info.Type);
					_sb.Append(" ");
				}

				_sb.Append(info.Name);
			}

			return _sb.ToString();
		}
		finally
		{
			_sb.Clear();
		}
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string InlineChanelType(this byte channelType)
	{
		const string EnumName = "ChannelType";

		return channelType switch
		{
			0 => $"{EnumName}.ReliableUnordered",
			1 => $"{EnumName}.Sequenced",
			2 => $"{EnumName}.ReliableOrdered",
			3 => $"{EnumName}.ReliableSequenced",
			4 => $"{EnumName}.Unreliable",
			_ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, null)
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string Inline(this RefKind refKind)
	{
		return refKind switch
		{
			RefKind.Ref => "ref",
			RefKind.Out => "out",
			RefKind.In => "in",
			_ => ""
		};
	}
}