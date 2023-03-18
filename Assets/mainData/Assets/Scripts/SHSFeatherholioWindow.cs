using UnityEngine;

public class SHSFeatherholioWindow : GUIDynamicWindow
{
	private GUIImage featherImage;

	private GUIImage featherBackground;

	private float fadeStartTime = 0.1f;

	private float fadeDurationTime = 1f;

	private Vector2 startLocation;

	private Vector2 endLocation;

	private float rotationDir = 1f;

	private float rotationDirRange = 4f;

	private float scaleReductionBase = 0.5f;

	private float alphaReducationBase = 1.6f;

	private float timeStarted;

	public SHSFeatherholioWindow(Vector2 endLocation, int collectedCount, int totalCount, string featherImagePath)
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		this.endLocation = endLocation;
		rotationDir = Random.Range(0f - rotationDirRange, rotationDirRange);
		featherImage = new GUIImage();
		featherImage.Id = "featherImage";
		featherImage.SetPositionAndSize(QuickSizingHint.ParentSize);
		featherImage.TextureSource = featherImagePath;
		Add(featherImage, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
		featherBackground = new GUIImage();
		featherBackground.Id = "featherBackground";
		featherBackground.SetPositionAndSize(QuickSizingHint.ParentSize);
		featherBackground.TextureSource = "gameworld_bundle|gameworld_token_background";
		Add(featherBackground, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
		timeStarted = Time.time;
	}

	public override void OnShow()
	{
		base.OnShow();
		Vector2 vector = new Vector2(GUIManager.ScreenRect.width / 2.1f, GUIManager.ScreenRect.height / 2.1f);
		startLocation = new Vector2(vector.x - Random.Range(5f, vector.x * 0.25f), GUIManager.ScreenRect.height - vector.y + Random.Range(15f, vector.y * 0.25f));
		SetPositionAndSize(AnchorAlignmentEnum.Middle, startLocation, new Vector2(73f, 74f));
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		float num2 = num - fadeStartTime;
		if (num > fadeStartTime)
		{
			featherImage.Alpha = 1f - num2 / fadeDurationTime * alphaReducationBase;
			featherImage.Scale = 1f - num2 / fadeDurationTime * scaleReductionBase;
			featherBackground.Alpha = featherImage.Alpha;
			featherBackground.Scale = featherImage.Scale;
			rotation += rotationDir;
			Vector2 position = new Vector2(startLocation.x + (endLocation.x - startLocation.x) * (num2 / fadeDurationTime), startLocation.y + (endLocation.y - startLocation.y) * num2 / fadeDurationTime);
			SetPositionAndSize(AnchorAlignmentEnum.Middle, position, new Vector2(73f, 74f));
		}
		if (featherImage.Alpha <= 0f)
		{
			Hide();
		}
	}
}
