using System;
using System.Text.RegularExpressions;

namespace HandlerEngine.Utilities
{
	public readonly struct SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
	{
		private const string VersionRegExPattern = @"^([vV].?)?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$";

		private const string VersionPrefix = "v.";

		private readonly uint _compact;

		private SemanticVersion(
			uint major,
			uint minor,
			uint patch)
		{
			CompactSemVer(major, minor, patch, out _compact);
		}

		private SemanticVersion(uint compact)
		{
			_compact = compact;
		}

		public uint this[int index]
		{
			get
			{
				return index switch
				{
					0 => Major,
					1 => Minor,
					2 => Patch,
					_ => throw new IndexOutOfRangeException()
				};
			}
		}

		public uint Major => Compact >> 24;
		public uint Minor => (Compact >> 12) & 0xFFF;
		public uint Patch => Compact & 0xFFF;

		public uint Compact => _compact;

#region IComparable<SemanticVersion> Implementation

		public int CompareTo(SemanticVersion other)
		{
			return Compact.CompareTo(other.Compact);
		}

#endregion

#region IEquatable<SemanticVersion> Implementation

		public bool Equals(SemanticVersion other)
		{
			return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
		}

#endregion

		public static SemanticVersion SemVer(
			uint major,
			uint minor,
			uint patch)
		{
			return new SemanticVersion(major, minor, patch);
		}

		public static SemanticVersion SemVerCompact(uint compact)
		{
			return new SemanticVersion(compact);
		}

		public override int GetHashCode()
		{
			return (int)_compact;
		}

		public SemanticVersion ReplaceValue(SemanticVersionPart part, uint value)
		{
			return part switch
			{
				SemanticVersionPart.Major => new SemanticVersion(value, Minor, Patch),
				SemanticVersionPart.Minor => new SemanticVersion(Major, value, Patch),
				SemanticVersionPart.Patch => new SemanticVersion(Major, Minor, value),
				_ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
			};
		}

		public override bool Equals(object obj)
		{
			return obj is SemanticVersion other && Equals(other);
		}

		public override string ToString()
		{
			return $"{VersionPrefix}{Major}.{Minor}.{Patch}";
		}

		public static SemanticVersion Parse(string input)
		{
			if(TryParse(input, out SemanticVersion result))
			{
				return result;
			}

			throw new ArgumentException(
				$"Input string is not compatible to version pattern\"{VersionRegExPattern}\"",
				nameof(input)
			);
		}

		public static SemanticVersion Semantic(ushort major, ushort minor, ushort patch)
		{
			return new SemanticVersion(major, minor, patch);
		}

		public static bool TryParse(string input, out SemanticVersion value)
		{
			Match match = Regex.Match(input, VersionRegExPattern);

			Group majorGroup = match.Groups["major"];
			Group minorGroup = match.Groups["minor"];
			Group patchGroup = match.Groups["patch"];

			if(!majorGroup.Success || !ushort.TryParse(majorGroup.Value, out ushort major) ||
			   !minorGroup.Success || !ushort.TryParse(minorGroup.Value, out ushort minor) ||
			   !patchGroup.Success || !ushort.TryParse(patchGroup.Value, out ushort patch))
			{
				value = new SemanticVersion();
				return false;
			}

			value = new SemanticVersion(major, minor, patch);

			return true;
		}

		public static bool operator >(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) == 1;
		}

		public static bool operator <(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) == -1;
		}

		public static bool operator >=(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) >= 0;
		}

		public static bool operator <=(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) <= 0;
		}

		public static bool operator ==(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) == 0;
		}

		public static bool operator !=(SemanticVersion c1, SemanticVersion c2)
		{
			return c1.CompareTo(c2) != 0;
		}

		private static void CompactSemVer(uint major, uint minor, uint patch, out uint semVer)
		{
			if(patch > 0xFFF)
			{
				throw new ArgumentOutOfRangeException(nameof(patch), "Patch value must be less than 4096");
			}

			if(minor > 0xFFF)
			{
				throw new ArgumentOutOfRangeException(nameof(minor), "Minor value must be less than 4096");
			}

			if(major > 0xFF)
			{
				throw new ArgumentOutOfRangeException(nameof(major), "Major value must be less than 256");
			}

			semVer = (major << 24) | (minor << 12) | patch;
		}
	}
}