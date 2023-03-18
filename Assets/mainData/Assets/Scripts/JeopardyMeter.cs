using CardGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JeopardyMeter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class Background
	{
		public static float kBackgroundWidth = 6.4f;

		public static float xGrowthTime = 0.2f;

		public static float yGrowthTime = 0.6f;

		public static float kMiddleThickness = 0.01f;

		private GameObject discardPanel;

		private GameObject bottom;

		private GameObject middle;

		private GameObject top;

		private AnimClipManager AnimationPieceManager;

		private float bottomHeight;

		private float topHeight;

		private float middleHeight;

		private JeopardyMeter jeopardyMeter;

		private float growHeight;

		public Background(JeopardyMeter jeopardyMeter, GameObject gameObj, GameObject discardPanel)
		{
			AnimationPieceManager = new AnimClipManager();
			this.jeopardyMeter = jeopardyMeter;
			this.discardPanel = discardPanel;
			bottom = gameObj.transform.Find("bottom").gameObject;
			middle = gameObj.transform.Find("middle").gameObject;
			top = gameObj.transform.Find("top").gameObject;
			Vector3 localScale = bottom.transform.localScale;
			bottomHeight = localScale.y;
			Vector3 localScale2 = top.transform.localScale;
			topHeight = localScale2.y;
			bottom.transform.localScale = Vector3.zero;
			middle.transform.localScale = Vector3.zero;
			middle.transform.localPosition = Vector3.zero;
			top.transform.localScale = Vector3.zero;
			top.transform.localPosition = Vector3.zero;
		}

		public void Update()
		{
			AnimationPieceManager.Update(Time.deltaTime);
		}

		private void GrowXAnimFunc(float t)
		{
			bottom.transform.localScale = new Vector3(t * kBackgroundWidth, bottomHeight, 1f);
			middle.transform.localScale = new Vector3(t * kBackgroundWidth, kMiddleThickness, 1f);
			top.transform.localScale = new Vector3(t * kBackgroundWidth, topHeight, 1f);
		}

		private void GrowYAnimFunc(float t)
		{
			float num = t * growHeight;
			float num2 = (bottomHeight + num) / 2f;
			float y = num2 + (topHeight + num) / 2f;
			middle.transform.localScale = new Vector3(kBackgroundWidth, num, 1f);
			middle.transform.localPosition = new Vector3(0f, num2, 0f);
			top.transform.localPosition = new Vector3(0f, y, 0f);
			discardPanel.transform.position = top.transform.position;
			discardPanel.transform.localPosition += new Vector3(0f, jeopardyMeter.discardPileOffset, 0.02f);
		}

		public void Grow(float growHeight)
		{
			AnimPath path = AnimClipBuilder.Path.Linear(0f, 1f, xGrowthTime);
			AnimPath path2 = AnimClipBuilder.Path.Linear(0f, 1f, yGrowthTime);
			middle.transform.localPosition = new Vector3(0f, (bottomHeight + kMiddleThickness) / 2f, 0f);
			top.transform.localPosition = new Vector3(0f, kMiddleThickness + (bottomHeight + topHeight) / 2f, 0f);
			this.growHeight = growHeight;
			AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, GrowXAnimFunc) | AnimClipBuilder.Custom.Function(path2, GrowYAnimFunc));
		}

		public void Collapse()
		{
			AnimPath path = AnimClipBuilder.Path.Linear(1f, 0f, xGrowthTime);
			AnimPath path2 = AnimClipBuilder.Path.Linear(1f, 0f, yGrowthTime);
			AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path2, GrowYAnimFunc) | AnimClipBuilder.Custom.Function(path, GrowXAnimFunc));
		}
	}

	public class Slider
	{
		private GameObject gameObj;

		private QuadTexture factorIcon;

		private QuadTexture damageText;

		private CardLayoutProperties layout;

		private List<MeshOpacity> opacityControls;

		private Dictionary<BattleCard.Factor, Texture2D> factorTextures;

		private List<Texture2D> numberTextures;

		private AnimClipManager AnimationPieceManager;

		private GameObject currentBlockEffect;

		private ShsAudioSource moveSFX;

		private static float kSliderOffset = 0.23756f;

		private Vector3 startScale;

		public Slider(GameObject slider, CardLayoutProperties layout)
		{
			AnimationPieceManager = new AnimClipManager();
			opacityControls = new List<MeshOpacity>();
			gameObj = slider;
			this.layout = layout;
			GameObject gameObject = gameObj.transform.Find("blockIcon").gameObject;
			factorIcon = gameObject.transform.Find("factor").gameObject.GetComponent<QuadTexture>();
			opacityControls.Add(gameObject.GetComponent<MeshOpacity>());
			GameObject gameObject2 = gameObj.transform.Find("counter").gameObject;
			damageText = gameObject2.GetComponent<QuadTexture>();
			GameObject gameObject3 = gameObj.transform.Find("arrows").gameObject;
			opacityControls.Add(gameObject3.GetComponent<MeshOpacity>());
			factorTextures = new Dictionary<BattleCard.Factor, Texture2D>();
			BattleCard.Factor[] factorList = BattleCard.FactorList;
			foreach (BattleCard.Factor factor in factorList)
			{
				Texture2D texture;
				if (GUIManager.Instance.LoadTexture("cardgame_bundle|mshs_cg_jeopardy_block_" + char.ToLower(BattleCard.FactorToChar(factor)), out texture))
				{
					factorTextures[factor] = texture;
				}
			}
			numberTextures = new List<Texture2D>();
			for (int j = 1; j <= 12; j++)
			{
				Texture2D texture2;
				if (GUIManager.Instance.LoadTexture(string.Format("cardgame_bundle|mshs_cg_jeopardycount_{0}", j), out texture2))
				{
					numberTextures.Add(texture2);
				}
			}
			foreach (MeshOpacity opacityControl in opacityControls)
			{
				opacityControl.SetOpacity(0f);
			}
			damageText.SetTexture(numberTextures[0]);
			Utils.ActivateTree(gameObj, false);
		}

		public void Update()
		{
			AnimationPieceManager.Update(Time.deltaTime);
		}

		private void ScaleAnimFunc(float t)
		{
			gameObj.transform.localScale = Vector3.Lerp(startScale, Vector3.one, t);
		}

		private void OpacityAnimFunc(float t)
		{
			foreach (MeshOpacity opacityControl in opacityControls)
			{
				opacityControl.SetOpacity(t);
			}
		}

		public void Grow(int damage, BattleCard.Factor factor)
		{
			Utils.ActivateTree(gameObj, true);
			factorIcon.SetTexture(factorTextures[factor]);
			damageText.SetTexture((damage != 0) ? numberTextures[(damage > 12) ? 12 : (damage - 1)] : null);
			foreach (MeshOpacity opacityControl in opacityControls)
			{
				opacityControl.SetOpacity(0f);
			}
			float num = layout.spacingY * -1f;
			int num2 = (layout.spacingY > 0f) ? 1 : (-1);
			Vector3 localPosition = layout.transform.localPosition;
			float y = localPosition.y + ((kSliderOffset + CardPile.kCardHeight / 2f) * (float)num2 + num);
			Vector3 localPosition2 = gameObj.transform.localPosition;
			localPosition2.y = y;
			gameObj.transform.localPosition = localPosition2;
			startScale = new Vector3(0f, 1f, 1f);
			gameObj.transform.localScale = startScale;
			AnimPath path = AnimClipBuilder.Path.Constant(0f, 0.4f);
			AnimPath path2 = AnimClipBuilder.Path.Linear(0f, 1f, 0.3f);
			AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, delegate
			{
			}) | AnimClipBuilder.Custom.Function(path2, ScaleAnimFunc) | AnimClipBuilder.Custom.Function(path2, OpacityAnimFunc));
		}

		public void Collapse()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			startScale = new Vector3(0f, 1f, 1f);
			AnimPath path = AnimClipBuilder.Path.Linear(1f, 0f, 0.3f);
			AnimClip animClip = AnimClipBuilder.Custom.Function(path, ScaleAnimFunc);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				Utils.ActivateTree(gameObj, false);
			};
			AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, OpacityAnimFunc) | animClip);
			if (moveSFX != null)
			{
				moveSFX.DestroyWhenFinished();
			}
		}

		public void TriggerBlockEffect(BattleCard.Factor factor)
		{
			string text = "defended_sequence";
			UnityEngine.Object effectByName = CardGameController.Instance.GetEffectByName(text);
			if (effectByName == null)
			{
				CspUtils.DebugLog("Unable to load effect " + text);
			}
			if (currentBlockEffect != null)
			{
				UnityEngine.Object.Destroy(currentBlockEffect);
			}
			currentBlockEffect = (UnityEngine.Object.Instantiate(effectByName) as GameObject);
			if (currentBlockEffect != null)
			{
				Utils.AttachGameObject(gameObj.transform.Find("blockIcon").gameObject, currentBlockEffect);
				currentBlockEffect.transform.localPosition = new Vector3(0f, 0f, 2f);
			}
			else
			{
				CspUtils.DebugLog("Unable to instantiate effect " + text);
			}
		}

		public void StopBlockEffect()
		{
			if (currentBlockEffect != null)
			{
				ParticleEmitter component = currentBlockEffect.GetComponent<ParticleEmitter>();
				if (component != null)
				{
					component.emit = false;
				}
			}
		}

		public void MoveTo(int cardCount, int damageDealt)
		{
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Expected O, but got Unknown
			if (cardCount <= 12)
			{
				float num = layout.spacingY * (float)((cardCount > 0) ? (cardCount - 1) : 0);
				int num2 = (layout.spacingY > 0f) ? 1 : (-1);
				Vector3 localPosition = layout.transform.localPosition;
				float y = localPosition.y + ((kSliderOffset + CardPile.kCardHeight / 2f) * (float)num2 + num);
				Vector3 oldPos = gameObj.transform.localPosition;
				Vector3 newPos = gameObj.transform.localPosition;
				newPos.y = y;
				AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 0f, 0.2f), delegate
				{
				});
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					AnimPath path = AnimClipBuilder.Path.Linear(0f, 1f, 0.3f);
					AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, delegate(float t)
					{
						gameObj.transform.localPosition = Vector3.Lerp(oldPos, newPos, t);
						if (t == 1f)
						{
							damageText.SetTexture(numberTextures[(damageDealt > 12) ? 1 : (damageDealt - 1)]);
							moveSFX = CardGameController.Instance.AudioManager.PlaySequential(CardGameAudioManager.SFX.Jeopardy, moveSFX);
						}
					}));
				};
				AnimationPieceManager.Add(animClip);
			}
		}
	}

	public class CardSlots
	{
		private GameObject gameObj;

		private CardPile jeopardyPile;

		private List<GameObject> cardSlots;

		private List<Vector3> cardPositions;

		private int cardCount;

		private AnimClipManager AnimationPieceManager;

		private GameObject currentBlockEffect;

		public CardSlots(GameObject gameObj, CardPile jeopardyPile)
		{
			this.jeopardyPile = jeopardyPile;
			this.gameObj = gameObj;
			AnimationPieceManager = new AnimClipManager();
			cardPositions = new List<Vector3>();
			Transform transform = gameObj.transform;
			cardSlots = new List<GameObject>();
			foreach (Transform item in transform)
			{
				GameObject gameObject = item.gameObject;
				cardSlots.Add(gameObject);
				Utils.ActivateTree(gameObject, false);
			}
		}

		public void Update()
		{
			AnimationPieceManager.Update(Time.deltaTime);
		}

		private void DealingCardsAnimFunc(float t)
		{
			int num = (cardCount <= 12) ? cardCount : 12;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = cardSlots[i];
				Transform transform = gameObject.transform;
				Vector3 vector = cardPositions[i];
				transform.localPosition = Vector3.Lerp(new Vector3(0f, 0f, vector.z), cardPositions[i], t);
			}
		}

		private void ScaleXAnimFunc(float t)
		{
			Vector3 localScale = gameObj.transform.localScale;
			localScale.x = t * 5f;
			gameObj.transform.localScale = localScale;
		}

		private void ScaleYAnimFunc(float t)
		{
			Vector3 localScale = gameObj.transform.localScale;
			localScale.y = t * 5f;
			gameObj.transform.localScale = localScale;
		}

		public void Grow(int cardCount)
		{
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected O, but got Unknown
			this.cardCount = cardCount;
			cardPositions.Clear();
			for (int i = 0; i < cardCount; i++)
			{
				int placementIndex = GetPlacementIndex(i);
				GameObject gameObject = cardSlots[placementIndex];
				Vector3 localPosition = gameObject.transform.localPosition;
				gameObject.transform.localPosition = new Vector3(0f, 0f, localPosition.z);
				Vector3 vector = jeopardyPile.NextPosition(i);
				vector -= jeopardyPile.LayoutProperties.gameObject.transform.localPosition;
				Vector3 a = vector;
				Vector3 localScale = gameObj.transform.localScale;
				vector = a * (1f / localScale.y);
				vector.z = localPosition.z;
				Utils.ActivateTree(gameObject, true);
				cardPositions.Add(vector);
			}
			Utils.ActivateTree(gameObj, true);
			Vector3 localScale2 = gameObj.transform.localScale;
			localScale2.x = 0f;
			gameObj.transform.localScale = localScale2;
			float yGrowthTime = Background.yGrowthTime;
			float xGrowthTime = Background.xGrowthTime;
			float num = (1f - (float)(cardCount / 12) * 0.73f) * yGrowthTime;
			float growTime = yGrowthTime - num;
			AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 0f, xGrowthTime + num), delegate
			{
			});
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Expected O, but got Unknown
				AnimClip animClip2 = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 1f, 0.1f), ScaleXAnimFunc);
				animClip2.OnFinished += (Action)(object)(Action)delegate
				{
					AnimPath path = AnimClipBuilder.Path.Linear(0f, 1f, growTime - 0.1f);
					AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(path, DealingCardsAnimFunc));
				};
				AnimationPieceManager.Add(animClip2);
			};
			AnimationPieceManager.Add(animClip);
		}

		public void Collapse()
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Expected O, but got Unknown
			cardPositions.Clear();
			for (int i = 0; i < cardCount; i++)
			{
				int placementIndex = GetPlacementIndex(i);
				GameObject gameObject = cardSlots[placementIndex];
				cardPositions.Add(gameObject.transform.localPosition);
			}
			float yGrowthTime = Background.yGrowthTime;
			float shrinkTime = (1f - (float)(cardCount / 12) * 0.73f) * yGrowthTime;
			float num = yGrowthTime - shrinkTime;
			AnimPath path = AnimClipBuilder.Path.Linear(1f, 0f, num - 0.1f);
			AnimClip animClip = AnimClipBuilder.Custom.Function(path, DealingCardsAnimFunc);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Expected O, but got Unknown
				AnimClip animClip2 = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 0f, shrinkTime - 0.1f), ScaleYAnimFunc);
				animClip2.OnFinished += (Action)(object)(Action)delegate
				{
					foreach (GameObject cardSlot in cardSlots)
					{
						Utils.ActivateTree(cardSlot, false);
					}
					cardCount = 0;
					gameObj.transform.localScale = new Vector3(5f, 5f, 1f);
					Utils.ActivateTree(gameObj, false);
				};
				AnimationPieceManager.Add(animClip2);
			};
			AnimationPieceManager.Add(animClip);
		}

		public void TriggerBlockEffect(int cardIdx)
		{
			UnityEngine.Object effectByName = CardGameController.Instance.GetEffectByName("factor block ripple");
			if (currentBlockEffect != null)
			{
				UnityEngine.Object.Destroy(currentBlockEffect);
			}
			currentBlockEffect = (UnityEngine.Object.Instantiate(effectByName) as GameObject);
			if (currentBlockEffect != null)
			{
				if (cardIdx < jeopardyPile.Count)
				{
					Vector3 localPosition = jeopardyPile.NextPosition(cardIdx) - new Vector3(1.554504f, 2.088385f, 0.01f);
					Utils.AttachGameObject(gameObj.transform.parent.gameObject, currentBlockEffect);
					currentBlockEffect.transform.localPosition = localPosition;
				}
				else
				{
					CspUtils.DebugLog("Jeopard meter card effect: specifying card index outside of the pile's range");
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to load factor block ripple effect");
			}
		}

		public void StopBlockEffect()
		{
			if (currentBlockEffect != null)
			{
				ParticleEmitter component = currentBlockEffect.GetComponent<ParticleEmitter>();
				component.emit = false;
			}
		}
	}

	public const int MAX_CARD_SLOTS = 12;

	public Background background;

	public Slider slider;

	public CardSlots cardSlots;

	public float animateTime = 0.6f;

	public int playerId;

	public float discardPileOffset;

	private AnimClipManager AnimationPieceManager;

	private CardGamePlayer player;

	private Vector3 discardOrigin;

	private static int GetPlacementIndex(int i)
	{
		return (i < 12) ? i : 11;
	}

	private void Awake()
	{
		AnimationPieceManager = new AnimClipManager();
	}

	private void Update()
	{
		AnimationPieceManager.Update(Time.deltaTime);
		if (background != null)
		{
			background.Update();
		}
		if (slider != null)
		{
			slider.Update();
		}
		if (cardSlots != null)
		{
			cardSlots.Update();
		}
	}

	public void Initialize(CardGamePlayer player)
	{
		this.player = player;
		background = new Background(this, base.transform.Find("Background").gameObject, player.Discard.GameObj.transform.parent.gameObject);
		slider = new Slider(base.transform.Find("Slider").gameObject, player.Jeopardy.LayoutProperties);
		cardSlots = new CardSlots(base.transform.Find("CardSlots").gameObject, player.Jeopardy);
	}

	public void Grow(int cardCount, int startingDamage, BattleCard.Factor factor)
	{
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DisablePassButton());
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.HidePokeButton());
		float pileHeight = player.Jeopardy.GetPileHeight((cardCount <= 12) ? cardCount : 12);
		background.Grow(pileHeight);
		cardSlots.Grow(cardCount);
		slider.Grow(startingDamage, factor);
	}

	public void Collapse()
	{
		background.Collapse();
		cardSlots.Collapse();
		slider.Collapse();
	}
}
