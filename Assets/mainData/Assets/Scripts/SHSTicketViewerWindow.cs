using UnityEngine;

public class SHSTicketViewerWindow : GUINotificationWindow
{
	private const float fadeStartTime = 5f;

	private const float fadeDurationTime = 0.5f;

	private const int logoOffset = 20;

	private const int textBlockOffset = 20;

	private const int textLineOffset = 10;

	private GUIImage ticketBackground;

	private GUILabel ticketLabel;

	private GUILabel ticketNumLabel;

	private static int offlineProfileTicketUpdateFakeage;

	public SHSTicketViewerWindow()
	{
		CspUtils.DebugLog("CALL STACK GIMME");
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		GUIContent gUIContent = new GUIContent();
		Vector2 vector = default(Vector2);
		UserProfile profile = AppShell.Instance.Profile;
		float num = 0f;
		ticketBackground = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(225f, 94f), Vector2.zero);
		ticketBackground.Position = Vector2.zero;
		ticketBackground.TextureSource = "GUI/Notifications/gameworld_pickup_toast_tickets";
		ticketNumLabel = new GUILabel();
		ticketNumLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 26, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		GUILabel gUILabel = ticketNumLabel;
		string text;
		if (profile != null)
		{
			text = profile.Tickets.ToString();
		}
		else
		{
			int num2 = ++offlineProfileTicketUpdateFakeage;
			text = num2.ToString();
		}
		gUILabel.Text = text;
		gUIContent.text = ticketNumLabel.Text;
		vector = ticketNumLabel.Style.UnityStyle.CalcSize(gUIContent);
		num = vector.y;
		ticketNumLabel.Size = new Vector2(vector.x, vector.y);
		GUILabel gUILabel2 = ticketNumLabel;
		Vector2 size = ticketBackground.Size;
		gUILabel2.Position = new Vector2(size.x * 0.5f - vector.x * 0.5f + 20f, 20f);
		ticketLabel = new GUILabel();
		ticketLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		ticketLabel.Text = "#tickets";
		gUIContent.text = ticketLabel.Text;
		vector = ticketLabel.Style.UnityStyle.CalcSize(gUIContent);
		ticketLabel.Size = new Vector2(vector.x, vector.y);
		GUILabel gUILabel3 = ticketLabel;
		Vector2 size2 = ticketBackground.Size;
		float x = size2.x * 0.5f - vector.x * 0.5f + 20f;
		Vector2 position = ticketNumLabel.Position;
		gUILabel3.Position = new Vector2(x, position.y + num - 10f);
		Add(ticketBackground);
		Add(ticketLabel);
		Add(ticketNumLabel);
		Vector2 position2 = new Vector2(100f, 400f);
		Vector2 size3 = ticketBackground.Size;
		float x2 = size3.x;
		Vector2 size4 = ticketBackground.Size;
		SetPositionAndSize(position2, new Vector2(x2, size4.y));
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		return base.InitializeResources(reload);
	}

	public override void OnUpdate()
	{
		float num = Time.time - timeStarted;
		float num2 = num - 5f;
		if (num > 5f)
		{
			Alpha = 1f - num2 / 0.5f;
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}
}
