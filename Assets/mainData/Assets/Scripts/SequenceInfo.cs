using System.Xml.Serialization;
using UnityEngine;

[XmlRoot(ElementName = "sequence")]
public class SequenceInfo
{
	protected string sequenceName;

	protected string sequencePrefabName;

	protected string sequenceAssetBundle;

	protected GameObject prefab;

	[XmlElement(ElementName = "sequence_name")]
	public string SequenceName
	{
		get
		{
			return sequenceName;
		}
		set
		{
			sequenceName = value;
		}
	}

	[XmlElement(ElementName = "sequence_prefab")]
	public string SequencePrefabName
	{
		get
		{
			return sequencePrefabName;
		}
		set
		{
			string[] array = value.Split('|');
			if (array.Length > 1)
			{
				sequenceAssetBundle = array[0];
				sequencePrefabName = array[1];
			}
			else
			{
				sequencePrefabName = array[0];
			}
		}
	}

	[XmlIgnore]
	public string SequenceAssetBundle
	{
		get
		{
			return sequenceAssetBundle;
		}
	}

	[XmlIgnore]
	public GameObject Prefab
	{
		get
		{
			if (prefab == null && HqController2.Instance != null && sequenceAssetBundle != null)
			{
				AssetBundle assetBundle = HqController2.Instance.GetAssetBundle(sequenceAssetBundle);
				if (assetBundle != null)
				{
					prefab = (assetBundle.Load(sequencePrefabName) as GameObject);
				}
			}
			return prefab;
		}
	}
}
