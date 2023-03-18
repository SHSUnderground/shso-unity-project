using UnityEngine;

public class DamagePopupAnimator : BrawlerPopupAnimator
{
	public Color topColor = new Color(1f, 0.73f, 0f, 1f);

	public Color shadowColor = new Color(0f, 0f, 0f, 0.5f);

	protected TextMesh textMesh;

	protected TextMesh shadowMesh;

	public override void SetText(string newPopupText)
	{
		base.SetText(newPopupText);
		textMesh.text = newPopupText;
		shadowMesh.text = newPopupText;
	}

	protected override void Awake()
	{
		base.Awake();
		textMesh = Utils.FindNodeInChildren(base.transform, "TopText").GetComponent<TextMesh>();
		shadowMesh = Utils.FindNodeInChildren(base.transform, "ShadowText").GetComponent<TextMesh>();
		AddTextMesh(textMesh, topColor);
		AddTextMesh(shadowMesh, shadowColor);
	}
}
