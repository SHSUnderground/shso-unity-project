using System.Runtime.CompilerServices;

public class VOHooks
{
	public delegate void ResolvedVOPlayedDelegate(ResolvedVOAction action);

	[CompilerGenerated]
	private ResolvedVOPlayedDelegate _003COnResolvedVOPlayed_003Ek__BackingField;

	public ResolvedVOPlayedDelegate OnResolvedVOPlayed
	{
		[CompilerGenerated]
		get
		{
			return _003COnResolvedVOPlayed_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003COnResolvedVOPlayed_003Ek__BackingField = value;
		}
	}

	public void ClearDelegates()
	{
		OnResolvedVOPlayed = null;
	}

	public void ResolvedVOPlayed(ResolvedVOAction action)
	{
		if (OnResolvedVOPlayed != null)
		{
			OnResolvedVOPlayed(action);
		}
	}
}
