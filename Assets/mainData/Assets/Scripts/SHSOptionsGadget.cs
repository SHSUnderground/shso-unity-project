using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSOptionsGadget : SHSGadget
{
	public class CreditsTab : GenericTab
	{
		public class CreditsWindow : GUISimpleControlWindow
		{
			public class CreditsReel : SHSSelectionWindow<CreditsItem, GUISimpleControlWindow>
			{
				public CreditsReel(GUISlider slider)
					: base(slider, 500f, new Vector2(500f, 32f), 15)
				{
					SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
					TopOffsetAdjustHeight = 386f;
					BottomOffsetAdjustHeight = 386f;
					slider.FireChanged();
					SetSize(new Vector2(544f, 386f));
					SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
					Offset = new Vector2(0f, -15f);
				}
			}

			public class CreditsItem : SHSSelectionItem<GUISimpleControlWindow>
			{
				private GUIDropShadowTextLabel titleName;

				public CreditsItem(DataItem data)
				{
					item = new GUISimpleControlWindow();
					itemSize = new Vector2(500f, 50f);
					titleName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(500f, 50f), new Vector2(0f, 0f));
					SetupTextForCredits(titleName, data.type);
					titleName.Text = data.TitleName;
					titleName.BackColorAlpha = 0.31f;
					item.Add(titleName);
				}
			}

			private GUISlider slider;

			private CreditsReel credits;

			private CreditsTab parentWindow;

			private AnimClip CreditScroll;

			public CreditsWindow(CreditsTab parentWindow)
			{
				this.parentWindow = parentWindow;
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
				SetSize(2000f, 2000f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-3f, 22f));
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(544f, 406f), new Vector2(-4f, -16f));
				gUIImage.TextureSource = "options_bundle|options_credits_backdrop";
				Add(gUIImage);
				slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 370f), new Vector2(255f, -8f));
				Add(slider);
				slider.ThumbButton.MouseDown += delegate
				{
					base.AnimationPieceManager.RemoveIfUnfinished(CreditScroll);
				};
				slider.OnMouseWheelChanged += delegate
				{
					BeginCreditScrollAtPointWithDelay(3f);
				};
				slider.ThumbButton.MouseUp += delegate
				{
					BeginCreditScrollAtPointWithDelay(3f);
				};
				credits = new CreditsReel(slider);
				Add(credits);
			}

			public void AddCreditsData(List<DataItem> data)
			{
				List<DataItem> list = new List<DataItem>();
				foreach (DataItem datum in data)
				{
					if (datum.type == LineType.Name)
					{
						list.Add(datum);
					}
					else
					{
						AddInAccData(list);
						if (datum.type == LineType.GuildTitle)
						{
							credits.AddItem(new CreditsItem(new DataItem(LineType.Name, string.Empty)));
							AddInCreditItem(datum);
							credits.AddItem(new CreditsItem(new DataItem(LineType.Name, string.Empty)));
						}
						else
						{
							AddInCreditItem(datum);
						}
					}
				}
				AddInAccData(list);
				slider.Value = 0f;
				credits.UpdateDisplay();
				BeginCreditScroll(0f);
			}

			private void AddInCreditItem(DataItem item)
			{
				if (string.IsNullOrEmpty(item.TitleName))
				{
					credits.AddItem(new CreditsItem(new DataItem(item.type, item.TitleName)));
				}
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(420f, 50f), new Vector2(0f, 0f));
				gUIDropShadowTextLabel.WordWrap = false;
				gUIDropShadowTextLabel.Overflow = true;
				SetupTextForCredits(gUIDropShadowTextLabel, item.type);
				gUIDropShadowTextLabel.Text = item.TitleName;
				while (true)
				{
					float textWidth = gUIDropShadowTextLabel.GetTextWidth();
					Vector2 size = gUIDropShadowTextLabel.Size;
					if (!(textWidth > size.x))
					{
						break;
					}
					string[] array = gUIDropShadowTextLabel.Text.Split(' ');
					string text2 = gUIDropShadowTextLabel.Text = array[0];
					int num = 1;
					if (num < array.Length)
					{
						while (true)
						{
							float textWidth2 = gUIDropShadowTextLabel.GetTextWidth();
							Vector2 size2 = gUIDropShadowTextLabel.Size;
							if (!(textWidth2 < size2.x))
							{
								break;
							}
							text2 = gUIDropShadowTextLabel.Text;
							if (num >= array.Length)
							{
								num++;
								break;
							}
							gUIDropShadowTextLabel.Text = text2 + " " + array[num];
							num++;
						}
						num--;
					}
					credits.AddItem(new CreditsItem(new DataItem(item.type, text2)));
					string text3 = string.Empty;
					for (int i = num; i < array.Length; i++)
					{
						text3 = text3 + " " + array[i];
					}
					gUIDropShadowTextLabel.Text = text3.Trim();
				}
				if (!string.IsNullOrEmpty(gUIDropShadowTextLabel.Text))
				{
					credits.AddItem(new CreditsItem(new DataItem(item.type, gUIDropShadowTextLabel.Text)));
				}
			}

			private void AddInAccData(List<DataItem> accNameDataItem)
			{
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(420f, 50f), new Vector2(0f, 0f));
				gUIDropShadowTextLabel.WordWrap = false;
				gUIDropShadowTextLabel.Overflow = true;
				SetupTextForCredits(gUIDropShadowTextLabel, LineType.Name);
				while (accNameDataItem.Count > 0)
				{
					string text = accNameDataItem[0].TitleName;
					int num = 1;
					for (int i = 1; i < accNameDataItem.Count; i++)
					{
						string text3 = gUIDropShadowTextLabel.Text = text + ", " + accNameDataItem[i].TitleName;
						float textWidth = gUIDropShadowTextLabel.GetTextWidth();
						Vector2 size = gUIDropShadowTextLabel.Size;
						if (textWidth > size.x)
						{
							break;
						}
						num++;
						text = text3;
					}
					credits.AddItem(new CreditsItem(new DataItem(LineType.Name, text)));
					accNameDataItem.RemoveRange(0, num);
				}
			}

			public override void OnShow()
			{
				if (!parentWindow.creditsLoaded)
				{
					parentWindow.LoadCreditsFromXml();
					parentWindow.creditsLoaded = true;
				}
				else
				{
					slider.Value = 0f;
					credits.UpdateDisplay();
					BeginCreditScroll(0f);
				}
				base.OnShow();
			}

			public void BeginCreditScrollAtPointWithDelay(float delay)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected O, but got Unknown
				AnimClip animClip = SHSAnimations.Generic.Wait(delay);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					BeginCreditScroll(slider.Value);
				};
				base.AnimationPieceManager.SwapOut(ref CreditScroll, animClip);
			}

			public void BeginCreditScroll(float start)
			{
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Expected O, but got Unknown
				AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(start, slider.Max, (slider.Max - start) / 90f), delegate(float x)
				{
					slider.Value = x;
				});
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					BeginCreditScroll(0f);
				};
				base.AnimationPieceManager.SwapOut(ref CreditScroll, animClip);
			}

			public static void SetupTextForCredits(GUIDropShadowTextLabel lbl, LineType type)
			{
				switch (type)
				{
				case LineType.GuildTitle:
					lbl.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 21, ColorUtil.FromRGB255(248, 231, 182), ColorUtil.FromRGB255(0, 23, 88), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
					break;
				case LineType.JobTitle:
					lbl.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 19, ColorUtil.FromRGB255(234, 211, 145), ColorUtil.FromRGB255(0, 23, 88), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
					break;
				case LineType.Name:
					lbl.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, ColorUtil.FromRGB255(170, 197, 72), ColorUtil.FromRGB255(0, 23, 88), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
					lbl.Bold = true;
					break;
				}
			}
		}

		public enum LineType
		{
			GuildTitle,
			JobTitle,
			Name
		}

		public class DataItem
		{
			public LineType type;

			public string TitleName;

			public DataItem(LineType type, string TitleName)
			{
				this.type = type;
				this.TitleName = TitleName;
			}
		}

		private CreditsWindow creditsWindow;

		private bool creditsLoaded;

		public CreditsTab()
		{
			FadeInOut fadeInOut = new FadeInOut(this);
			creditsWindow = new CreditsWindow(this);
			Add(creditsWindow);
			fadeInOut.RegisterFade(creditsWindow);
			Fade.RegisterFade(fadeInOut);
			CurrentTab = fadeInOut;
		}

		public void CreditsLoadedCallback(List<DataItem> data)
		{
			creditsWindow.AddCreditsData(data);
		}

		public void LoadCreditsFromXml()
		{
			AppShell.Instance.DataManager.LoadGameData("credits", CreditsLoadCallback);
		}

		public void CreditsLoadCallback(GameDataLoadResponse response, object extraData)
		{
			if (!string.IsNullOrEmpty(response.Error))
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				return;
			}
			List<DataItem> list = new List<DataItem>();
			DataWarehouse data = response.Data;
			foreach (DataWarehouse item in data.GetIterator("//guild"))
			{
				string attribute = item.Navigator.GetAttribute("title", string.Empty);
				list.Add(new DataItem(LineType.GuildTitle, attribute));
				foreach (DataWarehouse item2 in item.GetIterator("job"))
				{
					string attribute2 = item2.Navigator.GetAttribute("title", string.Empty);
					list.Add(new DataItem(LineType.JobTitle, attribute2));
					for (int i = 0; i < item2.GetCount("name"); i++)
					{
						string @string = item2.GetString("name", i);
						list.Add(new DataItem(LineType.Name, @string));
					}
				}
			}
			CreditsLoadedCallback(list);
		}
	}

	public class GameSettingsTab : GenericTab
	{
		private DataManager dataManager;

		private RadioButtonLine FullScreen;

		public GameSettingsTab(DataManager dataManager)
		{
			this.dataManager = dataManager;
			SubTab subTab = new SubTab(this, "#OPTIONS_SETTINGS", 1);
			RadioButtonLine ctrl = CreateAndAddRadioButtonLine(DataManager.Options.SquadName, new Vector2(-3f, -78f));
			RadioButtonLine ctrl2 = CreateAndAddRadioButtonLine(DataManager.Options.HeroName, new Vector2(-3f, -28f));
			RadioButtonLine ctrl3 = CreateAndAddRadioButtonLine(DataManager.Options.HelpfulHints, new Vector2(-3f, 22f));
			RadioButtonLine ctrl4 = CreateAndAddRadioButtonLine(DataManager.Options.ShowEmoteBar, new Vector2(-3f, 72f));
			FullScreen = CreateAndAddRadioButtonLine(DataManager.Options.FullScreen, new Vector2(-3f, 122f));
			subTab.RegisterFade(ctrl);
			subTab.RegisterFade(ctrl2);
			subTab.RegisterFade(ctrl3);
			subTab.RegisterFade(ctrl4);
			subTab.RegisterFade(FullScreen);
			CurrentTab = subTab;
		}

		public RadioButtonLine CreateAndAddRadioButtonLine(DataManager.Options option, Vector2 offset)
		{
			RadioButtonLine radioButtonLine = new RadioButtonLine(option, dataManager);
			radioButtonLine.Offset = offset;
			Add(radioButtonLine);
			return radioButtonLine;
		}

		public override void HandleResize(GUIResizeMessage message)
		{
			base.HandleResize(message);
			if (FullScreen != null)
			{
				FullScreen.radioButtonLinker.EnableButton((!Screen.fullScreen) ? 1 : 0);
			}
		}
	}

	public class GraphicsTab : GenericTab
	{
		public DataManager dataManager;

		public GraphicsTab(DataManager dataManager)
		{
			this.dataManager = dataManager;
			SubTab subTab = new SubTab(this, "#OPTIONS_QUICK", 1);
			RadioButtonLinker radioButtonLinker = new RadioButtonLinker(DataManager.Options.QuickOptions, dataManager);
			int num = 0;
			foreach (RadioButton value in radioButtonLinker.buttons.Values)
			{
				subTab.RegisterFade(value);
				value.Offset = new Vector2(-203f, -78 + 50 * num);
				Add(value);
				num++;
			}
			GUIImage port = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(299f, 300f), new Vector2(68f, 38f));
			port.TextureSource = "options_bundle|options_graphics_polaroid_fast";
			port.Rotation = 11f;
			Add(port);
			subTab.RegisterFade(port);
			radioButtonLinker.OnButtonsUpdated += delegate(int indexUpdated)
			{
				switch (indexUpdated)
				{
				case 0:
					port.TextureSource = "options_bundle|options_graphics_settings_fast";
					break;
				case 1:
					port.TextureSource = "options_bundle|options_graphics_settings_simple";
					break;
				case 2:
					port.TextureSource = "options_bundle|options_graphics_settings_good";
					break;
				case 3:
					port.TextureSource = "options_bundle|options_graphics_settings_beautiful";
					break;
				default:
					port.TextureSource = "options_bundle|options_graphics_settings_good";
					break;
				}
			};
			SubTab advOptions = new SubTab(this, "#OPTIONS_ADVANCED", 2);
			RadioButtonLine ctrl = CreateAndAddRadioButtonLine(DataManager.Options.DepthOfField, new Vector2(-3f, -78f));
			RadioButtonLine ctrl2 = CreateAndAddRadioButtonLine(DataManager.Options.Shadows, new Vector2(-3f, -28f));
			RadioButtonLine ctrl3 = CreateAndAddRadioButtonLine(DataManager.Options.PrestigeEffects, new Vector2(-3f, 22f));
			RadioButtonLine ctrl4 = CreateAndAddRadioButtonLine(DataManager.Options.ModelQuality, new Vector2(-3f, 72f));
			RadioButtonLine ctrl5 = CreateAndAddRadioButtonLine(DataManager.Options.RenderQuality, new Vector2(-3f, 122f));
			advOptions.RegisterFade(ctrl);
			advOptions.RegisterFade(ctrl2);
			advOptions.RegisterFade(ctrl3);
			advOptions.RegisterFade(ctrl4);
			advOptions.RegisterFade(ctrl5);
			radioButtonLinker.buttons[4].button.Click += delegate
			{
				CurrentTab = advOptions;
				FadeIn();
			};
			CurrentTab = subTab;
		}

		public RadioButtonLine CreateAndAddRadioButtonLine(DataManager.Options option, Vector2 offset)
		{
			RadioButtonLine radioButtonLine = new RadioButtonLine(option, dataManager);
			radioButtonLine.Offset = offset;
			Add(radioButtonLine);
			return radioButtonLine;
		}
	}

	public class HowToPlayTab : GenericTab
	{
		public enum PlayArea
		{
			Universal,
			GameWorld,
			Missions,
			HQ
		}

		public abstract class TabWindow : GUISimpleControlWindow
		{
			public const float WINDOW_Y_SIZE = 272f;

			public const float WINDOW_X_SIZE = 440f;

			protected PlayArea playArea;

			private GUISimpleControlWindow contentWindow;

			private GUISimpleControlWindow constraintWindow;

			protected List<GUILabel> labelList;

			public TabWindow()
			{
				SetSize(2000f, 2000f);
				SetPosition(QuickSizingHint.Centered);
				constraintWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(2000f, 272f), new Vector2(0f, 26f));
				Add(constraintWindow);
				contentWindow = GUIControl.CreateControlTopFrame<GUISimpleControlWindow>(new Vector2(2000f, 2000f), Vector2.zero);
				constraintWindow.Add(contentWindow);
			}

			public void Setup(List<GUILabel> labels)
			{
				for (int i = 0; i < labels.Count; i++)
				{
					GUILabel gUILabel = labels[i];
					if (gUILabel != null)
					{
						if (gUILabel.GetTextWidth() > 440f)
						{
							SetTextAndGetOverflow(gUILabel);
						}
						gUILabel.Offset = new Vector2(20f, 15 * i);
						contentWindow.Add(gUILabel);
					}
				}
				float num = (float)(labels.Count * 15) - 272f;
				if (num > 0f)
				{
					GUISlider slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 302f), new Vector2(224f, 23f));
					slider.UseMouseWheelScroll = true;
					slider.MouseScrollWheelAmount = 40f;
					slider.TickValue = 40f;
					slider.ArrowsEnabled = true;
					slider.Max = num + 50f;
					slider.Changed += delegate
					{
						contentWindow.Offset = new Vector2(0f, 0f - slider.Value);
					};
					Add(slider);
				}
			}

			private string SetTextAndGetOverflow(GUILabel lbl)
			{
				string[] array = lbl.Text.Split(' ');
				lbl.Text = array[0];
				string text = lbl.Text;
				int i;
				for (i = 1; i < array.Length; i++)
				{
					text = lbl.Text;
					lbl.Text = lbl.Text + " " + array[i];
					if (lbl.GetTextWidth() > 440f)
					{
						break;
					}
				}
				lbl.Text = text;
				string text2 = string.Empty;
				for (; i < array.Length; i++)
				{
					text2 = text2 + array[i] + " ";
				}
				return text2;
			}

			protected static GUILabel GetItem(string text)
			{
				GUILabel gUILabel = GUIControl.CreateControlTopFrame<GUILabel>(new Vector2(500f, 50f), Vector2.zero);
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 13, ColorUtil.FromRGB255(131, 129, 62), TextAnchor.MiddleLeft);
				gUILabel.Text = text;
				gUILabel.Overflow = true;
				gUILabel.WordWrap = false;
				return gUILabel;
			}

			protected static GUILabel GetTitle(string text)
			{
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlTopFrame<GUIDropShadowTextLabel>(new Vector2(500f, 50f), Vector2.zero);
				gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, ColorUtil.FromRGB255(76, 102, 14), ColorUtil.FromRGB255(136, 136, 47), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
				gUIDropShadowTextLabel.BackColorAlpha = 0.4f;
				gUIDropShadowTextLabel.Text = text;
				return gUIDropShadowTextLabel;
			}
		}

		public class UniversalWindow : TabWindow
		{
			public UniversalWindow()
			{
				playArea = PlayArea.Universal;
				labelList = new List<GUILabel>();
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_MOVEMENT"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_QUICK_MOVE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_MOUSE_MOVE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_KEYBOARD_MOVE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_JUMP"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_ACTION"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_USE_OBJECTS"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_POINTER_HAND"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_TELEPORTERS"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTES"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_GREET"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_LAUGH"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_RAISE_HAND"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_DANCE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_POSE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_APPROVE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_CHEER"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_CLAP"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_BLUFF"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_POINT"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_BOW"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_RUDE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_TAUNT"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_ANGRY"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_DISAPPROVE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_CONFUSED"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_SAD"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_SCARED"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_SNEEZE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_THINK"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_EMOTE_SHOCK"));
				labelList.Add(null);
				Setup(labelList);
			}
		}

		public class GameWorldWindow : TabWindow
		{
			public GameWorldWindow()
			{
				playArea = PlayArea.GameWorld;
				labelList = new List<GUILabel>();
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_USES_UNIVERSAL_CONTROLS"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_MOVEMENT"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_MOVEMENT_SPECIAL_JUMPS"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_MOVEMENT_ABILITY"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_MOVEMENT_WEB"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_MOVEMENT_HULK"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_ACTION"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_ACTION_SELECT_PLAYER"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_ACTION_DESELECT_PLAYER"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_ACTION_ACTIVATE_CHAT"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_ACTION_SEND_CHAT"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_POWER_EMOTES"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_POWER_EMOTES_POWER_EMOTE_1"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_POWER_EMOTES_POWER_EMOTE_2"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_GAMEWORLD_POWER_EMOTES_POWER_EMOTE_3"));
				labelList.Add(null);
				Setup(labelList);
			}
		}

		public class MissionsWindow : TabWindow
		{
			public MissionsWindow()
			{
				playArea = PlayArea.Missions;
				labelList = new List<GUILabel>();
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_USES_UNIVERSAL_CONTROLS"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_UNIVERSAL_ACTION"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_ATTACK"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_POWER_ATTACK"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_HERO_UP"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS_LEFT_MOUSE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS_RIGHT_MOUSE1"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS_RIGHT_MOUSE2"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS_POWER_UP_CONTAINERS_1"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_ACTION_OBJECTS_POWER_UP_CONTAINERS_2"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_MISSIONS_POWER_ATTACKS"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_POWER_ATTACKS_POWER_ATTACK_1"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_POWER_ATTACKS_POWER_ATTACK_2"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_MISSIONS_POWER_ATTACKS_POWER_ATTACK_3"));
				labelList.Add(null);
				Setup(labelList);
			}
		}

		public class HQWindow : TabWindow
		{
			public HQWindow()
			{
				playArea = PlayArea.HQ;
				labelList = new List<GUILabel>();
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_HQ_STANDARD"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_STANDARD_ROTATE"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_STANDARD_STAN"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_STANDARD_MOVE_CAMERA"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_STANDARD_PAUSE"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_HQ_PLAY_MODE"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_PLAY_MODE_USE_ITEM"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_PLAY_MODE_MOVE_OBJECT"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_PLAY_MODE_RELEASE"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetTitle("#OPTIONS_HOW_TO_PLAY_HQ_PAUSE_MODE"));
				labelList.Add(null);
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_PAUSE_MODE_USE_ITEM"));
				labelList.Add(TabWindow.GetItem("#OPTIONS_HOW_TO_PLAY_HQ_PAUSE_MODE_CLICK"));
				labelList.Add(null);
				Setup(labelList);
			}
		}

		private UniversalWindow uw;

		private GameWorldWindow gw;

		private MissionsWindow mw;

		private HQWindow hw;

		private Dictionary<PlayArea, List<GUILabel>> gameAreaLabelLookup;

		public HowToPlayTab(DataManager dataManager)
		{
			SubTab subTab = new SubTab(this, "#OPTIONS_TAB_UNIVERSAL", 1);
			SubTab subTab2 = new SubTab(this, "#OPTIONS_TAB_GAMEWORLD", 2);
			SubTab subTab3 = new SubTab(this, "#OPTIONS_TAB_MISSIONS", 3);
			uw = new UniversalWindow();
			Add(uw);
			subTab.RegisterFade(uw);
			gw = new GameWorldWindow();
			Add(gw);
			subTab2.RegisterFade(gw);
			mw = new MissionsWindow();
			Add(mw);
			subTab3.RegisterFade(mw);
			CurrentTab = subTab;
		}
	}

	public class SoundTab : GenericTab
	{
		public class SliderLine : GUISimpleControlWindow
		{
			private class SoundSlider : GUISlider
			{
				public SoundSlider()
				{
					base.Changed += delegate
					{
						UpdateSliderImage();
					};
				}

				public override void HandleResize(GUIResizeMessage message)
				{
					base.HandleResize(message);
					UpdateSliderInternalOffsets();
				}

				private void UpdateSliderInternalOffsets()
				{
					base.sliderCenter.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
					endCap.SetSize(0f, 0f);
					GUIImage sliderCenter = base.sliderCenter;
					Vector2 size = startCap.Size;
					sliderCenter.Offset = new Vector2(size.x, 0f);
					UpdateSliderImage();
				}

				private void UpdateSliderImage()
				{
					float percentage = base.Percentage;
					Vector2 size = Size;
					float val = percentage * (size.x - HorizontalCapWidth * 2f);
					GUIImage sliderCenter = base.sliderCenter;
					float x = Math.Max(val, 0f);
					Vector2 size2 = base.sliderCenter.Size;
					sliderCenter.Size = new Vector2(x, size2.y);
				}

				public override void OnShow()
				{
					base.OnShow();
					UpdateSliderInternalOffsets();
				}
			}

			private GUIDropShadowTextLabel nameLabel;

			private GUIImage bkg;

			private SoundSlider slider;

			public SliderLine(DataManager.Options option, DataManager dataManager)
			{
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
				SetSize(453f, 50f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
				bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(453f, 43f), Vector2.zero);
				bkg.TextureSource = "options_bundle|options_tabs_inner_frame";
				Add(bkg);
				nameLabel = GUIControl.CreateControl<GUIDropShadowTextLabel>(new Vector2(300f, 50f), new Vector2(182f, 0f), DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
				nameLabel.Offset = new Vector2(15f, 0f);
				nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(79, 106, 15), Color.white, new Vector2(1f, 1f), TextAnchor.MiddleLeft);
				nameLabel.Bold = true;
				nameLabel.Text = dataManager.GetName(option);
				Add(nameLabel);
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(318f, 41f), new Vector2(51f, 1f));
				gUIImage.TextureSource = "options_bundle|options_slider_containter";
				Add(gUIImage);
				slider = GUIControl.CreateControlFrameCentered<SoundSlider>(new Vector2(300f, 50f), new Vector2(50f, 0f));
				slider.IsRefreshSuppressed = true;
				slider.Orientation = GUISlider.SliderOrientationEnum.Horizontal;
				slider.SliderStartTexture = "options_bundle|options_slider_fill_left";
				slider.SliderCenterTexture = "options_bundle|options_slider_fill_center";
				slider.SliderEndTexture = "options_bundle|options_slider_fill_right";
				slider.ScrollButtonStyleInfo = new SHSButtonStyleInfo("options_bundle|options_slider_scrubber");
				slider.ScrollButtonSize = new Vector2(64f, 64f);
				slider.ScrollButtonHitTestType = HitTestTypeEnum.Circular;
				slider.ScrollButtonHitTestSize = new Vector2(0.625f, 0.625f);
				slider.barHeight = 23f;
				slider.HorizontalCapWidth = 13f;
				slider.HorizontalButtonOffset = 21f;
				slider.Min = 0f;
				slider.Max = 1f;
				slider.IsRefreshSuppressed = false;
				slider.RefreshLayout();
				slider.IsRefreshSuppressed = true;
				slider.Changed += delegate
				{
					dataManager.SliderMoved(option, slider.Value);
				};
				Add(slider);
				dataManager.RegisterSlider(option, slider);
				GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(200f, 200f), new Vector2(-100f, 0f));
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.black, TextAnchor.MiddleCenter);
				gUILabel.Text = "#OPTIONS_SLIDER_MIN";
				slider.Add(gUILabel);
				GUILabel gUILabel2 = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(200f, 200f), new Vector2(100f, 0f));
				gUILabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.black, TextAnchor.MiddleCenter);
				gUILabel2.Text = "#OPTIONS_SLIDER_MAX";
				slider.Add(gUILabel2);
				slider.ControlToFront(slider.ThumbButton);
			}
		}

		private DataManager dataManager;

		public SoundTab(DataManager dataManager)
		{
			this.dataManager = dataManager;
			SubTab subTab = new SubTab(this, "#OPTIONS_SOUND", 1);
			SliderLine ctrl = CreateAndAddSliderLine(DataManager.Options.MusicVolume, new Vector2(-3f, -78f));
			SliderLine ctrl2 = CreateAndAddSliderLine(DataManager.Options.EffectsVolume, new Vector2(-3f, -28f));
			SliderLine ctrl3 = CreateAndAddSliderLine(DataManager.Options.VOXVolume, new Vector2(-3f, 22f));
			subTab.RegisterFade(ctrl);
			subTab.RegisterFade(ctrl2);
			subTab.RegisterFade(ctrl3);
			CurrentTab = subTab;
		}

		public SliderLine CreateAndAddSliderLine(DataManager.Options option, Vector2 offset)
		{
			SliderLine sliderLine = new SliderLine(option, dataManager);
			sliderLine.Offset = offset;
			Add(sliderLine);
			return sliderLine;
		}
	}

	public class DataManager
	{
		public enum Options
		{
			DepthOfField,
			Shadows,
			PrestigeEffects,
			ModelQuality,
			RenderQuality,
			QuickOptions,
			SquadName,
			HeroName,
			HelpfulHints,
			FullScreen,
			ShowEmoteBar,
			MusicVolume,
			EffectsVolume,
			VOXVolume
		}

		public enum DepthOfFieldEnum
		{
			On,
			Off
		}

		public enum ShadowsEnum
		{
			Full,
			Blob,
			None
		}

		public enum PrestigeEffectsEnum
		{
			On,
			Off
		}

		public enum ModelQualityEnum
		{
			Fast,
			Simple,
			Good,
			Beautiful
		}

		public enum RenderQualityEnum
		{
			Fast,
			Simple,
			Good,
			Beautiful
		}

		public enum QuickOptionsEnum
		{
			Fast,
			Simple,
			Good,
			Beautiful,
			Custom
		}

		public enum SquadNameEnum
		{
			On,
			Off
		}

		public enum HeroNamesEnum
		{
			On,
			Off
		}

		public enum HelpfulHintsEnum
		{
			On,
			Off
		}

		public enum FullScreenEnum
		{
			On,
			Off
		}

		public enum ShowEmoteBarEnum
		{
			On,
			Off
		}

		public class OptionsState : IComparable<OptionsState>
		{
			public int DepthOfField;

			public int Shadows;

			public int PrestigeEffects;

			public int ModelQuality;

			public int RenderQuality;

			public int QuickOptions;

			public int SquadName;

			public int HeroNames;

			public int HelpfulHints;

			public int ShowEmoteBar;

			public int FullScreen;

			public float MusicVolume;

			public float EffectsVolume;

			public float VOXVolume;

			public OptionsState GetCopy()
			{
				OptionsState optionsState = new OptionsState();
				optionsState.DepthOfField = DepthOfField;
				optionsState.FullScreen = FullScreen;
				optionsState.HelpfulHints = HelpfulHints;
				optionsState.ShowEmoteBar = ShowEmoteBar;
				optionsState.HeroNames = HeroNames;
				optionsState.ModelQuality = ModelQuality;
				optionsState.RenderQuality = RenderQuality;
				optionsState.Shadows = Shadows;
				optionsState.SquadName = SquadName;
				optionsState.PrestigeEffects = PrestigeEffects;
				optionsState.MusicVolume = MusicVolume;
				optionsState.EffectsVolume = EffectsVolume;
				optionsState.VOXVolume = VOXVolume;
				return optionsState;
			}

			public int GetValue(Options option)
			{
				switch (option)
				{
				case Options.DepthOfField:
					return DepthOfField;
				case Options.FullScreen:
					return FullScreen;
				case Options.HelpfulHints:
					return HelpfulHints;
				case Options.ShowEmoteBar:
					return ShowEmoteBar;
				case Options.HeroName:
					return HeroNames;
				case Options.ModelQuality:
					return ModelQuality;
				case Options.QuickOptions:
					return QuickOptions;
				case Options.RenderQuality:
					return RenderQuality;
				case Options.Shadows:
					return Shadows;
				case Options.SquadName:
					return SquadName;
				case Options.PrestigeEffects:
					return PrestigeEffects;
				default:
					return -1;
				}
			}

			public float GetValueFloat(Options option)
			{
				switch (option)
				{
				case Options.MusicVolume:
					return MusicVolume;
				case Options.EffectsVolume:
					return EffectsVolume;
				case Options.VOXVolume:
					return VOXVolume;
				default:
					return -1f;
				}
			}

			public void SetValue(Options option, int Value)
			{
				switch (option)
				{
				case Options.DepthOfField:
					DepthOfField = Value;
					break;
				case Options.FullScreen:
					FullScreen = Value;
					break;
				case Options.HelpfulHints:
					HelpfulHints = Value;
					break;
				case Options.ShowEmoteBar:
					ShowEmoteBar = Value;
					break;
				case Options.HeroName:
					HeroNames = Value;
					break;
				case Options.ModelQuality:
					ModelQuality = Value;
					break;
				case Options.QuickOptions:
					QuickOptions = Value;
					break;
				case Options.RenderQuality:
					RenderQuality = Value;
					break;
				case Options.Shadows:
					Shadows = Value;
					break;
				case Options.SquadName:
					SquadName = Value;
					break;
				case Options.PrestigeEffects:
					PrestigeEffects = Value;
					break;
				}
			}

			public void SetValueFloat(Options option, float Value)
			{
				switch (option)
				{
				case Options.MusicVolume:
					MusicVolume = Value;
					break;
				case Options.EffectsVolume:
					EffectsVolume = Value;
					break;
				case Options.VOXVolume:
					VOXVolume = Value;
					break;
				}
			}

			public int CompareTo(OptionsState other)
			{
				if (other.DepthOfField == DepthOfField && other.FullScreen == FullScreen && other.HelpfulHints == HelpfulHints && other.ShowEmoteBar == ShowEmoteBar && other.HeroNames == HeroNames && other.ModelQuality == ModelQuality && other.RenderQuality == RenderQuality && other.Shadows == Shadows && other.SquadName == SquadName && other.PrestigeEffects == PrestigeEffects && other.MusicVolume == MusicVolume && other.EffectsVolume == EffectsVolume && other.VOXVolume == VOXVolume)
				{
					return 0;
				}
				return 1;
			}
		}

		private MainTabWindow parentWindow;

		public OptionsState currentState = new OptionsState();

		public OptionsState currentAppliedState = new OptionsState();

		private Dictionary<Options, RadioButtonLinker> optionLinker = new Dictionary<Options, RadioButtonLinker>();

		private Dictionary<Options, GUISlider> sliderUpdater = new Dictionary<Options, GUISlider>();

		public DataManager(MainTabWindow parentWindow)
		{
			this.parentWindow = parentWindow;
		}

		public void RegisterLinker(Options option, RadioButtonLinker linker)
		{
			optionLinker.Add(option, linker);
		}

		public void RegisterSlider(Options option, GUISlider slider)
		{
			sliderUpdater.Add(option, slider);
		}

		public void ReflectButtons()
		{
			UpdateQuickOptions();
			foreach (KeyValuePair<Options, RadioButtonLinker> item in optionLinker)
			{
				item.Value.EnableButton(currentState.GetValue(item.Key));
			}
			foreach (KeyValuePair<Options, GUISlider> item2 in sliderUpdater)
			{
				if (currentState.GetValueFloat(item2.Key) != item2.Value.Value)
				{
					item2.Value.Value = currentState.GetValueFloat(item2.Key);
				}
			}
		}

		public string[] GetButtonNames(Options option)
		{
			string[] collection;
			switch (option)
			{
			case Options.DepthOfField:
				collection = Enum.GetNames(typeof(DepthOfFieldEnum));
				break;
			case Options.FullScreen:
				collection = Enum.GetNames(typeof(FullScreenEnum));
				break;
			case Options.HelpfulHints:
				collection = Enum.GetNames(typeof(HelpfulHintsEnum));
				break;
			case Options.ShowEmoteBar:
				collection = Enum.GetNames(typeof(ShowEmoteBarEnum));
				break;
			case Options.HeroName:
				collection = Enum.GetNames(typeof(HeroNamesEnum));
				break;
			case Options.ModelQuality:
				collection = Enum.GetNames(typeof(ModelQualityEnum));
				break;
			case Options.QuickOptions:
				collection = Enum.GetNames(typeof(QuickOptionsEnum));
				break;
			case Options.RenderQuality:
				collection = Enum.GetNames(typeof(RenderQualityEnum));
				break;
			case Options.Shadows:
				collection = Enum.GetNames(typeof(ShadowsEnum));
				break;
			case Options.SquadName:
				collection = Enum.GetNames(typeof(SquadNameEnum));
				break;
			case Options.PrestigeEffects:
				collection = Enum.GetNames(typeof(PrestigeEffectsEnum));
				break;
			default:
				collection = new string[0];
				break;
			}
			List<string> list = new List<string>(collection).ConvertAll(delegate(string entry)
			{
				return "#OPTIONS_RB_OPTION_" + entry.ToUpper();
			});
			return list.ToArray();
		}

		public void ButtonClicked(Options option, int index)
		{
			currentState.SetValue(option, index);
			if (option == Options.QuickOptions)
			{
				SetSimpleGraphics(index);
			}
			ReflectButtons();
		}

		public void SliderMoved(Options option, float value)
		{
			currentState.SetValueFloat(option, value);
		}

		public void SetSimpleGraphics(int index)
		{
			Dictionary<Options, int> settings = GetSettings(index);
			foreach (KeyValuePair<Options, int> item in settings)
			{
				currentState.SetValue(item.Key, item.Value);
			}
		}

		public void UpdateQuickOptions()
		{
			int num = 0;
			while (true)
			{
				if (num < Enum.GetValues(typeof(QuickOptionsEnum)).Length)
				{
					bool flag = true;
					Dictionary<Options, int> settings = GetSettings(num);
					foreach (KeyValuePair<Options, int> item in settings)
					{
						if (currentState.GetValue(item.Key) != item.Value)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			currentState.SetValue(Options.QuickOptions, num);
		}

		public Dictionary<Options, int> GetSettings(int level)
		{
			Dictionary<Options, int> dictionary = new Dictionary<Options, int>();
			switch (level)
			{
			case 0:
				dictionary.Add(Options.DepthOfField, 1);
				dictionary.Add(Options.Shadows, 2);
				dictionary.Add(Options.PrestigeEffects, 1);
				dictionary.Add(Options.ModelQuality, 0);
				dictionary.Add(Options.RenderQuality, 0);
				break;
			case 1:
				dictionary.Add(Options.DepthOfField, 1);
				dictionary.Add(Options.Shadows, 2);
				dictionary.Add(Options.PrestigeEffects, 1);
				dictionary.Add(Options.ModelQuality, 1);
				dictionary.Add(Options.RenderQuality, 1);
				break;
			case 2:
				dictionary.Add(Options.DepthOfField, 1);
				dictionary.Add(Options.Shadows, 1);
				dictionary.Add(Options.PrestigeEffects, 0);
				dictionary.Add(Options.ModelQuality, 2);
				dictionary.Add(Options.RenderQuality, 2);
				break;
			case 3:
				dictionary.Add(Options.DepthOfField, 0);
				dictionary.Add(Options.Shadows, 0);
				dictionary.Add(Options.PrestigeEffects, 0);
				dictionary.Add(Options.ModelQuality, 3);
				dictionary.Add(Options.RenderQuality, 3);
				break;
			}
			return dictionary;
		}

		public string GetName(Options option)
		{
			switch (option)
			{
			case Options.DepthOfField:
				return "#OPTIONS_RB_DOF";
			case Options.FullScreen:
				return "#OPTIONS_RB_FULLSCREEN";
			case Options.HelpfulHints:
				return "#OPTIONS_RB_HELPHINTS";
			case Options.ShowEmoteBar:
				return "#OPTIONS_RB_SHOWEMOTEBAR";
			case Options.HeroName:
				return "#OPTIONS_RB_HERONAME";
			case Options.ModelQuality:
				return "#OPTIONS_RB_MODELQUALITY";
			case Options.QuickOptions:
				return "#OPTIONS_RB_QUICKOPTIONS";
			case Options.RenderQuality:
				return "#OPTIONS_RB_RENDER";
			case Options.Shadows:
				return "#OPTIONS_RB_SHADOWS";
			case Options.SquadName:
				return "#OPTIONS_RB_SQUADNAME";
			case Options.PrestigeEffects:
				return "#OPTIONS_RB_PRESTIGE";
			case Options.MusicVolume:
				return "#OPTIONS_RB_MUSICVOL";
			case Options.EffectsVolume:
				return "#OPTIONS_RB_EFFECTSVOL";
			case Options.VOXVolume:
				return "#OPTIONS_RB_VOVOL";
			default:
				return null;
			}
		}

		public bool HasChange()
		{
			return currentState.CompareTo(currentAppliedState) != 0;
		}

		public void LoadCurrentState()
		{
			currentState.DepthOfField = ((!GraphicsOptions.DOF) ? 1 : 0);
			currentState.FullScreen = ((!Screen.fullScreen) ? 1 : 0);
			currentState.HelpfulHints = ((ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ProTips, 1) != 1) ? 1 : 0);
			currentState.ShowEmoteBar = ((ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, 1) != 1) ? 1 : 0);
			currentState.HeroNames = ((PlayerPrefs.GetInt("heronames", 1) != 1) ? 1 : 0);
			currentState.ModelQuality = (int)GraphicsOptions.ModelQuality;
			currentState.RenderQuality = GetRenderQualityToInt(GraphicsOptions.RenderFidelity);
			currentState.Shadows = GetShadowToInt(GraphicsOptions.Shadows);
			currentState.SquadName = ((PlayerPrefs.GetInt("squadnames", 1) != 1) ? 1 : 0);
			currentState.PrestigeEffects = ((!GraphicsOptions.PrestigeEffects) ? 1 : 0);
			currentState.MusicVolume = AppShell.Instance.AudioManager.MixerSettings.MusicVolume;
			currentState.EffectsVolume = AppShell.Instance.AudioManager.MixerSettings.SoundFxVolume;
			currentState.VOXVolume = AppShell.Instance.AudioManager.MixerSettings.VOXVolume;
			currentAppliedState = currentState.GetCopy();
			ReflectButtons();
		}

		public void ApplyChanges()
		{
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Expected O, but got Unknown
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Expected O, but got Unknown
			bool flag = false;
			bool flag2 = false;
			if (currentAppliedState.DepthOfField != currentState.DepthOfField)
			{
				GraphicsOptions.DOF = (currentState.DepthOfField == 0);
				flag2 = true;
			}
			if (currentAppliedState.FullScreen != currentState.FullScreen)
			{
				AppShell.Instance.AutoFullScreenToggle();
				flag = true;
			}
			if (currentAppliedState.HelpfulHints != currentState.HelpfulHints)
			{
				ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.ProTips, (currentState.HelpfulHints == 0) ? 1 : 0);
			}
			if (currentAppliedState.ShowEmoteBar != currentState.ShowEmoteBar)
			{
				ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, (currentState.ShowEmoteBar == 0) ? 1 : 0);
				AppShell.Instance.EventMgr.Fire(this, new ShowEmoteBarSettingMessage());
			}
			if (currentAppliedState.HeroNames != currentState.HeroNames)
			{
				PlayerPrefs.SetInt("heronames", (currentState.HeroNames == 0) ? 1 : 0);
				PlayerBillboard[] array = Utils.FindObjectsOfType<PlayerBillboard>();
				PlayerBillboard[] array2 = array;
				foreach (PlayerBillboard playerBillboard in array2)
				{
					playerBillboard.RenderHeroNameEnabled = ((currentState.HeroNames == 0) ? true : false);
					playerBillboard.UpdateRenderers();
				}
			}
			if (currentAppliedState.ModelQuality != currentState.ModelQuality)
			{
				GraphicsOptions.ModelQuality = (GraphicsOptions.GraphicsQuality)currentState.ModelQuality;
				flag2 = true;
			}
			if (currentAppliedState.RenderQuality != currentState.RenderQuality)
			{
				GraphicsOptions.RenderFidelity = GetIntToRenderQuality(currentState.RenderQuality);
				flag2 = true;
			}
			if (currentAppliedState.Shadows != currentState.Shadows)
			{
				GraphicsOptions.Shadows = GetIntToShadow(currentState.Shadows);
				flag2 = true;
			}
			if (currentAppliedState.SquadName != currentState.SquadName)
			{
				PlayerPrefs.SetInt("squadnames", (currentState.SquadName == 0) ? 1 : 0);
				PlayerBillboard[] array3 = Utils.FindObjectsOfType<PlayerBillboard>();
				PlayerBillboard[] array4 = array3;
				foreach (PlayerBillboard playerBillboard2 in array4)
				{
					playerBillboard2.RenderSquadNameEnabled = ((currentState.SquadName == 0) ? true : false);
					playerBillboard2.UpdateRenderers();
				}
			}
			if (currentAppliedState.PrestigeEffects != currentState.PrestigeEffects)
			{
				GraphicsOptions.PrestigeEffects = (currentState.PrestigeEffects == 0);
				flag2 = true;
			}
			if (currentAppliedState.MusicVolume != currentState.MusicVolume)
			{
				AppShell.Instance.AudioManager.MixerSettings.MusicVolume = currentState.MusicVolume;
				ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioMusicVolume, currentState.MusicVolume);
			}
			if (currentAppliedState.EffectsVolume != currentState.EffectsVolume)
			{
				AppShell.Instance.AudioManager.MixerSettings.SoundFxVolume = currentState.EffectsVolume;
				ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioEffectsVolume, currentState.EffectsVolume);
			}
			if (currentAppliedState.VOXVolume != currentState.VOXVolume)
			{
				AppShell.Instance.AudioManager.MixerSettings.VOXVolume = currentState.VOXVolume;
				ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioVOXVolume, currentState.VOXVolume);
			}
			if (flag)
			{
				AnimClip animClip = SHSAnimations.Generic.Wait(Time.deltaTime + 0.01f);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					LoadCurrentState();
				};
				parentWindow.AnimationPieceManager.Add(animClip);
			}
			else
			{
				LoadCurrentState();
			}
			if (flag2)
			{
				ShowYesNoDialog("#OptionsRequestLogout", (Action)(object)(Action)delegate
				{
					AppShell.Instance.Quit();
				});
			}
		}

		public void ShowYesNoDialog(string text, Action del)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, text, delegate(string Id, DialogState state)
			{
				if (state == DialogState.Ok && del != null)
				{
					del.Invoke();
				}
			}, ModalLevelEnum.Default);
		}

		public void RestoreDefaults()
		{
			GraphicsOptions.Reset();
			RestoreAudioDefaults();
			if (PlayerPrefs.GetInt("squadnames") != 1 || PlayerPrefs.GetInt("heronames") != 0)
			{
				PlayerPrefs.SetInt("squadnames", 1);
				PlayerPrefs.SetInt("heronames", 1);
				PlayerBillboard[] array = Utils.FindObjectsOfType<PlayerBillboard>();
				PlayerBillboard[] array2 = array;
				foreach (PlayerBillboard playerBillboard in array2)
				{
					playerBillboard.RenderSquadNameEnabled = true;
					playerBillboard.RenderHeroNameEnabled = true;
					playerBillboard.UpdateRenderers();
				}
			}
			LoadCurrentState();
		}

		public void RestoreAudioDefaults()
		{
			AppShell.Instance.AudioManager.MixerSettings.MusicVolume = 0.75f;
			AppShell.Instance.AudioManager.MixerSettings.SoundFxVolume = 0.75f;
			AppShell.Instance.AudioManager.MixerSettings.VOXVolume = 0.75f;
			ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioMusicVolume, 0.75f);
			ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioEffectsVolume, 0.75f);
			ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.AudioVOXVolume, 0.75f);
		}

		public int GetRenderQualityToInt(QualityLevel q)
		{
			switch (q)
			{
			case QualityLevel.Fast:
				return 0;
			case QualityLevel.Simple:
				return 1;
			case QualityLevel.Good:
				return 2;
			case QualityLevel.Beautiful:
				return 3;
			default:
				return 1;
			}
		}

		public QualityLevel GetIntToRenderQuality(int i)
		{
			switch (i)
			{
			case 0:
				return QualityLevel.Fast;
			case 1:
				return QualityLevel.Simple;
			case 2:
				return QualityLevel.Good;
			case 3:
				return QualityLevel.Beautiful;
			default:
				return QualityLevel.Simple;
			}
		}

		public int GetShadowToInt(GraphicsOptions.ShadowLevel s)
		{
			switch (s)
			{
			case GraphicsOptions.ShadowLevel.None:
				return 2;
			case GraphicsOptions.ShadowLevel.Blob:
				return 1;
			case GraphicsOptions.ShadowLevel.Projected:
				return 0;
			default:
				return 1;
			}
		}

		public GraphicsOptions.ShadowLevel GetIntToShadow(int s)
		{
			switch (s)
			{
			case 2:
				return GraphicsOptions.ShadowLevel.None;
			case 1:
				return GraphicsOptions.ShadowLevel.Blob;
			case 0:
				return GraphicsOptions.ShadowLevel.Projected;
			default:
				return GraphicsOptions.ShadowLevel.Blob;
			}
		}
	}

	private class OptionsTopWindow : GadgetTopWindow
	{
		public OptionsTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(141f, 40f), new Vector2(2f, 7f));
			gUIImage2.TextureSource = "options_bundle|L_options_title";
			Add(gUIImage2);
		}
	}

	public class SixLabelText : GUISimpleControlWindow
	{
		private GUILabel frontText;

		private GUILabel offset1;

		private GUILabel offset2;

		private GUILabel offset3;

		private GUILabel offset4;

		private GUILabel dropShadow;

		public string Text
		{
			get
			{
				return frontText.Text;
			}
			set
			{
				frontText.Text = value;
				offset1.Text = value;
				offset2.Text = value;
				offset3.Text = value;
				offset4.Text = value;
				dropShadow.Text = value;
			}
		}

		public int VerticalKerning
		{
			get
			{
				return frontText.VerticalKerning;
			}
			set
			{
				frontText.VerticalKerning = value;
				offset1.VerticalKerning = value;
				offset2.VerticalKerning = value;
				offset3.VerticalKerning = value;
				offset4.VerticalKerning = value;
				dropShadow.VerticalKerning = value;
			}
		}

		public SixLabelText(Vector2 size, Vector2 offset)
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
			frontText = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(0f, 0f));
			offset1 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(1f, 1f));
			offset2 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(1f, -1f));
			offset3 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(-1f, 1f));
			offset4 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(-1f, -1f));
			dropShadow = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(0f, 0f));
			Add(dropShadow);
			Add(offset1);
			Add(offset2);
			Add(offset3);
			Add(offset4);
			Add(frontText);
		}

		public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color frontColor, Color surroundColor, Color dropShadowColor, Vector2 dropShadowOffset)
		{
			frontText.SetupText(fontFace, fontSize, frontColor, TextAnchor.MiddleCenter);
			offset1.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset2.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset3.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset4.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			dropShadow.SetupText(fontFace, fontSize, dropShadowColor, TextAnchor.MiddleCenter);
			dropShadow.Offset = dropShadowOffset;
		}

		public int CalculateKerningAndGetLines()
		{
			frontText.CalculateTextLayout();
			return frontText.LineCount;
		}
	}

	public class FadeFadeInOut
	{
		public List<FadeInOut> toFade = new List<FadeInOut>();

		public void RegisterFade(FadeInOut fio)
		{
			toFade.Add(fio);
		}

		public void SetState(bool on, FadeInOut fio)
		{
			if (on)
			{
				fio.SetState(on);
				toFade.ForEach(delegate(FadeInOut tf)
				{
					if (fio != tf)
					{
						tf.SetState(!on);
					}
				});
			}
			else
			{
				toFade.ForEach(delegate(FadeInOut tf)
				{
					tf.FullNotOn();
				});
			}
		}

		public void FadeIn(FadeInOut fio)
		{
			fio.FadeIn();
			toFade.ForEach(delegate(FadeInOut tf)
			{
				if (fio != tf)
				{
					tf.FadeOut();
				}
			});
		}

		public void FadeOut()
		{
			toFade.ForEach(delegate(FadeInOut tf)
			{
				tf.FullFadeOut();
			});
		}
	}

	public class FadeInOut
	{
		private GUIWindow window;

		public List<GUIControl> toFade = new List<GUIControl>();

		public List<GUIControl> toAntiFade = new List<GUIControl>();

		private AnimClip fadeAnim;

		public FadeInOut(GUIWindow window)
		{
			this.window = window;
		}

		public void RegisterFade(GUIControl ctrl)
		{
			toFade.Add(ctrl);
			ctrl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		}

		public void RegisterAntiFade(GUIControl ctrl)
		{
			toAntiFade.Add(ctrl);
			ctrl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		}

		public void SetState(bool on)
		{
			toFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = on;
				ctrl.Alpha = (on ? 1 : 0);
			});
			toAntiFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = !on;
				ctrl.Alpha = ((!on) ? 1 : 0);
			});
		}

		public void FadeIn()
		{
			window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeInVis(toFade, 0.2f) ^ SHSAnimations.Generic.FadeOutVis(toAntiFade, 0.2f));
		}

		public void FadeOut()
		{
			window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOutVis(toFade, 0.2f) ^ SHSAnimations.Generic.FadeInVis(toAntiFade, 0.2f));
		}

		public void FullNotOn()
		{
			toFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = false;
				ctrl.Alpha = 0f;
			});
			toAntiFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = false;
				ctrl.Alpha = 0f;
			});
		}

		public void FullFadeOut()
		{
			window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOutVis(toFade, 0.2f) ^ SHSAnimations.Generic.FadeOutVis(toAntiFade, 0.2f));
		}
	}

	public class MainTabWindow : GadgetCenterWindow
	{
		public class TextButton : GUISimpleControlWindow
		{
			public enum Type
			{
				one,
				two,
				apply,
				restore
			}

			private static readonly Color FRONT_COLOR = ColorUtil.FromRGB255(255, 255, 255);

			private static readonly Color SURROUND_COLOR = ColorUtil.FromRGB255(102, 129, 21);

			private static readonly Color DROPSHADOW_COLOR = ColorUtil.FromRGB255(71, 92, 8);

			public GUIAnimatedButton button;

			private SixLabelText textLabel;

			private GUIAnimatedButton highlight;

			private AnimClip highlightAnim;

			public TextButton(string text, Type t)
			{
				SetSize(200f, 200f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
				button = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), Vector2.zero);
				button.HitTestSize = new Vector2(0.57f, 0.2f);
				button.TextureSource = ButtonTexutreSource(t);
				button.SetupButton(0.96f, 1f, 0.91f);
				textLabel = new SixLabelText(new Vector2(140f, 50f), Vector2.zero);
				textLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, FRONT_COLOR, SURROUND_COLOR, DROPSHADOW_COLOR, new Vector2(2f, 2f));
				textLabel.Text = text;
				highlight = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), Vector2.zero);
				highlight.HitTestType = HitTestTypeEnum.Transparent;
				highlight.TextureSource = HighlightTexutreSource(t);
				highlight.SetupButton(0.96f, 1f, 0.91f);
				highlight.LinkToSourceButton(button);
				Add(button);
				Add(textLabel);
				Add(highlight);
			}

			public void Highlight(bool on)
			{
				AnimClip newPiece = (!on) ? AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(Alpha, 0.75f, 0.25f), this) : AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(Alpha, 1f, 0.25f), this);
				base.AnimationPieceManager.SwapOut(ref highlightAnim, newPiece);
			}

			private string ButtonTexutreSource(Type t)
			{
				switch (t)
				{
				case Type.one:
					return "options_bundle|options_button_1_normal";
				case Type.two:
					return "options_bundle|options_button_2_normal";
				case Type.apply:
					return "options_bundle|options_apply_button_normal";
				case Type.restore:
					return "options_bundle|options_restore_button_normal";
				default:
					return string.Empty;
				}
			}

			private string HighlightTexutreSource(Type t)
			{
				switch (t)
				{
				case Type.one:
					return "options_bundle|options_shine_1_normal";
				case Type.two:
					return "options_bundle|options_shine_2_normal";
				case Type.apply:
					return "options_bundle|options_apply_shine_normal";
				case Type.restore:
					return "options_bundle|options_restore_shine_normal";
				default:
					return string.Empty;
				}
			}
		}

		public DataManager dataManager;

		private GraphicsTab graphicsTab;

		private SoundTab soundTab;

		private HowToPlayTab howToPlayTab;

		private GameSettingsTab gameSettingsTab;

		private CreditsTab creditsTab;

		private TextButton graphicsButton;

		private TextButton soundButton;

		private TextButton howToPlayButton;

		private TextButton gameSettingsButton;

		private TextButton creditsButton;

		public MainTabWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(810f, 475f), new Vector2(0f, 32f));
			gUIImage.TextureSource = "options_bundle|options_inner_frame";
			Add(gUIImage);
			dataManager = new DataManager(this);
			graphicsTab = new GraphicsTab(dataManager);
			soundTab = new SoundTab(dataManager);
			howToPlayTab = new HowToPlayTab(dataManager);
			gameSettingsTab = new GameSettingsTab(dataManager);
			creditsTab = new CreditsTab();
			Add(graphicsTab);
			Add(soundTab);
			Add(howToPlayTab);
			Add(gameSettingsTab);
			Add(creditsTab);
			graphicsButton = CreateAndAddTextButton("#OPTIONS_OPTION_GRAPHICS", TextButton.Type.one, new Vector2(-285f, -127f), graphicsTab);
			soundButton = CreateAndAddTextButton("#OPTIONS_OPTION_SOUND", TextButton.Type.two, new Vector2(-280f, -64f), soundTab);
			howToPlayButton = CreateAndAddTextButton("#OPTIONS_OPTION_HOWTOPLAY", TextButton.Type.one, new Vector2(-275f, -1f), howToPlayTab);
			gameSettingsButton = CreateAndAddTextButton("#OPTIONS_OPTION_GAMESETTINGS", TextButton.Type.two, new Vector2(-270f, 62f), gameSettingsTab);
			creditsButton = CreateAndAddTextButton("#OPTIONS_OPTION_CREDIT", TextButton.Type.one, new Vector2(-265f, 125f), creditsTab);
			TextButton textButton = new TextButton("#OPTIONS_OPTION_APPLYCHANGES", TextButton.Type.apply);
			textButton.Offset = new Vector2(14f, 248f);
			textButton.button.Click += delegate
			{
				dataManager.ApplyChanges();
			};
			textButton.button.ToolTip = new NamedToolTipInfo("#options_tooltip_apply");
			AddSFXToButton(textButton.button);
			Add(textButton);
			TextButton textButton2 = new TextButton("#OPTIONS_OPTION_RESTORE", TextButton.Type.restore);
			textButton2.Offset = new Vector2(170f, 248f);
			textButton2.button.Click += delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				ShowDialog("#OPTIONS_OPTION_CONFIRM_RESTORE", (Action)(object)(Action)delegate
				{
					dataManager.RestoreDefaults();
				});
			};
			textButton2.button.ToolTip = new NamedToolTipInfo("#options_tooltip_restore");
			AddSFXToButton(textButton2.button);
			Add(textButton2);
			dataManager.LoadCurrentState();
		}

		public TextButton CreateAndAddTextButton(string text, TextButton.Type t, Vector2 offset, GenericTab relatedTab)
		{
			TextButton ta = new TextButton(text, t);
			ta.Offset = offset;
			ta.button.Click += delegate
			{
				ta.Highlight(true);
				FadeInAndOutAll(relatedTab);
			};
			AddSFXToButton(ta.button);
			Add(ta);
			return ta;
		}

		public void AddSFXToButton(GUIControl control)
		{
			control.MouseOver += delegate
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_hover_over"));
			};
			control.MouseDown += delegate
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
			};
			control.MouseUp += delegate
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_up"));
			};
		}

		public void FadeInAndOutAll(GenericTab tabToFadeIn)
		{
			tabToFadeIn.FadeIn();
			if (tabToFadeIn != graphicsTab)
			{
				graphicsTab.FadeOut();
				graphicsButton.Highlight(false);
			}
			if (tabToFadeIn != soundTab)
			{
				soundTab.FadeOut();
				soundButton.Highlight(false);
			}
			if (tabToFadeIn != howToPlayTab)
			{
				howToPlayTab.FadeOut();
				howToPlayButton.Highlight(false);
			}
			if (tabToFadeIn != gameSettingsTab)
			{
				gameSettingsTab.FadeOut();
				gameSettingsButton.Highlight(false);
			}
			if (tabToFadeIn != creditsTab)
			{
				creditsTab.FadeOut();
				creditsButton.Highlight(false);
			}
		}

		public override void OnShow()
		{
			graphicsTab.SetupOff();
			soundTab.SetupOff();
			howToPlayTab.SetupOff();
			gameSettingsTab.SetupOff();
			creditsTab.SetupOff();
			graphicsButton.button.FireMouseClick(null);
			base.OnShow();
		}
	}

	public abstract class GenericTab : GUISimpleControlWindow
	{
		public FadeFadeInOut Fade = new FadeFadeInOut();

		public FadeInOut CurrentTab;

		public GenericTab()
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(92f, 12f));
			SetControlFlag(ControlFlagSetting.AlphaCascade, false, false);
		}

		public void FadeIn()
		{
			Fade.FadeIn(CurrentTab);
		}

		public void FadeOut()
		{
			Fade.FadeOut();
		}

		public void SetupOn()
		{
			Fade.SetState(true, CurrentTab);
		}

		public void SetupOff()
		{
			Fade.SetState(false, null);
		}
	}

	public class RadioButtonLine : GUISimpleControlWindow
	{
		private GUIDropShadowTextLabel nameLabel;

		private GUIImage bkg;

		public RadioButtonLinker radioButtonLinker;

		public RadioButtonLine(DataManager.Options option, DataManager dataManager)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			SetSize(453f, 43f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			radioButtonLinker = new RadioButtonLinker(option, dataManager);
			bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(453f, 43f), Vector2.zero);
			bkg.TextureSource = "options_bundle|options_tabs_inner_frame";
			Add(bkg);
			nameLabel = GUIControl.CreateControl<GUIDropShadowTextLabel>(new Vector2(100f, 100f), new Vector2(182f, 0f), DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
			nameLabel.Offset = new Vector2(15f, 0f);
			nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(79, 106, 15), Color.white, new Vector2(1f, 1f), TextAnchor.MiddleLeft);
			nameLabel.Bold = true;
			nameLabel.Text = dataManager.GetName(option);
			nameLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			Add(nameLabel);
			int num = 0;
			foreach (RadioButton value in radioButtonLinker.buttons.Values)
			{
				value.Offset = new Vector2(-95 + 80 * num, 0f);
				Add(value);
				num++;
				if (num < radioButtonLinker.buttons.Count)
				{
					value.CollapseLabel();
				}
			}
		}
	}

	public class RadioButtonLinker
	{
		public Dictionary<int, RadioButton> buttons = new Dictionary<int, RadioButton>();

		public event Action<int> OnButtonsUpdated;

		public RadioButtonLinker(DataManager.Options option, DataManager dataManager)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			dataManager.RegisterLinker(option, this);
			string[] buttonNames = dataManager.GetButtonNames(option);
			for (int i = 0; i < buttonNames.Length; i++)
			{
				int current = i;
				Add(buttonNames[current], current, (Action)(object)(Action)delegate
				{
					dataManager.ButtonClicked(option, current);
				});
			}
		}

		public void Add(string buttonName, int buttonKey, Action onButtonClicked)
		{
			RadioButton radioButton = new RadioButton(buttonName);
			radioButton.button.Click += delegate
			{
				onButtonClicked.Invoke();
			};
			buttons.Add(buttonKey, radioButton);
		}

		public void EnableButton(int toEnableKey)
		{
			RadioButton radioButton = buttons[toEnableKey];
			radioButton.Active(true);
			foreach (RadioButton value in buttons.Values)
			{
				if (value != radioButton)
				{
					value.Active(false);
				}
			}
			if (this.OnButtonsUpdated != null)
			{
				this.OnButtonsUpdated(toEnableKey);
			}
		}
	}

	public class RadioButton : GUISimpleControlWindow
	{
		public GUIButton button;

		private GUIDropShadowTextLabel label;

		private bool currentActiveState;

		public RadioButton(string Text)
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			button = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(0f, 0f));
			button.HitTestType = HitTestTypeEnum.Circular;
			button.HitTestSize = new Vector2(0.5f, 0.5f);
			Add(button);
			label = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(300f, 50f), new Vector2(167f, 0f));
			label.FontFace = GUIFontManager.SupportedFontEnum.Komica;
			label.TextOffset = new Vector2(1f, 1f);
			Add(label);
			label.TextAlignment = TextAnchor.MiddleLeft;
			label.BackColor = Color.white;
			label.Text = Text;
			currentActiveState = true;
			Active(false);
		}

		public void Active(bool on)
		{
			if (currentActiveState != on)
			{
				currentActiveState = on;
				if (on)
				{
					button.StyleInfo = new SHSButtonStyleInfo("options_bundle|options_radio_button_selected");
					label.FrontColor = ColorUtil.FromRGB255(96, 127, 19);
					label.FontSize = 16;
					label.Bold = true;
					label.BackColorAlpha = 1f;
					label.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				}
				else
				{
					button.StyleInfo = new SHSButtonStyleInfo("options_bundle|options_radio_button");
					label.FrontColor = ColorUtil.FromRGB255(79, 106, 15);
					label.FontSize = 14;
					label.Bold = false;
					label.BackColorAlpha = 0.6f;
					label.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				}
			}
		}

		public void CollapseLabel()
		{
			label.Size = new Vector2(44f, 50f);
			label.Offset = new Vector2(39f, 0f);
		}
	}

	public class SubTab : FadeInOut
	{
		public SubTab(GenericTab genericTab, string name, int tabNum)
			: base(genericTab)
		{
			Vector2 offsetOnTabNum = GetOffsetOnTabNum(tabNum);
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(529f, 366f), new Vector2(0f, 0f));
			gUIImage.TextureSource = "options_bundle|options_tab_panel_" + tabNum;
			genericTab.Add(gUIImage);
			RegisterFade(gUIImage);
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), offsetOnTabNum);
			gUIButton.StyleInfo = new SHSButtonStyleInfo("options_bundle|options_tab", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			gUIButton.HitTestSize = new Vector2(0.531f, 0.219f);
			genericTab.Add(gUIButton);
			RegisterAntiFade(gUIButton);
			genericTab.ControlToBack(gUIButton);
			SixLabelText sixLabelText = new SixLabelText(new Vector2(110f, 56f), offsetOnTabNum + new Vector2(0f, 5f));
			sixLabelText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 23, Color.white, ColorUtil.FromRGB255(0, 114, 255), ColorUtil.FromRGB255(22, 77, 195), new Vector2(4f, 3f));
			sixLabelText.Text = name;
			genericTab.Add(sixLabelText);
			RegisterFade(sixLabelText);
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(110f, 56f), offsetOnTabNum + new Vector2(0f, -3f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 17, ColorUtil.FromRGB255(34, 77, 162), TextAnchor.MiddleCenter);
			gUILabel.Text = name;
			genericTab.Add(gUILabel);
			RegisterAntiFade(gUILabel);
			gUIButton.Click += delegate
			{
				genericTab.CurrentTab = this;
				genericTab.FadeIn();
			};
			sixLabelText.VerticalKerning = 16;
			gUILabel.VerticalKerning = 14;
			genericTab.Fade.RegisterFade(this);
		}

		public Vector2 GetOffsetOnTabNum(int tabNum)
		{
			switch (tabNum)
			{
			case 1:
				return new Vector2(-192f, -146f);
			case 2:
				return new Vector2(-63f, -148f);
			case 3:
				return new Vector2(66f, -150f);
			case 4:
				return new Vector2(195f, -152f);
			default:
				return Vector2.zero;
			}
		}
	}

	private static bool optionsCurrentlyShowing;

	private MainTabWindow mainTab;

	private OptionsTopWindow topWin;

	public static bool OptionsCurrentlyShowing
	{
		get
		{
			return optionsCurrentlyShowing;
		}
	}

	public SHSOptionsGadget()
	{
		mainTab = new MainTabWindow();
		topWin = new OptionsTopWindow();
		SetupOpeningTopWindow(topWin);
		SetupOpeningWindow(BackgroundType.OnePanel, mainTab);
		SetBackgroundImage("options_bundle|options_gadget_frame");
		CloseButton.Click -= base.CloseTheGadget;
		CloseButton.Click += delegate(GUIControl x, GUIClickEvent y)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			SHSOptionsGadget sHSOptionsGadget = this;
			if (mainTab.dataManager.HasChange())
			{
				ShowDialog("#OPTIONS_CONFIRM_CHANGES", (Action)(object)(Action)delegate
				{
					sHSOptionsGadget.CloseTheGadget(x, y);
				});
			}
			else
			{
				CloseTheGadget(x, y);
			}
		};
	}

	public override void OnShow()
	{
		optionsCurrentlyShowing = true;
		base.OnShow();
	}

	public override void OnHide()
	{
		optionsCurrentlyShowing = false;
		base.OnHide();
	}

	public static void ShowDialog(string text, Action onOk)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, text, delegate(string Id, DialogState state)
		{
			if (state == DialogState.Ok && onOk != null)
			{
				onOk.Invoke();
			}
		}, ModalLevelEnum.Default);
	}
}
