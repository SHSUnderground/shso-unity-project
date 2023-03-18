using CardGame;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardSADebugWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private SHSStyle headerStyle;

	private Vector2[] scrollPos;

	private List<CardGameAI> brains;

	private bool enabled;

	public SHSCardSADebugWindow(string PanelName)
		: base(PanelName, null)
	{
		SetBackground(new Color(1f, 0.4f, 1f, 1f));
		scrollPos = new Vector2[2];
		brains = new List<CardGameAI>();
		brains.Add(new CardGameAIPassive());
		brains.Add(new CardGameAIRandom());
		brains.Add(new CardGameAIMinMax());
	}

	public override void OnShow()
	{
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleHeader");
		scrollPos[0] = Vector2.zero;
		scrollPos[1] = Vector2.zero;
		base.OnShow();
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void Update()
	{
		base.Update();
		enabled = (!(CardGameController.Instance == null) && CardGameController.Instance.players != null && !(CardGameController.Instance.players[0] == null) && CardGameController.Instance.players[0].Deck != null);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (enabled)
		{
			DrawPane(0, 10f, 10f, 500f, base.rect.height - 50f);
			if (!(CardGameController.Instance.players[1] == null) && CardGameController.Instance.players[1].Deck != null)
			{
				DrawPane(1, 510f, 10f, 500f, base.rect.height - 50f);
			}
		}
	}

	private void DrawPane(int pane, float x, float y, float w, float h)
	{
		int num = 0;
		string text = string.Empty;
		CardGamePlayer cardGamePlayer = CardGameController.Instance.players[pane];
		GUILayout.BeginArea(new Rect(x, y, w, h));
		if (pane == 0)
		{
			int queueSize = CardGameController.Instance.animQ.QueueSize;
			GUILayout.Label(string.Format("Message queue: {0} [{1}]  Anim queue: {2} [{3}]", RoomAgent.QueueSize, (!RoomAgent.QueueBusy) ? "    " : RoomAgent.Instance.CurrentMessage.ToString(), queueSize, (queueSize <= 0) ? "    " : CardGameController.Instance.animQ.CurrentAnimation));
			if (GUILayout.Button("DumpMessageQueue"))
			{
				CspUtils.DebugLog("**** DUMPING CARD GAME MESSAGE QUEUE ****");
				foreach (CardGameEvent.ServerMessage queueContent in Singleton<RoomAgent>.instance.QueueContents)
				{
					CspUtils.DebugLog(queueContent.ToString());
				}
				CspUtils.DebugLog("**** END CARD GAME MESSAGE QUEUE DUMP ****");
			}
			if (GUILayout.Button("DumpAnimQueue"))
			{
				CspUtils.DebugLog("**** DUMPING CARD GAME ANIMATION QUEUE ****");
				foreach (CardGameAnimationQueue.QueuedAnimation content in CardGameController.Instance.animQ.Contents)
				{
					CspUtils.DebugLog(string.Format("Label: {0}, proceed: {1}", content.label, content.proceedOnFinish));
				}
				CspUtils.DebugLog("**** END CARD GAME ANIMATION QUEUE DUMP ****");
			}
		}
		if (pane == 1 && cardGamePlayer is CardGameAIPlayer)
		{
			CardGameAIPlayer cardGameAIPlayer = cardGamePlayer as CardGameAIPlayer;
			for (int i = 0; i < brains.Count; i++)
			{
				if (cardGameAIPlayer.brain.ToString() == brains[i].ToString())
				{
					num = i;
					text = brains[i].ToString();
					break;
				}
			}
			if (GUILayout.Button(text))
			{
				num = (num + 1) % brains.Count;
				cardGameAIPlayer.SetBrain(brains[num]);
			}
		}
		GUILayout.Label("Player " + (pane + 1) + ((!cardGamePlayer.MyTurn) ? string.Empty : " (YOUR TURN)"), GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		scrollPos[pane] = GUILayout.BeginScrollView(scrollPos[pane]);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal(GUILayout.Width(450f));
		if (GUILayout.Button("Pass"))
		{
			cardGamePlayer.SendPickCard(null, true, PickCardType.Attack);
		}
		if (GUILayout.Button("KillHand"))
		{
			cardGamePlayer.SendDebug(0, 0, 0);
		}
		if (GUILayout.Button("KillDeck"))
		{
			cardGamePlayer.SendDebug(1, 0, 0);
		}
		GUILayout.EndHorizontal();
		DrawSection(cardGamePlayer, "Played", pane, cardGamePlayer.Played);
		DrawSection(cardGamePlayer, "Show", pane, cardGamePlayer.Show);
		DrawSection(cardGamePlayer, "Hand", pane, cardGamePlayer.Hand);
		DrawSection(cardGamePlayer, "Keeper", pane, cardGamePlayer.Keepers);
		DrawSection(cardGamePlayer, "Private", pane, cardGamePlayer.Private);
		DrawSection(cardGamePlayer, "Jeopardy", pane, cardGamePlayer.Jeopardy);
		DrawSection(cardGamePlayer, "Stock", pane, cardGamePlayer.Stock);
		DrawSection(cardGamePlayer, "Discard", pane, cardGamePlayer.Discard);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DrawSection(CardGamePlayer player, string pname, int pid, CardPile pile)
	{
		int num = 0;
		GUILayout.BeginHorizontal(GUILayout.Width(450f));
		GUILayout.Label(pname + " (" + pile.Count + ")");
		GUILayout.EndHorizontal();
		foreach (BattleCard item in pile)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(500f));
			GUILayout.Label(item.ServerID.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
			GUILayout.Space(5f);
			GUILayout.Label(item.Type, GUILayout.Width(50f));
			GUILayout.Space(5f);
			GUILayout.Label(item.NameEng, GUILayout.Width(200f));
			GUILayout.Space(5f);
			if (GUILayout.Button("Stock"))
			{
				player.SendDebug(7, item.ServerID, 0);
			}
			if (GUILayout.Button("Hand"))
			{
				player.SendDebug(7, item.ServerID, 1);
			}
			if (GUILayout.Button("Played"))
			{
				player.SendDebug(7, item.ServerID, 2);
			}
			if (GUILayout.Button("Keeper"))
			{
				player.SendDebug(7, item.ServerID, 3);
			}
			if (GUILayout.Button("Show"))
			{
				player.SendDebug(7, item.ServerID, 4);
			}
			if (GUILayout.Button("Disc."))
			{
				player.SendDebug(7, item.ServerID, 5);
			}
			if (GUILayout.Button("Private"))
			{
				player.SendDebug(7, item.ServerID, 6);
			}
			if (GUILayout.Button("Jeopardy"))
			{
				player.SendDebug(7, item.ServerID, 7);
			}
			GUILayout.EndHorizontal();
			num++;
		}
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}
}
