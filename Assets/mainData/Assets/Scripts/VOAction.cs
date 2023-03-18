using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlRoot("vo-action")]
public class VOAction
{
	[XmlRoot("input")]
	public class VOActionInput
	{
		[CompilerGenerated]
		private string _003CClass_003Ek__BackingField;

		[CompilerGenerated]
		private string[] _003CParameters_003Ek__BackingField;

		[XmlElement("class")]
		public string Class
		{
			[CompilerGenerated]
			get
			{
				return _003CClass_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CClass_003Ek__BackingField = value;
			}
		}

		[XmlArrayItem("param")]
		[XmlArray("params")]
		public string[] Parameters
		{
			[CompilerGenerated]
			get
			{
				return _003CParameters_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CParameters_003Ek__BackingField = value;
			}
		}
	}

	[XmlRoot("clip")]
	public class VOActionClip
	{
		[CompilerGenerated]
		private string _003CBundle_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CAsset_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CTextID_003Ek__BackingField;

		[XmlElement("bundle")]
		public string Bundle
		{
			[CompilerGenerated]
			get
			{
				return _003CBundle_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CBundle_003Ek__BackingField = value;
			}
		}

		[XmlElement("asset")]
		public string Asset
		{
			[CompilerGenerated]
			get
			{
				return _003CAsset_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CAsset_003Ek__BackingField = value;
			}
		}

		[XmlElement("text-id")]
		public string TextID
		{
			[CompilerGenerated]
			get
			{
				return _003CTextID_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CTextID_003Ek__BackingField = value;
			}
		}
	}

	[CompilerGenerated]
	private string _003CName_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CDisabled_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CLocalOnly_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CPlayOverUI_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CPlayOnUnowned_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CTemplatePrefab_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CSequenceProbability_003Ek__BackingField;

	[CompilerGenerated]
	private VOActionInput[] _003CInputs_003Ek__BackingField;

	[CompilerGenerated]
	private VOActionClip[] _003CClips_003Ek__BackingField;

	[CompilerGenerated]
	private VORoutingInfo _003CRouting_003Ek__BackingField;

	[XmlElement("name")]
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return _003CName_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CName_003Ek__BackingField = value;
		}
	}

	[XmlElement("disabled")]
	public bool Disabled
	{
		[CompilerGenerated]
		get
		{
			return _003CDisabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CDisabled_003Ek__BackingField = value;
		}
	}

	[XmlElement("local")]
	public bool LocalOnly
	{
		[CompilerGenerated]
		get
		{
			return _003CLocalOnly_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CLocalOnly_003Ek__BackingField = value;
		}
	}

	[XmlElement("play-over-ui")]
	public bool PlayOverUI
	{
		[CompilerGenerated]
		get
		{
			return _003CPlayOverUI_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CPlayOverUI_003Ek__BackingField = value;
		}
	}

	[XmlElement("play-on-unowned")]
	public bool PlayOnUnowned
	{
		[CompilerGenerated]
		get
		{
			return _003CPlayOnUnowned_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CPlayOnUnowned_003Ek__BackingField = value;
		}
	}

	[XmlElement("template-prefab")]
	public string TemplatePrefab
	{
		[CompilerGenerated]
		get
		{
			return _003CTemplatePrefab_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CTemplatePrefab_003Ek__BackingField = value;
		}
	}

	[XmlElement("sequence-probability")]
	public float SequenceProbability
	{
		[CompilerGenerated]
		get
		{
			return _003CSequenceProbability_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CSequenceProbability_003Ek__BackingField = value;
		}
	}

	[XmlArrayItem("input")]
	[XmlArray("inputs")]
	public VOActionInput[] Inputs
	{
		[CompilerGenerated]
		get
		{
			return _003CInputs_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CInputs_003Ek__BackingField = value;
		}
	}

	[XmlArrayItem("clip")]
	[XmlArray("clips")]
	public VOActionClip[] Clips
	{
		[CompilerGenerated]
		get
		{
			return _003CClips_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CClips_003Ek__BackingField = value;
		}
	}

	[XmlElement("routing-info")]
	public VORoutingInfo Routing
	{
		[CompilerGenerated]
		get
		{
			return _003CRouting_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CRouting_003Ek__BackingField = value;
		}
	}

	private VOAction()
	{
		SequenceProbability = -1f;
	}
}
