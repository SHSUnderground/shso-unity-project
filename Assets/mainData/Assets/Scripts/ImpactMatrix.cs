using System;

public class ImpactMatrix
{
	public enum Type
	{
		Inherit,
		Flesh,
		Metal,
		Energy,
		Silent
	}

	public delegate void ImpactMatrixLoaded(ImpactMatrix instance);

	public static readonly int TYPECOUNT = Enum.GetValues(typeof(Type)).Length;

	private string[,] matrix = new string[TYPECOUNT, TYPECOUNT];

	private ImpactMatrixLoaded onLoad;

	public static Type Resolve(Type toResolve, Type defaultType)
	{
		if (toResolve == Type.Inherit && defaultType == Type.Inherit)
		{
			return Type.Silent;
		}
		if (toResolve == Type.Inherit)
		{
			return defaultType;
		}
		return toResolve;
	}

	public string GetEffect(Type attackType, Type receiveType)
	{
		return matrix[(int)attackType, (int)receiveType];
	}

	public void Load(string filename, ImpactMatrixLoaded onImpactMatrixLoaded)
	{
		onLoad = onImpactMatrixLoaded;
		AppShell.Instance.DataManager.LoadGameData("ImpactData/" + filename, OnLoaded, null);
	}

	private void OnLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		DataWarehouse data = response.Data.GetData("impact_matrix");
		for (int i = 0; i < TYPECOUNT; i++)
		{
			for (int j = 0; j < TYPECOUNT; j++)
			{
				string str = Enum.ToObject(typeof(Type), i).ToString().ToLower();
				string str2 = Enum.ToObject(typeof(Type), j).ToString().ToLower();
				string tagPath = str + "/" + str2;
				matrix[i, j] = data.TryGetString(tagPath, null);
			}
		}
		if (onLoad != null)
		{
			onLoad(this);
		}
	}
}
