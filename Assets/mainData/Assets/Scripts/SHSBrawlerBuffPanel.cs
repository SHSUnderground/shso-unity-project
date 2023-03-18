using System.Collections.Generic;
using UnityEngine;

public class SHSBrawlerBuffPanel : GUIControlWindow
{
	public class QueuedBuffIcon
	{
		public string icon;

		public string toolTip;

		public int ID;
	}

	protected List<Vector2> buffIconPositions;

	protected Vector2 iconSize = new Vector2(32f, 32f);

	protected List<GUIDrawTexture> icons;

	protected List<QueuedBuffIcon> visibleBuffList;

	protected int buffIconIDCount;

	protected float itemScale = 1f;

	public SHSBrawlerBuffPanel(float ItemScale)
	{
		itemScale = ItemScale;
	}

	public override bool InitializeResources(bool reload)
	{
		buffIconPositions = new List<Vector2>();
		buffIconPositions.Add(new Vector2(79f, 124f));
		buffIconPositions.Add(new Vector2(124f, 106f));
		buffIconPositions.Add(new Vector2(144f, 60f));
		CreateGUIElements();
		return base.InitializeResources(reload);
	}

	public void CreateGUIElements()
	{
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		icons = new List<GUIDrawTexture>();
		visibleBuffList = new List<QueuedBuffIcon>();
		Clear();
		for (int i = 0; i < buffIconPositions.Count; i++)
		{
			GUIDrawTexture gUIDrawTexture = new GUIDrawTexture();
			gUIDrawTexture.TextureSource = "brawler_bundle|generic";
			gUIDrawTexture.SetPosition(buffIconPositions[i] * itemScale);
			gUIDrawTexture.SetSize(iconSize * itemScale);
			gUIDrawTexture.ScaleMode = ScaleMode.StretchToFill;
			gUIDrawTexture.IsVisible = false;
			Add(gUIDrawTexture);
			icons.Add(gUIDrawTexture);
		}
	}

	public int AddBuff(string buffIcon, string description)
	{
		QueuedBuffIcon queuedBuffIcon = new QueuedBuffIcon();
		queuedBuffIcon.icon = buffIcon;
		queuedBuffIcon.ID = buffIconIDCount;
		queuedBuffIcon.toolTip = description;
		buffIconIDCount++;
		visibleBuffList.Add(queuedBuffIcon);
		return queuedBuffIcon.ID;
	}

	public void RemoveBuff(int buffID)
	{
		int i;
		for (i = 0; i < visibleBuffList.Count && visibleBuffList[i].ID != buffID; i++)
		{
		}
		if (i < visibleBuffList.Count)
		{
			visibleBuffList.RemoveAt(i);
		}
	}

	public void UpdateBuffIcons()
	{
		int count = buffIconPositions.Count;
		int num = count;
		if (visibleBuffList.Count < count)
		{
			num = visibleBuffList.Count;
		}
		int i;
		for (i = 0; i < num; i++)
		{
			icons[i].IsVisible = true;
			icons[i].TextureSource = visibleBuffList[i].icon;
			icons[i].ToolTip = new NamedToolTipInfo(visibleBuffList[i].toolTip);
		}
		for (; i < count; i++)
		{
			icons[i].IsVisible = false;
			icons[i].ToolTip = null;
		}
	}

	public override void Update()
	{
		if (isVisible)
		{
			int count = buffIconPositions.Count;
			if (count > visibleBuffList.Count)
			{
				count = visibleBuffList.Count;
			}
		}
		base.Update();
	}

	public void ClearBuffIcons()
	{
		visibleBuffList.Clear();
		UpdateBuffIcons();
	}
}
