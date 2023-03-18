using System;
using UnityEngine;

public class SHSLevelUpAnimation : GUIDynamicWindow
{
	protected const float START_SIZE = 10f;

	protected const float END_SIZE = 256f;

	protected const float WINDOW_SIZE = 300f;

	protected const float Y_OFFSET = 256f;

	protected const float X_OFFSET = 128f;

	protected GUIImage levelUpBGImage;

	protected GUIImage levelUpTextImage;

	protected bool animationFinished;

	public override bool InitializeResources(bool reload)
	{
		Vector2 offset = Vector2.zero;
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			offset = GUICommon.WorldToUIPoint(Camera.main, localPlayer.transform.position);
		}
		levelUpBGImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(10f, 10f), new Vector2(0f, 0f));
		levelUpBGImage.Anchor = AnchorAlignmentEnum.Middle;
		levelUpBGImage.Docking = DockingAlignmentEnum.Middle;
		levelUpBGImage.TextureSource = "common_bundle|mshs_levelup_star";
		Add(levelUpBGImage);
		levelUpTextImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(300f, 300f), new Vector2(0f, 0f));
		levelUpTextImage.TextureSource = "persistent_bundle|L_mshs_levelup_text";
		Add(levelUpTextImage);
		levelUpTextImage.Hide();
		SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset, new Vector2(300f, 300f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		AnimClip toAdd = AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(10f, 256f, 1f), levelUpBGImage) ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(offset.y, 190f, 1f), this) ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(offset.x, 350f, 1f), this);
		base.AnimationPieceManager.Add(toAdd);
		SpinLevelUp();
		animationFinished = false;
		AppShell.Instance.EventMgr.AddListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
		return base.InitializeResources(reload);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override void Hide()
	{
		AppShell.Instance.EventMgr.RemoveListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
		base.Hide();
	}

	protected void SpinLevelUp()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		if (levelUpBGImage != null)
		{
			AnimClip animClip = AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(0f, 360f, 1f), levelUpBGImage);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				Vector2 size = levelUpBGImage.Size;
				if (size.x < 256f)
				{
					SpinLevelUp();
				}
				else
				{
					animationFinished = true;
					ShowLevelUpTextImage();
				}
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	protected void ShowLevelUpTextImage()
	{
		if (levelUpTextImage != null)
		{
			levelUpTextImage.Show();
		}
	}

	protected void OnLevelUpWindowHide(LeveledUpAwardHiddenMessage e)
	{
		Hide();
	}
}
