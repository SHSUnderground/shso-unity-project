using UnityEngine;

public class SHSTicketholioWindow : GUINotificationWindow
{
	private GUIImage ticketImage;

	private float fadeStartTime = 0.1f;

	private float fadeDurationTime = 1f;

	private Vector2 startLocation;

	private Vector2 endLocation;

	private float rotationDir = 1f;

	private float rotationDirRange = 4f;

	private float scaleReductionBase = 0.5f;

	private float alphaReducationBase = 1.6f;

	public SHSTicketholioWindow()
		: this(Vector2.zero, 1)
	{
	}

	public SHSTicketholioWindow(Vector2 endLocation, int ticketsToShow)
	{
		this.endLocation = endLocation;
		rotationDir = Random.Range(0f - rotationDirRange, rotationDirRange);
		ticketImage = new GUIImage();
		ticketImage.SetPositionAndSize(QuickSizingHint.ParentSize);
		ticketImage.SetSize(new Vector2(83f, 51f));
		ticketImage.TextureSource = "notification_bundle|L_ticket_single";
		Add(ticketImage, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
	}

	public override void OnShow()
	{
		base.OnShow();
		Vector2 vector = new Vector2(GUIManager.ScreenRect.width / 2.1f, GUIManager.ScreenRect.height / 2.1f);
		startLocation = new Vector2(vector.x - Random.Range(5f, vector.x * 0.25f), GUIManager.ScreenRect.height - vector.y + Random.Range(15f, vector.y * 0.25f));
		SetPositionAndSize(AnchorAlignmentEnum.Middle, startLocation, new Vector2(73f, 74f));
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_tickets"));
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		float num2 = num - fadeStartTime;
		if (num > fadeStartTime)
		{
			ticketImage.Alpha = 1f - num2 / fadeDurationTime * alphaReducationBase;
			ticketImage.Scale = 1f - num2 / fadeDurationTime * scaleReductionBase;
			rotation += rotationDir;
			Vector2 position = new Vector2(startLocation.x + (endLocation.x - startLocation.x) * (num2 / fadeDurationTime), startLocation.y + (endLocation.y - startLocation.y) * num2 / fadeDurationTime);
			SetPositionAndSize(AnchorAlignmentEnum.Middle, position, new Vector2(83f, 51f));
		}
		if (ticketImage.Alpha <= 0f)
		{
			Hide();
		}
	}
}
