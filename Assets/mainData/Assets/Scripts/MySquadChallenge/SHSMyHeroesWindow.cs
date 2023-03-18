using System.Collections.Generic;
using UnityEngine;

namespace MySquadChallenge
{
	public class SHSMyHeroesWindow : GUISimpleControlWindow
	{
		private SHSHeroSelectionWindow _heroSelection;

		public SHSHeroSelectionWindow HeroSelection
		{
			get
			{
				return _heroSelection;
			}
		}

		public SHSMyHeroesWindow(List<SHSHeroSelectionItem> heroesToDisplay)
		{
			SetSize(340f, 502f);
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			GUISlider gUISlider = GUIControl.CreateControlAbsolute<GUISlider>(new Vector2(50f, 380f), new Vector2(281f, 66f));
			gUISlider.ArrowsEnabled = true;
			Add(gUISlider);
			_heroSelection = new SHSHeroSelectionWindow(gUISlider);
			_heroSelection.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(50f, 90f));
			_heroSelection.AddList(heroesToDisplay);
			_heroSelection.SortItemList();
			Add(_heroSelection);
			GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(266f, 54f), new Vector2(25f, 50f));
			gUIImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_searchfield";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(75f, 75f), new Vector2(220f, 38f));
			gUIImage2.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
			Add(gUIImage2);
			GUITextField searchField = GUIControl.CreateControlAbsolute<GUITextField>(new Vector2(192f, 54f), new Vector2(40f, 48f));
			searchField.ControlName = "searchFieldForMySquadGadget";
			searchField.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
			searchField.WordWrap = false;
			searchField.Changed += delegate
			{
				_heroSelection.Sort(searchField.Text);
			};
			Add(searchField);
		}

		public static List<SHSHeroSelectionItem> GetHeroListFromProfile(UserProfile profile, SHSHeroSelectionItem.OnHeroClick callback)
		{
			List<SHSHeroSelectionItem> list = new List<SHSHeroSelectionItem>();
			if (profile == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
			{
				list.Add(new SHSHeroSelectionItem(availableCostume.Value, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active, callback));
			}
			return list;
		}

		public static void AddShopHeroesToHeroList(SHSHeroSelectionItem.OnHeroClick callback, List<SHSHeroSelectionItem> heroList)
		{
			List<OwnableDefinition> ownablesByCategory = OwnableDefinition.getOwnablesByCategory(OwnableDefinition.Category.Hero);
			using (List<OwnableDefinition>.Enumerator enumerator = ownablesByCategory.GetEnumerator())
			{
				OwnableDefinition def;
				while (enumerator.MoveNext())
				{
					def = enumerator.Current;
					if (def.released != 0 && heroList.TrueForAll(delegate(SHSHeroSelectionItem item)
					{
						return item.HeroName != def.name;
					}))
					{
						heroList.Add(new SHSHeroSelectionItem(new HeroPersisted(def.name), SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Special, callback));
					}
				}
			}
		}

		public static void AddAllHeroesToHeroList(SHSHeroSelectionItem.OnHeroClick callback, List<SHSHeroSelectionItem> heroList)
		{
			if (!(AppShell.Instance == null) && AppShell.Instance.NewShoppingManager != null)
			{
				using (List<OwnableDefinition>.Enumerator enumerator = OwnableDefinition.allOwnables.GetEnumerator())
				{
					OwnableDefinition ownable;
					while (enumerator.MoveNext())
					{
						ownable = enumerator.Current;
						if (ownable.category == OwnableDefinition.Category.Hero && ownable.released != 0 && heroList.TrueForAll(delegate(SHSHeroSelectionItem item)
						{
							return item.HeroName != ownable.name;
						}))
						{
							heroList.Add(new SHSHeroSelectionItem(new HeroPersisted(ownable.name), SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Special, callback));
						}
					}
				}
			}
		}
	}
}
