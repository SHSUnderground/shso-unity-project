using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSZoneSelectorSliderWindow : GUIControlWindow
{
	public delegate void SelectedItemChangedDelegate();

	public static readonly Vector2 SLIDE_ITEM_SIZE = new Vector2(1024f, 512f);

	public static readonly Vector2 SLIDER_WINDOW_SIZE = new Vector2(750f, 100f);

	public static readonly Vector2 CONTENT_WINDOW_SIZE = new Vector2(786f, 600f);

	public static readonly Vector2 WINDOW_SIZE = new Vector2(856f, 700f);

	public SHSZoneChooserSlider slider;

	public List<SHSSlideZoneItem> zonesList;

	private readonly GUISimpleControlWindow contentWindow;

	private SHSSlideZoneItem selectedItem;

	private LinkedList<SHSSlideZoneItem> Front = new LinkedList<SHSSlideZoneItem>();

	private LinkedList<SHSSlideZoneItem> Back = new LinkedList<SHSSlideZoneItem>();

	private float MinFront;

	private float MinBack;

	public float fullSizeMod = 1f;

	private bool disableAnimation;

	public AnimClip centerAnimation;

	private bool slidingAnimationInProgress;

	public SHSSlideZoneItem SelectedItem
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

	public SHSZoneSelectorSliderWindow()
	{
		contentWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(CONTENT_WINDOW_SIZE, new Vector2(0f, -19f));
		zonesList = getZoneList();
		slider = new SHSZoneChooserSlider();
		slider.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		SHSZoneChooserSlider sHSZoneChooserSlider = slider;
		Vector2 sLIDER_WINDOW_SIZE = SLIDER_WINDOW_SIZE;
		sHSZoneChooserSlider.SetSize(sLIDER_WINDOW_SIZE.x, 56f);
		slider.Offset = new Vector2(0f, -154f);
		slider.Min = 1f;
		slider.Max = zonesList.Count;
		slider.UseMouseWheelScroll = true;
		slider.MouseScrollWheelAmount = 1f;
		slider.Value = 0f;
		slider.Changed += slider_Changed;
		slider.SetTooltipForArrows();
		int num = FillZoneView();
		Add(contentWindow);
		Add(slider);
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		SetSize(WINDOW_SIZE);
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		slider.Value = num;
	}

	public int FillZoneView()
	{
		string text = null;
		text = (string)AppShell.Instance.SharedHashTable["SocialSpaceLevelCurrent"];
		contentWindow.ControlList.ForEach(Remove);
		int result = 0;
		int num = 0;
		foreach (SHSSlideZoneItem zones in zonesList)
		{
			num++;
			contentWindow.Add(zones);
			if (string.Compare(zones.Zone.launchKey, text, true) == 0)
			{
				result = num;
			}
		}
		return result;
	}

	public override void OnShow()
	{
		slider.Value = Mathf.Round(slider.Value);
		slider.FireChanged();
		base.OnShow();
	}

	public void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		if (zonesList.Count == 0)
		{
			return;
		}
		MinFront = 2f;
		MinBack = 2f;
		Front.Clear();
		Back.Clear();
		for (int i = 0; i < zonesList.Count; i++)
		{
			float num = (float)i - (slider.Value - 1f);
			zonesList[i].Move(num, fullSizeMod, this);
			if (num >= 0f)
			{
				Front.AddLast(zonesList[i]);
				MinFront = Math.Min(Math.Abs(num), MinFront);
			}
			else
			{
				Back.AddFirst(zonesList[i]);
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
			centerAnimation = SHSZoneSelectorAnimations.CenterSlider(SelectedItem, zonesList.IndexOf(SelectedItem) + 1, slider.Value, slider, AnimateSlider);
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

	private void AddAllToContentWindow(LinkedList<SHSSlideZoneItem> toAdd)
	{
		while (toAdd.Count > 0)
		{
			contentWindow.ControlList.Add(toAdd.Last.Value);
			toAdd.RemoveLast();
		}
	}

	public void SlideItemClick(SHSSlideZoneItem item)
	{
		if (SelectedItem == item)
		{
			((SHSZoneSelectorWindow)Parent).goButton.FireMouseClick(new GUIClickEvent());
		}
		SelectedItem = item;
		AnimateCenter();
	}

	private List<SHSSlideZoneItem> getZoneList()
	{
		List<SHSSlideZoneItem> list = new List<SHSSlideZoneItem>();
		SHSSlideZoneItem item = new SHSSlideZoneItem(this, new Zone("villainville", new Vector2(394f, 84f), "villainville", new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds)));
		list.Add(item);
		item = new SHSSlideZoneItem(this, new Zone("daily_bugle", new Vector2(394f, 90f), "daily_bugle", null));
		list.Add(item);
		item = new SHSSlideZoneItem(this, new Zone("baxter_plaza", new Vector2(394f, 90f), "baxter_building", new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds)));
		list.Add(item);
		item = new SHSSlideZoneItem(this, new Zone("asgard", new Vector2(301f, 98f), "asgard", new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds)));
		list.Add(item);
		return list;
	}
}
