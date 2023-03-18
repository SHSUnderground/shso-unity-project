using System;
using UnityEngine;

public class SHSUIViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	public delegate void RetriveTreeNodeObject(IGUIContainer tree);

	private SHSSkin assignedSkin;

	private Vector2 scrollPos;

	private float renderW;

	private float renderH;

	private Texture2D leftArrow;

	private Texture2D rightArrow;

	private Texture2D midArrow;

	public SHSUIViewerWindow(string PanelName)
		: base(PanelName, null)
	{
		leftArrow = (Texture2D)Resources.Load("GUI/LineLeft");
		rightArrow = (Texture2D)Resources.Load("GUI/LineRight");
		midArrow = (Texture2D)Resources.Load("GUI/LineMiddle");
		SetBackground(new Color(1f, 0.4f, 1f, 1f));
	}

	public override void OnShow()
	{
		BuildTree(GUIManager.Instance.Root);
		scrollPos = Vector2.zero;
		base.OnShow();
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		scrollPos = GUI.BeginScrollView(base.rect, scrollPos, new Rect(0f, 0f, renderW, renderH));
		base.Draw(drawFlags);
		DrawTree(GUIManager.Instance.Root);
		GUI.EndScrollView();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}

	public void BuildTree(IGUIContainer topNode)
	{
		renderW = 0f;
		renderH = 0f;
		IterateTreeNodes(topNode, RetrieveTreeNodeObjectFunc);
	}

	public void DrawTree(IGUIContainer topNode)
	{
		IterateTreeNodes(topNode, DrawTreeNodeObjectFunc);
	}

	public void IterateTreeNodes(IGUIContainer tree, RetriveTreeNodeObject callback)
	{
		foreach (IGUIContainer item in tree.ControlList.FindAll(delegate(IGUIControl c)
		{
			return c is IGUIContainer;
		}))
		{
			callback(item);
			IterateTreeNodes(item, callback);
		}
	}

	private string formatName(IGUIContainer tnode)
	{
		return tnode.Id + "\n(" + tnode.ScreenRect.xMin + "," + tnode.ScreenRect.yMin + "," + tnode.ScreenRect.width + "," + tnode.ScreenRect.height + ")";
	}

	private void RetrieveTreeNodeObjectFunc(IGUIContainer tnode)
	{
		int marginX = Branch.marginX;
		int spanY = Branch.spanY;
		bool flag = true;
		Branch branch2 = tnode.GraphNode = new Branch(tnode);
		branch2.Text = tnode.Id;
		if (assignedSkin != null)
		{
			float minWidth;
			float maxWidth;
			assignedSkin.skin.box.CalcMinMaxWidth(new GUIContent(formatName(tnode)), out minWidth, out maxWidth);
			branch2.sz = new Vector2(maxWidth + 20f, 30f);
		}
		else
		{
			branch2.sz = new Vector2(150f, 30f);
		}
		branch2.delta = marginX;
		branch2.Location = new Vector2(marginX / 2, (tnode.Level - 1) * spanY);
		while (tnode.Parent != null && tnode.Parent.GraphNode != null)
		{
			Branch graphNode = tnode.GraphNode;
			IGUIContainer parent = tnode.Parent;
			Branch graphNode2 = parent.GraphNode;
			float num = graphNode2.sz.x + graphNode2.delta;
			float num2 = graphNode.sz.x + graphNode.delta;
			graphNode2.kidsW -= (flag ? 0f : graphNode.prevW);
			graphNode2.delta += (num2 + graphNode2.kidsW - num + Math.Abs(num2 + graphNode2.kidsW - num)) / 2f;
			num = graphNode2.sz.x + graphNode2.delta;
			graphNode2.kidsW += num2;
			graphNode.prevW = num2;
			if (flag)
			{
				graphNode.Location = new Vector2(graphNode2.Location.x + graphNode2.kidsW - num2, (tnode.Level - 1) * spanY);
				flag = false;
			}
			renderW = Math.Max(graphNode2.Location.x + num + (float)(Branch.marginX / 2), renderW);
			renderH = Math.Max((tnode.Level - 1) * spanY + spanY, renderH);
			tnode = parent;
		}
		if (flag && renderW > (float)marginX)
		{
			branch2.Location += new Vector2(renderW, 0f);
			renderW = Math.Max(branch2.Location.x + branch2.sz.x + branch2.delta, renderW);
		}
	}

	private void DrawTreeNodeObjectFunc(IGUIContainer tnode)
	{
		Branch graphNode = tnode.GraphNode;
		Rect rail = graphNode.rail;
		Color color = GUI.color;
		if (tnode.IsVisible && !tnode.IsActive)
		{
			GUI.color = new Color(1f, 0f, 0f);
		}
		else if (tnode.IsVisible)
		{
			GUI.color = new Color(0.3f, 0.8f, 3f);
		}
		else if (tnode.IsActive)
		{
			GUI.color = new Color(0.8f, 0.8f, 0f);
		}
		if (tnode.Parent != null && tnode.Parent.GraphNode != null)
		{
			Branch graphNode2 = tnode.Parent.GraphNode;
			Rect rail2 = graphNode2.rail;
			Texture2D image;
			Rect position;
			if (tnode.Parent.ControlList.Count == 1)
			{
				image = midArrow;
				position = new Rect(rail2.x + rail2.width / 2f - 5f, rail2.y + rail2.height, rail.x + rail.width / 2f - (rail2.x + rail2.width / 2f) + 10f, rail.y - (rail2.y + rail2.height));
			}
			else if (rail2.x > rail.x)
			{
				image = rightArrow;
				position = new Rect(rail.x + rail.width / 2f, rail2.y + rail2.height, rail2.x + rail2.width / 2f - (rail.x + rail.width / 2f), rail.y - (rail2.y + rail2.height));
			}
			else if (rail2.x < rail.x)
			{
				image = leftArrow;
				position = new Rect(rail2.x + rail2.width / 2f, rail2.y + rail2.height, rail.x + rail.width / 2f - (rail2.x + rail2.width / 2f), rail.y - (rail2.y + rail2.height));
			}
			else
			{
				image = leftArrow;
				position = new Rect(rail2.x + rail2.width / 2f, rail2.y + rail2.height, rail.x + rail.width / 2f - (rail2.x + rail2.width / 2f), rail.y - (rail2.y + rail2.height));
			}
			GUI.DrawTexture(position, image);
		}
		GUI.Box(rail, formatName(tnode));
		GUI.color = color;
	}

	private void DebugTreeNodeObjectFunc(IGUIContainer tnode)
	{
		CspUtils.DebugLog(tnode);
		CspUtils.DebugLog(tnode.GraphNode);
		Branch graphNode = tnode.GraphNode;
		Rect rail = graphNode.rail;
		if (tnode.Parent != null && tnode.Parent.GraphNode != null)
		{
			Branch graphNode2 = tnode.Parent.GraphNode;
			CspUtils.DebugLog(graphNode2);
			CspUtils.DebugLog(graphNode2.rail);
			Rect rail2 = graphNode2.rail;
			CspUtils.DebugLog(new Rect(rail.x, rail2.y + rail2.height, rail2.x - rail.x, rail.y - rail2.y + rail2.height));
		}
	}
}
