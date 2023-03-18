using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

public class VOActionDataManager : Singleton<VOActionDataManager>
{
	[XmlRoot("vo-action-data")]
	public class VOActionDataDefinition : StaticDataDefinition, IStaticDataDefinitionTxt
	{
		[CompilerGenerated]
		private VOAction[] _003CActions_003Ek__BackingField;

		[XmlArray("vo-actions")]
		[XmlArrayItem("vo-action")]
		public VOAction[] Actions
		{
			[CompilerGenerated]
			get
			{
				return _003CActions_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CActions_003Ek__BackingField = value;
			}
		}

		public void InitializeFromData(string xml)
		{
			Actions = Utils.XmlDeserialize<VOActionDataDefinition>(xml).Actions;
		}
	}

	public const string VO_ACTION_DATA_PATH = "Audio/vo_actions";

	protected static VOActionDataDefinition voActionData = new VOActionDataDefinition();

	protected TransactionMonitor loadTransaction;

	[CompilerGenerated]
	private bool _003CDataIsLoaded_003Ek__BackingField;

	[CompilerGenerated]
	private Dictionary<string, VOAction> _003CVOActions_003Ek__BackingField;

	public bool DataIsLoaded
	{
		[CompilerGenerated]
		get
		{
			return _003CDataIsLoaded_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CDataIsLoaded_003Ek__BackingField = value;
		}
	}

	public Dictionary<string, VOAction> VOActions
	{
		[CompilerGenerated]
		get
		{
			return _003CVOActions_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CVOActions_003Ek__BackingField = value;
		}
	}

	public VOActionDataManager()
	{
		DataIsLoaded = false;
		VOActions = new Dictionary<string, VOAction>();
	}

	public void LoadVOActionData(TransactionMonitor transactionParent, string stepName)
	{
		loadTransaction = transactionParent;
		if (loadTransaction != null && !string.IsNullOrEmpty(stepName))
		{
			loadTransaction.AddStep(stepName, TransactionMonitor.DumpTransactionStatus);
		}
		AppShell.Instance.DataManager.LoadGameData("Audio/vo_actions", OnVOActionDataLoaded, voActionData, stepName);
	}

	protected void OnVOActionDataLoaded(GameDataLoadResponse response, object extraData)
	{
		string text = extraData as string;
		if (string.IsNullOrEmpty(response.Error))
		{
			VOAction[] actions = (response.DataDefinition as VOActionDataDefinition).Actions;
			foreach (VOAction vOAction in actions)
			{
				VOActions.Add(vOAction.Name, vOAction);
			}
			DataIsLoaded = true;
			if (loadTransaction != null && !string.IsNullOrEmpty(text))
			{
				loadTransaction.CompleteStep(text);
			}
		}
		else if (loadTransaction != null && !string.IsNullOrEmpty(text))
		{
			loadTransaction.FailStep(text, response.Error);
		}
	}
}
