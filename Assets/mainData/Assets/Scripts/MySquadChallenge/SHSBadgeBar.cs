using UnityEngine;

namespace MySquadChallenge
{
	public class SHSBadgeBar : SHSStatBar
	{
		private MSCBadgeGUI _Badge;

		private GUIDrawTexture _BadgeLock;

		private GUIDrawTexture _badgeHealthLock;

		private string _hero = string.Empty;

		public SHSBadgeBar()
		{
			_Badge = new MSCBadgeGUI();
			_Badge.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-100f, 5f));
			_Badge.SetSize(new Vector2(50f, 50f));
			_Badge.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
			_Badge.HitTestType = HitTestTypeEnum.Circular;
			_Badge.IsVisible = false;
			Add(_Badge);
			_BadgeLock = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(64f, 64f), new Vector2(-30f, 5f));
			_BadgeLock.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
			_BadgeLock.HitTestSize = new Vector2(0.875f, 0.875f);
			_BadgeLock.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			_BadgeLock.IsVisible = false;
			_BadgeLock.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
			_BadgeLock.HitTestType = HitTestTypeEnum.Circular;
			_BadgeLock.IsVisible = false;
			Add(_BadgeLock);
			_badgeHealthLock = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(64f, 64f), new Vector2(120f, -75f));
			_badgeHealthLock.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
			_badgeHealthLock.HitTestSize = new Vector2(0.875f, 0.875f);
			_badgeHealthLock.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			_badgeHealthLock.IsVisible = false;
			_badgeHealthLock.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
			_badgeHealthLock.HitTestType = HitTestTypeEnum.Circular;
			_badgeHealthLock.IsVisible = false;
			Add(_badgeHealthLock);
		}

		public void SetHero(string hero)
		{
			_hero = hero;
			_Badge.setHero(hero);
			_Badge.setOwned(false);
			_Badge.IsVisible = false;
			_Badge.IsEnabled = false;
			_BadgeLock.IsVisible = false;
			_badgeHealthLock.IsVisible = false;
			_BadgeLock.IsVisible = true;
			_BadgeLock.ToolTip = new NamedToolTipInfo("#BADGE_NOT_AVAILABLE_YET", new Vector2(20f, 0f));
			_badgeHealthLock.IsVisible = true;
			_badgeHealthLock.ToolTip = new NamedToolTipInfo("#BADGE_NOT_AVAILABLE_YET", new Vector2(20f, 0f));
			OwnableDefinition def = OwnableDefinition.getDef(OwnableDefinition.HeroNameToHeroID[hero]);
			if (def == null)
			{
				return;
			}
			if (OwnableDefinition.HeroIDToBadgeID.ContainsKey(def.ownableTypeID))
			{
				_Badge.ownableTypeID = OwnableDefinition.HeroIDToBadgeID[def.ownableTypeID][0];
				if (NewShoppingManager.CatalogOwnableMap.ContainsKey(_Badge.ownableTypeID))
				{
					CspUtils.DebugLog("found ownable " + def.ownableTypeID);
					_BadgeLock.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
					_badgeHealthLock.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
					if (AppShell.Instance.Profile.Badges.ContainsKey(string.Empty + OwnableDefinition.HeroIDToBadgeID[def.ownableTypeID]))
					{
						_Badge.IsVisible = true;
						_Badge.IsEnabled = false;
						_Badge.setOwned(true);
						_BadgeLock.IsVisible = false;
						_badgeHealthLock.IsVisible = false;
					}
					else
					{
						_Badge.IsVisible = true;
						_Badge.IsEnabled = true;
						_BadgeLock.IsVisible = true;
						_badgeHealthLock.IsVisible = true;
					}
				}
				else
				{
					CspUtils.DebugLog("badge not in the catalog");
				}
			}
			else
			{
				CspUtils.DebugLog("badge does not exist for this hero");
			}
		}

		public override Vector2 GetBarOffset()
		{
			return new Vector2(35f, 92f);
		}

		public override Vector2 GetBarSize()
		{
			return new Vector2(360f, 198f);
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
			if (_hero == string.Empty)
			{
				return;
			}
			OwnableDefinition def = OwnableDefinition.getDef(OwnableDefinition.HeroNameToHeroID[_hero]);
			if (def == null)
			{
				return;
			}
			if (OwnableDefinition.HeroIDToBadgeID.ContainsKey(def.ownableTypeID))
			{
				_Badge.ownableTypeID = OwnableDefinition.HeroIDToBadgeID[def.ownableTypeID][0];
				if (NewShoppingManager.CatalogOwnableMap.ContainsKey(_Badge.ownableTypeID))
				{
					CspUtils.DebugLog("found ownable " + def.ownableTypeID);
					_BadgeLock.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
					_badgeHealthLock.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
					if (AppShell.Instance.Profile.Badges.ContainsKey(string.Empty + OwnableDefinition.HeroIDToBadgeID[def.ownableTypeID]))
					{
						_Badge.IsVisible = true;
						_Badge.IsEnabled = false;
						_Badge.setOwned(true);
						_BadgeLock.IsVisible = false;
						_badgeHealthLock.IsVisible = false;
					}
					else
					{
						_Badge.IsVisible = true;
						_Badge.IsEnabled = true;
						_BadgeLock.IsVisible = true;
						_badgeHealthLock.IsVisible = true;
					}
				}
				else
				{
					CspUtils.DebugLog("badge not in the catalog");
				}
			}
			else
			{
				CspUtils.DebugLog("badge does not exist for this hero");
			}
		}

		public bool LocksVisible()
		{
			return _BadgeLock.IsVisible;
		}
	}
}
