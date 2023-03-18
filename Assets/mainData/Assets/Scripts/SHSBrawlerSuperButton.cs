using UnityEngine;

public class SHSBrawlerSuperButton : GUIControlWindow
{
	private enum State
	{
		Inactive,
		Normal,
		Highlight,
		Pressed
	}

	private State currentState;

	private State lastState;

	private string[] stateNames = new string[4]
	{
		"normal",
		"normal",
		"highlight",
		"pressed"
	};

	private bool hover;

	private bool poweredUp;

	private bool mouseButtonDown;

	private GUIHotSpotButton clicker;

	private GUIImage buttonBg;

	private GUIImage buttonCap;

	private GUIImage buttonIcon;

	private GUIImage buttonText;

	public SHSBrawlerSuperButton()
	{
		currentState = State.Inactive;
		lastState = State.Inactive;
		hover = false;
		poweredUp = false;
		mouseButtonDown = false;
		clicker = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(Vector2.zero, Vector2.zero);
		clicker.Traits.HitTestType = HitTestTypeEnum.Circular;
		clicker.Traits.BlockTestType = BlockTestTypeEnum.Circular;
		clicker.IsVisible = false;
		Add(clicker);
		buttonBg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), Vector2.zero);
		buttonBg.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		buttonBg.IsVisible = false;
		Add(buttonBg);
		buttonCap = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), Vector2.zero);
		buttonCap.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		buttonCap.IsVisible = false;
		Add(buttonCap);
		buttonIcon = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), Vector2.zero);
		buttonIcon.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		buttonIcon.IsVisible = false;
		Add(buttonIcon);
		buttonText = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), Vector2.zero);
		buttonText.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		buttonText.IsVisible = false;
		Add(buttonText);
		clicker.MouseOver += delegate
		{
			hover = true;
			UpdateButtonState();
		};
		clicker.MouseOut += delegate
		{
			hover = false;
			UpdateButtonState();
		};
		clicker.MouseDown += delegate
		{
			mouseButtonDown = true;
			UpdateButtonState();
		};
		clicker.MouseUp += delegate
		{
			mouseButtonDown = false;
			if (hover)
			{
				AppShell.Instance.EventMgr.Fire(this, new PowerMoveMessage(-1));
			}
			UpdateButtonState();
		};
	}

	public void ChangePowerState(bool PowerUp)
	{
		if (!poweredUp && PowerUp)
		{
			PlayAvailableSFX();
		}
		poweredUp = PowerUp;
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		lastState = currentState;
		currentState = State.Inactive;
		if (poweredUp)
		{
			if (mouseButtonDown)
			{
				currentState = State.Pressed;
			}
			else if (hover)
			{
				currentState = State.Highlight;
			}
			else
			{
				currentState = State.Normal;
			}
		}
		if (lastState != currentState)
		{
			UpdateButtonGraphics();
		}
	}

	private void UpdateButtonGraphics()
	{
		int num = (int)currentState;
		buttonBg.TextureSource = "brawler_bundle|heroup_shadow_" + stateNames[num];
		buttonCap.TextureSource = "brawler_bundle|heroup_button_cap_" + stateNames[num];
		buttonIcon.TextureSource = "brawler_bundle|heroup_icon_" + stateNames[num];
		buttonText.TextureSource = "brawler_bundle|heroup_text_" + stateNames[num];
		if (lastState == State.Inactive)
		{
			clicker.SetSize(154f, 154f);
			buttonText.IsVisible = true;
			buttonIcon.IsVisible = true;
			clicker.IsVisible = true;
			buttonBg.IsVisible = true;
			buttonCap.IsVisible = true;
			base.AnimationPieceManager.ClearAll();
			base.AnimationPieceManager.Add(SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(256f, 256f), 1f, buttonBg) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(256f, 256f), 1f, buttonCap) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(256f, 256f), 1f, buttonIcon) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(256f, 256f), 1f, buttonText));
			base.AnimationPieceManager.Add(FadeBetweenIcons());
		}
		if (currentState == State.Inactive)
		{
			clicker.SetSize(0f, 0f);
			clicker.IsVisible = false;
			base.AnimationPieceManager.Add((SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(256f, 256f), 1f, buttonBg) ^ SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(256f, 256f), 1f, buttonCap) ^ SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(256f, 256f), 1f, buttonIcon) ^ SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(256f, 256f), 1f, buttonText)) | (SHSAnimations.Generic.ChangeVisibility(false, buttonBg) ^ SHSAnimations.Generic.ChangeVisibility(false, buttonCap) ^ SHSAnimations.Generic.ChangeVisibility(false, buttonIcon) ^ SHSAnimations.Generic.ChangeVisibility(false, buttonText)));
		}
	}

	protected AnimClip FadeBetweenIcons()
	{
		float num = 1f;
		return ((AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, num), buttonText) | SHSAnimations.Generic.Wait(num) | AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, num), buttonText) | SHSAnimations.Generic.Wait(num)) ^ (AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, num), buttonIcon) | SHSAnimations.Generic.Wait(num) | AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, num), buttonIcon) | SHSAnimations.Generic.Wait(num))) | new AnimClipFunction(0f, delegate
		{
			base.AnimationPieceManager.Add(FadeBetweenIcons());
		});
	}

	protected void PlayAvailableSFX()
	{
		GUIManager.Instance.BundleManager.LoadAsset("hud_bundle", "HUD_UI_HeroUp_Available_audio", null, delegate(Object obj, AssetBundle bundle, object extraData)
		{
			ShsAudioSource.PlayAutoSound(obj as GameObject);
		});
	}

	public void EnableButton(bool enable)
	{
		if (enable)
		{
			clicker.Traits.HitTestType = HitTestTypeEnum.Circular;
			clicker.Traits.BlockTestType = BlockTestTypeEnum.Circular;
		}
		else
		{
			clicker.Traits.HitTestType = HitTestTypeEnum.Transparent;
			clicker.Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		}
	}

	public override void SetVisible(bool visible, SetVisibleReason reason)
	{
		base.SetVisible(visible, reason);
		if (visible && currentState != 0)
		{
			clicker.SetSize(154f, 154f);
			buttonText.IsVisible = true;
			buttonIcon.IsVisible = true;
			clicker.IsVisible = true;
			buttonBg.IsVisible = true;
			buttonCap.IsVisible = true;
			base.AnimationPieceManager.ClearAll();
			base.AnimationPieceManager.Add(FadeBetweenIcons());
		}
	}
}
