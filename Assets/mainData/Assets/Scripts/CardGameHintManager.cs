using System.Collections.Generic;
using UnityEngine;

public class CardGameHintManager
{
	protected long currentTimerId = -1L;

	protected BattleCard hintCard;

	protected bool isOpponentCard;

	protected GameObject targetingArrow;

	protected Quaternion PASS_ROTATION = Quaternion.AngleAxis(-90f, Vector3.forward);

	protected Quaternion PLAYER_CARD_ROTATION = Quaternion.identity;

	protected Quaternion OPPONENT_CARD_ROTATION = Quaternion.AngleAxis(180f, Vector3.forward);

	protected GameObject playerSidePanel;

	protected Animation hintAnimation;

	protected bool isKeeper;

	public void StartTimer(List<int> validCards, CardGamePlayer player, CardGamePlayer opponent)
	{
		if (currentTimerId != -1)
		{
			AppShell.Instance.TimerMgr.CancelTimer(currentTimerId, false);
		}
		hintCard = null;
		isOpponentCard = false;
		playerSidePanel = player.SidePanel.gameObject;
		isKeeper = false;
		if (validCards.Count > 0)
		{
			List<BattleCard> list = new List<BattleCard>();
			foreach (int validCard in validCards)
			{
				BattleCard battleCard = opponent.FindCardByIndex(validCard);
				if (battleCard != null)
				{
					list.Add(battleCard);
					isOpponentCard = true;
				}
			}
			if (list.Count == 0)
			{
				foreach (int validCard2 in validCards)
				{
					BattleCard battleCard2 = player.FindCardByIndex(validCard2);
					if (battleCard2 != null)
					{
						list.Add(battleCard2);
						isOpponentCard = false;
					}
				}
			}
			if (list.Count > 1)
			{
				list.Sort(delegate(BattleCard a, BattleCard b)
				{
					return b.Level.CompareTo(a.Level);
				});
				int level = list[0].Level;
				while (list[list.Count - 1].Level < level)
				{
					list.RemoveAt(list.Count - 1);
				}
				if (list.Count > 1)
				{
					list.Sort(delegate(BattleCard a, BattleCard b)
					{
						return b.Damage.CompareTo(a.Damage);
					});
					int damage = list[0].Damage;
					while (list[list.Count - 1].Damage < damage)
					{
						list.RemoveAt(list.Count - 1);
					}
				}
			}
			hintCard = list[0];
			if (hintCard != null)
			{
				CspUtils.DebugLog("Hint manager chose " + hintCard.NameEng + " Level: " + hintCard.Level + " Damage: " + hintCard.Damage);
			}
			else
			{
				CspUtils.DebugLog("Hint manager chose Pass");
			}
			if (player.Keepers.Contains(hintCard))
			{
				isKeeper = true;
			}
			else if (opponent.Keepers.Contains(hintCard))
			{
				isKeeper = true;
			}
		}
		if (targetingArrow != null)
		{
			Utils.ActivateTree(targetingArrow, false);
		}
		currentTimerId = AppShell.Instance.TimerMgr.CreateTimer(15f, ShowHint);
	}

	public void HideHint()
	{
		if (currentTimerId != -1)
		{
			AppShell.Instance.TimerMgr.CancelTimer(currentTimerId, false);
		}
		if (targetingArrow != null)
		{
			Disable();
		}
	}

	public void ShowHint(long timerId, bool canceled)
	{
		if (timerId != currentTimerId)
		{
			return;
		}
		if (targetingArrow == null && CardGameController.Instance != null)
		{
			Object @object = CardGameController.Instance.CardGameBundle.Load("HintArrow");
			if (@object != null)
			{
				targetingArrow = (Object.Instantiate(@object) as GameObject);
				if (targetingArrow != null)
				{
					targetingArrow.transform.localPosition = Vector3.zero;
					Utils.SetLayerTree(targetingArrow, 9);
					Transform transform = targetingArrow.transform.FindChild("animation");
					if (transform != null)
					{
						hintAnimation = transform.gameObject.animation;
					}
				}
			}
			else
			{
				CspUtils.DebugLog("Could not get targeting arrow prefab.");
			}
		}
		if (!(targetingArrow != null))
		{
			return;
		}
		if (hintAnimation != null)
		{
			hintAnimation.Play("hintArrowAnim_fade");
			hintAnimation.CrossFade("hintArrowAnim_idle", 1f);
		}
		if (hintCard != null && hintCard.CardObj != null)
		{
			if (hintCard.CardObj.transform.parent != null)
			{
				targetingArrow.transform.parent = hintCard.CardObj.transform;
				if (isOpponentCard)
				{
					if (isKeeper)
					{
						targetingArrow.transform.localScale = new Vector3(5f, 5f, 1f);
					}
					else
					{
						targetingArrow.transform.localScale = new Vector3(3f, 3f, 1f);
					}
					targetingArrow.transform.localPosition = new Vector3(1.5f, -5f, 0f);
					targetingArrow.transform.localRotation = OPPONENT_CARD_ROTATION;
				}
				else
				{
					if (isKeeper)
					{
						targetingArrow.transform.localScale = new Vector3(5f, 5f, 1f);
						targetingArrow.transform.localPosition = new Vector3(1.6f, 5f, 0f);
					}
					else
					{
						targetingArrow.transform.localScale = new Vector3(3f, 3f, 1f);
						targetingArrow.transform.localPosition = new Vector3(1.6f, 4.4f, 0f);
					}
					targetingArrow.transform.localRotation = PLAYER_CARD_ROTATION;
				}
			}
		}
		else if (playerSidePanel != null)
		{
			targetingArrow.transform.parent = playerSidePanel.transform;
			targetingArrow.transform.localScale = new Vector3(6f, 6f, 1f);
			targetingArrow.transform.localPosition = new Vector3(10f, 17.5f, -0.75f);
			targetingArrow.transform.localRotation = PASS_ROTATION;
			CspUtils.DebugLog("Show Pass hint arrow!");
		}
		Utils.ActivateTree(targetingArrow, true);
	}

	public void Disable()
	{
		if (targetingArrow != null)
		{
			Object.Destroy(targetingArrow);
			targetingArrow = null;
		}
		if (currentTimerId != -1)
		{
			AppShell.Instance.TimerMgr.CancelTimer(currentTimerId, false);
		}
	}
}
