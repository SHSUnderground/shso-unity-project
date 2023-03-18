using System;
using UnityEngine;

public class SHSZoneChooser : SHSGadget
{
	public class ZoneChooser : GadgetCenterWindow
	{
		public class BigButton : GUISimpleControlWindow
		{
			public GUIHotSpotButton hotSpot;

			public GUIImage glow;

			public GUIAnimatedButton sidesButton;

			public GUIAnimatedButton mainButton;

			public GUIAnimatedButton nameButton;

			private float defaultNormalState;

			private bool CurrentZoneButton;

			private Vector2 nameSize;

			private float highlightState;

			protected MouseClickDelegate currentClick;

			private AnimClip fadeInOutAnim;

			private AnimClip pulseFadeAnim;

			public BigButton(ZoneChooser ParentWindow, string imagePath, string warpPath, float highlightState, Vector2 nameSize, string currentSocialSpace, ContentReference requiredContent, Vector2 glowSize)
			{
				SetSize(800f, 800f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
				this.highlightState = highlightState;
				this.nameSize = nameSize;
				hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(300f, 500f), new Vector2(0f, 0f));
				glow = GUIControl.CreateControlFrameCentered<GUIImage>(glowSize, new Vector2(0f, 0f));
				glow.Alpha = 0f;
				glow.TextureSource = "zonechooser_bundle|zone_" + imagePath + "_glow";
				Add(glow);
				defaultNormalState = 0.8f;
				CurrentZoneButton = false;
				if (string.Compare(currentSocialSpace, warpPath, true) == 0)
				{
					defaultNormalState = 0.85f;
					CurrentZoneButton = true;
				}
				sidesButton = CreateAndAddButton("zone_" + imagePath + "_sides", 0f, new Vector2(512f, 512f), new Vector2(0f, 0f));
				mainButton = CreateAndAddButton("zone_" + imagePath + "_main", 0.08f, new Vector2(512f, 512f), new Vector2(0f, 0f));
				nameButton = CreateAndAddButton("L_zone_name_" + imagePath, 0.16f, nameSize, new Vector2(0f, 71f));
				Add(hotSpot);
				ChangeClickBehavior(delegate
				{
					CspUtils.DebugLog("ChangeClickBehavior " + currentSocialSpace);
					AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "switch_zone", OwnableDefinition.simpleZoneName(warpPath), 3f);
					AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = warpPath;
					AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
				});
				hotSpot.MouseOver += delegate
				{
					ParentWindow.StopPulsing(this);
					base.AnimationPieceManager.SwapOut(ref fadeInOutAnim, SHSAnimations.Generic.FadeIn(glow, 0.3f));
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("zoneselect_hover"));
				};
				hotSpot.MouseOut += delegate
				{
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Expected O, but got Unknown
					AnimClip animClip = SHSAnimations.Generic.FadeOut(glow, 0.3f);
					animClip.OnFinished += (Action)(object)(Action)delegate
					{
						ParentWindow.ResumePulsing();
					};
					base.AnimationPieceManager.SwapOut(ref fadeInOutAnim, animClip);
				};
				if (requiredContent != null)
				{
					hotSpot.ConfigureRequiredContent(requiredContent);
				}
			}

			public void SetPercentSize(float perc)
			{
				hotSpot.SetSize(new Vector2(300f, 500f) * perc);
				glow.SetSize(new Vector2(452f, 533f) * Mathf.Max(perc, 1f));
				sidesButton.SetSize(new Vector2(512f, 512f) * perc);
				mainButton.SetSize(new Vector2(512f, 512f) * perc);
				nameButton.SetSize(nameSize * perc);
			}

			public void ChangeClickBehavior(MouseClickDelegate newClickFunc)
			{
				if (currentClick != null)
				{
					hotSpot.Click -= currentClick;
				}
				currentClick = newClickFunc;
				hotSpot.Click += currentClick;
			}

			public override void OnShow()
			{
				base.OnShow();
				PulseFadeIfAllowed();
			}

			public void PulseFadeIfAllowed()
			{
				if (CurrentZoneButton)
				{
					base.AnimationPieceManager.Remove(fadeInOutAnim);
					BeginPulseFade();
				}
			}

			public void BeginPulseFade()
			{
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Expected O, but got Unknown
				AnimClip animClip = AnimClipBuilder.Absolute.Alpha((AnimClipBuilder.Path.Cos((float)Math.PI, 1f, 1.7f) + 1f) * 1f, glow);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					BeginPulseFade();
				};
				base.AnimationPieceManager.SwapOut(ref pulseFadeAnim, animClip);
			}

			public void EndPulseFade(BigButton trigger)
			{
				if (trigger == this)
				{
					base.AnimationPieceManager.Remove(pulseFadeAnim);
					return;
				}
				base.AnimationPieceManager.Remove(pulseFadeAnim);
				base.AnimationPieceManager.SwapOut(ref fadeInOutAnim, SHSAnimations.Generic.FadeOut(glow, 0.3f));
			}

			public GUIAnimatedButton CreateAndAddButton(string path, float delay, Vector2 size, Vector2 offset)
			{
				GUIAnimatedButton gUIAnimatedButton = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(size, offset);
				gUIAnimatedButton.SetupButton(defaultNormalState, highlightState, 0.75f);
				gUIAnimatedButton.HighlightPath = delegate(float CP, float HP)
				{
					return BounceInPath(CP, HP, delay, 0.35f);
				};
				gUIAnimatedButton.TextureSource = "zonechooser_bundle|" + path;
				gUIAnimatedButton.HitTestType = HitTestTypeEnum.Transparent;
				gUIAnimatedButton.LinkToSourceButton(hotSpot);
				Add(gUIAnimatedButton);
				return gUIAnimatedButton;
			}

			protected static AnimPath BounceInPath(float current, float target, float delay, float time)
			{
				return AnimClipBuilder.Path.Constant(current, delay) | SHSAnimations.GenericPaths.LinearWith2xSingleWiggle(current, target, time);
			}
		}

		protected BigButton dailyBugle;

		protected BigButton baxterPlaza;

		protected BigButton asgard;

		public ZoneChooser()
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, false, true);
			string currentSocialSpace = null;
			if (SocialSpaceController.Instance != null)
			{
				currentSocialSpace = (string)AppShell.Instance.SharedHashTable["SocialSpaceLevelCurrent"];
			}
			dailyBugle = new BigButton(this, "daily_bugle", "daily_bugle", 1f, new Vector2(394f, 90f), currentSocialSpace, null, new Vector2(452f, 533f));
			dailyBugle.Offset = new Vector2(-274f, 52f);
			dailyBugle.Rotation = -15f;
			dailyBugle.SetPercentSize(0.78f);
			Add(dailyBugle);
			baxterPlaza = new BigButton(this, "baxter_plaza", "baxter_building", 1f, new Vector2(394f, 90f), currentSocialSpace, new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds), new Vector2(452f, 533f));
			baxterPlaza.Offset = new Vector2(276f, 58f);
			baxterPlaza.Rotation = 15f;
			baxterPlaza.nameButton.Offset = new Vector2(0f, 62f);
			baxterPlaza.SetPercentSize(0.78f);
			Add(baxterPlaza);
			asgard = new BigButton(this, "asgard", "asgard", 0.9f, new Vector2(301f, 98f), currentSocialSpace, new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds), new Vector2(512f, 512f));
			asgard.Offset = new Vector2(-16f, 8f);
			asgard.Rotation = 0f;
			asgard.SetPercentSize(1.04f);
			asgard.nameButton.Offset = new Vector2(17f, 77f);
			Add(asgard);
		}

		public void TutorialMode()
		{
			dailyBugle.ChangeClickBehavior(delegate
			{
			});
			dailyBugle.Offset = new Vector2(-188f, 25f);
			dailyBugle.SetPercentSize(1f);
			Remove(baxterPlaza);
			Remove(asgard);
		}

		public void ResumePulsing()
		{
			dailyBugle.PulseFadeIfAllowed();
			baxterPlaza.PulseFadeIfAllowed();
			asgard.PulseFadeIfAllowed();
		}

		public void StopPulsing(BigButton trigger)
		{
			dailyBugle.EndPulseFade(trigger);
			baxterPlaza.EndPulseFade(trigger);
			asgard.EndPulseFade(trigger);
		}
	}

	private class TopWindow : GadgetTopWindow
	{
		public TopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(203f, 42f), Vector2.zero);
			gUIImage2.TextureSource = "zonechooser_bundle|L_title_select_zone";
			Add(gUIImage2);
		}
	}

	private TopWindow tw;

	private ZoneChooser zc;

	public SHSZoneChooser()
	{
		tw = new TopWindow();
		zc = new ZoneChooser();
		SetupOpeningTopWindow(tw);
		SetupOpeningWindow(BackgroundType.OnePanel, zc);
	}

	public void TutorialMode()
	{
		CloseButton.IsVisible = false;
		CloseButton.IsEnabled = false;
		zc.TutorialMode();
	}
}
