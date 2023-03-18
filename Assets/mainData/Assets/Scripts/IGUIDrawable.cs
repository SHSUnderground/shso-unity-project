using System.Collections;

public interface IGUIDrawable
{
	void DrawPreprocess();

	void InitiateDraw(GUIControl.DrawModeSetting DrawFlags);

	void Draw(GUIControl.DrawModeSetting DrawFlags);

	void DrawFinalize();

	void DebugDraw(BitArray DrawFlags);
}
