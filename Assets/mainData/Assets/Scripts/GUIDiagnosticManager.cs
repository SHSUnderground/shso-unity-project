using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GUIDiagnosticManager
{
	public class DiagTextureInfo
	{
		public int index;

		public string textureName;

		public TextureFormat format;

		public DateTime timeLoaded;

		public float bytes;

		public Vector2 size;

		public bool bundled;

		public int loadCount = 1;
	}

	public class DiagUpdateInfo
	{
		public enum UpdateModeEnum
		{
			Inactive,
			Active
		}

		public class UpdateInfoSet
		{
			public double minUpdateTime;

			public double maxUpdateTime;

			public double maxSameFrameUpdate = 1.0;

			public double avgUpdateTime;

			public double lastUpdateStart;

			public double lastUpdateEnd;

			public int lastUpdateFrame;

			public int sameFrameUpdateAccum;

			public int updateCount = 1;
		}

		public string path;

		public UpdateModeEnum currentMode;

		public Dictionary<UpdateModeEnum, UpdateInfoSet> updateSetDict;

		public DiagUpdateInfo()
		{
			updateSetDict = new Dictionary<UpdateModeEnum, UpdateInfoSet>();
			updateSetDict[UpdateModeEnum.Inactive] = new UpdateInfoSet();
			updateSetDict[UpdateModeEnum.Active] = new UpdateInfoSet();
		}
	}

	[Flags]
	public enum DiagOptionsEnum
	{
		TextureSize = 0x1,
		UpdateCount = 0x2,
		All = 0x1111
	}

	[CompilerGenerated]
	private bool _003CEnableLabelBoundsChecking_003Ek__BackingField;

	public bool EnableLabelBoundsChecking
	{
		[CompilerGenerated]
		get
		{
			return _003CEnableLabelBoundsChecking_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CEnableLabelBoundsChecking_003Ek__BackingField = value;
		}
	}

	public GUIDiagnosticManager(GUIManager manager)
	{
		EnableLabelBoundsChecking = (PlayerPrefs.GetInt("lbChecking", 0) == 1);
	}

	public void Update()
	{
	}

	public void TextureLoaded(string texturePath, Texture2D texture)
	{
	}

	public void GUIControlBeginUpdate(IGUIControl control)
	{
	}

	public void GUIControlEndUpdate(IGUIControl control)
	{
	}

	public string LogTextureReport()
	{
		return null;
	}

	public string LogUpdateReport()
	{
		return null;
	}

	private string getFileName(string prefix, string extension)
	{
		DateTime now = DateTime.Now;
		return prefix + "_" + now.ToString("yyyy") + "_" + now.ToString("MM") + "_" + now.ToString("dd") + "_" + now.ToString("hh") + "_" + now.ToString("mm") + "_" + now.ToString("ss") + "." + extension;
	}

	public void GUIAssetComparison()
	{
	}

	public void GUIAssetComparisonCumulative()
	{
	}
}
