using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class PlayerStatusDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public class Status
	{
		public string name;

		public string icon;

		public string emote;

		public Status()
		{
		}

		public Status(string name, string icon, string emote)
		{
			this.name = name;
			this.icon = icon;
			this.emote = emote;
		}

		public void SerializeToBinary(ShsSerializer.ShsWriter writer)
		{
			writer.Write(name);
		}

		public static Status DeserializeFromBinary(ShsSerializer.ShsReader reader)
		{
			string text = reader.ReadString();
			return Instance.GetStatus(text);
		}
	}

	public static readonly string DefaultStatusName = "default";

	public static readonly string DefaultStatusEmote = "think";

	protected Dictionary<string, Status> statuses = new Dictionary<string, Status>();

	protected TransactionMonitor loadTransaction;

	[CompilerGenerated]
	private static PlayerStatusDefinition _003CInstance_003Ek__BackingField;

	public static PlayerStatusDefinition Instance
	{
		[CompilerGenerated]
		get
		{
			return _003CInstance_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CInstance_003Ek__BackingField = value;
		}
	}

	public PlayerStatusDefinition(TransactionMonitor transaction)
	{
		loadTransaction = transaction;
		transaction.AddStep("playerstatuses");
		statuses.Add(DefaultStatusName, new Status(DefaultStatusName, string.Empty, DefaultStatusEmote));
	}

	public Status GetStatus(string name)
	{
		Status value = null;
		statuses.TryGetValue(name.ToLower(), out value);
		return value;
	}

	public static void OnDataLoaded(GameDataLoadResponse response, object extraData)
	{
		PlayerStatusDefinition playerStatusDefinition = response.DataDefinition as PlayerStatusDefinition;
		if (!string.IsNullOrEmpty(response.Error))
		{
			playerStatusDefinition.loadTransaction.FailStep("playerstatuses", response.Error);
			Instance = null;
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
		}
		else
		{
			Instance = playerStatusDefinition;
			playerStatusDefinition.loadTransaction.CompleteStep("playerstatuses");
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		foreach (DataWarehouse item in data.GetIterator("//status"))
		{
			Status status = new Status();
			status.name = item.GetString("name").ToLower();
			status.icon = item.GetString("icon");
			status.emote = item.GetString("emote");
			statuses.Add(status.name, status);
		}
	}
}
