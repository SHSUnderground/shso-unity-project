using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
	public class CardPile : List<BattleCard>
	{
		public static readonly float kCardWidth = 4.264f;

		public static readonly float kCardHeight = 5.965f;

		protected GameObject gameObj;

		protected CardLayoutProperties layoutProperties;

		public GameObject GameObj
		{
			get
			{
				return gameObj;
			}
			set
			{
				gameObj = value;
				layoutProperties = Utils.GetComponent<CardLayoutProperties>(gameObj);
			}
		}

		public CardLayoutProperties LayoutProperties
		{
			get
			{
				return layoutProperties;
			}
		}

		public string CardAnimation
		{
			get
			{
				return layoutProperties.cardAnimationName;
			}
		}

		public float GetPileWidth()
		{
			return GetPileWidth(Count);
		}

		public float GetPileWidth(int count)
		{
			float num = Math.Abs(layoutProperties.spacingX);
			float num2 = (float)((count != 0) ? (count - 1) : 0) * num;
			float num3 = kCardWidth;
			Vector3 localScale = layoutProperties.transform.localScale;
			return num2 + num3 * localScale.x;
		}

		public float GetPileHeight()
		{
			return GetPileHeight(Count);
		}

		public float GetPileHeight(int count)
		{
			float num = Math.Abs(layoutProperties.spacingY);
			float num2 = (float)((count != 0) ? (count - 1) : 0) * num;
			float num3 = kCardHeight;
			Vector3 localScale = layoutProperties.transform.localScale;
			return num2 + num3 * localScale.y;
		}

		public bool ContainsKey(int ID)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BattleCard current = enumerator.Current;
					if (current.ServerID == ID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public BattleCard Lookup(int ID)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BattleCard current = enumerator.Current;
					//CspUtils.DebugLog("Lookup current.ServerID=" + current.ServerID + "  type=" + current.Type);  // CSP
					if (current.ServerID == ID)
					{
						return current;
					}
				}
			}
			return null;
		}

		public void CreateDummy(int ID, BattleCard card, GameObject prefab)
		{
			card.CardObj = (GameObject)UnityEngine.Object.Instantiate(prefab);
			if (card.CardObj == null)
			{
				CspUtils.DebugLog("Failed to instantiate a card.  Is fullCardPrefab empty?");
				return;
			}
			Utils.AttachGameObject(gameObj.transform.parent.gameObject, card.CardObj);
			card.CardObj.transform.localPosition = NextPosition();
			card.CardObj.transform.localRotation = NextRotation();
			card.CardObj.transform.localScale = NextScale();
			Add(card);

			//////////// block added by CSP - for applying texture....maybe should have called SortedCardList.InstantiateCard() instead of CreateDummy() ///////////////////////////
			if (card.MiniTexture != null)
			{
				//CspUtils.DebugLog("ApplyCardTexture called: " + NewCard.MiniTexture.ToString());
				
				
				if ((bool)card.FullTexture)
				{
					Renderer renderer = card.CardObj.GetComponentInChildren(typeof(Renderer)) as Renderer;
					CspUtils.DebugLog("setting mainTexture to " + card.MiniTexture);  // CSP
					renderer.materials[renderer.materials.Length - 1].mainTexture = card.MiniTexture;
				}
				else
				{
					CspUtils.DebugLog("Texture not found for card '" + card.Name + "'");
				}
				
			}
			else {
				//CspUtils.DebugLog("ApplyCardTexture NewCard.MiniTexture is null! ");
			}
			///////////////////////////////////////////////////////////////
		}

		public Vector3 NextPosition(int CardIndex)
		{
			Vector3 vector = default(Vector3);
			if (layoutProperties.MaxCardsBeforeOverlapping == 0 || Count <= layoutProperties.MaxCardsBeforeOverlapping)
			{
				vector.x = layoutProperties.spacingX + layoutProperties.offsetX;
				vector.y = layoutProperties.spacingY + layoutProperties.offsetY;
				vector.z = layoutProperties.spacingZ;
			}
			else if (!layoutProperties.overlapAsStack)
			{
				float num = (float)(layoutProperties.MaxCardsBeforeOverlapping - 1) / (float)(Count - 1);
				vector.x = layoutProperties.spacingX * num + layoutProperties.offsetX;
				vector.y = layoutProperties.spacingY * num + layoutProperties.offsetY;
				vector.z = layoutProperties.spacingZ * num;
			}
			else
			{
				vector.x = layoutProperties.spacingX + layoutProperties.offsetX;
				vector.y = layoutProperties.spacingY + layoutProperties.offsetY;
				vector.z = layoutProperties.spacingZ;
				if (CardIndex > layoutProperties.MaxCardsBeforeOverlapping)
				{
					return layoutProperties.gameObject.transform.localPosition + vector * layoutProperties.MaxCardsBeforeOverlapping;
				}
			}
			if (layoutProperties.shape == CardLayoutProperties.LayoutType.Grid)
			{
				int num2 = CardIndex / layoutProperties.gridRowWidth;
				float num3 = CardIndex % layoutProperties.gridRowWidth;
				vector.y *= -num2;
				vector.x *= num3 - ((float)layoutProperties.gridRowWidth / 2f - 0.5f);
				vector.x += layoutProperties.offsetX;
				vector.y += layoutProperties.offsetY;
				return layoutProperties.gameObject.transform.localPosition + vector;
			}
			if (layoutProperties.shape == CardLayoutProperties.LayoutType.Arch)
			{
				int num4 = (CardIndex % 2 == 0) ? 1 : (-1);
				return layoutProperties.gameObject.transform.localPosition + vector * ((CardIndex + 1) / 2) * num4;
			}
			if (layoutProperties.shape == CardLayoutProperties.LayoutType.Hand)
			{
				return layoutProperties.gameObject.transform.localPosition + vector * CardIndex;
			}
			return layoutProperties.gameObject.transform.localPosition + vector * CardIndex;
		}

		public Vector3 NextPosition()
		{
			return NextPosition(Count);
		}

		public Quaternion NextRotation()
		{
			Quaternion localRotation = gameObj.transform.localRotation;
			if (layoutProperties.randomRotation != 0f)
			{
				float angle = UnityEngine.Random.value * layoutProperties.randomRotation - layoutProperties.randomRotation / 2f;
				Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);
				localRotation *= quaternion;
			}
			return localRotation;
		}

		public Vector3 NextScale()
		{
			return gameObj.transform.localScale;
		}

		public void RefreshLayout()
		{
			int num = 0;
			int count = Count;
			int visibleCardCount = layoutProperties.visibleCardCount;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BattleCard current = enumerator.Current;
					if (visibleCardCount == -1 || count-- <= visibleCardCount)
					{
						current.IsVisible = true;
					}
					else
					{
						current.IsVisible = false;
					}
					current.CardObj.transform.localPosition = NextPosition(num++);
					current.CardObj.transform.localRotation = NextRotation();
					current.CardObj.transform.localScale = NextScale();
				}
			}
		}

		public void PlayEffect()
		{
			if (layoutProperties.cardEffectPrefab != null)
			{
				GameObject child = UnityEngine.Object.Instantiate(layoutProperties.cardEffectPrefab) as GameObject;
				Utils.AttachGameObject(gameObj, child);
			}
		}
	}
}
