using LitJson;
using System.Collections.Generic;

public class BlueprintManager
{
	private static Dictionary<int, Blueprint> _allBlueprints = new Dictionary<int, Blueprint>();

	private static List<Blueprint> _realBlueprints = new List<Blueprint>();

	private static List<Blueprint> _everyBlueprint = new List<Blueprint>();

	private static Dictionary<int, Blueprint> _blueprintsByOwnableOutput = new Dictionary<int, Blueprint>();

	public static bool blueprintsLoaded = false;

	private static bool openCraftAfterLoad = false;

	private static int openCraftInitialOwnableID = -1;

	public static void parseBlueprints(string blueprintList)
	{
		BlueprintMasterJsonData blueprintMasterJsonData = JsonMapper.ToObject<BlueprintMasterJsonData>(blueprintList);
		IEnumerable<BlueprintJsonData> blueprints = blueprintMasterJsonData.blueprints;
		foreach (BlueprintJsonData item in blueprints)
		{
			Blueprint blueprint = new Blueprint(item);
			_allBlueprints.Add(item.id, blueprint);
			_everyBlueprint.Add(blueprint);
			if (blueprint.partsOnly == 0)
			{
				_realBlueprints.Add(blueprint);
			}
			_blueprintsByOwnableOutput.Add(item.output_id, blueprint);
		}
		IEnumerable<BlueprintPieceJsonData> pieces = blueprintMasterJsonData.pieces;
		foreach (BlueprintPieceJsonData item2 in pieces)
		{
			if (_allBlueprints.ContainsKey(item2.sid))
			{
				Blueprint blueprint = _allBlueprints[item2.sid];
				blueprint.addPiece(new BlueprintPiece(item2));
			}
			else
			{
				CspUtils.DebugLog("Found a blueprint piece whose master blueprint does not exist.  Piece ID is " + item2.id + " scavenger blueprint ID is " + item2.sid);
			}
		}
		blueprintsLoaded = true;
		if (openCraftAfterLoad)
		{
			CraftingWindow.requestCraftingWindow(openCraftInitialOwnableID);
		}
	}

	public static void loadBlueprints(bool openCraftWhenComplete, int initialOwnableID = -1)
	{
		openCraftAfterLoad = openCraftWhenComplete;
		openCraftInitialOwnableID = initialOwnableID;
		AppShell.Instance.ServerConnection.LoadBlueprints();
	}

	public static List<Blueprint> getBlueprints(bool includeParts = false)
	{
		if (!includeParts)
		{
			return _realBlueprints;
		}
		return _everyBlueprint;
	}

	public static Blueprint getBlueprint(int blueprintID)
	{
		return _allBlueprints[blueprintID];
	}

	public static Blueprint getBlueprintFromOutput(int ownableTypeID)
	{
		if (_blueprintsByOwnableOutput.ContainsKey(ownableTypeID))
		{
			return _blueprintsByOwnableOutput[ownableTypeID];
		}
		return null;
	}
}
