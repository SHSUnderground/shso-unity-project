using System;
using UnityEngine;

public class NotificationWindow : GUISimpleControlWindow
{
	public enum BlockSizeType
	{
		Width,
		Height
	}

	protected GUIImageWithEvents _background;

	protected NotificationData.NotificationType _targetDataType;

	protected NotificationHUD _parent;

	protected NotificationData _data;

	public AnimClip animClip;

	protected bool _fadeInComplete;

	public NotificationWindow(NotificationData.NotificationType targetDataType)
	{
		_targetDataType = targetDataType;
	}

	public NotificationData getData()
	{
		return _data;
	}

	public virtual void init(NotificationHUD parent, NotificationData data)
	{
		if (parent != null)
		{
			_parent = parent;
		}
		_data = data;
		SetSize(new Vector2(231f, 100f));
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
	}

	public virtual void update()
	{
	}

	public virtual void activate()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Alpha = 0f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), this);
		AnimClip animClip2 = SHSAnimations.Generic.Wait(4f);
		AnimClip animClip3 = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), this);
		this.animClip = animClip;
		this.animClip |= animClip2;
		this.animClip |= animClip3;
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			_fadeInComplete = true;
		};
		animClip3.OnFinished += (Action)(object)(Action)delegate
		{
			_parent.notificationComplete(this);
		};
		_parent.animManager.Add(this.animClip);
	}

	public void resetFade(float delayDuration = 3f)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		if (_fadeInComplete)
		{
			Alpha = 1f;
			_parent.animManager.Remove(this.animClip);
			AnimClip animClip = SHSAnimations.Generic.Wait(delayDuration);
			AnimClip animClip2 = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), this);
			this.animClip = animClip;
			this.animClip |= animClip2;
			animClip2.OnFinished += (Action)(object)(Action)delegate
			{
				_parent.notificationComplete(this);
			};
			_parent.animManager.Add(this.animClip);
		}
	}

	public virtual bool absorb(NotificationData data)
	{
		return false;
	}

	protected void updateLabelSize(GUILabel label)
	{
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = label.Text;
		label.Size = label.Style.UnityStyle.CalcSize(gUIContent);
	}

	public static float GetTextBlockSize(GUILabel[] textLabels, BlockSizeType type)
	{
		float num = 0f;
		if (type == BlockSizeType.Width)
		{
			for (int i = 0; i < textLabels.Length; i++)
			{
				Vector2 vector = textLabels[i].Style.UnityStyle.CalcSize(new GUIContent(textLabels[i].Text));
				if (num < vector.x)
				{
					num = vector.x;
				}
			}
		}
		return num;
	}

	public static Vector2 GetTextBlockSize(GUILabel[] textLabels)
	{
		return new Vector2(GetTextBlockSize(textLabels, BlockSizeType.Width), GetTextBlockSize(textLabels, BlockSizeType.Height));
	}
}
