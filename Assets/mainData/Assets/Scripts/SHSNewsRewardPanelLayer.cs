using UnityEngine;

public class SHSNewsRewardPanelLayer : SHSNewsRewardDayLayer
{
	public SHSNewsRewardPanelLayer(string rewardPanelTextureSource)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(84f, 94f), Vector2.zero);
		gUIImage.TextureSource = rewardPanelTextureSource;
		layerControls.Add(gUIImage);
	}
}
