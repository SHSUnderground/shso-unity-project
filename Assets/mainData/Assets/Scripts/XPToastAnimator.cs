using System;
using System.Collections.Generic;
using UnityEngine;

public class XPToastAnimator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public const float TOTAL_OFFSET = 1.5f;

	public float lifetime = 2f;

	protected string text;

	private Color frontColor = ColorUtil.FromRGB255(249, 226, 145);

	private Color middleColor = ColorUtil.FromRGB255(13, 55, 0);

	private Color backColor = ColorUtil.FromRGB255(8, 32, 0);

	protected MeshRenderer frontMesh;

	protected List<MeshRenderer> middleMeshs = new List<MeshRenderer>();

	protected MeshRenderer backMesh;

	protected MeshRenderer iconMesh;

	private TextMesh[] meshes;

	private MeshRenderer[] textrenderers;

	private AnimClipManager apm = new AnimClipManager();

	private float lastUpdate = 1f;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	private void Start()
	{
		//Shader shader = Shader.Find("GUI/Text Shader Z");   // commented out by CSP
		textrenderers = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		meshes = Utils.GetComponents<TextMesh>(base.gameObject, Utils.SearchChildren);
		TextMesh[] array = meshes;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = text;
		}
		MeshRenderer[] array2 = textrenderers;
		foreach (MeshRenderer meshRenderer in array2)
		{
			string name = meshRenderer.gameObject.name;
			if (name.Contains("front"))
			{
				frontMesh = meshRenderer;
				meshRenderer.material.color = frontColor;
				//meshRenderer.material.shader = shader;   // commented out by CSP
			}
			else if (name.Contains("middle"))
			{
				meshRenderer.material.color = backColor;
				middleMeshs.Add(meshRenderer);
				//meshRenderer.material.shader = shader;   // commented out by CSP
			}
			else if (name.Contains("back"))
			{
				backMesh = meshRenderer;
				meshRenderer.material.color = backColor;
				//meshRenderer.material.shader = shader;   // commented out by CSP
			}
			else if (name.Contains("icon"))
			{
				iconMesh = meshRenderer;
				Texture mainTexture = GUIManager.Instance.LoadTexture("gameworld_bundle|L_game_world_xp_gain_icon");
				iconMesh.material.mainTexture = mainTexture;
			}
			else
			{
				CspUtils.DebugLog("XP Toast Animator, found mystery meshrenderer called '" + meshRenderer.gameObject.name + "'!");
			}
		}
		Animate();
	}

	public void Update()
	{
		apm.Update(Time.deltaTime);
	}

	public void Animate()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		AnimClip pieceOne = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 0f, lifetime), SetAlpha);
		AnimClip pieceTwo = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 0f, lifetime), delegate(float x)
		{
			float num = AbsPosOffset(lastUpdate) - AbsPosOffset(x);
			lastUpdate = x;
			Vector3 position = base.transform.position;
			position.y -= num;
			base.transform.position = position;
		});
		AnimClip animClip = pieceOne ^ pieceTwo;
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		};
		apm.Add(animClip);
	}

	private float AbsPosOffset(float x)
	{
		return (1f - Mathf.Pow(x, 2f)) * 1.5f;
	}

	private void SetAlpha(float newAlpha)
	{
		frontMesh.material.color = new Color(frontColor.r, frontColor.g, frontColor.b, newAlpha);
		foreach (MeshRenderer middleMesh in middleMeshs)
		{
			middleMesh.material.color = new Color(middleColor.r, middleColor.g, middleColor.b, newAlpha);
		}
		backMesh.material.color = new Color(backColor.r, backColor.g, backColor.b, newAlpha);
		if (iconMesh != null)
		{
			Color color = iconMesh.material.color;
			color.a = newAlpha;
			iconMesh.material.color = color;
		}
	}
}
