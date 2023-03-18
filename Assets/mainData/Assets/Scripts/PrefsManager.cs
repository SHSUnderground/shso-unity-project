using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefsManager
{
	public enum PrefType
	{
		FavoriteMissions = 1
	}

	public class PrefData
	{
		public PrefType type;

		private string value = string.Empty;

		public PrefData(PrefType type)
		{
			this.type = type;
		}

		public PrefData(PrefType type, string value)
		{
			this.type = type;
			this.value = value;
		}

		public virtual void setValue(string value)
		{
			this.value = value;
			savePref(this);
		}

		public virtual string getValue()
		{
			return value;
		}
	}

	public class FavoriteMissionPref : PrefData
	{
		private List<int> _favoriteMissionIDs = new List<int>();

		public FavoriteMissionPref()
			: base(PrefType.FavoriteMissions)
		{
		}

		public FavoriteMissionPref(PlayerPrefItem basePref)
			: base(PrefType.FavoriteMissions)
		{
			string[] array = basePref.value.Split(',');
			foreach (string text in array)
			{
				if (!(text == string.Empty))
				{
					_favoriteMissionIDs.Add(Convert.ToInt32(text));
				}
			}
		}

		public List<int> getFavoriteMissions()
		{
			return _favoriteMissionIDs;
		}

		public void addFavoriteMission(int newMissionID)
		{
			if (!_favoriteMissionIDs.Contains(newMissionID))
			{
				_favoriteMissionIDs.Add(newMissionID);
				savePref(this);
			}
		}

		public void removeFavoriteMission(int newMissionID)
		{
			if (_favoriteMissionIDs.Contains(newMissionID))
			{
				_favoriteMissionIDs.Remove(newMissionID);
				savePref(this);
			}
		}

		public override string getValue()
		{
			string text = string.Empty;
			foreach (int favoriteMissionID in _favoriteMissionIDs)
			{
				text = text + favoriteMissionID + ",";
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}
	}

	public class PlayerPrefItem : ShsCollectionItem
	{
		public int prefID;

		public string value;

		public override bool InitializeFromData(DataWarehouse data)
		{
			prefID = data.TryGetInt("pref_id", 0);
			value = data.TryGetString("value", string.Empty);
			return true;
		}

		public override string GetKey()
		{
			return string.Empty + prefID;
		}

		public override void UpdateFromData(DataWarehouse data)
		{
		}
	}

	public class PlayerPrefCollection : ShsCollectionBase<PlayerPrefItem>
	{
		protected const string ELEMENT_NAME = "pref";

		protected const string KEY_NAME = "pref_id";

		protected bool readOnly;

		public PlayerPrefCollection()
		{
			collectionElementName = "pref";
			keyName = "pref_id";
		}

		public PlayerPrefCollection(DataWarehouse xmlData)
			: this()
		{
			xmlData.Parse();
			InitializeFromData(xmlData);
		}
	}

	private static Dictionary<PrefType, PrefData> _prefs = new Dictionary<PrefType, PrefData>();

	public static void 	init(DataWarehouse data)
	{
		_prefs = new Dictionary<PrefType, PrefData>();
		if (data != null)
		{
			PlayerPrefCollection playerPrefCollection = new PlayerPrefCollection(data);
			foreach (PlayerPrefItem value in playerPrefCollection.Values)
			{
				int prefID = value.prefID;
				if (prefID == 1)
				{
					_prefs.Add(PrefType.FavoriteMissions, new FavoriteMissionPref(value));
				}
			}
		}
		if (!_prefs.ContainsKey(PrefType.FavoriteMissions))
		{
			_prefs.Add(PrefType.FavoriteMissions, new FavoriteMissionPref());
		}
	}

	public static List<int> getFavoriteMissions()
	{
		return (_prefs[PrefType.FavoriteMissions] as FavoriteMissionPref).getFavoriteMissions();
	}

	public static void addFavoriteMission(int newMissionID)
	{
		(_prefs[PrefType.FavoriteMissions] as FavoriteMissionPref).addFavoriteMission(newMissionID);
	}

	public static void removeFavoriteMission(int newMissionID)
	{
		(_prefs[PrefType.FavoriteMissions] as FavoriteMissionPref).removeFavoriteMission(newMissionID);
	}

	private static void savePref(PrefData pref)
	{
		CspUtils.DebugLog("savePref " + pref.type + " " + pref.getValue());
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("pref_id", (int)pref.type);
		wWWForm.AddField("value", pref.getValue());
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/set-pref/", delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("savePref failure: " + response.Status + ":" + response.Body);
			}
			else
			{
				CspUtils.DebugLog("savePref success: " + response.Status + ":" + response.Body);
			}
		}, wWWForm.data);
	}
}
