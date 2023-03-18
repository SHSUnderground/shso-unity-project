using UnityEngine;

namespace CardGame
{
	public class PlayerData
	{
		public enum PlayerKind
		{
			Local,
			Network,
			Computer
		}

		public string Name;

		public string Hero;

		public string Status;

		public string DeckRecipe;

		public int DeckId;

		public long UserId;

		public PlayerKind Kind;

		[HideInInspector]
		public int PlayerId;

		public int NetId;

		public PlayerData()
		{
			Kind = PlayerKind.Local;
			NetId = 0;
			PlayerId = -1;
			Name = string.Empty;
			Hero = string.Empty;
			Status = string.Empty;
			DeckId = -1;
			UserId = -1L;
		}
	}
}
