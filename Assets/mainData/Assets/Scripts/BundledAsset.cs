using System.Runtime.CompilerServices;

public class BundledAsset
{
	[CompilerGenerated]
	private string _003CBundle_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CAsset_003Ek__BackingField;

	public string Bundle
	{
		[CompilerGenerated]
		get
		{
			return _003CBundle_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CBundle_003Ek__BackingField = value;
		}
	}

	public string Asset
	{
		[CompilerGenerated]
		get
		{
			return _003CAsset_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CAsset_003Ek__BackingField = value;
		}
	}

	public BundledAsset(string bundle, string asset)
	{
		Bundle = bundle;
		Asset = asset;
	}
}
