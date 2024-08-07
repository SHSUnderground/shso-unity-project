using System.Runtime.CompilerServices;

public class StartExpendableEffectMessage : ShsEventMessage
{
	[CompilerGenerated]
	private int _003CUserId_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CEffectData_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CExpendableId_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CCanceled_003Ek__BackingField;

	public int UserId
	{
		[CompilerGenerated]
		get
		{
			return _003CUserId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CUserId_003Ek__BackingField = value;
		}
	}

	public string EffectData
	{
		[CompilerGenerated]
		get
		{
			return _003CEffectData_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CEffectData_003Ek__BackingField = value;
		}
	}

	public string ExpendableId
	{
		[CompilerGenerated]
		get
		{
			return _003CExpendableId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CExpendableId_003Ek__BackingField = value;
		}
	}

	public bool Canceled
	{
		[CompilerGenerated]
		get
		{
			return _003CCanceled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CCanceled_003Ek__BackingField = value;
		}
	}

	public StartExpendableEffectMessage(int userId, string effectData, string expendableId)
	{
		UserId = userId;
		EffectData = effectData;
		ExpendableId = expendableId;
	}

	public bool AppliesTo(string characterName)
	{
		return true;
	}
}
