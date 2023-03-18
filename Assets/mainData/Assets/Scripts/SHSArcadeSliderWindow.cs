using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSArcadeSliderWindow : GUIControlWindow
{
	public delegate void SelectedItemChangedDelegate();

	public static readonly Vector2 SLIDE_ITEM_SIZE = new Vector2(256f, 256f);

	public static readonly Vector2 VIEW_WINDOW_SIZE = new Vector2(549f, 301f);

	public static readonly Vector2 CONTENT_WINDOW_SIZE = new Vector2(549f, 250f);

	public ArcadeSlider slider;

	public List<SlideArcadeItem> gamesList;

	private readonly GUISimpleControlWindow contentWindow;

	private SlideArcadeItem selectedItem;

	private LinkedList<SlideArcadeItem> Front = new LinkedList<SlideArcadeItem>();

	private LinkedList<SlideArcadeItem> Back = new LinkedList<SlideArcadeItem>();

	private float MinFront;

	private float MinBack;

	public float fullSizeMod = 1f;

	private bool disableAnimation;

	public AnimClip centerAnimation;

	private bool slidingAnimationInProgress;

	public SlideArcadeItem SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			if (slider.IsEnabled && value != selectedItem && selectedItem != null)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("shop_slider_tick"));
			}
			if (selectedItem != value)
			{
				selectedItem = value;
				if (this.SelectedItemChanged != null)
				{
					this.SelectedItemChanged();
				}
			}
		}
	}

	public bool SlidingAnimationInProgress
	{
		get
		{
			return slidingAnimationInProgress;
		}
	}

	public event SelectedItemChangedDelegate SelectedItemChanged;

	public SHSArcadeSliderWindow()
	{
		contentWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(CONTENT_WINDOW_SIZE, new Vector2(0f, 4f));
		gamesList = getGamesList();
		slider = new ArcadeSlider();
		slider.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		ArcadeSlider arcadeSlider = slider;
		Vector2 vIEW_WINDOW_SIZE = VIEW_WINDOW_SIZE;
		arcadeSlider.SetSize(vIEW_WINDOW_SIZE.x, 56f);
		slider.Min = 1f;
		slider.Max = gamesList.Count;
		slider.UseMouseWheelScroll = true;
		slider.MouseScrollWheelAmount = 1f;
		slider.Value = 0f;
		slider.Changed += slider_Changed;
		slider.SetTooltipForArrows();
		foreach (SlideArcadeItem games in gamesList)
		{
			contentWindow.Add(games);
		}
		Add(contentWindow);
		Add(slider);
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		SetSize(VIEW_WINDOW_SIZE);
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-121f, -83f));
	}

	public override void OnShow()
	{
		slider.Value = Mathf.Round(slider.Value);
		slider.FireChanged();
		base.OnShow();
	}

	public void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		if (gamesList.Count == 0)
		{
			return;
		}
		MinFront = 2f;
		MinBack = 2f;
		Front.Clear();
		Back.Clear();
		for (int i = 0; i < gamesList.Count; i++)
		{
			float num = (float)i - (slider.Value - 1f);
			gamesList[i].Move(num, fullSizeMod, this);
			if (num >= 0f)
			{
				Front.AddLast(gamesList[i]);
				MinFront = Math.Min(Math.Abs(num), MinFront);
			}
			else
			{
				Back.AddFirst(gamesList[i]);
				MinBack = Math.Min(Math.Abs(num), MinBack);
			}
		}
		contentWindow.ControlList.Clear();
		if (MinFront < MinBack)
		{
			SelectedItem = Front.First.Value;
			AddAllToContentWindow(Back);
			AddAllToContentWindow(Front);
		}
		else
		{
			SelectedItem = Back.First.Value;
			AddAllToContentWindow(Front);
			AddAllToContentWindow(Back);
		}
		if (slider.IsDragging && !base.AnimationInProgress)
		{
			base.AnimationPieceManager.ClearAll();
			centerAnimation = null;
		}
		if (!slider.IsDragging && !disableAnimation)
		{
			AnimateCenter();
		}
	}

	public void AnimateCenter()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		if (!base.AnimationInProgress)
		{
			base.AnimationPieceManager.ClearAll();
			centerAnimation = SHSArcadeAnimations.CenterSlider(SelectedItem, gamesList.IndexOf(SelectedItem) + 1, slider.Value, slider, AnimateSlider);
			centerAnimation.OnFinished += (Action)(object)new Action(centerAnimation_OnFinished);
			base.AnimationPieceManager.Add(centerAnimation);
			slidingAnimationInProgress = true;
		}
	}

	private void centerAnimation_OnFinished()
	{
		slidingAnimationInProgress = false;
	}

	public void AnimateSlider(float x)
	{
		disableAnimation = true;
		slider.Value = x;
		disableAnimation = false;
	}

	private void AddAllToContentWindow(LinkedList<SlideArcadeItem> toAdd)
	{
		while (toAdd.Count > 0)
		{
			contentWindow.ControlList.Add(toAdd.Last.Value);
			toAdd.RemoveLast();
		}
	}

	public void SlideItemClick(SlideArcadeItem item)
	{
		SelectedItem = item;
		AnimateCenter();
	}

	private List<SlideArcadeItem> getGamesList()
	{
		List<SlideArcadeItem> list = new List<SlideArcadeItem>();
		foreach (ArcadeGame value in AppShell.Instance.ArcadeManager.Games.Values)
		{
			list.Add(new SlideArcadeItem(this, value));
		}
		return list;
	}
}
