using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSCharacterSelect : SHSGadget.GadgetLeftWindow
{
	public class CharacterSelection : SHSSelectionWindow<CharacterItem, SHSItemLoadingWindow>
	{
		public CharacterSelection(GUISlider slider)
			: base(slider, SelectionWindowType.ThreeAcross)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		}

		public void Sort(string sort)
		{
			string text = GetSearchName(sort);
			bool flag = false;
			while (!flag)
			{
				foreach (CharacterItem item in items)
				{
					item.active = item.NameMatch(text);
					if (item.active)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (text == string.Empty)
					{
						return;
					}
					text = text.Substring(0, text.Length - 1);
				}
			}
			RequestARefresh();
		}
	}

	public class CharacterItemHeroHead : GUIButton
	{
		private HeroPersisted heroData;

		[CompilerGenerated]
		private bool _003CForcedHover_003Ek__BackingField;

		public bool ForcedHover
		{
			[CompilerGenerated]
			get
			{
				return _003CForcedHover_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CForcedHover_003Ek__BackingField = value;
			}
		}

		public override bool Hover
		{
			get
			{
				return base.Hover || ForcedHover;
			}
		}

		public HeroPersisted HeroData
		{
			get
			{
				return heroData;
			}
			set
			{
				heroData = value;
			}
		}
	}

	public class CharacterItem : SHSSelectionItem<SHSItemLoadingWindow>, IComparable<CharacterItem>
	{
		public CharacterItemHeroHead heroHead;

		private string name;

		private string searchName;

		[CompilerGenerated]
		private bool _003CAvailable_003Ek__BackingField;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public bool Available
		{
			[CompilerGenerated]
			get
			{
				return _003CAvailable_003Ek__BackingField;
			}
			[CompilerGenerated]
			protected set
			{
				_003CAvailable_003Ek__BackingField = value;
			}
		}

		public event HeroClickedDelegate HeroClicked;

		public CharacterItem(HeroPersisted hero, HeroClickedDelegate heroClickDelegate, SelectionState defaultState)
		{
			name = hero.Name;
			searchName = GetSearchName(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterName);
			item = new SHSItemLoadingWindow();
			item.HitTestType = HitTestTypeEnum.Transparent;
			itemSize = new Vector2(103f, 103f);
			currentState = defaultState;
			heroHead = GUIControl.CreateControlAbsolute<CharacterItemHeroHead>(new Vector2(103f, 103f), new Vector2(0f, 0f));
			heroHead.StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + name, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			heroHead.HeroData = hero;
			item.Add(heroHead);
			if (defaultState.ToString() == "Special")
			{
				GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(32f, 35f), new Vector2(55f, 55f));
				gUIImage.TextureSource = "persistent_bundle|gadget_character_icon_lock";
				item.Add(gUIImage);
			}
			if (heroClickDelegate != null)
			{
				this.HeroClicked = (HeroClickedDelegate)Delegate.Combine(this.HeroClicked, heroClickDelegate);
			}
			heroHead.HitTestType = HitTestTypeEnum.Circular;
			heroHead.HitTestSize = new Vector2(0.6f, 0.6f);
			heroHead.Click += OnHeroHeadClicked;
			heroHead.ToolTip = new NamedToolTipInfo(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterName);
			Available = true;
		}

		public void OnHeroHeadClicked(GUIControl sender, GUIClickEvent EventData)
		{
			CspUtils.DebugLog("OnHeroHeadClicked ");
			if (this.HeroClicked != null)
			{
				this.HeroClicked(name);
				TriggerSelectedVO();
			}
		}

		public int CompareTo(CharacterItem other)
		{
			if (currentState == other.currentState)
			{
				return name.CompareTo(other.name);
			}
			if (currentState.ToString() == "Special")
			{
				return 1;
			}
			if (other.currentState.ToString() == "Special")
			{
				return -1;
			}
			return name.CompareTo(other.name);
		}

		public bool NameMatch(string partName)
		{
			return searchName.Contains(partName);
		}

		public bool TriggerPreSelectVO()
		{
			ResolvedVOAction vO = VOManager.Instance.GetVO("pre_select", null, new VOInputString(name));
			if (vO != null)
			{
				vO.CustomHandler = new PreSelectVOActionHandler(this);
				VOManager.Instance.PlayResolvedVO(vO);
				return true;
			}
			return false;
		}

		public void TriggerSelectedVO()
		{
			bool flag = UnityEngine.Random.value <= 0.2f;
			bool flag2 = false;
			if (flag)
			{
				flag2 = (VOManager.Instance.PlayVO("alt_character_name", new VOInputString(name)) != null);
			}
			if (!flag || !flag2)
			{
				VOManager.Instance.PlayVO("character_name", new VOInputString(name));
			}
		}
	}

	public delegate void HeroClickedDelegate(string heroName);

	private const int recentVOCharactersQueueSize = 4;

	private const float alternateHeroChance = 0.2f;

	private CharacterSelection characterSelection;

	private GUIImage searchBackground;

	private GUIImage searchGlass;

	private GUIImage bkg;

	private GUIImage bkg2;

	private GUITextField search;

	private GUISlider slider;

	private float voTimer = -1f;

	private readonly FRange voTimerPeriodSeconds = new FRange(6f, 10f);

	private readonly FRange voTimerInitialDelay = new FRange(3f, 4f);

	private Queue<CharacterItem> recentVOCharacters = new Queue<CharacterItem>();

	public Vector2 CharacterSelectionPosition
	{
		get
		{
			return characterSelection.Position;
		}
	}

	public Vector2 CharacterSelectionSize
	{
		get
		{
			return characterSelection.Size;
		}
	}

	public event HeroClickedDelegate HeroClicked;

	public SHSCharacterSelect(UserProfile profile, HeroClickedDelegate heroClickDelegate)
		: this(GetHeroListFromProfile(profile, heroClickDelegate))
	{
	}

	public SHSCharacterSelect(UserProfile profile, HeroClickedDelegate heroClickDelegate, string frontFrameTextureSource, string backFrameTextureSource)
		: this(GetHeroListFromProfile(profile, heroClickDelegate), frontFrameTextureSource, backFrameTextureSource)
	{
	}

	public SHSCharacterSelect(List<CharacterItem> heroesToDisplay)
		: this(heroesToDisplay, "persistent_bundle|leftselectwindow_frontframe", "persistent_bundle|leftselectwindow_backframe")
	{
	}

	public SHSCharacterSelect(List<CharacterItem> heroesToDisplay, string frontFrameTextureSource, string backFrameTextureSource)
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), Vector2.zero);
		bkg.TextureSource = backFrameTextureSource;
		bkg2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), Vector2.zero);
		bkg2.TextureSource = frontFrameTextureSource;
		slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 400f), new Vector2(131f, 30f));
		characterSelection = new CharacterSelection(slider);
		characterSelection.AddList(heroesToDisplay);
		characterSelection.SortItemList();
		characterSelection.SetSize(227f, 380f);
		characterSelection.SetPosition(54f, 89f);
		foreach (CharacterItem item in heroesToDisplay)
		{
			item.HeroClicked += OnHeroClicked;
		}
		searchBackground = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(253f, 55f), new Vector2(0f, -191f));
		searchBackground.TextureSource = "persistent_bundle|curved_searchbar";
		searchBackground.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		searchGlass = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(81f, 81f), new Vector2(102f, -190f));
		searchGlass.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
		search = GUIControl.CreateControlFrameCentered<GUITextField>(new Vector2(178f, 55f), new Vector2(-20f, -191f));
		search.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(81, 82, 81), TextAnchor.MiddleLeft);
		search.Rotation = -3f;
		search.WordWrap = false;
		search.ToolTip = new NamedToolTipInfo("#characterselect_text_search_tt");
		search.Changed += delegate
		{
			characterSelection.Sort(search.Text);
		};
		Add(bkg);
		Add(characterSelection);
		Add(bkg2);
		Add(searchBackground);
		Add(searchGlass);
		Add(search);
		Add(slider);
		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, this);
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.3f, this);
		voTimer = Time.time + voTimerInitialDelay.RandomValue;
	}

	public void AddHero(HeroPersisted item, HeroClickedDelegate onClick)
	{
		CharacterItem characterItem = new CharacterItem(item, onClick, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active);
		characterSelection.AddItem(characterItem);
		characterSelection.SortItemList();
		characterItem.HeroClicked += OnHeroClicked;
	}

	public bool HasHero(string heroName)
	{
		return characterSelection.Find(delegate(CharacterItem i)
		{
			return heroName == i.heroHead.HeroData.Name;
		}) != null;
	}

	public void SetCharacterSelectionSize(Vector2 size)
	{
		characterSelection.SetSize(size);
	}

	public void SetCharacterSelectionPosition(Vector2 position)
	{
		characterSelection.SetPosition(position);
	}

	public void ApplyOffsetToCharacterSelectWindow(Vector2 offset)
	{
		for (int i = 0; i < controlList.Count; i++)
		{
			GUIControl gUIControl = (GUIControl)controlList[i];
			Vector2 offset2 = gUIControl.Offset;
			float x = offset2.x + offset.x;
			Vector2 offset3 = gUIControl.Offset;
			gUIControl.Offset = new Vector2(x, offset3.y + offset.y);
		}
	}

	private void OnHeroClicked(string heroName)
	{
		if (this.HeroClicked != null)
		{
			this.HeroClicked(heroName);
		}
	}

	public void EnableHero(string heroName, bool enable)
	{
		CharacterItem characterItem = characterSelection.Find(delegate(CharacterItem item)
		{
			return item.Name == heroName;
		});
		if (characterItem != null)
		{
			characterItem.heroHead.IsEnabled = enable;
		}
	}

	public void EnableAllHero(bool enable)
	{
		foreach (CharacterItem item in characterSelection.items)
		{
			item.heroHead.IsEnabled = enable;
		}
	}

	public bool IsHeroEnabled(string heroName)
	{
		CharacterItem characterItem = characterSelection.Find(delegate(CharacterItem item)
		{
			return item.Name == heroName;
		});
		if (characterItem == null)
		{
			return false;
		}
		return characterItem.heroHead.IsEnabled;
	}

	public override void Update()
	{
		base.Update();
		if (Time.time >= voTimer && IsVisible && !slider.IsDragging)
		{
			voTimer = Time.time + voTimerPeriodSeconds.RandomValue;
			PlayCharacterSelectVO();
		}
	}

	protected void PlayCharacterSelectVO()
	{
		List<CharacterItem> visibleItems = characterSelection.GetVisibleItems(false);
		List<CharacterItem> list = new List<CharacterItem>();
		foreach (CharacterItem item in visibleItems)
		{
			if (item.Available)
			{
				list.Add(item);
			}
		}
		foreach (CharacterItem recentVOCharacter in recentVOCharacters)
		{
			list.Remove(recentVOCharacter);
		}
		while (list.Count == 0 && recentVOCharacters.Count > 0)
		{
			CharacterItem characterItem = recentVOCharacters.Dequeue();
			if (characterItem.Available && visibleItems.Contains(characterItem))
			{
				list.Add(characterItem);
			}
		}
		bool flag = false;
		while (list.Count > 0 && !flag)
		{
			CharacterItem characterItem2 = list[UnityEngine.Random.Range(0, list.Count)];
			if (characterItem2.TriggerPreSelectVO())
			{
				flag = true;
				recentVOCharacters.Enqueue(characterItem2);
				if (recentVOCharacters.Count > 4)
				{
					recentVOCharacters.Dequeue();
				}
			}
			else
			{
				list.Remove(characterItem2);
			}
		}
		if (!flag && recentVOCharacters.Count > 0)
		{
			recentVOCharacters.Dequeue();
		}
	}

	public static List<CharacterItem> GetHeroListFromProfile(UserProfile profile, HeroClickedDelegate heroClickDelegate)
	{
		List<CharacterItem> list = new List<CharacterItem>();
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
		{
			HeroPersisted value = null;
			if (!profile.AvailableCostumes.TryGetValue(availableCostume.Key, out value))
			{
				CspUtils.DebugLog("Hero <" + availableCostume.Key + "> being added to hero UI was not found in the hero collection!");
			}
			else
			{
				list.Add(new CharacterItem(value, heroClickDelegate, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active));
			}
		}
		return list;
	}

	public static string GetSearchName(string name)
	{
		name = name.ToLowerInvariant();
		name = name.Replace("_", string.Empty);
		name = name.Replace("'", string.Empty);
		name = name.Replace("-", string.Empty);
		name = name.Replace(" ", string.Empty);
		name = name.Replace(".", string.Empty);
		name = name.Replace("!", string.Empty);
		return name;
	}
}
