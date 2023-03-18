using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class QuestItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<QuestItem>
{
	public enum QuestType
	{
		OfTheDay,
		Regular,
		Unpurchased
	}

	private string questName;

	private QuestType questType;

	[CompilerGenerated]
	private bool _003CIsSelected_003Ek__BackingField;

	public string QuestName
	{
		get
		{
			return AppShell.Instance.stringTable[questName];
		}
		set
		{
			questName = value;
		}
	}

	public virtual CardQuest CardQuest
	{
		get
		{
			return null;
		}
		protected set
		{
		}
	}

	public bool IsSelected
	{
		[CompilerGenerated]
		get
		{
			return _003CIsSelected_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CIsSelected_003Ek__BackingField = value;
		}
	}

	public QuestItem(string QuestName, QuestType questType)
	{
		this.QuestName = QuestName;
		this.questType = questType;
		item = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(220f, 78f), Vector2.zero);
		itemSize = new Vector2(250f, 100f);
		IsSelected = false;
	}

	public int CompareTo(QuestItem other)
	{
		if (questType != other.questType)
		{
			return questType.CompareTo(other.questType);
		}
		return QuestName.CompareTo(other.QuestName);
	}

	public virtual void OnSelected()
	{
	}

	public virtual void OnDeselected()
	{
	}

	public void OnBecomeVisible(GUIControl sender)
	{
		if (IsSelected)
		{
			OnSelected();
		}
		else
		{
			OnDeselected();
		}
	}

	protected void AddSFXHandlers(GUIControl control)
	{
		control.MouseOver += PlayMouseOverSFX;
		control.MouseDown += PlayMouseDownSFX;
		control.MouseUp += PlayMouseUpSFX;
	}

	private void PlayMouseOverSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_hover_over"));
	}

	private void PlayMouseDownSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
	}

	private void PlayMouseUpSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_up"));
	}
}
