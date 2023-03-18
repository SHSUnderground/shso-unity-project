using System;
using UnityEngine;

public class NewEmoteChatBar : GUIDynamicWindow
{
	public OpenChatWidget openChatWidget;

	public GUISimpleControlWindow iconBar;

	public static bool CurrentVisibility = true;

	public static bool IsOpenChatVisible;

	private bool openChatVisible;

	private bool animationInProgress;

	public NewEmoteChatBar()
	{
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		EmoteGoodiesBar emoteBarEmotes = new EmoteGoodiesBar(this);
		Add(emoteBarEmotes);
		openChatWidget = new OpenChatWidget(this);
		Add(openChatWidget);
		base.AnimationOnOpen = delegate
		{
			return EmoteGoodiesBar.AnimOpenAndClose.Open(emoteBarEmotes);
		};
		base.AnimationOnClose = delegate
		{
			return EmoteGoodiesBar.AnimOpenAndClose.Close(emoteBarEmotes);
		};
		base.AnimationOpenFinished += delegate
		{
			base.AnimationPieceManager.Add(EmoteGoodiesBar.AnimOpenAndClose.AnimateOpenComplete(emoteBarEmotes));
		};
		iconBar = emoteBarEmotes;
	}

	public void RequestOpenOpenChat()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if (!animationInProgress && !base.AnimationInProgress)
		{
			if (openChatVisible)
			{
				openChatWidget.Focus();
				return;
			}
			animationInProgress = true;
			AnimClip animClip = EmoteChatAnim.ToOpenChat(this);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				animationInProgress = false;
				openChatWidget.Focus();
				openChatVisible = true;
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	public void RequestOpenEmoteChat()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		if (!animationInProgress && !base.AnimationInProgress)
		{
			animationInProgress = true;
			AnimClip animClip = EmoteChatAnim.ToEmoteChat(this);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				animationInProgress = false;
				openChatVisible = false;
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		IsOpenChatVisible = true;
	}

	public override void OnHide()
	{
		base.OnHide();
		IsOpenChatVisible = false;
	}
}
