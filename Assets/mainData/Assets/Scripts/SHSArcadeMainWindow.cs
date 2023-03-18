using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SHSArcadeMainWindow : SHSGadget.GadgetCenterWindow
{
	public class HighScoreJson
	{
		public int rank;

		public string publicName = string.Empty;

		public int total;

		public int score;
	}

	private class BestScoreJson
	{
		public int rank;

		public string publicName = string.Empty;

		public int total;

		public int score;
	}

	private SHSPlayerHighScoreWindow highScoreWindow;

	private SHSLeaderBoardWindow leaderboardWindow;

	private SHSArcadePlayInstructions instructionsWindow;

	private SHSArcadeSliderWindow sliderWindow;

	private bool updateFlag;

	private float updateDelay;

	private Dictionary<string, List<HighScoreJson>> cachedHighScoreJson;

	private Dictionary<string, BestScoreJson> cachedBestScoreJson;

	private static int RequestId;

	private static int PersonalRequestId;

	public SHSArcadeMainWindow(SHSArcadeGadget gadget)
	{
		sliderWindow = new SHSArcadeSliderWindow();
		sliderWindow.Id = "sliderWindow";
		sliderWindow.SelectedItemChanged += sliderWindow_SelectedItemChanged;
		Add(sliderWindow);
		highScoreWindow = GUIControl.CreateControlBottomFrameCentered<SHSPlayerHighScoreWindow>(new Vector2(172f, 141f), new Vector2(-290f, -189f));
		highScoreWindow.Id = "highScoreWindow";
		Add(highScoreWindow);
		leaderboardWindow = GUIControl.CreateControlBottomFrameCentered<SHSLeaderBoardWindow>(new Vector2(333f, 150f), new Vector2(-21f, -175f));
		leaderboardWindow.Id = "leaderboardWindow";
		Add(leaderboardWindow);
		instructionsWindow = GUIControl.CreateControlBottomFrameCentered<SHSArcadePlayInstructions>(new Vector2(215f, 364f), new Vector2(287f, -305f));
		instructionsWindow.Id = "instructionWindow";
		Add(instructionsWindow);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Id = "playButton";
		gUIButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_mshs_newspaper_button_play");
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -50f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += delegate
		{
			AppShell.Instance.QueueLocationInfo();
			string keyword = sliderWindow.SelectedItem.Game.Keyword;
			AppShell.Instance.SharedHashTable["AracdeGame"] = keyword;
			CspUtils.DebugLog("Launching: " + keyword);
			AppShell.Instance.Transition(GameController.ControllerType.ArcadeShell);
		};
		Add(gUIButton);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Id = "quitButton";
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_arcade_launcher_quit_button");
		gUIButton2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(146f, -40f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.Click += delegate
		{
			gadget.CloseGadget();
		};
		Add(gUIButton2);
		cachedHighScoreJson = new Dictionary<string, List<HighScoreJson>>();
		cachedBestScoreJson = new Dictionary<string, BestScoreJson>();
	}

	private void sliderWindow_SelectedItemChanged()
	{
		updateFlag = true;
		highScoreWindow.FadeOut();
		leaderboardWindow.FadeOut();
		instructionsWindow.FadeOut();
	}

	public override void OnUpdate()
	{
		if (updateFlag && !sliderWindow.slider.IsDragging && !sliderWindow.SlidingAnimationInProgress)
		{
			updateFlag = false;
			instructionsWindow.FadeIn();
			RefreshArcadeGameData();
		}
		base.OnUpdate();
	}

	private void RefreshArcadeGameData()
	{
		ArcadeGame game = sliderWindow.SelectedItem.Game;
		string gameKey = game.Keyword;
		string empty = string.Empty;
		instructionsWindow.UpdateInstructions(game.HelpImage, game.Description);
		if (cachedBestScoreJson.ContainsKey(gameKey))
		{
			UpdateBestScore(cachedBestScoreJson[gameKey]);
		}
		else
		{
			int personalrequestId = ++PersonalRequestId;
			empty = "arcade$highscore/" + gameKey + "/?a=" + AppShell.Instance.Profile.UserId;
			AppShell.Instance.WebService.StartRequest(empty, delegate(ShsWebResponse response)
			{
				OnPlayerUrl(response, gameKey, personalrequestId);
			}, null, ShsWebService.ShsWebServiceType.Text);
		}
		if (cachedHighScoreJson.ContainsKey(gameKey))
		{
			UpdateArcadeScores(cachedHighScoreJson[gameKey]);
			return;
		}
		int requestId = ++RequestId;
		empty = "arcade$highscore/" + gameKey + "/top5";
		AppShell.Instance.WebService.StartRequest(empty, delegate(ShsWebResponse response)
		{
			OnGameUrl(response, gameKey, requestId);
		}, null, ShsWebService.ShsWebServiceType.Text);
	}

	private void OnPlayerUrl(ShsWebResponse response, string gameKey, int requestId)
	{
		//Discarded unreachable code: IL_0097
		if (requestId == PersonalRequestId)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("Cant get game high scores. Reason:" + response.Status + ":" + response.Body);
				return;
			}
			try
			{
				BestScoreJson value = JsonMapper.ToObject<BestScoreJson>(response.Body);
				cachedBestScoreJson[gameKey] = value;
			}
			catch (Exception)
			{
				CspUtils.DebugLog("couldn't parse best score:" + response.Body);
				return;
			}
			UpdateBestScore(cachedBestScoreJson[gameKey]);
		}
	}

	private void OnGameUrl(ShsWebResponse response, string gameKey, int requestId)
	{
		//Discarded unreachable code: IL_00b7
		if (requestId == RequestId)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("Cant get game high scores. Reason:" + response.Status + ":" + response.Body);
				CspUtils.DebugLog(response.Body);
			}
			else
			{
				List<HighScoreJson> list;
				try
				{
					list = JsonMapper.ToObject<List<HighScoreJson>>(response.Body);
					cachedHighScoreJson[gameKey] = list;
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog("Couldn't parse high score list:" + response.Body);
					CspUtils.DebugLog(ex.Message);
					return;
				}
				UpdateArcadeScores(list);
			}
		}
	}

	private void UpdateArcadeScores(IEnumerable<HighScoreJson> jsonDict)
	{
		leaderboardWindow.ClearPlayerSlots();
		IOrderedEnumerable<HighScoreJson> orderedEnumerable = Enumerable.OrderBy<HighScoreJson, int>(jsonDict, delegate(HighScoreJson highscoreplayer)
		{
			return highscoreplayer.rank;
		});
		foreach (HighScoreJson item in orderedEnumerable)
		{
			leaderboardWindow.UpdatePlayerSlot(item.rank - 1, item.publicName, item.score);
		}
		leaderboardWindow.FadeIn();
	}

	private void UpdateBestScore(BestScoreJson jsonScore)
	{
		highScoreWindow.UpdateBestScore(jsonScore.score);
		highScoreWindow.FadeIn();
	}
}
