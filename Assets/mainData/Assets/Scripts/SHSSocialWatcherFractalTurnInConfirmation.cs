using System.Text.RegularExpressions;
using UnityEngine;

public class SHSSocialWatcherFractalTurnInConfirmation : SHSCommonDialogWindow
{
	private WatcherFractalTurnin _owner;

	private bool _canTurnIn;

	private int _balance;

	private GUIImage watcherIcon;

	private GUIImage watcherPortraitFrame;

	private GUIImage fractalIcon;

	private GUITextField searchField;

	public SHSSocialWatcherFractalTurnInConfirmation(WatcherFractalTurnin owner, int balance, bool alreadyTurnedIn)
		: base(string.Empty, string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false)
	{
		_owner = owner;
		_balance = balance;
		SHSFractalsActivity fractals = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
		_canTurnIn = (_balance >= fractals.MaxFractals || alreadyTurnedIn);
		float d = 0.65f;
		Vector2 position = new Vector2(895f, 30f) * d;
		Vector2 size = new Vector2(203f, 194f) * d;
		Vector2 position2 = new Vector2(870f, 0f) * d;
		Vector2 size2 = new Vector2(256f, 256f) * d;
		watcherPortraitFrame = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		watcherPortraitFrame.TextureSource = "common_bundle|mshs_common_hud_character_frame";
		watcherPortraitFrame.SetPosition(position);
		watcherPortraitFrame.SetSize(size);
		watcherPortraitFrame.IsVisible = true;
		Add(watcherPortraitFrame);
		watcherIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		watcherIcon.TextureSource = "characters_bundle|watcher_boss_HUD_default";
		watcherIcon.SetPosition(position2);
		watcherIcon.SetSize(size2);
		watcherIcon.IsVisible = true;
		Add(watcherIcon);
		fractalIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		fractalIcon.SetPosition(new Vector2(145f, base.size.y / 3f));
		fractalIcon.SetSize(new Vector2(84f, 84f));
		fractalIcon.TextureSource = "goodieseffects_bundle|goody_effects_fractal_shopping";
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(108f, 28f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_help");
		gUIButton.Click += delegate
		{
			SHSOkDialogWindow sHSOkDialogWindow = new SHSOkDialogWindow();
			sHSOkDialogWindow.TitleText = "#FRACTAL_INSTRUCTIONS_TITLE";
			sHSOkDialogWindow.Text = "#FRACTAL_INSTRUCTIONS";
			GUIManager.Instance.ShowDynamicWindow(sHSOkDialogWindow, ModalLevelEnum.Full);
		};
		Add(gUIButton);
		text.FrontColor = new Color(0f, 10f / 51f, 14f / 51f, 1f);
		text.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		type = NotificationType.Common;
		descriptionText.FontSize = 25;
		descriptionKerning = 18;
		if (_canTurnIn)
		{
			if (_balance > 0)
			{
				TitleText = "#WATCHER_FRACAL_TURN_IN";
				Text = string.Empty;
				GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(128f, 128f), new Vector2(base.size.x / 2f - 38f, 108f));
				gUIButton2.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_left");
				gUIButton2.Click += delegate
				{
					if (alreadyTurnedIn)
					{
						searchField.Text = Mathf.Min(1, _balance).ToString();
					}
					else
					{
						searchField.Text = Mathf.Min(fractals.MaxFractals, _balance).ToString();
					}
				};
				Add(gUIButton2);
				GUIButton gUIButton3 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(128f, 128f), new Vector2(base.size.x / 2f + 283f, 108f));
				gUIButton3.StyleInfo = new SHSButtonStyleInfo("common_bundle|arrow_right");
				gUIButton3.Click += delegate
				{
					searchField.Text = _balance.ToString();
				};
				Add(gUIButton3);
				fractalIcon.SetPosition(new Vector2(base.size.x / 2f + 243f, 138f));
				fractalIcon.SetSize(new Vector2(64f, 64f));
				GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
				gUIImage.SetPosition(new Vector2(base.size.x / 2f + 50f, 118f));
				gUIImage.SetSize(new Vector2(266f, 104f));
				gUIImage.TextureSource = "communication_bundle|fractal_turn_in_quantity_box";
				Add(gUIImage);
				searchField = GUIControl.CreateControl<GUITextField>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
				searchField.Text = "10";
				searchField.SetPosition(gUIImage.Position + new Vector2(40f, 5f));
				searchField.SetSize(new Vector2(200f, 104f));
				searchField.ControlName = "searchFieldForMySquadGadget";
				searchField.SetupText(GUIFontManager.SupportedFontEnum.Komica, 22, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
				searchField.WordWrap = false;
				searchField.Changed += delegate
				{
					Regex regex = new Regex("[^0-9]");
					searchField.Text = regex.Replace(searchField.Text, string.Empty);
					if (searchField.Text != string.Empty)
					{
						searchField.Text = Mathf.Min(int.Parse(searchField.Text), _balance).ToString();
					}
				};
				searchField.LostFocus += delegate
				{
					if (!alreadyTurnedIn)
					{
						searchField.Text = Mathf.Max(int.Parse(searchField.Text), fractals.MaxFractals).ToString();
					}
				};
				Add(searchField);
			}
			else
			{
				TitleText = "#WATCHER_COLLECT_MORE_FRACTALS";
				Text = string.Empty;
			}
		}
		else
		{
			TitleText = string.Format(AppShell.Instance.stringTable["#WATCHER_FRACAL_UNDER_TURN_IN_THRESHOLD"], fractals.MaxFractals);
			Text = string.Format(AppShell.Instance.stringTable["#WATCHER_FRACTAL_BALANCE_UNDER_THRESHOLD"], _balance.ToString());
		}
		Add(fractalIcon);
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x + 100f, vector.y));
		FinalizeAllUIPositioning();
	}

	public override void OnOk()
	{
		if (_canTurnIn && _balance > 0 && searchField.Text != string.Empty && int.Parse(searchField.Text) > 0)
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("fractal_type_id", (int)_owner.fractalType);
			wWWForm.AddField("amount", searchField.Text);
			AppShell.Instance.WebService.StartRequest("resources$users/turn_in_fractals.py", delegate(ShsWebResponse innerResponse)
			{
				if (innerResponse.Status == 200)
				{
					EmotesDefinition.EmoteDefinition emoteByCommand = EmotesDefinition.Instance.GetEmoteByCommand("cheer");
					if (emoteByCommand != null)
					{
						Object @object = Object.FindObjectOfType(typeof(PlayerInputController));
						if (@object != null)
						{
							Component component = @object as Component;
							if (component != null && (bool)component.gameObject)
							{
								AppShell.Instance.EventMgr.Fire(component.gameObject, new EmoteMessage(component.gameObject, emoteByCommand.id));
							}
						}
					}
					_owner.PlayTurnInSequence();
				}
			}, wWWForm.data);
		}
		base.OnOk();
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		watcherIcon = null;
		watcherPortraitFrame = null;
		fractalIcon = null;
		searchField = null;
		_owner.InUse = false;
	}
}
