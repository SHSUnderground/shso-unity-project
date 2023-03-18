using UnityEngine;

public class SHSBrawlerObjectiveWindow : GUIControlWindow
{
	private GUIImage bgd;

	private GUIImage goal;

	private GUIImage objectiveIcon;

	private GUITBCloseButton closeButton;

	private GUILabel ordersText;

	private SHSTimerEx displayTime;

	public SHSBrawlerObjectiveWindow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		SetSize(512f, 256f);
		bgd = new GUIImage();
		bgd.SetPositionAndSize(new Vector2(65f, 84f), new Vector2(381f, 172f));
		bgd.TextureSource = "brawler_bundle|background";
		bgd.Color = new Color(1f, 1f, 1f, 1f);
		bgd.IsVisible = true;
		Add(bgd);
		goal = new GUIImage();
		goal.SetPositionAndSize(new Vector2(32f, 54f), new Vector2(256f, 128f));
		goal.TextureSource = "brawler_bundle|L_goal";
		goal.Color = new Color(1f, 1f, 1f, 1f);
		goal.IsVisible = true;
		Add(goal);
		ordersText = new GUILabel();
		ordersText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, Color.black, TextAnchor.MiddleLeft);
		ordersText.SetPosition(131f, 123f);
		ordersText.SetSize(286f, 90f);
		ordersText.Rotation = 0f;
		ordersText.Color = new Color(10f / 51f, 58f / 255f, 24f / 85f, 1f);
		ordersText.Alpha = 1f;
		ordersText.IsVisible = true;
		ordersText.IsEnabled = true;
		Add(ordersText);
		objectiveIcon = new GUIImage();
		objectiveIcon.SetPositionAndSize(new Vector2(340f, 64f), new Vector2(128f, 128f));
		objectiveIcon.IsVisible = false;
		Add(objectiveIcon);
		closeButton = GUIControl.CreateControlTopLeftFrame<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(410f, 100f));
		closeButton.Click += delegate
		{
			Hide();
		};
		Add(closeButton);
		displayTime = new SHSTimerEx();
		displayTime.Stop();
		displayTime.OnTimerEvent += onTimerEvent;
		Add(displayTime);
	}

	public void DisplayOrders(string newOrders, string newObjectiveIcon, float ordersTime)
	{
		ordersText.Text = newOrders;
		displayTime.Duration = ordersTime;
		IsVisible = true;
		displayTime.Start();
		if (!string.IsNullOrEmpty(newObjectiveIcon))
		{
			objectiveIcon.TextureSource = newObjectiveIcon;
			objectiveIcon.IsVisible = true;
		}
		else
		{
			objectiveIcon.IsVisible = false;
		}
	}

	private void onTimerEvent(SHSTimerEx.TimerEventType Type, int data)
	{
		if (Type == SHSTimerEx.TimerEventType.Completed)
		{
			IsVisible = false;
			AppShell.Instance.EventMgr.Fire(this, new BrawlerMissionBriefCompleteMessage());
		}
	}
}
