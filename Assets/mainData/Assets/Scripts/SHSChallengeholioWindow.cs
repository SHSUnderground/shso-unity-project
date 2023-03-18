using UnityEngine;

public class SHSChallengeholioWindow : GUIDynamicWindow
{
	private GUIImage challengeImage;

	private float startTime = 0.75f;

	private float durationTime = 0.5f;

	private Vector2 startLocation;

	private Vector2 endLocation;

	private float rotationDir = 1f;

	private float rotationDirRange = 4f;

	private float scaleReductionBase = 0.5f;

	private float timeStarted;

	public SHSChallengeholioWindow(Vector2 endLocation, string challengeImagePath)
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		this.endLocation = endLocation;
		rotationDir = Random.Range(0f - rotationDirRange, rotationDirRange);
		challengeImage = new GUIImage();
		challengeImage.SetPositionAndSize(QuickSizingHint.ParentSize);
		challengeImage.TextureSource = challengeImagePath;
		Add(challengeImage, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
		timeStarted = Time.time;
	}

	public override void OnShow()
	{
		base.OnShow();
		startLocation = new Vector2(GUIManager.ScreenRect.width / 2.1f, GUIManager.ScreenRect.height / 2.5f);
		SetPositionAndSize(AnchorAlignmentEnum.Middle, startLocation, new Vector2(73f, 62f));
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		float num2 = num - startTime;
		float num3 = num2 / durationTime;
		if (num3 >= 1f)
		{
			Hide();
		}
		else if (num > startTime)
		{
			challengeImage.Scale = 1f - num3 * scaleReductionBase;
			rotation += rotationDir;
			Vector2 position = new Vector2(startLocation.x + (endLocation.x - startLocation.x) * num3, startLocation.y + (endLocation.y - startLocation.y) * num3);
			SetPositionAndSize(AnchorAlignmentEnum.Middle, position, new Vector2(73f, 62f));
		}
	}
}
