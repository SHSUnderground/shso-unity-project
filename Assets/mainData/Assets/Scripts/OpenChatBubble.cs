using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class OpenChatBubble : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float FadeInOutTime = 0.5f;

	private const float HORZ_LINE_SIZE = 3.5f;

	private const float BackgroundScaleChangePerLine = 0.1f;

	private const float BackgroundScaleConstant = -0.52f;

	private const float BackgroundOffsetChangePerLine = -0.16f;

	private const float BackgroundOffsetConstant = 0.23f;

	private const float TextOffsetPerLine = -0.29f;

	private const float TextOffsetConstant = 0.298f;

	private const float NameOffsetPerLine = -0.29f;

	private const float NameOffsetConstant = 0.14f;

	private const float IconBackwardsOffset = -0.75f;

	private const float TextXOffset = 1.74f;

	public Renderer icon;

	public MeshFilter IconMeshFilter;

	public TextMesh nameTxt;

	public MeshRenderer nameRender;

	public TextMesh messageTxt;

	public MeshRenderer messageRender;

	public Material CSPMaterial;

	private Material nameMaterial;

	private Material messageMaterial;

	public GameObject offsetObject;

	private AnimClipManager apm = new AnimClipManager();

	private int numberOfLines;

	public float NameZOffset = 1f;

	public float TextZOffset = 1f;

	private AnimClip fadeInOut;

	private void Awake()
	{
		if (nameRender.materials.Length < 1 || messageRender.materials.Length < 1)
		{
			CspUtils.DebugLog("materials not set for the name or messege render");
		}
		nameMaterial = nameRender.materials[0];

		// Material[] mats = messageRender.materials; // CSP added
		// mats[0] = CSPMaterial;   // CSP added because material assignment is getting set wrong
		// messageRender.materials = mats; // CSP added
		messageMaterial = messageRender.materials[0];

		
		#if UNITY_EDITOR
			CspUtils.DebugLog("messageMaterial=" + AssetDatabase.GetAssetPath(messageMaterial));
			CspUtils.DebugLog("CSPMaterial=" + AssetDatabase.GetAssetPath(CSPMaterial));
		#endif
		//// block commented out by CSP for testing  //////////////////
		//Shader shader = Shader.Find("GUI/Text Shader Z");
		//messageMaterial.shader = shader;
		//nameMaterial.shader = shader;
		//////////////////////////////////////////////////////////////////
		UpdateAlpha(0f);
	}

	public void SetIconTextureByName(string newIcon)
	{
		Texture texture = GUIManager.Instance.BundleManager.LoadAsset("gameworld_bundle", newIcon) as Texture;
		if (texture != null)
		{
			if (icon != null)
			{
				icon.material.SetTexture("_MainTex", texture);
			}
		}
		else
		{
			CspUtils.DebugLog("Could not find icon texture <" + newIcon + "> when trying to set player status icon.");
		}
	}

	public void SetupBubble(OpenChatMessage msg)
	{
		SetupText(msg);
		SetupName(msg);
		SetupBackground(msg);
	}

	private void SetupBackground(OpenChatMessage msg)
	{
		string iconTextureByName = "mshs_open_chat_speechbubble_generic";
		switch (GetConvertedSelfOpenChatMessage(msg.messageStyle))
		{
		case OpenChatMessage.MessageStyle.Friend:
			iconTextureByName = "mshs_open_chat_speechbubble_friend";
			break;
		case OpenChatMessage.MessageStyle.Sub:
			iconTextureByName = "mshs_open_chat_speechbubble_shieldagent";
			break;
		case OpenChatMessage.MessageStyle.NonSub:
			iconTextureByName = "mshs_open_chat_speechbubble_generic";
			break;
		}
		SetIconTextureByName(iconTextureByName);
		DefineBkgMeshSize();
		icon.transform.localPosition = new Vector3(0f, -0.75f, 0.23f + (float)numberOfLines * -0.16f);
		icon.transform.localScale = new Vector3(-2.0355f, 1.5f, 1.5f);
	}

	private void DefineBkgMeshSize()
	{
		float num = -0.52f + (float)numberOfLines * 0.1f;
		Vector3[] vertices = new Vector3[8]
		{
			new Vector3(-1f, 0f, -1f - num),
			new Vector3(1f, 0f, -1f - num),
			new Vector3(-1f, 0f, -0.42f - num),
			new Vector3(1f, 0f, -0.42f - num),
			new Vector3(-1f, 0f, 0.42f + num),
			new Vector3(1f, 0f, 0.42f + num),
			new Vector3(-1f, 0f, 1f + num),
			new Vector3(1f, 0f, 1f + num)
		};
		Vector2[] uv = new Vector2[8]
		{
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, 0.71f),
			new Vector2(1f, 0.71f),
			new Vector2(0f, 0.31f),
			new Vector2(1f, 0.31f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f)
		};
		int[] triangles = new int[18]
		{
			0,
			3,
			1,
			0,
			2,
			3,
			2,
			5,
			3,
			2,
			4,
			5,
			4,
			6,
			7,
			4,
			7,
			5
		};
		IconMeshFilter.mesh.Clear();
		IconMeshFilter.mesh.vertices = vertices;
		IconMeshFilter.mesh.uv = uv;
		IconMeshFilter.mesh.triangles = triangles;
		IconMeshFilter.mesh.RecalculateNormals();
	}

	private void SetupText(OpenChatMessage msg)
	{
		ConvertToMultiLineAndAdd(msg.message);
		TextZOffset = -0.29f * (float)numberOfLines + 0.298f;
		messageRender.transform.localPosition = new Vector3(1.74f, 0f, TextZOffset);
		if (msg.messageStyle != OpenChatMessage.MessageStyle.System)
		{
			messageMaterial.color = GUILabel.GenColor(30, 30, 30);
		}
		else
		{
			messageMaterial.color = CommunicationManager.GetMessageColor(msg.messageStyle);
		}
	}

	private void SetupName(OpenChatMessage msg)
	{
		if (msg.messageStyle != OpenChatMessage.MessageStyle.System)
		{
			nameTxt.text = msg.sendingPlayerName + " :";
		}
		else
		{
			nameTxt.text = "[System] :";
		}
		nameMaterial.color = CommunicationManager.GetMessageColor(msg.messageStyle);
		NameZOffset = -0.29f * (float)numberOfLines + 0.14f;
		nameRender.transform.localPosition = new Vector3(1.74f, 0f, NameZOffset);
	}

	private OpenChatMessage.MessageStyle GetConvertedSelfOpenChatMessage(OpenChatMessage.MessageStyle main)
	{
		if (main == OpenChatMessage.MessageStyle.Self)
		{
			return (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow)) ? OpenChatMessage.MessageStyle.NonSub : OpenChatMessage.MessageStyle.Sub;
		}
		return main;
	}

	private void ConvertToMultiLineAndAdd(string text)
	{
		List<string> list = new List<string>();
		while (!string.IsNullOrEmpty(text))
		{
			text = AddACorrectLine(list, text);
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in list)
		{
			stringBuilder.AppendLine(item);
		}
		messageTxt.text = stringBuilder.ToString();
		numberOfLines = list.Count;
	}

	private string AddACorrectLine(List<string> correct, string text)
	{
		messageTxt.text = string.Empty;
		string[] array = text.Split(' ');
		string item = string.Empty;
		int num = 0;
		bool flag = false;
		while (!flag)
		{
			if (num < array.Length)
			{
				item = messageTxt.text;
				messageTxt.text = (messageTxt.text + " " + array[num]).Trim();
				num++;
				Vector3 size = messageRender.bounds.size;
				if (size.x >= 3.5f)
				{
					flag = true;
				}
			}
			else
			{
				item = messageTxt.text;
				num++;
				flag = true;
			}
		}
		num--;
		if (num == 0)
		{
			return AddACorrectLineByCharacter(correct, text);
		}
		correct.Add(item);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = num; i < array.Length; i++)
		{
			stringBuilder.Append(array[i]);
			stringBuilder.Append(" ");
		}
		return stringBuilder.ToString();
	}

	private string AddACorrectLineByCharacter(List<string> correct, string text)
	{
		messageTxt.text = string.Empty;
		char[] array = text.ToCharArray();
		string item = string.Empty;
		int num = 0;
		bool flag = false;
		while (!flag)
		{
			if (num < array.Length)
			{
				item = messageTxt.text;
				messageTxt.text += array[num];
				num++;
				Vector3 size = messageRender.bounds.size;
				if (size.x >= 3.5f)
				{
					flag = true;
				}
			}
			else
			{
				item = messageTxt.text;
				num++;
				flag = true;
			}
		}
		num--;
		if (num == 0)
		{
			num = 1;
			item = array[0].ToString() + "?????";
		}
		correct.Add(item);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = num; i < array.Length; i++)
		{
			stringBuilder.Append(array[i]);
		}
		return stringBuilder.ToString();
	}

	public void Update()
	{
		//#if UNITY_EDITOR
		//	CspUtils.DebugLog("update() messageMaterial=" + AssetDatabase.GetAssetPath(messageMaterial));
		//messageRender.materials[0] = messageMaterial;  // added by CSP for testing
		
		// Material[] mats = messageRender.materials; // CSP added
		// mats[0] = CSPMaterial;   // CSP added because material assignment is getting set wrong
		// messageRender.materials = mats; // CSP added
		// messageMaterial = messageRender.materials[0]; // CSP added

		base.transform.rotation = Camera.main.transform.rotation;
		CalculateTextXOffset();
		CalculateTextZOffset();
		apm.Update(Time.deltaTime);
	}

	public void CalculateTextXOffset()
	{
		Vector3 position = Camera.main.transform.position;
		Vector3 position2 = icon.transform.position;
		Vector3 normalized = RemoveY(position2 - position).normalized;
		position = base.transform.position;
		Vector3 normalized2 = RemoveY(position2 - position).normalized;
		float num = 1f;
		if (normalized2 != normalized)
		{
			Vector3 vector = Vector3.Cross(normalized2, normalized);
			num = Mathf.Sign(vector.y);
		}
		float num2 = Vector3.Angle(normalized2, normalized) * num;
		float num3 = Mathf.Tan(num2 * ((float)Math.PI / 180f)) * -0.75f;
		float amount = 1.74f - num3;
		HorizOffsetText(ref nameTxt, amount);
		HorizOffsetText(ref messageTxt, amount);
	}

	public void CalculateTextZOffset()
	{
		Vector3 position = Camera.main.transform.position;
		Vector3 position2 = icon.transform.position;
		Vector3 normalized = (position2 - position).normalized;
		position = base.transform.position;
		Vector3 vector = normalized * (position2 - position).magnitude;
		float y = vector.y;
		VertOffsetText(ref nameTxt, y + NameZOffset);
		VertOffsetText(ref messageTxt, y + TextZOffset);
	}

	private void VertOffsetText(ref TextMesh tran, float amount)
	{
		Vector3 localPosition = tran.transform.localPosition;
		localPosition.z = amount;
		tran.transform.localPosition = localPosition;
	}

	private Vector3 RemoveY(Vector3 tr)
	{
		return new Vector3(tr.x, 0f, tr.z);
	}

	private void HorizOffsetText(ref TextMesh tran, float amount)
	{
		Vector3 localPosition = tran.transform.localPosition;
		localPosition.x = amount;
		tran.transform.localPosition = localPosition;
	}

	public void FadeIn()
	{
		AnimClip newPiece = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 1f, 0.5f), UpdateAlpha);
		apm.SwapOut(ref fadeInOut, newPiece);

		///// block added by CSP //////////////
		// Color c = this.gameObject.renderer.material.color;
		// c.a = 1;  // opaque
		// this.gameObject.renderer.material.color = c; 
		//////////////////////////////////////////
	}

	public void FadeOut()
	{
		AnimClip newPiece = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 0f, 0.5f), UpdateAlpha);
		apm.SwapOut(ref fadeInOut, newPiece);

		///// block added by CSP //////////////
		// Color c = this.gameObject.renderer.material.color;
		// c.a = 0;  // transparent
		// this.gameObject.renderer.material.color = c; 
		//////////////////////////////////////////
	}

	private void UpdateAlpha(float x)
	{
		UpdateAlpha(icon.material, x);
		UpdateAlpha(nameMaterial, x);
		UpdateAlpha(messageMaterial, x);
	}

	private void UpdateAlpha(Material m, float x)
	{
		Color color = m.color;
		color.a = x;
		m.color = color;
	}
}
