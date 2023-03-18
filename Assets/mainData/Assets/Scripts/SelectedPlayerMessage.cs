using UnityEngine;

public class SelectedPlayerMessage : ShsEventMessage
{
	public int SelectedPlayerId;

	public string SelectedPlayerName;

	public GameObject SelectedPlayer;

	public string SelectedHeroName;

	public SelectedPlayerMessage(int SelectedPlayerId, string SelectedPlayerName, string SelectedHeroName, GameObject SelectedPlayer)
	{
		this.SelectedPlayerId = SelectedPlayerId;
		this.SelectedPlayerName = SelectedPlayerName;
		this.SelectedPlayer = SelectedPlayer;
		this.SelectedHeroName = SelectedHeroName;
	}
}
