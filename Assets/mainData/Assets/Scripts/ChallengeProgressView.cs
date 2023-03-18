using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class ChallengeProgressView
{
	[XmlRoot(ElementName = "server_counters")]
	public class ServerCounterXml
	{
		[XmlRoot(ElementName = "server_counter")]
		public class ServerCounterInfo
		{
			public string counter_type;

			public long value;
		}

		[XmlElement("server_counter")]
		public ServerCounterInfo[] items;
	}

	public static void ShowSingleCheckbox(GUISimpleControlWindow window, ChallengeInfo info, bool largeWindow)
	{
		string label = "UnNamed";
		XmlNode xmlNode = info.DisplayNode["checkbox"];
		if (xmlNode != null)
		{
			label = xmlNode.InnerText;
		}
		SHSMySquadChallengeCheckboxContainer sHSMySquadChallengeCheckboxContainer = new SHSMySquadChallengeCheckboxContainer(new SHSMySquadChallengeCheckboxContainer.CheckboxInfo[1]
		{
			new SHSMySquadChallengeCheckboxContainer.CheckboxInfo(label, AppShell.Instance.ChallengeManager.IsChallengeCompleted(info.ChallengeId))
		}, window.Size, largeWindow);
		if (largeWindow)
		{
			sHSMySquadChallengeCheckboxContainer.SetPosition(GUIControl.DockingAlignmentEnum.BottomMiddle, GUIControl.AnchorAlignmentEnum.BottomMiddle, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f));
		}
		else
		{
			sHSMySquadChallengeCheckboxContainer.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
			sHSMySquadChallengeCheckboxContainer.SetSize(new Vector2(46f, 46f));
		}
		window.Add(sHSMySquadChallengeCheckboxContainer);
	}

	public static void ShowSquadLevelProgress(GUISimpleControlWindow window, ChallengeInfo info, bool largeWindow)
	{
		float currentValue = AppShell.Instance.Profile.SquadLevel;
		float maxValue = 0f;
		XmlNode xmlNode = info.DisplayNode["goal"];
		if (xmlNode != null)
		{
			maxValue = float.Parse(xmlNode.InnerText);
		}
		SHSMySquadChallengeProgressMeter control = BuildProgressMeter(largeWindow, currentValue, maxValue);
		window.Add(control);
	}

	public static void ShowServerCounterProgress(GUISimpleControlWindow window, ChallengeInfo info, bool largeWindow)
	{
		float maxValue = 0f;
		string counterName = null;
		XmlNode xmlNode = info.DisplayNode["goal"];
		if (xmlNode != null)
		{
			maxValue = float.Parse(xmlNode.InnerText);
		}
		XmlNode xmlNode2 = info.DisplayNode["counter_name"];
		if (xmlNode2 != null)
		{
			counterName = xmlNode2.InnerText;
		}
		SHSMySquadChallengeProgressMeter progressMeter = BuildProgressMeter(largeWindow, 0f, 0f);
		window.Add(progressMeter);
		if (counterName != null)
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("counter_type", counterName);
			AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/server_counters/", delegate(ShsWebResponse response)
			{
				if (response.Status == 200)
				{
					try
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerCounterXml));
						ServerCounterXml serverCounterXml = (ServerCounterXml)xmlSerializer.Deserialize(new StringReader(response.Body));
						if (serverCounterXml.items != null && serverCounterXml.items.Length > 0)
						{
							ServerCounterXml.ServerCounterInfo[] items = serverCounterXml.items;
							foreach (ServerCounterXml.ServerCounterInfo serverCounterInfo in items)
							{
								if (serverCounterInfo.counter_type == counterName)
								{
									progressMeter.SetValues(serverCounterInfo.value, maxValue);
									break;
								}
							}
						}
						else
						{
							progressMeter.SetValues(0f, 0f);
						}
					}
					catch (Exception message)
					{
						CspUtils.DebugLog(message);
					}
					CspUtils.DebugLog("Server Counters response: " + response.Body);
				}
			}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
		}
	}

	public static void ShowProgressMeter(GUISimpleControlWindow window, ChallengeInfo info, bool largeWindow)
	{
		float currentValue = 0f;
		float maxValue = 0f;
		if (AppShell.Instance.ChallengeManager.IsChallengeCompleted(info.ChallengeId))
		{
			currentValue = 1f;
			maxValue = 1f;
		}
		else if (AppShell.Instance.ChallengeManager.CurrentChallenge != null && info.ChallengeId == AppShell.Instance.ChallengeManager.CurrentChallenge.Id)
		{
			ChallengeCounter challengeCounter = AppShell.Instance.ChallengeManager.CurrentChallenge as ChallengeCounter;
			if (challengeCounter != null && challengeCounter.CounterGoal > 0)
			{
				currentValue = challengeCounter.CounterValue;
				maxValue = challengeCounter.CounterGoal;
			}
		}
		SHSMySquadChallengeProgressMeter control = BuildProgressMeter(largeWindow, currentValue, maxValue);
		window.Add(control);
	}

	protected static SHSMySquadChallengeProgressMeter BuildProgressMeter(bool large, float currentValue, float maxValue)
	{
		SHSMySquadChallengeProgressMeter sHSMySquadChallengeProgressMeter;
		if (large)
		{
			sHSMySquadChallengeProgressMeter = new SHSMySquadProgressMeterLarge();
			sHSMySquadChallengeProgressMeter.Id = "progressMeterLarge";
		}
		else
		{
			sHSMySquadChallengeProgressMeter = new SHSMySquadProgressMeterSmall();
			sHSMySquadChallengeProgressMeter.Id = "progressMeterSmall";
		}
		if (maxValue > 0f)
		{
			sHSMySquadChallengeProgressMeter.SetValues(currentValue, maxValue);
		}
		return sHSMySquadChallengeProgressMeter;
	}

	public static void ShowBitCounterMultiCheckBox(GUISimpleControlWindow window, ChallengeInfo info, bool largeWindow)
	{
		List<string> list = new List<string>();
		XmlNodeList xmlNodeList = info.DisplayNode.SelectNodes("//label");
		foreach (XmlNode item in xmlNodeList)
		{
			list.Add(item.InnerText);
		}
		if (list.Count <= 0)
		{
			return;
		}
		SHSMySquadChallengeCheckboxContainer.CheckboxInfo[] array = new SHSMySquadChallengeCheckboxContainer.CheckboxInfo[list.Count];
		if (AppShell.Instance.ChallengeManager.IsChallengeCompleted(info.ChallengeId))
		{
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = new SHSMySquadChallengeCheckboxContainer.CheckboxInfo(list[i], true);
			}
		}
		else if (AppShell.Instance.ChallengeManager.CurrentChallenge != null && info.ChallengeId == AppShell.Instance.ChallengeManager.CurrentChallenge.Id)
		{
			ChallengeBitCounter challengeBitCounter = AppShell.Instance.ChallengeManager.CurrentChallenge as ChallengeBitCounter;
			if (challengeBitCounter != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					byte bit = Convert.ToByte(j);
					bool check = challengeBitCounter.IsBitSet(bit);
					array[j] = new SHSMySquadChallengeCheckboxContainer.CheckboxInfo(list[j], check);
				}
			}
		}
		SHSMySquadChallengeCheckboxContainer sHSMySquadChallengeCheckboxContainer = new SHSMySquadChallengeCheckboxContainer(array, window.Size, largeWindow);
		if (largeWindow)
		{
			sHSMySquadChallengeCheckboxContainer.SetPosition(GUIControl.DockingAlignmentEnum.TopLeft, GUIControl.AnchorAlignmentEnum.TopLeft, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f));
		}
		else
		{
			sHSMySquadChallengeCheckboxContainer.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, sHSMySquadChallengeCheckboxContainer.Offset);
			sHSMySquadChallengeCheckboxContainer.SetSize(new Vector2(250f, 46f));
		}
		window.Add(sHSMySquadChallengeCheckboxContainer);
	}
}
