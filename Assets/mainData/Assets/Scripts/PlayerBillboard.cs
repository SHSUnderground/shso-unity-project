using UnityEngine;

public class PlayerBillboard : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const float HEIGHT_ABOVE_HEAD = -0.3f;

	public bool billboard;

	private TextMesh[] meshes;

	private MeshRenderer[] textrenderers;

	private MeshRenderer shieldIconRenderer;

	public HairTrafficController htc;

	public float targetHeight = 2.3f;

	private bool initialized;

	public int playerID;

	protected bool isTargeted;

	protected float targetedTimer;

	private bool acquiredReferences;

	public Texture shieldIconTexture;

	private string _defaultShieldIconTexture = "persistent_bundle|SHIELD_icon_flat";

	public Color nonShieldFontColor;

	public Color shieldFontColor;

	public Color friendFontColor;

	public Color moderatorFontColor;

	public Color targetedNonShieldFontColor;

	public Color targetedShieldFontColor;

	public Color targetedFriendFontColor;

	public Color targetedModeratorFontColor;

	protected Color targetedColor;

	public Color dropShadowFontColor;

	private SpawnData.PlayerType playerType;

	protected string squadName = string.Empty;

	protected string squadTitle = string.Empty;

	protected string heroName = string.Empty;

	private bool renderSquadNameEnabled;

	private bool renderSquadTitleEnabled;

	private bool renderHeroNameEnabled;

	protected bool shieldIconEnabled;

	private GameObject playerObject;

	private string heroBeingUsed = string.Empty;

	public GameObject playerTag;  // added by CSP

	private PlayerTag pt = null;  // added by CSP

	public Font txtFont;  // added by CSP

	public string SquadName
	{
		get
		{
			return squadName;
		}
		set
		{
			squadName = value;
			if (!initialized)
			{
				return;
			}
			TextMesh[] array = meshes;
			foreach (TextMesh textMesh in array)
			{
				if (textMesh.gameObject.name.Contains("Squad"))
				{
					textMesh.text = squadName;
				}
			}
			UpdateShieldPosition();
		}
	}

	public string SquadTitle
	{
		get
		{
			return squadTitle;
		}
		set
		{
			squadTitle = value;
			if (!initialized)
			{
				return;
			}
			TextMesh[] array = meshes;
			foreach (TextMesh textMesh in array)
			{
				if (textMesh.gameObject.name.Contains("Title"))
				{
					textMesh.text = squadTitle;
				}
			}
			UpdateShieldPosition();
		}
	}

	public string HeroName
	{
		get
		{
			return heroName;
		}
		set
		{
			heroName = value.ToUpper();
			if (!initialized)
			{
				return;
			}
			TextMesh[] array = meshes;
			foreach (TextMesh textMesh in array)
			{
				if (textMesh.gameObject.name.Contains("Hero"))
				{
					textMesh.text = heroName;
				}
			}
			UpdateShieldPosition();
		}
	}

	public bool RenderSquadNameEnabled
	{
		get
		{
			return renderSquadNameEnabled;
		}
		set
		{
			renderSquadNameEnabled = value;
			if (textrenderers == null)
			{
				return;
			}
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Squad"))
				{
					meshRenderer.enabled = renderSquadNameEnabled;
				}
			}
		}
	}

	public bool RenderSquadTitleEnabled
	{
		get
		{
			return renderSquadTitleEnabled;
		}
		set
		{
			renderSquadTitleEnabled = value;
			if (textrenderers == null)
			{
				return;
			}
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Title"))
				{
					meshRenderer.enabled = renderSquadTitleEnabled;
				}
			}
		}
	}

	public bool RenderHeroNameEnabled
	{
		get
		{
			return renderHeroNameEnabled;
		}
		set
		{
			renderHeroNameEnabled = value;
			if (textrenderers == null)
			{
				return;
			}
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Hero"))
				{
					meshRenderer.enabled = renderHeroNameEnabled;
				}
			}
		}
	}

	public PlayerBillboard()
	{
		billboard = false;
	}

	private void Start()
	{
		if (playerID == AppShell.Instance.ServerConnection.GetGameUserId())
		{
			TitleManager.refresh();
		}
		if (TitleManager.playerTitles.ContainsKey(playerID))
		{
			TitleManager.broadcastChange(TitleManager.playerTitles[playerID]);
		}

		// CSP - following block added to use temporary player tag until billboard is working.//
		//  GameObject playerObject = base.gameObject.transform.parent.gameObject;
		//  GameObject obj = (GameObject) Instantiate(playerTag);
 		//  obj.transform.parent = playerObject.transform;
		//  pt = obj.GetComponent<PlayerTag>();
		//  pt.npcNamePos = playerObject.transform;

		// GameObject gameObject = base.gameObject.transform.parent.gameObject;
		// string text = "Unknown Entity";
		// if (gameObject != null)
		// {
		// 	SpawnData component = Utils.GetComponent<SpawnData>(gameObject);
		// 	if (component != null)			
		// 		component.GetSquadRelation(out text, out playerType, false);		
		// }
		// pt.npcName = text;  // player user name
		
		/////////////////////////////////////////////////////////////////////////////////////

	}

	private void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<PlayerChangedSquadInfoMessage>(OnPlayerSquadInfoChanged);
		AppShell.Instance.EventMgr.AddListener<SelectedPlayerMessage>(OnTargetPlayer);
		AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(OnFriendsListUpdated);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<PlayerChangedSquadInfoMessage>(OnPlayerSquadInfoChanged);
		AppShell.Instance.EventMgr.RemoveListener<SelectedPlayerMessage>(OnTargetPlayer);
		AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(OnFriendsListUpdated);
	}

	private void acquireReferences()
	{
		textrenderers = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		if (textrenderers.Length > 0)
		{
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Front"))
				{
					meshRenderer.material = GUIManager.Instance.FontManager[GUIFontManager.SupportedFontEnum.Komica].material;
					meshRenderer.material.color = nonShieldFontColor;
					if (meshRenderer.gameObject.name.Contains("Hero"))
					{
						meshRenderer.enabled = RenderHeroNameEnabled;
					}
					else if (meshRenderer.gameObject.name.Contains("Title"))
					{
						meshRenderer.enabled = RenderSquadTitleEnabled;
					}
					else
					{
						meshRenderer.enabled = RenderSquadNameEnabled;
					}
					meshRenderer.material.shader = Shader.Find("GUI/Text Shader Z");
				}
				else if (meshRenderer.gameObject.name.Contains("Back"))
				{
					meshRenderer.material = GUIManager.Instance.FontManager[GUIFontManager.SupportedFontEnum.Komica].material;
					meshRenderer.material.color = dropShadowFontColor;
					if (meshRenderer.gameObject.name.Contains("Hero"))
					{
						meshRenderer.enabled = RenderHeroNameEnabled;
					}
					else if (meshRenderer.gameObject.name.Contains("Title"))
					{
						meshRenderer.enabled = RenderSquadTitleEnabled;
					}
					else
					{
						meshRenderer.enabled = RenderSquadNameEnabled;
					}
					meshRenderer.material.shader = Shader.Find("GUI/Text Shader Z");
				}
				else if (meshRenderer.gameObject.name.Contains("Icon"))
				{
					shieldIconRenderer = meshRenderer;
				}
			}
		}
		meshes = Utils.GetComponents<TextMesh>(base.gameObject, Utils.SearchChildren);
		TextMesh[] array2 = meshes;
		foreach (TextMesh textMesh in array2)
		{
			//textMesh.font = GUIManager.Instance.FontManager[GUIFontManager.SupportedFontEnum.Komica];
			textMesh.font = txtFont;  // added by CSP
			if (textMesh.gameObject.name.Contains("Squad"))
			{
				textMesh.fontSize = 30;
				textMesh.text = squadName;
			}
			else if (textMesh.gameObject.name.Contains("Title"))
			{
				textMesh.fontSize = 24;
				textMesh.text = squadTitle;
			}
			else
			{
				textMesh.fontSize = 24;
				textMesh.text = heroName;
			}
		}
		shieldIconTexture = GUIManager.Instance.LoadTexture(_defaultShieldIconTexture);
		GameObject gameObject = GetComponentInChildren<GUIQuad>().gameObject;
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		component.material.mainTexture = shieldIconTexture;
		//Shader shader = Shader.Find("Marvel/Base/Self-Illuminated-Full Bright");  // commented out by CSP
		Shader shader = Shader.Find("Diffuse");  // added by CSP
		component.material.shader = shader;   
		component.material.color = new Color(1f, 1f, 1f, 1f);
		acquiredReferences = true;
		initialized = true;
	}

	protected void FindHighestTransform(Transform parent, ref Transform currentHighest)
	{
		if (!(currentHighest == null))
		{
			Vector3 position = parent.position;
			float y = position.y;
			Vector3 position2 = currentHighest.position;
			if (!(y > position2.y))
			{
				goto IL_0032;
			}
		}
		currentHighest = parent;
		goto IL_0032;
		IL_0032:
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			FindHighestTransform(child, ref currentHighest);
		}
	}

	public void OnPlayerSquadInfoChanged(PlayerChangedSquadInfoMessage message)
	{
		if (!(base.gameObject.transform.parent == null) && message.targetUserId == playerID)
		{
			SquadTitle = "The Crabalicous";  //AppShell.Instance.stringTable.GetString(message.title);  // CSP
			if (SquadTitle != string.Empty)
			{
				RenderSquadTitleEnabled = true;
			}
			else
			{
				RenderSquadTitleEnabled = false;
			}
			GameObject gameObject;
			if (message.medallionSource == string.Empty || message.medallionSource == "-1")
			{
				shieldIconTexture = GUIManager.Instance.LoadTexture(_defaultShieldIconTexture);
				gameObject = GetComponentInChildren<GUIQuad>().gameObject;
				gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
				shieldIconEnabled = ((playerType & SpawnData.PlayerType.ShieldAgent) != 0);
			}
			else
			{
				string medallionSource = message.medallionSource;
				shieldIconTexture = GUIManager.Instance.LoadTexture(medallionSource);
				gameObject = GetComponentInChildren<GUIQuad>().gameObject;
				gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
				shieldIconEnabled = true;
			}
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.material.mainTexture = shieldIconTexture;
			UpdateRenderers();
		}
	}

	public void OnTargetPlayer(SelectedPlayerMessage message)
	{
		isTargeted = false;
		if (base.gameObject.transform.parent == null)
		{
			return;
		}
		if (base.gameObject.transform.parent.gameObject == message.SelectedPlayer)
		{
			isTargeted = true;
			targetedTimer = 0f;
		}
		meshes = Utils.GetComponents<TextMesh>(base.gameObject, Utils.SearchChildren);
		TextMesh[] array = meshes;
		foreach (TextMesh textMesh in array)
		{
			if (textMesh.gameObject.name.Contains("Squad"))
			{
				textMesh.fontSize = ((!isTargeted) ? 30 : 35);
			}
			else
			{
				textMesh.fontSize = 24;
			}
		}
		SetColor();
		UpdateRenderers();
	}

	public void Configure()
	{
		if (!acquiredReferences)
		{
			acquireReferences();
		}
		if (BrawlerController.Instance != null)
		{
			shieldIconRenderer.enabled = false;
			return;
		}
		if (GameController.GetController().isTestScene)
		{
			shieldIconRenderer.enabled = false;
			return;
		}
		string text = "Not Logged In";
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (gameObject != null)
		{
			SpawnData component = Utils.GetComponent<SpawnData>(gameObject);
			if (component != null)
			{
				component.GetSquadRelation(out text, out playerType, false);
				NetworkComponent component2 = Utils.GetComponent<NetworkComponent>(gameObject);
				if ((bool)component2)
				{
					playerID = component2.goNetId.childId;
				}
			}
			else
			{
				text = "Unknown Entity";
			}
		}
		else
		{
			text = "Unknown Entity";
		}
		SquadName = text;

		if (gameObject.name.Contains("placeholder"))
		{
			HeroName = string.Empty;
		}
		else
		{
			HeroName = AppShell.Instance.CharacterDescriptionManager[gameObject.name].CharacterFamily;
		}
		Configure(playerType);
	}

	public void Configure(SpawnData.PlayerType playerData)
	{
		playerType = playerData;
		if (!acquiredReferences)
		{
			acquireReferences();
		}
		shieldIconEnabled = ((playerType & SpawnData.PlayerType.ShieldAgent) != 0);
		SetColor();
		UpdateRenderers();
	}

	private void SetColor()
	{
		bool flag = (playerType & SpawnData.PlayerType.Blocked) != 0;
		bool flag2 = (playerType & SpawnData.PlayerType.Moderator) != 0;
		bool flag3 = (playerType & SpawnData.PlayerType.Friend) != 0;
		bool flag4 = (playerType & SpawnData.PlayerType.ShieldAgent) != 0;
		if (flag2)
		{
			targetedColor = ((!isTargeted) ? moderatorFontColor : targetedModeratorFontColor);
		}
		else if (!flag && flag3)
		{
			targetedColor = ((!isTargeted) ? friendFontColor : targetedFriendFontColor);
		}
		else if (!flag && flag4)
		{
			targetedColor = ((!isTargeted) ? shieldFontColor : targetedShieldFontColor);
		}
		else
		{
			targetedColor = ((!isTargeted) ? nonShieldFontColor : targetedNonShieldFontColor);
		}
	}

	public void UpdateRenderers()
	{
		RenderSquadNameEnabled = (!string.IsNullOrEmpty(SquadName) && PlayerPrefs.GetInt("squadnames", 1) == 1);
		RenderSquadTitleEnabled = (!string.IsNullOrEmpty(SquadTitle) && PlayerPrefs.GetInt("squadtitles", 1) == 1);
		RenderHeroNameEnabled = (!string.IsNullOrEmpty(HeroName) && PlayerPrefs.GetInt("heronames", 1) == 1);
		shieldIconRenderer.enabled = (RenderSquadNameEnabled && shieldIconEnabled);
		Camera main = Camera.main;
		Vector3 lhs = main.transform.position - base.transform.position;
		Vector3 b = Vector3.Dot(lhs, main.transform.forward) * main.transform.forward;
		base.transform.LookAt(base.transform.position - b);
		Vector3 localPosition = default(Vector3);
		MeshRenderer[] array = textrenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			localPosition = meshRenderer.gameObject.transform.localPosition;
			if (meshRenderer.gameObject.name.Contains("Front"))
			{
				if (meshRenderer.gameObject.name.Contains("Squad"))
				{
					meshRenderer.enabled = RenderSquadNameEnabled;
				}
				else if (meshRenderer.gameObject.name.Contains("Title"))
				{
					meshRenderer.enabled = RenderSquadTitleEnabled;
				}
				else
				{
					meshRenderer.enabled = RenderHeroNameEnabled;
				}
				meshRenderer.material.color = targetedColor;
			}
			else if (meshRenderer.gameObject.name.Contains("Back"))
			{
				if (meshRenderer.gameObject.name.Contains("Squad"))
				{
					meshRenderer.enabled = RenderSquadNameEnabled;
				}
				else if (meshRenderer.gameObject.name.Contains("Title"))
				{
					meshRenderer.enabled = RenderSquadTitleEnabled;
				}
				else
				{
					meshRenderer.enabled = RenderHeroNameEnabled;
				}
				localPosition.x = 0.2f;
				if (!meshRenderer.gameObject.name.Contains("Title"))
				{
					localPosition.y = -0.2f;
				}
			}
			meshRenderer.gameObject.transform.localPosition = localPosition;
		}
		UpdateShieldPosition();
	}

	private void UpdateShieldPosition()
	{
		float num = 0f;
		MeshRenderer[] array = textrenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.gameObject.name.Contains("Squad"))
			{
				Vector3 size = meshRenderer.bounds.size;
				if (size.x > num)
				{
					Vector3 size2 = meshRenderer.bounds.size;
					num = size2.x;
				}
			}
			else if (meshRenderer.gameObject.name.Contains("Title") && RenderSquadTitleEnabled)
			{
				Vector3 size3 = meshRenderer.bounds.size;
				if (size3.x > num)
				{
					Vector3 size4 = meshRenderer.bounds.size;
					num = size4.x;
				}
			}
			else if (meshRenderer.gameObject.name.Contains("Hero") && RenderHeroNameEnabled)
			{
				Vector3 size5 = meshRenderer.bounds.size;
				if (size5.x > num)
				{
					Vector3 size6 = meshRenderer.bounds.size;
					num = size6.x;
				}
			}
		}
		num *= 10f;
		GUIQuad componentInChildren = GetComponentInChildren<GUIQuad>();
		if (componentInChildren != null)
		{
			GameObject gameObject = componentInChildren.gameObject;
			Vector3 localPosition = gameObject.transform.localPosition;
			localPosition.x = 0f - num / 2f;
			localPosition.x -= 3.5f;
			gameObject.transform.localPosition = localPosition;
		}
	}

	public void OnFriendsListUpdated(FriendListUpdatedMessage msg)
	{
		UpdateRelation();
	}

	public void UpdateRelation()
	{
		if (!(base.gameObject != null) || !(base.gameObject.transform != null) || !(base.gameObject.transform.parent != null) || !(base.gameObject.transform.parent.gameObject != null))
		{
			return;
		}
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (gameObject != null)
		{
			SpawnData component = Utils.GetComponent<SpawnData>(gameObject);
			if (component != null)
			{
				string empty = string.Empty;
				component.GetSquadRelation(out empty, out playerType, false);
				SetColor();
				UpdateRenderers();
			}
		}
	}

	private void UpdateTransform()
	{
		playerObject = base.gameObject.transform.parent.gameObject;
		heroBeingUsed = playerObject.name;
		float num = 0f;
		if (htc.MasterHeroOffsetTable.ContainsKey(heroBeingUsed))
		{
			num = htc.GetYOffset(playerObject);
		}
		Transform transform = Utils.FindNodeInChildren(playerObject.transform, "export_node");
		if (transform != null)
		{
			float num2 = num;
			Vector3 localScale = transform.localScale;
			num = num2 * localScale.x;
		}
		Vector3 position = htc.HairTrafficPoint.transform.position;
		Vector3 position2 = htc.HairTrafficPoint.transform.position;
		position.x = position2.x;
		Vector3 position3 = htc.HairTrafficPoint.transform.position;
		position.y = position3.y + -0.3f + num;
		Vector3 position4 = htc.HairTrafficPoint.transform.position;
		position.z = position4.z;
		base.transform.position = position;
	}

	private void Update()
	{
		if (!initialized)
		{
			acquireReferences();
		}
		if (!billboard)
		{
			return;
		}
		UpdateTransform();
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		Vector3 lhs = main.transform.position - base.transform.position;
		Vector3 b = Vector3.Dot(lhs, main.transform.forward) * main.transform.forward;
		base.transform.LookAt(base.transform.position - b);
		if (isTargeted)
		{
			targetedTimer += Time.deltaTime * 2f;
			if (targetedTimer > 2f)
			{
				targetedTimer -= 2f;
			}
			float t = (!(targetedTimer > 1f)) ? targetedTimer : (2f - targetedTimer);
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Front"))
				{
					Color color = new Color(Mathf.Lerp(targetedColor.r, 1f, t), Mathf.Lerp(targetedColor.g, 1f, t), Mathf.Lerp(targetedColor.b, 1f, t));
					meshRenderer.material.color = color;
				}
				
				////////////// CSP added this block //////////////////////////////////
				if (!(meshRenderer.gameObject.name.Contains("Icon"))) {  // CSP
					//Material[] mats = meshRenderer.materials; // CSP added
					//mats[0] = CSPMaterial;   // CSP added because material assignment is getting set wrong
					//messageRender.materials = mats; // CSP added
					Destroy(meshRenderer.material);
					meshRenderer.material = txtFont.material; 
				}

				///////////////////////////////////////////////////////////////////////////
				
			}
		}
		UpdateShieldPosition();

		//////////// block added by CSP //////////////////////////////////////////

		textrenderers = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		if (textrenderers.Length > 0)
		{
			MeshRenderer[] array = textrenderers;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.gameObject.name.Contains("Name"))
				{
					Destroy(meshRenderer.material);
					meshRenderer.material = txtFont.material;
					meshRenderer.material.shader = txtFont.material.shader;
				}
				if (meshRenderer.gameObject.name.Contains("Back")) 
				{
					meshRenderer.material.color = Color.black;
				}
				if (meshRenderer.gameObject.name.Contains("SquadBackName")) 
				{
					Transform temp = meshRenderer.gameObject.transform;
					temp.localPosition = new Vector3(meshRenderer.gameObject.transform.localPosition.x, 4.4f, meshRenderer.gameObject.transform.localPosition.z); 
					meshRenderer.gameObject.transform.localPosition =  temp.localPosition;   
				}
			}
		}
		
	
		//Debug.Log("gc mem=" + System.GC.GetTotalMemory(true));
		//if (System.GC.GetTotalMemory(true) > 2500000000) {
		//	Debug.Log("gc collected!");
		//	CspUtils.garbageCollect();
		//}
		//////////////////////////////////////////////////////////////////////////////

	}



	private void OnDrawGizmos()
	{
		MeshRenderer[] components = Utils.GetComponents<MeshRenderer>(base.gameObject, Utils.SearchChildren);
		MeshRenderer[] array = components;
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.gameObject.name.Contains("Front") || meshRenderer.gameObject.name.Contains("Icon"))
			{
				Vector3 size = meshRenderer.bounds.size;
				size.z = 0.01f;
				Vector3 position = meshRenderer.transform.position;
				position.y += size.y / 2f;
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(position, size);
			}
		}
	}
}
