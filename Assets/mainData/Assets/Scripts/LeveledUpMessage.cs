using System.Collections;
using System.Runtime.CompilerServices;

public class LeveledUpMessage : ShsEventMessage
{
	[CompilerGenerated]
	private string _003CHero_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CNewLevel_003Ek__BackingField;

	[CompilerGenerated]
	private string _003COwnableTypeId_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CHeroCurrentXp_003Ek__BackingField;

	public string Hero
	{
		[CompilerGenerated]
		get
		{
			return _003CHero_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CHero_003Ek__BackingField = value;
		}
	}

	public int NewLevel
	{
		[CompilerGenerated]
		get
		{
			return _003CNewLevel_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CNewLevel_003Ek__BackingField = value;
		}
	}

	public string OwnableTypeId
	{
		[CompilerGenerated]
		get
		{
			return _003COwnableTypeId_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003COwnableTypeId_003Ek__BackingField = value;
		}
	}

	public int HeroCurrentXp
	{
		[CompilerGenerated]
		get
		{
			return _003CHeroCurrentXp_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CHeroCurrentXp_003Ek__BackingField = value;
		}
	}

	public LeveledUpMessage(Hashtable payload)
	{
		if (payload.ContainsKey("hero_name"))
		{
			Hero = payload["hero_name"].ToString();
		}
		if (payload.ContainsKey("new_level"))
		{
			NewLevel = int.Parse(payload["new_level"].ToString());
		}
		if (payload.ContainsKey("ownable_type_id"))
		{
			OwnableTypeId = payload["ownable_type_id"].ToString();
		}
		if (payload.ContainsKey("hero_current_xp"))
		{
			int result;
			if (!int.TryParse(payload["hero_current_xp"].ToString(), out result))
			{
				CspUtils.DebugLog("hero_current_xp not int format.");
				HeroCurrentXp = -1;
			}
			HeroCurrentXp = result;
		}
		else
		{
			CspUtils.DebugLog("Level up message does not include hero_current_xp");
			HeroCurrentXp = -1;
		}
	}

	public override string ToString()
	{
		return string.Format("Hero: {0}, NewLevel: {1}, OwnableId: {2}, Xp: {3}", Hero, NewLevel, OwnableTypeId, HeroCurrentXp);
	}
}
