using UnityEngine;

public class EmoteChatAnim : SHSAnimations
{
	public static AnimClip ToOpenChat(NewEmoteChatBar bar)
	{
		return SlideOpenChatWidget(bar, 0.4f, true) ^ SlideEmoteBar(bar, 0.4f, false);
	}

	public static AnimClip ToEmoteChat(NewEmoteChatBar bar)
	{
		return SlideOpenChatWidget(bar, 0.4f, false) ^ SlideEmoteBar(bar, 0.4f, true);
	}

	public static AnimClip SlideEmoteBar(NewEmoteChatBar bar, float animationTime, bool toEmoteBar)
	{
		Vector2 start = (!toEmoteBar) ? EmoteBarEmotes.VISIBLE_OFFSET : EmoteBarEmotes.HIDDEN_OFFSET;
		Vector2 end = (!toEmoteBar) ? EmoteBarEmotes.HIDDEN_OFFSET : EmoteBarEmotes.VISIBLE_OFFSET;
		AnimPath path = GenericPaths.LinearWithBounce(0f, 1f, animationTime);
		return Custom.Function(path, delegate(float t)
		{
			bar.iconBar.Offset = Vector2.Lerp(start, end, t);
		});
	}

	public static AnimClip SlideOpenChatWidget(NewEmoteChatBar bar, float animationTime, bool toOpenChat)
	{
		Vector2 start = (!toOpenChat) ? OpenChatWidget.VISIBLE_OFFSET : OpenChatWidget.HIDDEN_OFFSET;
		Vector2 end = (!toOpenChat) ? OpenChatWidget.HIDDEN_OFFSET : OpenChatWidget.VISIBLE_OFFSET;
		AnimPath path = GenericPaths.LinearWithBounce(0f, 1f, animationTime);
		return Custom.Function(path, delegate(float t)
		{
			bar.openChatWidget.Offset = Vector2.Lerp(start, end, t);
		});
	}

	public static float LocationX(float loc, float offset)
	{
		return Mathf.Pow(loc, 0.9f) * 62f + offset;
	}

	public static float LocationY(float loc, float offset)
	{
		return loc * offset;
	}
}
