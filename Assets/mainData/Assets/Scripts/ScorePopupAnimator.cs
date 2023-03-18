using System.Collections.Generic;
using UnityEngine;

public class ScorePopupAnimator : BrawlerPopupAnimator
{
	public Color topLayer;

	public Color middleLayer;

	public Color bottomLayer;

	private MeshRenderer _medalMesh;

	private List<TextMesh> _scoreMeshList;

	private static Dictionary<string, Texture> _medalTextures;

	public void SetMedal(string medal)
	{
		if (string.IsNullOrEmpty(medal))
		{
			CspUtils.DebugLog("medal string is " + medal);
			return;
		}
		if (!_medalTextures.ContainsKey(medal))
		{
			string text = "brawler_bundle|brawler_points_" + medal;
			Texture texture = GUIManager.Instance.LoadTexture(text);
			
			if (!(texture != null))
			{
				CspUtils.DebugLog("BrawlerMedalPopup: unable to load medal texture <" + text + ">");
				return;
			}
			_medalTextures.Add(medal, texture);
		}
		_medalMesh.material.mainTexture = _medalTextures[medal];
	}

	public override void SetText(string newPopupText)
	{
		base.SetText(newPopupText);
		foreach (TextMesh scoreMesh in _scoreMeshList)
		{
			scoreMesh.text = newPopupText;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (_medalTextures == null)
		{
			_medalTextures = new Dictionary<string, Texture>();
		}
		_scoreMeshList = new List<TextMesh>();
		TextMesh[] components = Utils.GetComponents<TextMesh>(base.gameObject, Utils.SearchChildren);
		if (components != null)
		{
			TextMesh[] array = components;
			foreach (TextMesh textMesh in array)
			{
				string name = textMesh.transform.parent.name;
				if (name == "TopLayer")
				{
					textMesh.renderer.material.renderQueue = 3003;
					AddTextMesh(textMesh, topLayer);
				}
				else if (name == "MiddleLayer")
				{
					textMesh.renderer.material.renderQueue = 3002;
					AddTextMesh(textMesh, middleLayer);
				}
				else if (name == "BottomLayer")
				{
					textMesh.renderer.material.renderQueue = 3001;
					AddTextMesh(textMesh, bottomLayer);
				}
				else
				{
					CspUtils.DebugLog("failed to add mesh <" + textMesh.gameObject.name + "> because parent <" + name + "> is not a layer");
				}
				if (textMesh.gameObject.name == "ScoreText")
				{
					_scoreMeshList.Add(textMesh);
				}
				if (textMesh.gameObject.name == "PointsText")
				{
					textMesh.text = AppShell.Instance.stringTable["#score_popup"];
				}
			}
		}
		_medalMesh = Utils.FindNodeInChildren(base.transform, "MedalIcon").GetComponent<MeshRenderer>();
		_medalMesh.material.shader = Shader.Find("Marvel/Base/Self-Illuminated-Full Bright");
		_medalMesh.material.color = new Color(1f, 1f, 1f, 1f);
		_medalMesh.material.renderQueue = 3000;
		AddMesh(_medalMesh, _medalMesh.material.color);
	}
}
