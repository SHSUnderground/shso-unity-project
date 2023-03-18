using System.Collections.Generic;
using UnityEngine;

namespace MySquadChallenge
{
	public class SHSHeroItemBar : SHSStatBar
	{
		private GUIDrawTexture[] _heroItems;

		private GUIDrawTexture[] _lockIcons;

		private static readonly int _MaxHeroItems = 5;

		private static readonly Vector2 _HeroItemSize = new Vector2(160f, 160f);

		private static readonly int[] _HeroItemRequiredLevels = new int[5]
		{
			2,
			4,
			6,
			8,
			10
		};

		private static readonly float[] _LockIconXOffsets = new float[5]
		{
			138f,
			185f,
			236f,
			278f,
			332f
		};

		private static readonly float[] _HeroItemXOffsets = new float[5]
		{
			112f,
			162f,
			212f,
			256f,
			304f
		};

		public SHSHeroItemBar()
		{
			CreateBarTitle("#HEROLIST_HEROITEMS");
			Vector2 size = _HeroItemSize * 0.3f;
			Vector2 size2 = SHSStatBar.LockIconSize * 0.91f;
			_heroItems = new GUIDrawTexture[_MaxHeroItems];
			_lockIcons = new GUIDrawTexture[_MaxHeroItems];
			for (int i = 0; i < _MaxHeroItems; i++)
			{
				_heroItems[i] = CreateIcon(size, new Vector2(_HeroItemXOffsets[i], 6f));
				_lockIcons[i] = CreateLockIcon(size2, new Vector2(_LockIconXOffsets[i], 18f), MySquadDataManager.GetUnlockAtLevelText(_HeroItemRequiredLevels[i]));
			}
		}

		public void SetHeroItems(string hero, int level)
		{
			List<ItemDefinition> theFiveItemUnlockables = MySquadDataManager.GetTheFiveItemUnlockables(hero);
			for (int i = 0; i < _MaxHeroItems; i++)
			{
				if (theFiveItemUnlockables.Count > i)
				{
					ItemDefinition itemDefinition = theFiveItemUnlockables[i];
					SetIconTexture(_heroItems[i], "items_bundle", itemDefinition.Icon);
					bool flag = _HeroItemRequiredLevels[i] <= level;
					_heroItems[i].IsEnabled = flag;
					_lockIcons[i].IsVisible = !flag;
					HoverHelpInfo hoverHelpInfo = (!AppShell.Instance.Profile.AvailableItems.ContainsKey(itemDefinition.Id)) ? ((HoverHelpInfo)new GenericHoverHelpInfo(itemDefinition.Name, itemDefinition.Description, "items_bundle|" + itemDefinition.Icon, _HeroItemSize)) : ((HoverHelpInfo)new InventoryHoverHelpInfo(AppShell.Instance.Profile.AvailableItems[itemDefinition.Id]));
					hoverHelpInfo.extendLeft = true;
					hoverHelpInfo.horizontalFlipOverride = HoverHelpInfo.FlipOverride.On;
					_heroItems[i].ToolTip = hoverHelpInfo;
				}
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(15f, 251f);
		}
	}
}
