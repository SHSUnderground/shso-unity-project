using UnityEngine;

public abstract class SHSMySquadChallengeProgressMeter : GUISimpleControlWindow
{
	protected GUIImage meterBar;

	protected GUIImage leftEnd;

	protected GUIImage rightEnd;

	protected GUILabel meterLabel;

	protected GUILabel loadingLabel;

	protected int meterWidth;

	protected int meterHeight;

	protected int xOffset;

	protected int yOffset;

	protected float currentValue;

	protected float maxValue;

	protected float PercentComplete
	{
		set
		{
			loadingLabel.IsVisible = false;
			float num = value * (float)(meterWidth - xOffset);
			Vector2 size = leftEnd.Size;
			float x = size.x;
			Vector2 size2 = rightEnd.Size;
			if (num > x + size2.x + 5f)
			{
				leftEnd.IsVisible = true;
				Vector2 size3 = leftEnd.Size;
				float x2 = size3.x;
				Vector2 size4 = rightEnd.Size;
				float x3 = num - (x2 + size4.x);
				meterBar.SetSize(new Vector2(x3, meterHeight));
				meterBar.IsVisible = true;
				GUIImage gUIImage = rightEnd;
				Vector2 position = meterBar.Position;
				float x4 = position.x;
				Vector2 size5 = meterBar.Size;
				gUIImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(Mathf.Floor(x4 + size5.x), yOffset));
				rightEnd.IsVisible = true;
			}
			else
			{
				Vector2 size6 = leftEnd.Size;
				float x5 = size6.x;
				Vector2 size7 = rightEnd.Size;
				if (num >= x5 + size7.x)
				{
					leftEnd.IsVisible = true;
					rightEnd.IsVisible = true;
				}
				else
				{
					leftEnd.IsVisible = false;
					rightEnd.IsVisible = false;
				}
				GUIImage gUIImage2 = rightEnd;
				Vector2 position2 = leftEnd.Position;
				float x6 = position2.x;
				Vector2 size8 = leftEnd.Size;
				gUIImage2.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(x6 + size8.x, yOffset));
				meterBar.IsVisible = false;
			}
			meterLabel.Text = string.Format("{0:0}/{1:0}", currentValue, maxValue);
			GUIContent content = new GUIContent(meterLabel.Text);
			Vector2 size9 = meterLabel.Style.UnityStyle.CalcSize(content);
			size9.x += 1f;
			meterLabel.SetSize(size9);
			meterLabel.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(0f, 0f));
			meterLabel.IsVisible = true;
		}
	}

	public SHSMySquadChallengeProgressMeter()
	{
		InitializeMeter();
	}

	public void SetValues(float CurrentValue, float MaxValue)
	{
		currentValue = CurrentValue;
		maxValue = MaxValue;
		if (maxValue > 0f)
		{
			PercentComplete = currentValue / maxValue;
		}
		else
		{
			PercentComplete = 0f;
		}
	}

	protected abstract void InitializeMeter();
}
