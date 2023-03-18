using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChatBubbleManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private struct ChatMessageQueueObj
	{
		public OpenChatMessage msg;

		public bool alive;

		public float timeStep;

		public float lifeSpan;

		public ChatMessageQueueObj(OpenChatMessage msg, float lifeSpan)
		{
			this.msg = msg;
			this.lifeSpan = lifeSpan;
			timeStep = 0f;
			alive = false;
		}
	}

	private const float HEIGHT_ABOVE_HEAD = 0.2f;

	private const float normalMessageLifeSpan = 15f;

	private const float bubbleOffset = 0.75f;

	private const float shortLifeSpan = 3f;

	private const float mediumLifeSpan = 4f;

	private const float largeLifeSpan = 5f;

	private const int shortMessageLength = 10;

	private const int mediumMessageLength = 30;

	private const int largeMessageLength = 50;

	private Hashtable lengthTable;

	private AnimClipManager apm = new AnimClipManager();

	protected OpenChatBubble bubble;

	public HairTrafficController htc;

	private List<ChatMessageQueueObj> messageQueue = new List<ChatMessageQueueObj>();

	private float lastOffset = -1f;

	GameObject sphere; // CSP added for testing

	private GameObject openChatPrefabObject; // added by CSP

	private Hashtable LengthTable
	{
		get
		{
			if (lengthTable == null)
			{
				lengthTable = new Hashtable();
				lengthTable[10] = 3f;
				lengthTable[30] = 4f;
				lengthTable[50] = 5f;
			}
			return lengthTable;
		}
	}

	public void ShowChat(OpenChatMessage msg)
	{
		if (bubble == null)
		{
			Object @object = Resources.Load("GUI/3D/OpenChatPrefab");
			if (@object != null)
			{
				GameObject gameObject = Object.Instantiate(@object) as GameObject;
				openChatPrefabObject = gameObject;  // added by CSP
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = new Vector3(0f, 3f, 0f);
				bubble = Utils.GetComponent<OpenChatBubble>(gameObject);

				//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube); // CSP added for testing
				//cube.transform.parent = base.gameObject.transform; // CSP added for testing
				//cube.transform.localPosition = new Vector3(0f, 3f, 0f); // CSP added for testing
			}
		}
		int length = msg.message.Length;
		int num = 10;
		num = ((length <= 10 || length > 50) ? num : 30);
		num = ((length < 50) ? num : 50);
		float lifeSpan = (float)LengthTable[num];
		messageQueue.Add(new ChatMessageQueueObj(msg, lifeSpan));
	}

	private void CreateBubble(ChatMessageQueueObj queueObj)
	{
		if (bubble != null)
		{
			//openChatPrefabObject.active = true;  // added by CSP
			bubble.SetupBubble(queueObj.msg);
			bubble.FadeIn();
		}

	    //sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // CSP added for testing
        
	}

	private void UpdateMessageQueue()
	{
		if (messageQueue.Count > 0)
		{
			ChatMessageQueueObj chatMessageQueueObj = messageQueue[0];
			if (!chatMessageQueueObj.alive)
			{
				chatMessageQueueObj.alive = true;
				chatMessageQueueObj.timeStep = 0f;
				CreateBubble(chatMessageQueueObj);
			}
			chatMessageQueueObj.timeStep += Time.deltaTime;
			messageQueue[0] = chatMessageQueueObj;
			float num = (messageQueue.Count <= 1) ? 15f : chatMessageQueueObj.lifeSpan;
			if (chatMessageQueueObj.timeStep >= num)
			{
				HideCurrentChat();
			}
		}
	}

	private void UpdateBubblePosition()
	{
		if (!(bubble != null))
		{
			return;
		}
		string name = base.gameObject.name;
		float num = 0f;
		if (htc.MasterHeroOffsetTable.ContainsKey(name))
		{
			num = htc.GetYOffset(base.gameObject);
		}
		else
		{
			if (lastOffset == -1f)
			{
				lastOffset = htc.GetHairOffset();
			}
			num = (lastOffset = htc.GetSmoothHairOffset(lastOffset));
		}
		Vector3 position = htc.HairTrafficPoint.transform.position;
		Vector3 position2 = htc.HairTrafficPoint.transform.position;
		position.x = position2.x;
		Vector3 position3 = htc.HairTrafficPoint.transform.position;
		position.y = position3.y + 0.2f + num;
		Vector3 position4 = htc.HairTrafficPoint.transform.position;
		position.z = position4.z;
		bubble.transform.position = position;
		//sphere.transform.position = position;   // CSP added for testing
	}

	private void Update()
	{
		apm.Update(Time.deltaTime);
		UpdateMessageQueue();
		UpdateBubblePosition();

		if (openChatPrefabObject != null) // added by CSP
			openChatPrefabObject.transform.localScale = new Vector3(1f, 1f, 1f);  // added by CSP
	}

	public void HideCurrentChat()
	{
		if (bubble != null)
		{
			messageQueue.RemoveAt(0);
			bubble.FadeOut();
			if (messageQueue.Count == 0)
			{
				htc.ToggleBillboard(true);
			}
			Destroy(openChatPrefabObject); //openChatPrefabObject.active  = false;  // added by CSP
		}
	}

	public void HideAllChat()
	{
		if (bubble != null)
		{
			bubble.FadeOut();
			//Destroy(openChatPrefabObject); //openChatPrefabObject.active  = false;  // added by CSP
		}
		messageQueue.Clear();

		
	}

	public static void SetChatMessage(GameObject player, OpenChatMessage msg)
	{
		OpenChatBubbleManager openChatBubbleManager = Utils.GetComponent<OpenChatBubbleManager>(player);
		if (openChatBubbleManager == null)
		{
			openChatBubbleManager = Utils.AddComponent<OpenChatBubbleManager>(player);
		}
		HairTrafficController component = player.GetComponent<HairTrafficController>();
		if (component != null)
		{
			component.BeginningOpenChat(openChatBubbleManager);
			openChatBubbleManager.htc = component;
		}
		openChatBubbleManager.ShowChat(msg);
	}
}
