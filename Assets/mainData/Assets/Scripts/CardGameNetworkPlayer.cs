using UnityEngine;

[AddComponentMenu("CardGame/Private/Network Player")]
internal class CardGameNetworkPlayer : CardGamePlayer
{
	protected override void OnIntroSequenceFinished(CardGameEvent.IntroSequenceFinished msg)
	{
		base.OnIntroSequenceFinished(msg);
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ShowPokeButton());
	}
}
