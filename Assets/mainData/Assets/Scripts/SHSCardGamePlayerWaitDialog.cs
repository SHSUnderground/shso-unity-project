using UnityEngine;

public class SHSCardGamePlayerWaitDialog : SHSCommonDialogWindow
{
	private GUIImage[] loadingIcons;

	private int currentLoadingTexture = -1;

	private float loadingAnimationSpeed = 0.1f;

	private float currentTime;

	public SHSCardGamePlayerWaitDialog()
		: base("common_bundle|L_mshs_button_quit_text", typeof(SHSDialogCancelButton))
	{
		loadingIcons = new GUIImage[20];
		for (int i = 0; i < loadingIcons.Length; i++)
		{
			GUIImage gUIImage = loadingIcons[i] = new GUIImage();
			gUIImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(230f, 60f));
			gUIImage.SetSize(64f, 64f);
			gUIImage.IsVisible = true;
			gUIImage.Alpha = 0f;
			gUIImage.TextureSource = string.Format("common_bundle|loading_stars_{0:00}", i + 1);
			Add(gUIImage);
		}
		currentTime = Time.time;
	}

	public override void OnUpdate()
	{
		if (Time.time - currentTime > loadingAnimationSpeed)
		{
			for (int i = 0; i < loadingIcons.Length; i++)
			{
				loadingIcons[i].Alpha = 0f;
			}
			currentLoadingTexture++;
			if (currentLoadingTexture >= loadingIcons.Length)
			{
				currentLoadingTexture = 0;
			}
			loadingIcons[currentLoadingTexture].Alpha = 1f;
			currentTime = Time.time;
		}
		base.OnUpdate();
	}
}
