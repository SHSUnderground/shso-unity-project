using System.Runtime.CompilerServices;

public class MissionManifestEntry
{
	public enum MissionManifestEntryTypeEnum
	{
		Purchase,
		Free,
		WorkInProgress
	}

	[CompilerGenerated]
	private string _003CTypeId_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CMissionKey_003Ek__BackingField;

	public string TypeId
	{
		[CompilerGenerated]
		get
		{
			return _003CTypeId_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CTypeId_003Ek__BackingField = value;
		}
	}

	public string MissionKey
	{
		[CompilerGenerated]
		get
		{
			return _003CMissionKey_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CMissionKey_003Ek__BackingField = value;
		}
	}

	public MissionManifestEntryTypeEnum MissionType
	{
		get
		{
			int num = int.Parse(TypeId);
			if (num == 0)
			{
				return MissionManifestEntryTypeEnum.Free;
			}
			if (num < 0)
			{
				return MissionManifestEntryTypeEnum.WorkInProgress;
			}
			return MissionManifestEntryTypeEnum.Purchase;
		}
	}

	public MissionManifestEntry(MissionManifestEntryJson json)
	{
		TypeId = json.ownable_type_id.ToString();
		MissionKey = json.name.Trim();
	}
}
