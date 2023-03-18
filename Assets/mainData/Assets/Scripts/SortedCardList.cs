using CardGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SortedCardList : List<CardListCard>
{
	public enum SortMethod
	{
		Block,
		CardName,
		Effects,
		Expansion,
		Factor,
		HeroName,
		Level,
		Rarity
	}

	protected delegate int CompareDelegate(CardListCard a, CardListCard b);

	private GameObject cardPrefab;

	public GameObject parentContainer;

	public bool saved = true;

	protected GameObject gameObj;

	protected CardLayoutProperties layoutProperties;

	protected SortMethod primarySort = SortMethod.CardName;

	public CardGroup sourceGroup;

	public int TotalCards
	{
		get
		{
			int num = 0;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CardListCard current = enumerator.Current;
					num += current.Available;
				}
				return num;
			}
		}
	}

	public GameObject GameObj
	{
		get
		{
			return gameObj;
		}
		set
		{
			gameObj = value;
		}
	}

	public float TotalHeight
	{
		get
		{
			return layoutProperties.spacingY * Mathf.Ceil((float)CountVisible() / (float)layoutProperties.gridRowWidth);
		}
	}

	public float ScrollableHeight
	{
		get
		{
			return layoutProperties.spacingY * Mathf.Max(0f, Mathf.Ceil((float)CountVisible() / (float)layoutProperties.gridRowWidth) - (float)layoutProperties.visibleRowCount);
		}
	}

	public SortMethod PrimarySort
	{
		get
		{
			return primarySort;
		}
		set
		{
			primarySort = value;
		}
	}

	public SortedCardList(GameObject go, GameObject cardpf)
	{
		sourceGroup = new CardGroup();
		gameObj = go;
		cardPrefab = cardpf;
		layoutProperties = Utils.GetComponent<CardLayoutProperties>(gameObj);
		parentContainer = gameObj.transform.parent.gameObject;
	}

	public SortedCardList(CardGroup cc, GameObject go, GameObject cardpf)
	{
		sourceGroup = cc;
		gameObj = go;
		cardPrefab = cardpf;
		layoutProperties = Utils.GetComponent<CardLayoutProperties>(gameObj);
		parentContainer = gameObj.transform.parent.gameObject;
		Import();
	}

	public void Reset(bool destructive)
	{
		if (sourceGroup == null || destructive)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CardListCard current = enumerator.Current;
					UnityEngine.Object.Destroy(current.CardObj);
				}
			}
			Clear();
			sourceGroup = new CardGroup();
		}
		else
		{
			using (Enumerator enumerator2 = GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CardListCard current2 = enumerator2.Current;
					current2.Available = 0;
				}
			}
			Import();
		}
		saved = true;
		FreshLayout();
	}

	public int CountVisible()
	{
		int num = 0;
		for (int i = 0; i < Count; i++)
		{
			if (!this[i].isFiltered)
			{
				num++;
			}
		}
		return num;
	}

	public CardListCard AddCard(BattleCard newCard, int qty)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (CardListCard.EquivalentCards(current, newCard))
				{
					if (current.Available == 0)
					{
						current.SetColor(Color.white);
					}
					current.Available += qty;
					if (current.counterComponent != null)
					{
						current.counterComponent.SetCount(current.Available);
					}
					return current;
				}
			}
		}
		CardListCard cardListCard = new CardListCard(newCard, qty);
		Add(cardListCard);
		InstantiateCard(cardListCard);
		saved = false;
		return cardListCard;
	}

	public CardListCard AddCard(BattleCard newCard)
	{
		return AddCard(newCard, 1);
	}

	public CardListCard AddCard(CardListCard newCard)
	{
		return AddCard(newCard, 1);
	}

	public bool RemoveCard(CardListCard card)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (CardListCard.EquivalentCards(current, card))
				{
					UnityEngine.Object.Destroy(current.CardObj);
					Remove(current);
					saved = false;
					return true;
				}
			}
		}
		return false;
	}

	public int SubtractCard(CardListCard card, int qty, bool destroyOnZero)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (CardListCard.EquivalentCards(current, card))
				{
					qty = ((qty >= current.Available) ? current.Available : qty);
					current.Available -= qty;
					if (current.Available < 1)
					{
						if (destroyOnZero)
						{
							RemoveCard(current);
						}
						else
						{
							current.SetColor(new Color(1f, 1f, 1f, 0.3f));
						}
					}
					if (current.counterComponent != null)
					{
						current.counterComponent.SetCount(current.Available);
					}
					saved = false;
					return qty;
				}
			}
		}
		return 0;
	}

	public void Import()
	{
		foreach (BattleCard item in sourceGroup)
		{
			AddCard(item, 1);
		}
		saved = true;
	}

	public override string ToString()
	{
		string text = string.Empty;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				string text2 = text;
				text = text2 + current.Type.ToString() + ":" + current.Available.ToString() + ";";
			}
			return text;
		}
	}

	protected void InstantiateCard(CardListCard NewCard)
	{
		NewCard.CardObj = (GameObject)UnityEngine.Object.Instantiate(cardPrefab);
		if (NewCard.CardObj == null)
		{
			CspUtils.DebugLog("Failed to instantiate a copy of the card.  Is the fullCardPrefab or miniCardPrefab empty?");
			return;
		}
		Utils.AttachGameObject(parentContainer, NewCard.CardObj);
		NewCard.CardObj.transform.localPosition = NextPosition();
		NewCard.CardObj.transform.localRotation = NextRotation();
		NewCard.CardObj.transform.localScale = NextScale();
		
		//////////  black added by CSP for testing /////////////////
		// if (NewCard.Name != null) {
		// 	CspUtils.DebugLog("ApplyCardTexture NewCard.Name : " + NewCard.Name);
		// }
		// else {
		// 	CspUtils.DebugLog("ApplyCardTexture NewCard.Name is null! ");
		// }
		// if (NewCard.FullTexture != null) {
		// 	NewCard.MiniTexture = NewCard.FullTexture;
		// }
		// else {
		// 	CspUtils.DebugLog("ApplyCardTexture NewCard.FullTexture is null! ");
		// }
		///////////////////////////////////////////////////////////////

		if (NewCard.MiniTexture != null)
		{
			//CspUtils.DebugLog("ApplyCardTexture called: " + NewCard.MiniTexture.ToString());
			ApplyCardTexture(NewCard);
		}
		else {
			//CspUtils.DebugLog("ApplyCardTexture NewCard.MiniTexture is null! ");
		}
	}

	private void ApplyCardTexture(BattleCard BlankCard)
	{
		if ((bool)BlankCard.FullTexture)
		{
			Renderer renderer = BlankCard.CardObj.GetComponentInChildren(typeof(Renderer)) as Renderer;
			CspUtils.DebugLog("setting mainTexture to " + BlankCard.MiniTexture);  // CSP
			renderer.materials[renderer.materials.Length - 1].mainTexture = BlankCard.MiniTexture;
		}
		else
		{
			CspUtils.DebugLog("Texture not found for card '" + BlankCard.Name + "'");
		}
	}

	public Vector3 NextPosition(int CardIndex)
	{
		Vector3 b = new Vector3(layoutProperties.spacingX, layoutProperties.spacingY, layoutProperties.spacingZ);
		if (layoutProperties.MaxCardsBeforeOverlapping != 0 && CountVisible() >= layoutProperties.MaxCardsBeforeOverlapping)
		{
			float num = (float)(layoutProperties.MaxCardsBeforeOverlapping - 1) / (float)(CountVisible() - 1);
			b *= num;
		}
		int num2 = CardIndex / layoutProperties.gridRowWidth;
		float num3 = CardIndex % layoutProperties.gridRowWidth;
		b.y *= -num2;
		b.x *= num3 - ((float)layoutProperties.gridRowWidth / 2f - 0.5f);
		b.x += layoutProperties.offsetX;
		b.y += layoutProperties.offsetY;
		return layoutProperties.gameObject.transform.localPosition + b;
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

	public void FreshLayout()
	{
		int num = 0;
		for (int i = 0; i < Count; i++)
		{
			if (!this[i].isFiltered)
			{
				Utils.ActivateTree(this[i].CardObj, true);
				this[i].CardObj.transform.localPosition = NextPosition(num++);
				this[i].CardObj.transform.localRotation = NextRotation();
				this[i].CardObj.transform.localScale = NextScale();
				this[i].UpdateCounter();
			}
			else
			{
				Utils.ActivateTree(this[i].CardObj, false);
			}
		}
	}

	protected CompareDelegate SortTypeToDelegate(SortMethod sm)
	{
		switch (sm)
		{
		case SortMethod.Block:
			return CompareBlock;
		case SortMethod.CardName:
			return CompareCardName;
		case SortMethod.Effects:
			return CompareEffects;
		case SortMethod.Expansion:
			return CompareExpansion;
		case SortMethod.Factor:
			return CompareFactor;
		case SortMethod.HeroName:
			return CompareHeroName;
		case SortMethod.Level:
			return CompareLevel;
		case SortMethod.Rarity:
			return CompareRarity;
		default:
			return null;
		}
	}

	public int Compare(CardListCard a, CardListCard b)
	{
		CompareDelegate compareDelegate = SortTypeToDelegate(primarySort);
		CompareDelegate compareDelegate2 = CompareLevel;
		if (primarySort == SortMethod.Level)
		{
			compareDelegate2 = CompareFactor;
		}
		int num = compareDelegate(a, b);
		if (num == 0)
		{
			num = compareDelegate2(a, b);
		}
		return num;
	}

	protected int CompareBlock(CardListCard a, CardListCard b)
	{
		BattleCard.Factor[] blockFactors = a.GetBlockFactors();
		BattleCard.Factor[] blockFactors2 = b.GetBlockFactors();
		return blockFactors[0].CompareTo(blockFactors2[0]);
	}

	protected int CompareCardName(CardListCard a, CardListCard b)
	{
		return a.Name.CompareTo(b.Name);
	}

	protected int CompareExpansion(CardListCard a, CardListCard b)
	{
		return 0;
	}

	protected int CompareEffects(CardListCard a, CardListCard b)
	{
		return a.AbilityText.CompareTo(b.AbilityText);
	}

	protected int CompareFactor(CardListCard a, CardListCard b)
	{
		BattleCard.Factor[] attackFactors = a.AttackFactors;
		BattleCard.Factor[] attackFactors2 = b.AttackFactors;
		return attackFactors[0].CompareTo(attackFactors2[0]);
	}

	protected int CompareHeroName(CardListCard a, CardListCard b)
	{
		int num = a.RulesHeroName[0].CompareTo(b.RulesHeroName[0]);
		return (num != 0 || a.RulesHeroName.Length <= 1 || b.RulesHeroName.Length <= 1) ? num : a.RulesHeroName[1].CompareTo(b.RulesHeroName[1]);
	}

	protected int CompareLevel(CardListCard a, CardListCard b)
	{
		return a.Level.CompareTo(b.Level);
	}

	protected int CompareRarity(CardListCard a, CardListCard b)
	{
		return -a.Rarity.CompareTo(b.Rarity);
	}

	public void ApplyFilters(List<CardFilter> filterStates)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				int num = 0;
				current.isFiltered = true;
				int length = Enum.GetValues(typeof(CardFilterType)).Length;
				for (int num2 = length - 1; num2 >= 0; num2--)
				{
					if (filterStates[num2].active && filterStates[num2].Test(current))
					{
						num |= 1;
					}
					num <<= 1;
				}
				num >>= 1;
				if ((AllBlockFiltersOff(filterStates) || (num & 0x3F) != 0) && (AllFactorFiltersOff(filterStates) || (num & 0xFC0) != 0) && (AllTeamFiltersOff(filterStates) || (num & 0x3F000) != 0) && (!filterStates[20].active || (num & 0x100000) != 0))
				{
					current.isFiltered = false;
				}
			}
		}
		FreshLayout();
	}

	private bool AllBlockFiltersOff(List<CardFilter> filterStates)
	{
		for (int i = 0; i <= 5; i++)
		{
			if (filterStates[i].active)
			{
				return false;
			}
		}
		return true;
	}

	private bool AllFactorFiltersOff(List<CardFilter> filterStates)
	{
		for (int i = 6; i <= 11; i++)
		{
			if (filterStates[i].active)
			{
				return false;
			}
		}
		return true;
	}

	private bool AllTeamFiltersOff(List<CardFilter> filterStates)
	{
		for (int i = 12; i <= 17; i++)
		{
			if (filterStates[i].active)
			{
				return false;
			}
		}
		return true;
	}

	public bool ContainsCard(CardListCard card)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (current.Type == card.Type)
				{
					return true;
				}
			}
		}
		return false;
	}

	public int CountByBlock(BattleCard.Factor factor)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (current.HasBlock(factor))
				{
					num += current.Available;
				}
			}
			return num;
		}
	}

	public int CountByFactor(BattleCard.Factor factor)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (current.HasFactor(factor))
				{
					num += current.Available;
				}
			}
			return num;
		}
	}

	public int CountByLevel(int level)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (current.Level == level)
				{
					num += current.Available;
				}
			}
			return num;
		}
	}

	public int CountByRarity(BattleCard.CardRarity r)
	{
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CardListCard current = enumerator.Current;
				if (current.Rarity == r)
				{
					num += current.Available;
				}
			}
			return num;
		}
	}
}
