using CardGame;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameBoosterPackContentsWindow : GUIDynamicWindow
{
	private GUIImage background;

	private GUIImage boosterPackRenderTexture;

	private DeckBuilderController DeckBuilder;

	private SortedCardList cardList;

	private GUIImage zoomImage;

	private bool mouseOver;

	public SHSCardGameBoosterPackContentsWindow()
	{
		DeckBuilder = (DeckBuilderController)GameController.GetController();
		SetSize(new Vector2(1022f, 512f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		background = new GUIImage();
		background.TextureSource = "common_bundle|black";
		background.Alpha = 0.75f;
		background.SetSize(new Vector2(670f, 376f));
		background.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, Vector2.zero);
		Add(background);
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, Vector2.zero, new Vector2(630f, 380f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow.HitTestType = HitTestTypeEnum.Rect;
		gUISimpleControlWindow.MouseOver += delegate
		{
			mouseOver = true;
		};
		gUISimpleControlWindow.MouseOut += delegate
		{
			mouseOver = false;
		};
		Add(gUISimpleControlWindow);
		boosterPackRenderTexture = new GUIImage();
		boosterPackRenderTexture.Id = "boosterPackRenderTexture";
		boosterPackRenderTexture.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(1024f, 512f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		boosterPackRenderTexture.Texture = DeckBuilder.boosterPackCamera.targetTexture;
		gUISimpleControlWindow.Add(boosterPackRenderTexture);
		zoomImage = new GUIImage();
		zoomImage.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, Vector2.zero, new Vector2(366f, 512f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(zoomImage);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Id = "closeBoosterPackButton";
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(200f, 0f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_okbutton");
		gUIButton.Click += delegate
		{
			Hide();
		};
		Add(gUIButton);
		AppShell.Instance.EventMgr.AddListener<BoosterPackResponseMessage>(OnBoosterPackResponse);
		AppShell.Instance.EventMgr.AddListener<CardCollection3DMessage>(On3DEvent);
		AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseEnter, null));
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (mouseOver)
		{
			GameObject gameObject = null;
			gameObject = DeckBuilder.PickCardPanel(DeckBuilder.boosterPackCamera, boosterPackRenderTexture);
			if ((bool)gameObject)
			{
				AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseEnter, gameObject));
			}
		}
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<BoosterPackResponseMessage>(OnBoosterPackResponse);
		AppShell.Instance.EventMgr.RemoveListener<CardCollection3DMessage>(On3DEvent);
		cardList.Reset(true);
	}

	protected void On3DEvent(CardCollection3DMessage msg)
	{
		if (msg.Event != 0)
		{
			return;
		}
		if (msg.Sender != null)
		{
			CardProperties cardProperties = msg.Sender.GetComponent(typeof(CardProperties)) as CardProperties;
			if (cardProperties != null)
			{
				zoomImage.Texture = cardProperties.Card.FullTexture;
				zoomImage.Alpha = 1f;
			}
		}
		else
		{
			zoomImage.Alpha = 0f;
		}
	}

	public void OnBoosterPackResponse(BoosterPackResponseMessage response)
	{
		string text = (string)response.payload["cards_awarded"];
		CspUtils.DebugLog("In booster pack window, cards_awarded: " + text);
		CardGroup cardGroup2 = new CardGroup();
		Dictionary<string, int> cardCollection = CardManager.ParseRecipe(text);
		DeckBuilder.PopulateCardGroup(cardGroup2, cardCollection, delegate(CardGroup cardGroup)
		{
			cardList = new SortedCardList(cardGroup, DeckBuilder.BoosterPackGrid, DeckBuilder.fullCardPrefab);
			cardList.PrimarySort = SortedCardList.SortMethod.Rarity;
			cardList.Sort(cardList.Compare);
			cardList.FreshLayout();
			Shader shader = Shader.Find("Marvel/Base/Self-Illuminated-Full Bright Card");
			if (shader == null)
			{
				CspUtils.DebugLog("Unable to find Marvel/Base/Self-Illuminated-Full Bright Card shader for booster pack cards");
			}
			else
			{
				foreach (CardListCard card in cardList)
				{
					MeshRenderer componentInChildren = card.CardObj.GetComponentInChildren<MeshRenderer>();
					if (componentInChildren == null)
					{
						CspUtils.DebugLog("Unable to find renderer component on zoomed card");
					}
					else
					{
						componentInChildren.materials[componentInChildren.materials.Length - 1].shader = shader;
					}
				}
			}
		}, true);
	}
}
