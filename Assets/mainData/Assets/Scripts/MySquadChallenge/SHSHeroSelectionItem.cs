using System;
using UnityEngine;

namespace MySquadChallenge
{
	public class SHSHeroSelectionItem : SHSSelectionItem<SHSItemLoadingWindow>, IComparable<SHSHeroSelectionItem>
	{
		public delegate void OnHeroClick(string hero);

		private GUIButton _heroHead;

		private string _heroName;

		private string _heroSearchName;

		public GUIButton HeroHead
		{
			get
			{
				return _heroHead;
			}
		}

		public string HeroName
		{
			get
			{
				return _heroName;
			}
		}

		public SHSHeroSelectionItem(HeroPersisted hero, SelectionState defaultState, OnHeroClick callback)
		{
			_heroName = hero.Name;
			_heroSearchName = SHSHeroSelectionWindow.GetSearchName(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterName);
			item = new SHSItemLoadingWindow();
			item.HitTestType = GUIControl.HitTestTypeEnum.Transparent;
			itemSize = new Vector2(103f, 103f);
			currentState = defaultState;
			_heroHead = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(103f, 103f), Vector2.zero);
			_heroHead.StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + HeroName, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			_heroHead.HitTestType = GUIControl.HitTestTypeEnum.Circular;
			_heroHead.HitTestSize = new Vector2(0.6f, 0.6f);
			_heroHead.Click += delegate
			{
				if (callback != null)
				{
					callback(HeroName);
				}
			};
			_heroHead.ToolTip = new GUIControl.NamedToolTipInfo(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterName);
			item.Add(_heroHead);
			if (defaultState == SelectionState.Special)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(32f, 35f), new Vector2(24f, 18f));
				gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
				item.Add(gUIImage);
			}
			else
			{
				GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(39f, 32f), new Vector2(24f, 18f));
				gUIImage2.TextureSource = "mysquadgadget_bundle|L_mshs_mysquad_character_icon_level_container";
				item.Add(gUIImage2);
				GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(80f, 80f), new Vector2(23f, 19f));
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
				if (hero.Level == hero.MaxLevel)
				{
					gUILabel.Text = MySquadDataManager.GetMax();
					gUILabel.FontSize = 9;
				}
				else
				{
					gUILabel.Text = hero.Level.ToString();
				}
				item.Add(gUILabel);
			}
			if (hero.ShieldAgentOnly)
			{
				currentState = SelectionState.Subscription;
			}
		}

		public int CompareTo(SHSHeroSelectionItem other)
		{
			if (currentState == other.currentState)
			{
				return HeroName.CompareTo(other.HeroName);
			}
			if (currentState == SelectionState.Special)
			{
				return 1;
			}
			if (other.currentState == SelectionState.Special)
			{
				return -1;
			}
			return HeroName.CompareTo(other.HeroName);
		}

		public bool NameMatch(string partName)
		{
			return _heroSearchName.Contains(partName);
		}
	}
}
