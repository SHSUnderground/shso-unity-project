using System;
using UnityEngine;

public class SHSCardGameInitialSelectionWindow : SHSCardGameGadgetCenterWindowBase
{
	public class PlayButton : GlowButton
	{
		public class ButtonAnimations : SHSAnimations
		{
			public static AnimClip AnimateButtonNormal(GUIAnimatedButton hero, Vector2 targetSize, Vector2 targetOffset)
			{
				Vector2 offset = hero.Offset;
				AnimClip pieceOne = Absolute.OffsetX(Path.Linear(offset.x, targetOffset.x, 0.1f), hero);
				Vector2 offset2 = hero.Offset;
				AnimClip pieceOne2 = pieceOne ^ Absolute.OffsetY(Path.Linear(offset2.y, targetOffset.y, 0.1f), hero);
				Vector2 size = hero.Size;
				AnimClip pieceOne3 = pieceOne2 ^ Absolute.SizeX(Path.Linear(size.x, targetSize.x, 0.1f), hero);
				Vector2 size2 = hero.Size;
				return pieceOne3 ^ Absolute.SizeY(Path.Linear(size2.y, targetSize.y, 0.1f), hero);
			}

			public static AnimClip AnimateButtonHighlight(GUIAnimatedButton hero, Vector2 targetSize, Vector2 targetOffset)
			{
				Vector2 offset = hero.Offset;
				AnimClip pieceOne = Absolute.OffsetX(GlowButton.BounceInPath(offset.x, targetOffset.x, 0.07f, 0.35f), hero);
				Vector2 offset2 = hero.Offset;
				AnimClip pieceOne2 = pieceOne ^ Absolute.OffsetY(GlowButton.BounceInPath(offset2.y, targetOffset.y, 0.13f, 0.35f), hero);
				Vector2 size = hero.Size;
				AnimClip pieceOne3 = pieceOne2 ^ Absolute.SizeX(GlowButton.BounceInPath(size.x, targetSize.x, 0.07f, 0.35f), hero);
				Vector2 size2 = hero.Size;
				return pieceOne3 ^ Absolute.SizeY(GlowButton.BounceInPath(size2.y, targetSize.y, 0.13f, 0.35f), hero);
			}

			public static AnimClip AnimateButtonPressed(GUIAnimatedButton hero, Vector2 targetSize, Vector2 targetOffset)
			{
				Vector2 offset = hero.Offset;
				AnimClip pieceOne = Absolute.OffsetX(Path.Linear(offset.x, targetOffset.x, 0.1f), hero);
				Vector2 offset2 = hero.Offset;
				AnimClip pieceOne2 = pieceOne ^ Absolute.OffsetY(Path.Linear(offset2.y, targetOffset.y, 0.1f), hero);
				Vector2 size = hero.Size;
				AnimClip pieceOne3 = pieceOne2 ^ Absolute.SizeX(Path.Linear(size.x, targetSize.x, 0.1f), hero);
				Vector2 size2 = hero.Size;
				return pieceOne3 ^ Absolute.SizeY(Path.Linear(size2.y, targetSize.y, 0.1f), hero);
			}
		}

		public GUIButton play;

		private GUIAnimatedButton playText;

		private GUIAnimatedButton cards;

		private GUIAnimatedButton hero1;

		private GUIAnimatedButton hero2;

		private AnimClip OverOutAnimation;

		public PlayButton()
		{
			play = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(639f, 279f), new Vector2(0f, 0f));
			play.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|cardlauncher_playbutton_bg");
			Add(play);
			Add(glow);
			cards = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_playbutton_cards", 0f, new Vector2(660f, 259f), new Vector2(0f, 0f));
			cards.SetupButton(0.88f, 1f, 0.89f);
			Add(cards);
			hero1 = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_playbutton_orangeguy", 0.08f, new Vector2(215f, 181f), new Vector2(-194f, -60f));
			hero1.SetupButton(0.88f, 1f, 0.89f);
			hero1.HighlightPath = null;
			hero1.NormalPath = null;
			hero1.PressedPath = null;
			Add(hero1);
			hero2 = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_playbutton_purpleguy", 0.08f, new Vector2(220f, 216f), new Vector2(172f, 52f));
			hero2.SetupButton(0.88f, 1f, 0.89f);
			hero2.HighlightPath = null;
			hero2.NormalPath = null;
			hero2.PressedPath = null;
			Add(hero2);
			playText = GenAnimatedButtonPart("cardgamegadget_bundle|L_cardlauncher_playbutton_text", 0.16f, new Vector2(270f, 115f), new Vector2(0f, 0f));
			playText.SetupButton(0.88f, 1f, 0.89f);
			Add(playText);
			cards.LinkToSourceButton(play);
			hero1.LinkToSourceButton(play);
			hero2.LinkToSourceButton(play);
			playText.LinkToSourceButton(play);
			SetupMove();
			play.MouseOver += base.FadeInGlow;
			play.MouseOut += base.FadeOutGlow;
		}

		public void SetupMove()
		{
			hero1.OnButtonStateChanged += delegate(GUIAnimatedButton.HoverStates newState)
			{
				switch (newState)
				{
				case GUIAnimatedButton.HoverStates.Normal:
					base.AnimationPieceManager.SwapOut(ref OverOutAnimation, ButtonAnimations.AnimateButtonNormal(hero1, new Vector2(215f, 181f) * 0.85f, new Vector2(-200f, -60f) * 0.85f) ^ ButtonAnimations.AnimateButtonNormal(hero2, new Vector2(220f, 216f) * 0.85f, new Vector2(172f, 52f) * 0.85f));
					break;
				case GUIAnimatedButton.HoverStates.Highlight:
					base.AnimationPieceManager.SwapOut(ref OverOutAnimation, ButtonAnimations.AnimateButtonHighlight(hero1, new Vector2(215f, 181f), new Vector2(-200f, -60f)) ^ ButtonAnimations.AnimateButtonHighlight(hero2, new Vector2(220f, 216f), new Vector2(172f, 52f)));
					break;
				case GUIAnimatedButton.HoverStates.Pressed:
					base.AnimationPieceManager.SwapOut(ref OverOutAnimation, ButtonAnimations.AnimateButtonPressed(hero1, new Vector2(215f, 181f) * 0.85f, new Vector2(-200f, -60f) * 0.85f) ^ ButtonAnimations.AnimateButtonPressed(hero2, new Vector2(220f, 216f) * 0.85f, new Vector2(172f, 52f) * 0.85f));
					break;
				}
			};
		}

		protected override Vector2[] GetGlowPath()
		{
			return new Vector2[8]
			{
				new Vector2(281f, 118f),
				new Vector2(276f, 124f),
				new Vector2(-284f, 124f),
				new Vector2(-289f, 116f),
				new Vector2(-306f, -107f),
				new Vector2(-300f, -117f),
				new Vector2(294f, -128f),
				new Vector2(303f, -119f)
			};
		}
	}

	public class LearnToPlay : GlowButton
	{
		public GUIButton learnToPlay;

		private GUIAnimatedButton hero;

		private GUIAnimatedButton text;

		public LearnToPlay()
		{
			learnToPlay = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			learnToPlay.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|cardlauncher_helpbutton_bg");
			learnToPlay.HitTestType = HitTestTypeEnum.Alpha;
			Add(learnToPlay);
			Add(glow);
			hero = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_helpbutton_graphic", 0f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(hero);
			text = GenAnimatedButtonPart("cardgamegadget_bundle|L_cardlauncher_helpbutton_text", 0.08f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(text);
			hero.LinkToSourceButton(learnToPlay);
			text.LinkToSourceButton(learnToPlay);
			learnToPlay.MouseOver += base.FadeInGlow;
			learnToPlay.MouseOut += base.FadeOutGlow;
		}

		protected override Vector2[] GetGlowPath()
		{
			return new Vector2[8]
			{
				new Vector2(112f, 77f),
				new Vector2(103f, 84f),
				new Vector2(-91f, 78f),
				new Vector2(-101f, 67f),
				new Vector2(-116f, -66f),
				new Vector2(-110f, -75f),
				new Vector2(103f, -85f),
				new Vector2(113f, -78f)
			};
		}
	}

	public class BuyCards : GlowButton
	{
		public GUIButton buyCards;

		private GUIAnimatedButton hero;

		private GUIAnimatedButton text;

		public BuyCards()
		{
			buyCards = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			buyCards.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|cardlauncher_buycards_bg");
			buyCards.HitTestType = HitTestTypeEnum.Alpha;
			Add(buyCards);
			Add(glow);
			hero = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_buycards_graphic", 0f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(hero);
			text = GenAnimatedButtonPart("cardgamegadget_bundle|L_cardlauncher_buycards_text", 0.08f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(text);
			hero.LinkToSourceButton(buyCards);
			text.LinkToSourceButton(buyCards);
			buyCards.MouseOver += base.FadeInGlow;
			buyCards.MouseOut += base.FadeOutGlow;
		}

		protected override Vector2[] GetGlowPath()
		{
			return new Vector2[8]
			{
				new Vector2(115f, 78f),
				new Vector2(110f, 84f),
				new Vector2(-110f, 84f),
				new Vector2(-115f, 78f),
				new Vector2(-115f, -78f),
				new Vector2(-110f, -84f),
				new Vector2(110f, -84f),
				new Vector2(115f, -78f)
			};
		}
	}

	public class MyCardsAndDecks : GlowButton
	{
		public GUIButton myCardsAndDecks;

		private GUIAnimatedButton hero;

		private GUIAnimatedButton text;

		public override bool IsEnabled
		{
			get
			{
				return base.IsEnabled;
			}
			set
			{
				base.IsEnabled = value;
				myCardsAndDecks.IsEnabled = value;
				hero.IsEnabled = value;
				text.IsEnabled = value;
				glow.IsEnabled = value;
			}
		}

		public MyCardsAndDecks()
		{
			myCardsAndDecks = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			myCardsAndDecks.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|cardlauncher_mycards_bg");
			myCardsAndDecks.HitTestType = HitTestTypeEnum.Alpha;
			Add(myCardsAndDecks);
			Add(glow);
			hero = GenAnimatedButtonPart("cardgamegadget_bundle|cardlauncher_mycards_graphic", 0f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(hero);
			text = GenAnimatedButtonPart("cardgamegadget_bundle|L_cardlauncher_mycards_text", 0.08f, new Vector2(256f, 256f), new Vector2(0f, 0f));
			Add(text);
			hero.LinkToSourceButton(myCardsAndDecks);
			text.LinkToSourceButton(myCardsAndDecks);
			myCardsAndDecks.MouseOver += base.FadeInGlow;
			myCardsAndDecks.MouseOut += base.FadeOutGlow;
		}

		protected override Vector2[] GetGlowPath()
		{
			return new Vector2[8]
			{
				new Vector2(93f, 68f),
				new Vector2(87f, 74f),
				new Vector2(-104f, 81f),
				new Vector2(-109f, 76f),
				new Vector2(-113f, -77f),
				new Vector2(-103f, -87f),
				new Vector2(102f, -80f),
				new Vector2(109f, -67f)
			};
		}
	}

	public abstract class GlowButton : GUISimpleControlWindow
	{
		public const float HIGHLIGHT_ANIMATION_TIME = 0.35f;

		protected SHSGlowOutlineWindow glow;

		private AnimClip currentFadeAnimation;

		public GlowButton()
		{
			glow = new SHSGlowOutlineWindow(GetGlowPath());
			glow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			glow.IsVisible = false;
			glow.SetSize(2000f, 2000f);
			glow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		}

		protected GUIAnimatedButton GenAnimatedButtonPart(string textureSource, float delay, Vector2 size, Vector2 offset)
		{
			GUIAnimatedButton gUIAnimatedButton = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(size, offset);
			gUIAnimatedButton.SetupButton(0.95f, 1f, 0.89f);
			gUIAnimatedButton.TextureSource = textureSource;
			gUIAnimatedButton.HitTestType = HitTestTypeEnum.Transparent;
			gUIAnimatedButton.HighlightPath = delegate(float CurrentPercentage, float HighlightPercentage)
			{
				return BounceInPath(CurrentPercentage, HighlightPercentage, delay, 0.35f);
			};
			return gUIAnimatedButton;
		}

		protected static AnimPath BounceInPath(float current, float target, float delay, float time)
		{
			return AnimClipBuilder.Path.Constant(current, delay) | SHSAnimations.GenericPaths.LinearWith2xSingleWiggle(current, target, time);
		}

		protected abstract Vector2[] GetGlowPath();

		public override void OnShow()
		{
			glow.Highlight(false);
			base.OnShow();
		}

		protected void FadeInGlow(GUIControl sender, GUIMouseEvent EventData)
		{
			glow.IsVisible = true;
			glow.Highlight(true);
			base.AnimationPieceManager.SwapOut(ref currentFadeAnimation, SHSAnimations.Generic.FadeIn(glow, 0.3f));
		}

		protected void FadeOutGlow(GUIControl sender, GUIMouseEvent EventData)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			base.AnimationPieceManager.SwapOut(ref currentFadeAnimation, SHSAnimations.Generic.FadeOut(glow, 0.3f));
			currentFadeAnimation.OnFinished += (Action)(object)(Action)delegate
			{
				glow.Highlight(false);
				glow.IsVisible = false;
			};
		}
	}

	public SHSCardGameInitialSelectionWindow(SHSCardGameGadgetWindow mainWindow)
		: base(mainWindow)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(673f, 522f), new Vector2(0f, 0f));
		gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_screen1_bg";
		Add(gUIImage);
		PlayButton playButton = GUIControl.CreateControlFrameCentered<PlayButton>(new Vector2(2000f, 2000f), new Vector2(0f, -55f));
		Add(playButton);
		LearnToPlay learnToPlay = GUIControl.CreateControlFrameCentered<LearnToPlay>(new Vector2(256f, 256f), new Vector2(-244f, 183f));
		Add(learnToPlay);
		BuyCards buyCards = GUIControl.CreateControlFrameCentered<BuyCards>(new Vector2(256f, 256f), new Vector2(0f, 179f));
		Add(buyCards);
		MyCardsAndDecks myCardsAndDecks = GUIControl.CreateControlFrameCentered<MyCardsAndDecks>(new Vector2(256f, 256f), new Vector2(244f, 183f));
		Add(myCardsAndDecks);
		playButton.play.Click += play_Click;
		learnToPlay.learnToPlay.Click += learnToPlay_Click;
		buyCards.buyCards.Click += buyCards_Click;
		myCardsAndDecks.myCardsAndDecks.Click += myCardsAndDecks_Click;
	}

	private void play_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (AppShell.Instance.Profile.FirstCardGame)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#CARDGAME_TUTORIAL_PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					learnToPlay_Click(null, null);
				}
				else
				{
					mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
				}
				AppShell.Instance.Profile.FirstCardGame = false;
				AppShell.Instance.Profile.PersistExtendedData();
			}, ModalLevelEnum.Default);
		}
		else
		{
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		}
	}

	private void learnToPlay_Click(GUIControl sender, GUIClickEvent EventData)
	{
		GUIManager.Instance.ShowDynamicWindow(new SHSCardGameTutorialWindow(mainWindow), ModalLevelEnum.Default);
	}

	private void buyCards_Click(GUIControl sender, GUIClickEvent EventData)
	{
		ShoppingWindow shoppingWindow = new ShoppingWindow(NewShoppingManager.ShoppingCategory.Card);
		shoppingWindow.launch();
	}

	private void myCardsAndDecks_Click(GUIControl sender, GUIClickEvent EventData)
	{
		mainWindow.CloseGadget();
		AppShell.Instance.Transition(GameController.ControllerType.DeckBuilder);
	}
}
