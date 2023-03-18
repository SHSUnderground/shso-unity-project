using System;
using UnityEngine;

public class SHSCardGameDealerChatWindow : GUIControlWindow
{
	private GUIDrawTexture centerPiece;

	private GUIDrawTexture rightPiece;

	private GUILabel label;

	public string Text
	{
		get
		{
			if (label != null)
			{
				return label.Text;
			}
			return null;
		}
	}

	public SHSCardGameDealerChatWindow()
	{
		SetSize(new Vector3(680f, 48f));
		SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(4f, 4f));
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, false);
		Alpha = 0f;
		GUIDrawTexture gUIDrawTexture = new GUIDrawTexture();
		gUIDrawTexture.SetSize(new Vector2(27f, 46f));
		gUIDrawTexture.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, Vector2.zero);
		gUIDrawTexture.TextureSource = "cardgame_bundle|mshs_cg_message_box_left";
		Add(gUIDrawTexture);
		centerPiece = new GUIDrawTexture();
		centerPiece.SetSize(new Vector2(1f, 46f));
		centerPiece.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(27f, 0f));
		centerPiece.TextureSource = "cardgame_bundle|mshs_cg_message_box_center";
		Add(centerPiece);
		rightPiece = new GUIDrawTexture();
		rightPiece.SetSize(new Vector2(27f, 46f));
		rightPiece.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(28f, 0f));
		rightPiece.TextureSource = "cardgame_bundle|mshs_cg_message_box_right";
		Add(rightPiece);
		label = new GUILabel();
		label.SetSize(new Vector2(600f, 46f));
		label.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(14f, 0f));
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
		label.WordWrap = false;
		Add(label);
	}

	public override void OnAdded(IGUIContainer addedTo)
	{
		base.OnAdded(addedTo);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.DealerChat>(OnDealerChat);
	}

	public override void OnRemoved(IGUIContainer removedFrom)
	{
		base.OnRemoved(removedFrom);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.DealerChat>(OnDealerChat);
	}

	protected void OnDealerChat(CardGameEvent.DealerChat evt)
	{
		switch (evt.msgId)
		{
		case 129:
		case 130:
		case 131:
		case 132:
		case 133:
		case 134:
		case 135:
		case 136:
		case 150:
		case 151:
		case 152:
		case 153:
		case 154:
		case 155:
		case 156:
		case 300:
		case 301:
		case 600:
		case 601:
			if (!string.IsNullOrEmpty(evt.text))
			{
				string text = AppShell.Instance.stringTable[evt.text];
				string text2 = (evt.args == null) ? text : string.Format(text, evt.args);
				CardGameEvent.DealerChat.MessageType msgType = evt.msgType;
				if (msgType != CardGameEvent.DealerChat.MessageType.Prompt)
				{
				}
				AddMessage(text2);
				if (Alpha == 0f)
				{
					base.AnimationPieceManager.Add(AnimClipBuilder.Absolute.Alpha(AnimPath.Linear(0f, 1f, 1f), this));
				}
				else if (Alpha < 1f)
				{
					base.AnimationPieceManager.ClearAll();
					base.AnimationPieceManager.Add(AnimClipBuilder.Absolute.Alpha(AnimPath.Linear(Alpha, 1f, 0.2f), this));
				}
			}
			break;
		}
	}

	private void AddMessage(string text)
	{
		CspUtils.DebugLog("Dealer chat: " + text);
		label.Text = text;
		int num = label.LongestLine - 18;
		centerPiece.SetSize(num, 45f);
		rightPiece.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(27 + num, 0f));
	}

	public void FadeOut(float animTime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimPath.Linear(Alpha, 0f, animTime), this);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			label.Text = string.Empty;
		};
		base.AnimationPieceManager.Add(animClip);
	}
}
