using System;
using UnityEngine;

public class SHSCardGamePokeTimer : SHSTimerEx
{
	private GUIImage rotatingBG;

	private GUILabel numbers;

	private GUILabel numberShadow;

	private string timeText;

	public float StartingTime
	{
		get
		{
			return base.Duration;
		}
		set
		{
			base.Duration = value;
		}
	}

	public SHSCardGamePokeTimer(int fontSize, Vector2 numberOffset)
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "cardgame_bundle|player_timer_layer_c";
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, -15f));
		gUIImage.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(gUIImage);
		rotatingBG = new GUIImage();
		rotatingBG.TextureSource = "cardgame_bundle|player_timer_layer_b";
		rotatingBG.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, -15f));
		rotatingBG.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(rotatingBG);
		numbers = new GUILabel();
		numbers.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, numberOffset);
		numbers.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numbers.SetupText(GUIFontManager.SupportedFontEnum.Zooom, fontSize, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
		numberShadow = new GUILabel();
		numberShadow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(numberOffset.x + 2f, numberOffset.y + 2f));
		numberShadow.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		numberShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, fontSize, GUILabel.GenColor(61, 61, 61), TextAnchor.MiddleCenter);
		Add(numberShadow);
		Add(numbers);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.TextureSource = "cardgame_bundle|player_timer_layer_a";
		gUIImage2.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, -15f));
		gUIImage2.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(gUIImage2);
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		timeText = Convert.ToInt32(base.TimeLeft).ToString();
		if (base.TimerState == TimerStateEnum.Idle)
		{
			timeText = Convert.ToInt32(base.Duration).ToString();
		}
		numbers.Text = timeText;
		numberShadow.Text = timeText;
	}

	public void StartTimer()
	{
		Start();
		StartRotating();
	}

	public void StartRotating()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Absolute.Rotation(AnimPath.Linear(rotatingBG.Rotation, rotatingBG.Rotation + 360f, 1f), rotatingBG);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			if (base.TimerState == TimerStateEnum.Running)
			{
				StartRotating();
			}
		};
		base.AnimationPieceManager.Add(animClip);
	}
}
