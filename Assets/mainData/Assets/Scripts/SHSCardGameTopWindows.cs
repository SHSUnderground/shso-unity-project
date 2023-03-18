using UnityEngine;

public class SHSCardGameTopWindows
{
	public class CardGameTopWindow : SHSGadget.GadgetTopWindow
	{
		public CardGameTopWindow(float x, float y, string simplePath)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(x, y), Vector2.zero);
			gUIImage2.TextureSource = simplePath;
			Add(gUIImage2);
		}
	}
}
