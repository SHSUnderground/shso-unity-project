using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class ExpendablesManager
{
	public delegate void ExpendHandlerCompleteCallback(IExpendHandler handler);

	public const int REQUEST_INVALID = -1;

	public const string DEFAULT_PREREQUISITE_METHOD = "ExpendablesPrerequisiteHandlerDefault";

	private const float PERSIST_REFRESH_TIME = 60f;

	public static ExpendablesManager instance;

	public static int RequestId = 1;

	public readonly Dictionary<int, IExpendHandler> activeExpendQueue;

	public readonly Dictionary<int, IExpendHandler> expendHistoryQueue;

	public readonly Dictionary<string, ExpendableDefinition> ExpendableTypes;

	public readonly Dictionary<string, ExpendHandlerMapping> HandlerMappings;

	private readonly Dictionary<string, ExpendablesPrerequisiteHandlerBase> PrerequisiteHandlers;

	private float nextExpendableStartTime;

	private readonly Queue<StartExpendableEffectMessage> pendingStartMessages;

	private float _lastPersistTime;

	public float NextExpendableStartTime
	{
		get
		{
			return nextExpendableStartTime;
		}
	}

	public ExpendablesManager()
	{
		ExpendableTypes = new Dictionary<string, ExpendableDefinition>();
		HandlerMappings = new Dictionary<string, ExpendHandlerMapping>();
		activeExpendQueue = new Dictionary<int, IExpendHandler>();
		expendHistoryQueue = new Dictionary<int, IExpendHandler>();
		PrerequisiteHandlers = new Dictionary<string, ExpendablesPrerequisiteHandlerBase>();
		AppShell.Instance.EventMgr.AddListener<ConsumedPotionMessage>(OnConsumedPotionMessage);
		AppShell.Instance.EventMgr.AddListener<StartExpendableEffectMessage>(OnStartEffectMessage);
		AppShell.Instance.EventMgr.AddListener<StopExpendableEffectMessage>(OnStopEffectMessage);
		pendingStartMessages = new Queue<StartExpendableEffectMessage>();
		_lastPersistTime = Time.time;
		instance = this;
	}

	~ExpendablesManager()
	{
		AppShell.Instance.EventMgr.RemoveListener<ConsumedPotionMessage>(OnConsumedPotionMessage);
		AppShell.Instance.EventMgr.RemoveListener<StartExpendableEffectMessage>(OnStartEffectMessage);
		AppShell.Instance.EventMgr.RemoveListener<StopExpendableEffectMessage>(OnStopEffectMessage);
	}

	public ExpendableDefinition FallbackDefinition(string ownableId)
	{
		ExpendableDefinition expendableDefinition = new ExpendableDefinition();
		expendableDefinition.Name = "Unknown";
		expendableDefinition.Description = "Unknown";
		expendableDefinition.InventoryIcon = string.Empty;
		expendableDefinition.HoverHelpIcon = string.Empty;
		expendableDefinition.ShoppingIcon = string.Empty;
		expendableDefinition.PreEffectHandler = "preeffect_fallback_handler";
		expendableDefinition.ExpendEffectHandler = "expend_fallback_handler";
		expendableDefinition.OwnableTypeId = ownableId;
		return expendableDefinition;
	}

	public void InitializeFromData(DataWarehouse xml)
	{
		XPathNavigator value = xml.GetValue("expendables");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("expendable", string.Empty);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExpendableDefinition));
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			ExpendableDefinition expendableDefinition = xmlSerializer.Deserialize(new StringReader(outerXml)) as ExpendableDefinition;
			if (expendableDefinition != null)
			{
				ExpendableTypes[expendableDefinition.OwnableTypeId] = expendableDefinition;
				string text = expendableDefinition.PrerequisiteMethod;
				if (string.IsNullOrEmpty(text))
				{
					expendableDefinition.PrerequisiteMethod = "ExpendablesPrerequisiteHandlerDefault";
					text = "ExpendablesPrerequisiteHandlerDefault";
				}
				if (!PrerequisiteHandlers.ContainsKey(text))
				{
					ExpendablesPrerequisiteHandlerBase value2 = createPrereqHandler(text);
					PrerequisiteHandlers.Add(text, value2);
				}
			}
		}
		XPathNavigator value3 = xml.GetValue("//expendables/handler_mappings");
		XPathNodeIterator xPathNodeIterator2 = value3.SelectChildren("handler_mapping", string.Empty);
		XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(ExpendHandlerMapping));
		while (xPathNodeIterator2.MoveNext())
		{
			string outerXml2 = xPathNodeIterator2.Current.OuterXml;
			ExpendHandlerMapping expendHandlerMapping = xmlSerializer2.Deserialize(new StringReader(outerXml2)) as ExpendHandlerMapping;
			if (expendHandlerMapping != null)
			{
				HandlerMappings[expendHandlerMapping.HandlerName] = expendHandlerMapping;
			}
		}
	}

	public bool hasActiveEffect(string effectName)
	{
		int[] array = new int[AppShell.Instance.ExpendablesManager.activeExpendQueue.Count];
		AppShell.Instance.ExpendablesManager.activeExpendQueue.Keys.CopyTo(array, 0);
		int[] array2 = array;
		foreach (int key in array2)
		{
			IExpendHandler expendHandler = AppShell.Instance.ExpendablesManager.activeExpendQueue[key];
			if (expendHandler.ExpendableDefinition != null && expendHandler.ExpendableDefinition.CombatEffects.Count > 0)
			{
				foreach (ExpendCombatEffect combatEffect in expendHandler.ExpendableDefinition.CombatEffects)
				{
					if (combatEffect.name == effectName)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void CanExpend(string[] ownables, ref Dictionary<string, PrerequisiteCheckResult> resultDictionary)
	{
		resultDictionary.Clear();
		foreach (string text in ownables)
		{
			resultDictionary.Add(text, CanExpend(text));
		}
	}

	public PrerequisiteCheckResult CanExpend(ExpendableDefinition expendableDefinition)
	{
		string prerequisiteMethod = expendableDefinition.PrerequisiteMethod;
		ExpendablesPrerequisiteHandlerBase value = null;
		if (!PrerequisiteHandlers.TryGetValue(prerequisiteMethod, out value))
		{
			PrerequisiteCheckResult result = default(PrerequisiteCheckResult);
			result.State = PrerequisiteCheckStateEnum.InternalStateInvalid;
			result.StateExplanation = "#EXP_INTERNAL_INVALID";
			return result;
		}
		return value.Check(this, expendableDefinition);
	}

	public PrerequisiteCheckResult CanExpend(string ownableTypeId)
	{
		ExpendableDefinition value = null;
		if (ExpendableTypes.TryGetValue(ownableTypeId, out value))
		{
			return CanExpend(value);
		}
		PrerequisiteCheckResult result = default(PrerequisiteCheckResult);
		result.State = PrerequisiteCheckStateEnum.InternalStateInvalid;
		result.StateExplanation = "#EXP_INTERNAL_INVALID";
		return result;
	}

	public int UseExpendable(string ownableTypeId)
	{
		return UseExpendable(ownableTypeId, AppShell.Instance.Profile.SelectedCostume, null);
	}

	public int UseExpendable(string ownableTypeId, ExpendHandlerCompleteCallback callback)
	{
		return UseExpendable(ownableTypeId, AppShell.Instance.Profile.SelectedCostume, callback);
	}

	public void Persist()
	{
		if (AppShell.Instance.Profile != null)
		{
			string uri = "resources$users/potion_persist.py";
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("shsoid", AppShell.Instance.ServerConnection.getNotificationServer().PlayerId);
			CspUtils.DebugLog("PLAYER ID: " + AppShell.Instance.ServerConnection.getNotificationServer().PlayerId);
			if ((bool)AppShell.Instance.WebService)
			{
				AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
				{
					CspUtils.DebugLog(response);
					if (response.Status != 200)
					{
						CspUtils.DebugLog("Persist Effect Request failure: " + response.Status + ":" + response.Body);
					}else {
										
						string msg = response.Body;
						string[] expiredPotions = msg.Split('|');
						
						foreach(var message in expiredPotions){
							if(message != null && message != ""){
								CspUtils.DebugLog("ATTEMPTING TO CLOSE EFFECT!");
								CspUtils.DebugLog(message);
								string[] potion = message.Split(',');
								string OwnableID =  potion[0].ToString();
								CancelEffect(OwnableID);
								CspUtils.DebugLog("Closed Effect!");
							}	
						}
						
						/*int num = int.Parse(array[2]);
						string text = array[3];
						string text2 = array[4];
						AppShell.Instance.EventMgr.Fire(this, new StopExpendableEffectMessage(num, text2, text));*/
					}
				}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
			}
		}
	}

	public void CancelEffect(string ownableTypeId)
	{
		string uri = "resources$users/potion_cancel";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("potion_id", ownableTypeId);
		wWWForm.AddField("request_id", ++RequestId);
		/*AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("Cancel Effect Request failure: " + response.Status + ":" + response.Body);
			}
			else
			{
				CspUtils.DebugLog("Cancel Effect Request success: " + response.Status + ":" + response.Body);
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);*/
		AppShell.Instance.EventReporter.CancelPotion(Int32.Parse(ownableTypeId), ++RequestId);
	}

	public int UseExpendable(string ownableTypeId, string heroname, ExpendHandlerCompleteCallback callback)
	{
		OwnableDefinition def = OwnableDefinition.getDef(int.Parse(ownableTypeId));
		if (def != null && def.category == OwnableDefinition.Category.MysteryBox)
		{
			foreach (KeyValuePair<string, MysteryBox> item in AppShell.Instance.Profile.MysteryBoxesCollection)
			{
				if (item.Value.Definition.ownableTypeID == int.Parse(ownableTypeId))
				{
					if (item.Value.Quantity > 0)
					{
						item.Value.Quantity--;
						break;
					}
					return 0;
				}
			}
			SHSMysteryBoxOpeningWindow dialogWindow = new SHSMysteryBoxOpeningWindow(def.ownableTypeID, def.name, null);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
			return 0;
		}
		if (!ExpendableTypes.ContainsKey(ownableTypeId))
		{
			CspUtils.DebugLog("Asked to expend type: " + ownableTypeId + ", but that type is not defined");
			return -1;
		}
		ExpendableDefinition expendableDefinition = ExpendableTypes[ownableTypeId];
		PrerequisiteCheckResult prerequisiteCheckResult = CanExpend(expendableDefinition);
		if (prerequisiteCheckResult.State != 0)
		{
			CspUtils.DebugLog("Expend check failed... " + prerequisiteCheckResult.StateExplanation);
			return -1;
		}
		if (string.IsNullOrEmpty(expendableDefinition.ExpendEffectHandler))
		{
			CspUtils.DebugLog("No handler assigned for: " + expendableDefinition.OwnableTypeId);
			return -1;
		}
		ExpendHandlerMapping value;
		if (!HandlerMappings.TryGetValue(expendableDefinition.ExpendEffectHandler, out value))
		{
			CspUtils.DebugLog("Can't map key: " + expendableDefinition.ExpendEffectHandler + " to a handler class");
			return -1;
		}
		IExpendHandler handler = instantiateHandler(value.HandlerClassName);
		if (handler == null)
		{
			CspUtils.DebugLog("Unable to create class:" + value.HandlerClassName);
			return -1;
		}
		if (!expendableDefinition.CategoryList.Contains(ExpendableDefinition.Categories.Internal))
		{
			BehaviorManager component = GameController.GetController().LocalPlayer.GetComponent<BehaviorManager>();
			component.requestChangeBehavior<BehaviorExpendablePrepare>(false);
		}
		int requestId = RequestId++;
		handler.Initialize(requestId, expendableDefinition, OnExpendableHandlerComplete, callback);
		activeExpendQueue[requestId] = handler;
		string text = "resources$users/potion.py";
		WWWForm wWWForm = new WWWForm();
		// wWWForm.AddField("user_id", AppShell.Instance.Profile.UserId.ToString());
		wWWForm.AddField("hero_name", heroname);
		wWWForm.AddField("potion_id", ownableTypeId);
		wWWForm.AddField("request_id", requestId);
		handler.LogExpendAction("Sending request: " + text + " with request id:" + requestId);
		//Using shso-events.as by Doggo
		/*AppShell.Instance.WebService.StartRequest(text, delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				BehaviorManager component2 = GameController.GetController().LocalPlayer.GetComponent<BehaviorManager>();
				component2.forceChangeBehavior(Type.GetType(component2.defaultBehaviorType));
				CspUtils.DebugLog("Expend Action Request failure: " + response.Status + ":" + response.Body);
				handler.OnExpendServerAuthorizationFailed(new ConsumedPotionMessage(false, response.Body, int.Parse(ownableTypeId), requestId, -1));
			}
		}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);*/
		AppShell.Instance.EventReporter.ConsumePotion(heroname, Int32.Parse(ownableTypeId),requestId);
		return requestId;
	}

	private void OnExpendableHandlerComplete(IExpendHandler expendHandler)
	{
		if (activeExpendQueue.ContainsKey(expendHandler.ExpendRequestId))
		{
			activeExpendQueue.Remove(expendHandler.ExpendRequestId);
			expendHistoryQueue.Add(expendHandler.ExpendRequestId, expendHandler);
			if (expendHandler.State == ExpendHandlerState.TimedOut || expendHandler.State == ExpendHandlerState.Failed)
			{
				BehaviorManager component = GameController.GetController().LocalPlayer.GetComponent<BehaviorManager>();
				component.forceChangeBehavior(Type.GetType(component.defaultBehaviorType));
			}
		}
		else
		{
			CspUtils.DebugLog("Handler with request ID:" + expendHandler.ExpendRequestId + " is not listed in the active queue");
		}
	}

	public void Update()
	{
		if (Time.time - _lastPersistTime >= 60f)
		{
			_lastPersistTime = Time.time;
			Persist();
		}
		if (activeExpendQueue.Count > 0)
		{
			int[] array = new int[activeExpendQueue.Count];
			activeExpendQueue.Keys.CopyTo(array, 0);
			int[] array2 = array;
			foreach (int key in array2)
			{
				IExpendHandler expendHandler = activeExpendQueue[key];
				expendHandler.Update();
			}
		}
		int count = pendingStartMessages.Count;
		for (int j = 0; j < count; j++)
		{
			StartExpendableEffectMessage startExpendableEffectMessage = pendingStartMessages.Dequeue();
			if (!startExpendableEffectMessage.Canceled)
			{
				OnStartEffectMessage(startExpendableEffectMessage);
			}
		}
	}

	private IExpendHandler instantiateHandler(string handlerName)
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type type = executingAssembly.GetType(handlerName, false, true);
		if (type == null)
		{
			CspUtils.DebugLog("Cant create class from name: " + handlerName);
			return null;
		}
		if (!typeof(IExpendHandler).IsAssignableFrom(type))
		{
			CspUtils.DebugLog("Type: " + handlerName + " does not implement the IChallenge interface.");
			return null;
		}
		IExpendHandler expendHandler = Activator.CreateInstance(type) as IExpendHandler;
		if (expendHandler == null)
		{
			CspUtils.DebugLog("Attempt to create instance of challenge " + handlerName + "failed");
			return null;
		}
		return expendHandler;
	}

	private ExpendablesPrerequisiteHandlerBase createPrereqHandler(string handlerName)
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type type = executingAssembly.GetType(handlerName, false, true);
		if (type == null)
		{
			CspUtils.DebugLog("Cant create class from name: " + handlerName);
			return null;
		}
		if (!typeof(ExpendablesPrerequisiteHandlerBase).IsAssignableFrom(type))
		{
			CspUtils.DebugLog("Type: " + handlerName + " does not inherit ExpendablesPrerequisiteHandlerBase.");
			return null;
		}
		ExpendablesPrerequisiteHandlerBase expendablesPrerequisiteHandlerBase = Activator.CreateInstance(type) as ExpendablesPrerequisiteHandlerBase;
		if (expendablesPrerequisiteHandlerBase == null)
		{
			CspUtils.DebugLog("Attempt to create instance of " + handlerName + "failed");
			return null;
		}
		return expendablesPrerequisiteHandlerBase;
	}

	private void OnConsumedPotionMessage(ConsumedPotionMessage message)
	{
		IExpendHandler expendHandler = null;
		foreach (IExpendHandler value in activeExpendQueue.Values)
		{
			if (value.ExpendRequestId == message.RequestId)
			{
				expendHandler = value;
			}
		}
		if (expendHandler != null)
		{
			if (message.Success)
			{
				expendHandler.OnExpendServerAuthorizationSuccess(message);
				nextExpendableStartTime = Time.time + (float)AppShell.Instance.ExpendablesManager.ExpendableTypes[message.OwnableTypeId.ToString()].Cooldown;
				ExpendableCollection expendablesCollection = AppShell.Instance.Profile.ExpendablesCollection;
				if (expendablesCollection != null && expendablesCollection.ContainsKey(message.OwnableTypeId.ToString()))
				{
					expendablesCollection[message.OwnableTypeId.ToString()].Quantity = message.PotionsRemaining;
					if (message.PotionsRemaining <= 0)
					{
						expendablesCollection.Remove(message.OwnableTypeId.ToString());
					}
				}
			}
			else
			{
				expendHandler.OnExpendServerAuthorizationFailed(message);
			}
		}
		else
		{
			CspUtils.DebugLog("Can't find corresponding handler for consumed potion: " + message.OwnableTypeId + " with request Id: " + message.RequestId);
		}
	}

	private void OnStartEffectMessage(StartExpendableEffectMessage message)
	{
		if (message == null)
		{
			CspUtils.DebugLog("EffectMessage == null");
			return;
		}
		if (AppShell.Instance == null)
		{
			CspUtils.DebugLog("AppShell.Instance is null? Not likely but...");
			return;
		}
		if (AppShell.Instance.ServerConnection == null)
		{
			CspUtils.DebugLog("AppShell.Instance.ServerConnection is null");
			return;
		}
		if (AppShell.Instance.ServerConnection.Game == null)
		{
			CspUtils.DebugLog("AppShell.Instance.ServerConnection.Game == null");
			return;
		}
		GameObject gameObjectFromNetId = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(new GoNetId(GoNetId.PLAYER_ID_FLAG, message.UserId));
		if (gameObjectFromNetId == null)
		{
			pendingStartMessages.Enqueue(message);
			return;
		}
		CharacterGlobals component = gameObjectFromNetId.GetComponent<CharacterGlobals>();
		if (component == null)
		{
			CspUtils.DebugLog(string.Format("Asked to start effect for SFS Player {0} but the players CharacterGlobals is null!", message.UserId));
			return;
		}
		if (!ExpendableTypes.ContainsKey(message.ExpendableId))
		{
			CspUtils.DebugLog(string.Format("Unknown ExpendableID {0} - no ExpendableDefinition to apply for this potion!", message.ExpendableId));
			return;
		}
		ExpendableDefinition expendableDefinition = ExpendableTypes[message.ExpendableId];
		if (component.definitionData.CharacterName == "mr_placeholder")
		{
			pendingStartMessages.Enqueue(message);
			return;
		}
		if (component.effectsList == null || !component.effectsList.Initialized || !component.effectsList.BundlesLoaded)
		{
			pendingStartMessages.Enqueue(message);
			return;
		}
		if (AppShell.Instance.ServerConnection.GetGameUserId() == message.UserId)
		{
			bool flag = false;
			foreach (KeyValuePair<int, IExpendHandler> item in activeExpendQueue)
			{
				if (item.Value.OwnableTypeId == message.ExpendableId)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ExpendHandlerMapping value;
				if (!HandlerMappings.TryGetValue(expendableDefinition.ExpendEffectHandler, out value))
				{
					CspUtils.DebugLog("Can't map key: " + expendableDefinition.ExpendEffectHandler + " to a handler class");
					return;
				}
				IExpendHandler expendHandler = instantiateHandler(value.HandlerClassName);
				if (expendHandler == null)
				{
					CspUtils.DebugLog("Unable to create class:" + value.HandlerClassName);
					return;
				}
				int num = RequestId++;
				expendHandler.Initialize(num, expendableDefinition, OnExpendableHandlerComplete, null);
				activeExpendQueue[num] = expendHandler;
			}
		}
		if (!message.AppliesTo(component.definitionData.CharacterName))
		{
			CspUtils.DebugLog(string.Format("Asked to start effect for SFS Player {0} but the player is not using the character {1}.  Player character is {2}", message.UserId, message.EffectData, component.definitionData.CharacterName));
			return;
		}
		BehaviorManager component2 = gameObjectFromNetId.GetComponent<BehaviorManager>();
		if (component2 == null)
		{
			CspUtils.DebugLog("Attempting to apply ExpendableDefinition but the player has no BehaviorManager defined!");
			return;
		}
		BehaviorEffectExpendable behaviorEffectExpendable = component2.requestChangeBehavior<BehaviorEffectExpendable>(false);
		if (behaviorEffectExpendable == null)
		{
			CspUtils.DebugLog("No BehaviorEffectExpendable defined for this potion.");
		}
		else
		{
			behaviorEffectExpendable.Initialize(expendableDefinition, null);
		}
	}

	private void OnStopEffectMessage(StopExpendableEffectMessage message)
	{
		CspUtils.DebugLog("OnStopEffectMessage");
		if (AppShell.Instance.ServerConnection.GetGameUserId() == message.UserId)
		{
			IExpendHandler expendHandler = null;
			foreach (KeyValuePair<int, IExpendHandler> item in activeExpendQueue)
			{
				if (item.Value.ExpendableDefinition.OwnableTypeId == message.ExpendableId)
				{
					expendHandler = item.Value;
					break;
				}
			}
			if (expendHandler != null)
			{
				ExpendHandlerBase expendHandlerBase = expendHandler as ExpendHandlerBase;
				if (expendHandlerBase != null)
				{
					expendHandlerBase.OnExpendComplete();
				}
				else
				{
					activeExpendQueue.Remove(expendHandler.ExpendRequestId);
					expendHistoryQueue.Add(expendHandler.ExpendRequestId, expendHandler);
				}
			}
		}
		foreach (StartExpendableEffectMessage pendingStartMessage in pendingStartMessages)
		{
			if (pendingStartMessage.UserId == message.UserId && pendingStartMessage.EffectData == message.EffectData && pendingStartMessage.ExpendableId == message.ExpendableId)
			{
				pendingStartMessage.Canceled = true;
				return;
			}
		}
		GameObject gameObjectFromNetId = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(new GoNetId(GoNetId.PLAYER_ID_FLAG, message.UserId));
		if (!(gameObjectFromNetId != null))
		{
			return;
		}
		CharacterGlobals component = gameObjectFromNetId.GetComponent<CharacterGlobals>();
		if (!(component != null) || !message.AppliesTo(component.definitionData.CharacterName) || !ExpendableTypes.ContainsKey(message.ExpendableId))
		{
			return;
		}
		ExpendableDefinition expendableDefinition = ExpendableTypes[message.ExpendableId];
		if (expendableDefinition.Parameters != null)
		{
			bool flag = false;
			bool flag2 = false;
			string[] array = null;
			float num = 1f;
			foreach (ExpendHandlerParameters parameter in expendableDefinition.Parameters)
			{
				switch (parameter.Key)
				{
				case "effect_sequence_persistent":
					array = parameter.Value;
					break;
				case "scaleplayer":
					flag = true;
					num = Convert.ToSingle(parameter.Value[0]);
					break;
				case "addController":
					flag2 = true;
					break;
				}
			}
			if (flag)
			{
				Transform transform = Utils.FindNodeInChildren(component.gameObject.transform, "export_node");
				if (transform != null)
				{
					transform.localScale = Vector3.one;
					Transform transform2 = component.gameObject.transform;
					Vector3 localPosition = component.gameObject.transform.localPosition;
					float x = localPosition.x;
					Vector3 localPosition2 = component.gameObject.transform.localPosition;
					float y = localPosition2.y + 0.6f;
					Vector3 localPosition3 = component.gameObject.transform.localPosition;
					transform2.localPosition = new Vector3(x, y, localPosition3.z);
				}
			}
			if (array != null)
			{
				EffectSequence[] components = Utils.GetComponents<EffectSequence>(gameObjectFromNetId, Utils.SearchChildren);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = text;
					if (text2.IndexOf("|") > 0)
					{
						text2 = text2.Substring(text2.IndexOf("|") + 1);
					}
					EffectSequence[] array3 = components;
					foreach (EffectSequence effectSequence in array3)
					{
						if (effectSequence.name.ToLowerInvariant().StartsWith(text2.ToLowerInvariant()))
						{
							effectSequence.StopSequence(true);
							break;
						}
					}
					if (text2 == "InvisibleWomanFadeController")
					{
						InvisibleWomanFadeController component2 = Utils.GetComponent<InvisibleWomanFadeController>(gameObjectFromNetId);
						if (component2 != null)
						{
							component2.Disconnect();
							UnityEngine.Object.Destroy(component2);
						}
					}
					else if (flag2)
					{
						CspUtils.DebugLog("OnStopEffectMessage trying to cancel " + text2);
						Type type = Type.GetType(text2);
						Component component3 = component.gameObject.GetComponent(type);
						if ((bool)component3)
						{
							UnityEngine.Object.Destroy(component3);
						}
					}
				}
			}
		}
		if (expendableDefinition.CombatEffects.Count > 0)
		{
			foreach (ExpendCombatEffect combatEffect in expendableDefinition.CombatEffects)
			{
				if (combatEffect.name == "BuffPotionSpeedIncrease")
				{
					CharacterMotionController component4 = gameObjectFromNetId.GetComponent<CharacterMotionController>();
					component4.removeSpeedMultiplier(1.5f);
					if (component.activeSidekick != null)
					{
						component4 = component.activeSidekick.motionController;
						component4.removeSpeedMultiplier(1.5f);
					}
				}
			}
		}
	}
}
