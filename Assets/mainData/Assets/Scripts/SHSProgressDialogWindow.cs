using System;
using UnityEngine;

public class SHSProgressDialogWindow : SHSCommonDialogWindow
{
	private GUILabel textLabel;

	private GUIDrawTexture[] loadingIcons;

	private int currentLoadingTexture = -1;

	private float loadingAnimationSpeed = 0.1f;

	private float currentTime;

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
			if (textLabel != null)
			{
				textLabel.Text = Text;
			}
		}
	}

	public SHSProgressDialogWindow()
	{
		loadingIcons = new GUIDrawTexture[7];
		for (int i = 0; i < loadingIcons.Length; i++)
		{
			GUIDrawTexture gUIDrawTexture = loadingIcons[i] = new GUIDrawTexture();
			gUIDrawTexture.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(55 + i * 45, -125f));
			gUIDrawTexture.SetSize(45f, 45f);
			gUIDrawTexture.IsVisible = true;
			gUIDrawTexture.Alpha = 0f;
			gUIDrawTexture.TextureSource = "GUI/loading/loading_star";
			Add(gUIDrawTexture);
		}
		textLabel = new GUILabel();
		textLabel.SetPositionAndSize(61f, 57f, 305f, 168f);
		textLabel.Text = "Fourscore and seven years ago, our forefathers set forth on this continent a new nation, conceived in liberty.";
		textLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(0, 0, 0), TextAnchor.UpperLeft);
		textLabel.Bold = true;
		Add(textLabel);
		cancelButton = new SHSDialogCancelButton();
		cancelButton.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(90f, -25f));
		cancelButton.SetSize(104f, 104f);
		currentTime = Time.time;
	}

	public override void OnUpdate()
	{
		if (Time.time - currentTime > loadingAnimationSpeed)
		{
			for (int i = 0; i < loadingIcons.Length; i++)
			{
				loadingIcons[i].Alpha = 0.1f;
			}
			currentLoadingTexture++;
			if (currentLoadingTexture >= loadingIcons.Length)
			{
				currentLoadingTexture = 0;
			}
			loadingIcons[currentLoadingTexture].Alpha = 1f;
			for (int j = 1; j < 3; j++)
			{
				int num = currentLoadingTexture - j;
				if (num < 0)
				{
					num = loadingIcons.Length + (currentLoadingTexture - j);
				}
				loadingIcons[num].Alpha = 0.2f + 1f / (float)Math.Pow(2.0, (float)j);
			}
			currentTime = Time.time;
		}
		base.OnUpdate();
	}

	public override void OnShow()
	{
		base.OnShow();
		textLabel.Text = Text;
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}
}
