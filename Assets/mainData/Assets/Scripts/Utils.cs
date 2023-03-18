using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class Utils
{
	public class SearchParentsToken
	{
	}

	public class SearchChildrenToken
	{
	}

	public class XmlWriterCreator
	{
		public XmlWriter Writer;

		private StringBuilder BrowserOutput;

		public IEnumerator Create(string file)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			if (IsWebPlayer)
			{
				BrowserOutput = new StringBuilder();
				Writer = XmlWriter.Create(BrowserOutput, settings);
			}
			else
			{
				Writer = XmlWriter.Create(file, settings);
			}
			yield return 0;
		}

		public void Close()
		{
			Writer.Close();
			if (IsWebPlayer)
			{
				Application.ExternalCall("ShowXML", BrowserOutput.ToString());
			}
		}
	}

	public class XmlReaderCreator
	{
		public XmlReader Reader;

		public IEnumerator Create(string file)
		{
			if (IsWebPlayer)
			{
				string dir = Path.GetDirectoryName(Application.absoluteURL);
				WWW www = new WWW(Path.Combine(dir, file));
				yield return www;
				MemoryStream stream = new MemoryStream(www.bytes);
				Reader = XmlReader.Create(stream);
			}
			else
			{
				Reader = XmlReader.Create(file);
			}
		}
	}

	public delegate void VoidAction();

	public static SearchParentsToken SearchParents;

	public static SearchChildrenToken SearchChildren;

	public static bool IsWebPlayer
	{
		get
		{
			return Application.platform == RuntimePlatform.OSXWebPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer;
		}
	}

	public static void ForEachTree(GameObject root, Action<GameObject> action)
	{
		action(root);
		foreach (Transform item in root.transform)
		{
			ForEachTree(item.gameObject, action);
		}
	}

	public static IEnumerable<Transform> WalkTree(Transform root)
	{
		Stack<Transform> stack = new Stack<Transform>();
		stack.Push(root);
		while (stack.Count != 0)
		{
			Transform top = stack.Pop();
			yield return top;
			if (top.childCount != 0)
			{
				foreach (Transform child in top)
				{
					stack.Push(child);
				}
			}
		}
	}

	public static void SetLayerTree(GameObject root, int layer)
	{
		root.layer = layer;
		foreach (Transform item in root.transform)
		{
			SetLayerTree(item.gameObject, layer);
		}
	}

	public static void SetLayerTreeFiltered(GameObject root, int layer, int layerMask)
	{
		if (((1 << root.layer) & layerMask) != 0)
		{
			root.layer = layer;
		}
		foreach (Transform item in root.transform)
		{
			SetLayerTreeFiltered(item.gameObject, layer, layerMask);
		}
	}

	public static void ActivateTree(GameObject root, bool active)
	{
		ForEachTree(root, delegate(GameObject g)
		{
			g.active = active;
		});
	}

	public static void ActivateRenderers(GameObject root, bool active)
	{
		LodBase componentInChildren = root.GetComponentInChildren<LodBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetVisible(active);
		}
		Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (!(renderer is SkinnedMeshRenderer) || !(componentInChildren != null))
			{
				renderer.enabled = active;
			}
		}
	}

	public static void ActivateSkinnedMeshRenderers(GameObject root, bool active)
	{
		LodBase componentInChildren = root.GetComponentInChildren<LodBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetVisible(active);
			return;
		}
		SkinnedMeshRenderer componentInChildren2 = root.GetComponentInChildren<SkinnedMeshRenderer>();
		componentInChildren2.enabled = active;
	}

	public static void BroadcastUnloadEvent()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = (GameObject)array[i];
			if (gameObject != null)
			{
				gameObject.SendMessage("OnUnload", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static string GetGameObjectFullName(GameObject go)
	{
		if (go == null)
		{
			return "null";
		}
		Stack<string> stack = new Stack<string>();
		while (true)
		{
			stack.Push(go.name);
			if (go.transform.parent == null)
			{
				break;
			}
			go = go.transform.parent.gameObject;
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (stack.Count > 0)
		{
			stringBuilder.Append("/");
			stringBuilder.Append(stack.Pop());
		}
		return stringBuilder.ToString();
	}

	public static string GetBasePrefabName(GameObject go)
	{
		int num = go.name.LastIndexOf("(Clone)");
		if (num >= 0)
		{
			return go.name.Substring(0, num);
		}
		return go.name;
	}

	public static T GetComponentRecursive<T>(GameObject o) where T : Component
	{
		while (o != null)
		{
			T component = GetComponent<T>(o);
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				return component;
			}
			if (o.transform.parent == null)
			{
				break;
			}
			o = o.transform.parent.gameObject;
		}
		return (T)null;
	}

	public static T GetComponent<T>(GameObject g) where T : Component
	{
		if (g == null)
		{
			CspUtils.DebugLog("null Game Object passed into GetComponent()");
			return (T)null;
		}
		return g.GetComponent(typeof(T)) as T;
	}

	public static T GetComponent<T>(Component c) where T : Component
	{
		return c.GetComponent(typeof(T)) as T;
	}

	public static T GetComponent<T>(string tag) where T : Component
	{
		GameObject gameObject = GameObject.FindWithTag(tag);
		if (gameObject == null)
		{
			CspUtils.DebugLog("No GameObject tagged as " + tag + " was found.");
			return (T)null;
		}
		return GetComponent<T>(gameObject);
	}

	public static T GetComponent<T>(GameObject go, SearchParentsToken token) where T : Component
	{
		UnityEngine.Object @object = null;
		while (go != null && @object == null)
		{
			@object = go.GetComponent(typeof(T));
			if (@object == null)
			{
				Transform parent = go.transform.parent;
				go = ((!(parent != null)) ? null : parent.gameObject);
			}
		}
		return @object as T;
	}

	public static T GetComponent<T>(Component c, SearchParentsToken token) where T : Component
	{
		return GetComponent<T>(c.gameObject, token);
	}

	public static T GetComponent<T>(GameObject go, SearchChildrenToken token) where T : Component
	{
		return go.GetComponentInChildren(typeof(T)) as T;
	}

	public static T GetComponent<T>(Component c, SearchChildrenToken token) where T : Component
	{
		return c.GetComponentInChildren(typeof(T)) as T;
	}

	public static T[] GetComponents<T>(GameObject go) where T : Component
	{
		return go.GetComponents<T>();
	}

	public static T[] GetComponents<T>(Component c) where T : Component
	{
		return GetComponents<T>(c.gameObject);
	}

	public static T[] GetComponents<T>(GameObject go, SearchChildrenToken token) where T : Component
	{
		return GetComponents<T>(go, token, false);
	}

	public static T[] GetComponents<T>(Component c, SearchChildrenToken token) where T : Component
	{
		return GetComponents<T>(c, token, false);
	}

	public static T[] GetComponents<T>(GameObject go, SearchChildrenToken token, bool includeInactive) where T : Component
	{
		return go.GetComponentsInChildren<T>(includeInactive);
	}

	public static T[] GetComponents<T>(Component c, SearchChildrenToken token, bool includeInactive) where T : Component
	{
		return GetComponents<T>(c.gameObject, token, includeInactive);
	}

	public static T GetComponentAlways<T>(GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return go.AddComponent<T>();
		}
		return component;
	}

	public static T AddComponent<T>(GameObject g) where T : Component
	{
		return g.AddComponent(typeof(T)) as T;
	}

	public static T[] FindObjectsOfType<T>() where T : class
	{
		return Array.ConvertAll(UnityEngine.Object.FindObjectsOfType(typeof(T)), delegate(UnityEngine.Object o)
		{
			return o as T;
		});
	}

	public static IEnumerable<T> EnumerateObjectsOfType<T>() where T : class
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(T));
		foreach (UnityEngine.Object o in array)
		{
			yield return o as T;
		}
	}

	public static T XmlDeserialize<T>(XmlReader xml)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		return (T)xmlSerializer.Deserialize(xml);
	}

	public static T XmlDeserialize<T>(string xml)
	{
		//Discarded unreachable code: IL_0029
		using (StringReader textReader = new StringReader(xml))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			return (T)xmlSerializer.Deserialize(textReader);
		}
	}

	public static T XmlDeserializeInner<T>(string innerXml)
	{
		return XmlDeserializeInner<T>(innerXml, new XmlSerializer(typeof(T)));
	}

	public static T XmlDeserializeInner<T>(string innerXml, XmlSerializer serializer)
	{
		string text = null;
		Attribute[] customAttributes = Attribute.GetCustomAttributes(typeof(T), typeof(XmlRootAttribute));
		text = ((customAttributes == null || customAttributes.Length <= 0) ? typeof(T).Name : ((XmlRootAttribute)customAttributes[0]).ElementName);
		string s = string.Format("<{0}>{1}</{0}>", text, innerXml);
		return (T)serializer.Deserialize(new StringReader(s));
	}

	public static T XmlDeserialize<T>(DataWarehouse data) where T : class
	{
		return XmlDeserialize<T>(data, new XmlSerializer(typeof(T)));
	}

	public static T XmlDeserialize<T>(DataWarehouse data, XmlSerializer serializer) where T : class
	{
		return serializer.Deserialize(data.Navigator.ReadSubtree()) as T;
	}

	public static T XmlDeserializeElement<T>(XmlReader xml, string element)
	{
		xml.ReadStartElement(element);
		T result = XmlDeserialize<T>(xml);
		xml.ReadEndElement();
		return result;
	}

	public static T XmlDeserializeValue<T>(XmlReader xml, string element, T def)
	{
		//Discarded unreachable code: IL_002b, IL_0038
		try
		{
			xml.ReadStartElement(element);
			T result = (T)xml.ReadContentAs(typeof(T), null);
			xml.ReadEndElement();
			return result;
		}
		catch
		{
			return def;
		}
	}

	public static T XmlReadAttribute<T>(XmlReader xml, string attribute, T def)
	{
		xml.MoveToAttribute(attribute);
		T result = (T)xml.ReadContentAs(typeof(T), null);
		xml.MoveToElement();
		return result;
	}

	public static void XmlSerialize<T>(XmlWriter xml, T obj)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		xmlSerializer.Serialize(xml, obj);
	}

	public static void XmlSerializeElement<T>(XmlWriter xml, string element, T obj)
	{
		xml.WriteStartElement(element);
		XmlSerialize(xml, obj);
		xml.WriteEndElement();
	}

	public static void XmlSerializeValue<T>(XmlWriter xml, string element, T obj)
	{
		xml.WriteStartElement(element);
		xml.WriteValue(obj);
		xml.WriteEndElement();
	}

	public static void XmlWriteAttribute<T>(XmlWriter xml, string attribute, T value)
	{
		xml.WriteStartAttribute(attribute);
		xml.WriteValue(value);
		xml.WriteEndAttribute();
	}

	public static void XmlWriteAttribute(XmlWriter xml, string attribute, float value)
	{
		XmlWriteAttribute(xml, attribute, (double)value);
	}

	public static void XmlSerializeValue(XmlWriter xml, string element, Vector2 v)
	{
		xml.WriteStartElement(element);
		XmlWriteAttribute(xml, "x", (double)v.x);
		XmlWriteAttribute(xml, "y", (double)v.y);
		xml.WriteEndElement();
	}

	public static void XmlSerializeValue(XmlWriter xml, string element, Vector3 v)
	{
		xml.WriteStartElement(element);
		XmlWriteAttribute(xml, "x", (double)v.x);
		XmlWriteAttribute(xml, "y", (double)v.y);
		XmlWriteAttribute(xml, "z", (double)v.z);
		xml.WriteEndElement();
	}

	public static void XmlSerializeValue(XmlWriter xml, string element, Vector4 v)
	{
		xml.WriteStartElement(element);
		XmlWriteAttribute(xml, "x", (double)v.x);
		XmlWriteAttribute(xml, "y", (double)v.y);
		XmlWriteAttribute(xml, "z", (double)v.z);
		XmlWriteAttribute(xml, "w", (double)v.w);
		xml.WriteEndElement();
	}

	public static void XmlSerializeValue(XmlWriter xml, string element, Quaternion q)
	{
		xml.WriteStartElement(element);
		XmlWriteAttribute(xml, "x", (double)q.x);
		XmlWriteAttribute(xml, "y", (double)q.y);
		XmlWriteAttribute(xml, "z", (double)q.z);
		XmlWriteAttribute(xml, "w", (double)q.w);
		xml.WriteEndElement();
	}

	public static Vector3 ToVector3(XmlNode node)
	{
		return new Vector3(XmlConvert.ToSingle(node.Attributes["x"].Value), XmlConvert.ToSingle(node.Attributes["y"].Value), XmlConvert.ToSingle(node.Attributes["z"].Value));
	}

	public static Quaternion ToQuaternion(XmlNode node)
	{
		return new Quaternion(XmlConvert.ToSingle(node.Attributes["x"].Value), XmlConvert.ToSingle(node.Attributes["y"].Value), XmlConvert.ToSingle(node.Attributes["z"].Value), XmlConvert.ToSingle(node.Attributes["w"].Value));
	}

	public static T[] AppendArray<T>(T[] array, T element)
	{
		T[] array2 = new T[(array == null) ? 1 : (array.Length + 1)];
		array.CopyTo(array2, 0);
		array2[array2.Length - 1] = element;
		return array2;
	}

	public static T[] ToArray<T>(IEnumerable e)
	{
		List<T> list = new List<T>();
		foreach (T item in e)
		{
			list.Add(item);
		}
		return list.ToArray();
	}

	public static T[] ToArray<T>(IEnumerable<T> e)
	{
		List<T> list = new List<T>();
		foreach (T item in e)
		{
			list.Add(item);
		}
		return list.ToArray();
	}

	public static IEnumerable<LinkedListNode<T>> RemovableEnumerate<T>(LinkedList<T> list)
	{
		LinkedListNode<T> node = list.First;
		while (node != null)
		{
			LinkedListNode<T> next = node.Next;
			yield return node;
			node = next;
		}
	}

	public static T1 EnumCast<T1, T2>(T2 input)
	{
		return (T1)Enum.Parse(typeof(T1), input.ToString());
	}

	public static IEnumerable<T> Distinct<T>(IEnumerable<T> input)
	{
		Dictionary<T, bool> valueSet = new Dictionary<T, bool>();
		bool returnedNull = false;
		foreach (T value in input)
		{
			if (value == null)
			{
				if (!returnedNull)
				{
					yield return default(T);
					returnedNull = true;
				}
			}
			else if (!valueSet.ContainsKey(value))
			{
				yield return value;
				valueSet.Add(value, true);
			}
		}
	}

	public static IEnumerable<TValue> Map<TKey, TValue>(IEnumerable<TKey> input, Converter<TKey, TValue> fn)
	{
		foreach (TKey key in input)
		{
			yield return fn(key);
		}
	}

	public static IEnumerable<KeyValuePair<TKey, TValue>> MapPairs<TKey, TValue>(IEnumerable<TKey> input, Converter<TKey, TValue> fn)
	{
		foreach (TKey key in input)
		{
			yield return new KeyValuePair<TKey, TValue>(key, fn(key));
		}
	}

	public static IEnumerable<T> Filter<T>(IEnumerable<T> input, Converter<T, bool> predicate)
	{
		foreach (T value in input)
		{
			if (predicate(value))
			{
				yield return value;
			}
		}
	}

	public static IEnumerable<T> SkipNulls<T>(IEnumerable<T> input)
	{
		return Filter(input, delegate(T value)
		{
			return !value.Equals(default(T));
		});
	}

	public static IEnumerable<TChild> Expand<TRoot, TChild>(IEnumerable<TRoot> roots, Converter<TRoot, IEnumerable<TChild>> fn)
	{
		foreach (TRoot root in roots)
		{
			foreach (TChild item in fn(root))
			{
				yield return item;
			}
		}
	}

	public static Transform FindNodeInChildren(Transform parent, string childName, bool ignoreCase)
	{
		Transform transform = null;
		bool flag = false;
		if (ignoreCase)
		{
			childName = childName.ToLower();
		}
		foreach (Transform item in parent.transform)
		{
			if ((!ignoreCase) ? (item.gameObject.name == childName) : (item.gameObject.name.ToLower() == childName))
			{
				return item;
			}
			transform = FindNodeInChildren(item, childName, ignoreCase);
			if (transform != null)
			{
				return transform;
			}
		}
		return transform;
	}

	public static Transform FindNodeInChildren(Transform parent, string childName)
	{
		return FindNodeInChildren(parent, childName, false);
	}

	public static void AttachChildrenToGameObject(GameObject dest, GameObject source)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in source.transform)
		{
			list.Add(item);
		}
		foreach (Transform item2 in list)
		{
			AttachGameObject(dest.transform, item2);
		}
	}

	public static void AttachGameObject(GameObject parent, GameObject child)
	{
		AttachGameObject(parent.transform, child.transform);
	}

	public static void AttachGameObject(Transform parent, Transform child)
	{
		Vector3 localPosition = child.localPosition;
		Vector3 localScale = child.localScale;
		Quaternion localRotation = child.localRotation;
		child.parent = parent;
		child.localPosition = localPosition;
		child.localScale = localScale;
		child.localRotation = localRotation;
	}

	public static void DetachGameObject(GameObject go)
	{
		Vector3 localPosition = go.transform.localPosition;
		Vector3 localScale = go.transform.localScale;
		Quaternion localRotation = go.transform.localRotation;
		go.transform.parent = null;
		go.transform.localPosition = localPosition;
		go.transform.localScale = localScale;
		go.transform.localRotation = localRotation;
	}

	public static string RemoveDirectoryLevel(string filePath)
	{
		string directoryName = Path.GetDirectoryName(filePath);
		int num = directoryName.LastIndexOf(Path.PathSeparator);
		if (num == -1)
		{
			return directoryName;
		}
		return directoryName.Substring(0, num);
	}

	public static KeyCodeEntry GetKeyCode(string keyName, string modifier)
	{
		//Discarded unreachable code: IL_008f, IL_00c8
		try
		{
			int result;
			if (int.TryParse(keyName, out result))
			{
				keyName = "Alpha" + result.ToString();
			}
			KeyCode code = (KeyCode)(int)Enum.Parse(typeof(KeyCode), keyName);
			string a = modifier.ToLower();
			bool shift = false;
			bool control = false;
			bool alt = false;
			if (a == "shift")
			{
				shift = true;
			}
			if (a == "alt")
			{
				alt = true;
			}
			if (a == "control")
			{
				control = true;
			}
			return new KeyCodeEntry(code, control, alt, shift);
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Could not get KeyCode entry from " + keyName + " Exception: " + ex.Message);
			return default(KeyCodeEntry);
		}
	}

	public static bool FindObjectUnderCursor(out RaycastHit hit)
	{
		Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		int layerMask = 1077760;
		return Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMask) && !SHSInput.IsOverUI();
	}

	public static bool FindNextObjectUnderCursor(GameObject objectToIgnore, out RaycastHit hit)
	{
		if (!SHSInput.IsOverUI())
		{
			Ray ray = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
			int layerMask = 1077760;
			RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, layerMask);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (raycastHit.collider.gameObject != objectToIgnore)
				{
					hit = raycastHit;
					return true;
				}
			}
		}
		hit = default(RaycastHit);
		return false;
	}

	public static bool IsLocalPlayer(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		SpawnData component = GetComponent<SpawnData>(player);
		if (component == null)
		{
			return false;
		}
		return component.spawnType == CharacterSpawn.Type.LocalPlayer;
	}

	public static bool IsLocalPlayer(CharacterGlobals charGlobals)
	{
		if (charGlobals == null || charGlobals.spawnData == null)
		{
			return false;
		}
		return charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer;
	}

	public static bool IsPlayer(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		if (GetComponent<PlayerCombatController>(player) != null)
		{
			return true;
		}
		SpawnData component = GetComponent<SpawnData>(player);
		if (component == null)
		{
			return false;
		}
		return (component.spawnType & CharacterSpawn.Type.Player) != 0;
	}

	public static bool IsCharacterSpawned(GameObject character)
	{
		if (character == null)
		{
			return false;
		}
		SpawnData component = GetComponent<SpawnData>(character);
		if (component == null)
		{
			return false;
		}
		return component.SpawnEventFired;
	}

	public static int GetGazillionID(int targetNetId)
	{
		int result = -1;
		if (GameController.GetController().isTestScene)
		{
			return result;
		}
		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(targetNetId, out value);
		if (value != null)
		{
			try
			{
				return value.PlayerId;
			}
			catch
			{
				CspUtils.DebugLog("Target <" + targetNetId + "> has an invalid Gazillion ID.  Further interactions may fail");
				return -1;
			}
		}
		CspUtils.DebugLog("Target <" + targetNetId + "> does not have a valid Gazillion ID.  Further interactions may fail");
		return result;
	}

	public static Color ColorFromBytes(byte r, byte g, byte b, byte a)
	{
		return new Color((float)(int)r / 255f, (float)(int)g / 255f, (float)(int)b / 255f, (float)(int)a / 255f);
	}

	public static void DelayedDestroy(GameObject destroyObject)
	{
		DelayedDestroy(destroyObject, 5f);
	}

	public static void DelayedDestroy(GameObject destroyObject, float delay)
	{
		DelayedDestroy(destroyObject, delay, false);
	}

	public static void DelayedDestroy(GameObject destroyObject, float delay, bool log)
	{
		GameObject gameObject = new GameObject(destroyObject.name + "_DestructTimer");
		gameObject.transform.parent = destroyObject.transform.parent;
		TimedSelfDestruct timedSelfDestruct = gameObject.AddComponent(typeof(TimedSelfDestruct)) as TimedSelfDestruct;
		timedSelfDestruct.Begin(destroyObject, delay, log);
		ActivateTree(destroyObject, false);
	}

	public static void DelayedDestroyNetworkedChildren(GameObject gameObject)
	{
		NetworkComponent[] componentsInChildren = gameObject.GetComponentsInChildren<NetworkComponent>(true);
		NetworkComponent[] array = componentsInChildren;
		foreach (NetworkComponent networkComponent in array)
		{
			networkComponent.transform.parent = gameObject.transform.parent;
			DelayedDestroy(networkComponent.gameObject, 5f);
		}
	}

	public static string LiteralScrambler(string literal, bool descramble)
	{
		string text = string.Empty;
		int num = literal.Length % 10;
		int num2 = descramble ? 1 : (-1);
		foreach (char c in literal)
		{
			char c2;
			if (descramble && c == '!')
			{
				c2 = (char)(92 + num * num2);
			}
			else
			{
				c2 = (char)(c + num * num2);
				if (!descramble && c2 == '\\')
				{
					c2 = '!';
				}
			}
			text += c2;
			num++;
			if (num > 10)
			{
				num = 1;
			}
		}
		if (!descramble)
		{
			CspUtils.DebugLog("Unscrambled literal '" + literal + "' becomes '" + text + "'");
		}
		return text;
	}

	public static bool ColorEqual(Color colorA, Color colorB)
	{
		return colorA.r == colorB.r && colorA.g == colorB.g && colorA.b == colorB.b && colorA.a == colorB.a;
	}

	public static IEnumerable<XPathNavigator> Enumerate(XPathNodeIterator iterator)
	{
		foreach (XPathNavigator item in iterator)
		{
			yield return item;
		}
	}

	public static bool ParseAsBool(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		if (value.Trim() == "1")
		{
			return true;
		}
		bool result;
		bool.TryParse(value, out result);
		return result;
	}

	public static string BitFieldDisplay(BitArray array)
	{
		return BitFieldDisplay(array, false);
	}

	public static string BitFieldDisplay(BitArray array, bool fullDisplay)
	{
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		foreach (bool item in array)
		{
			stringBuilder.Append((!item) ? "0" : "1");
		}
		return stringBuilder.ToString();
	}

	public static string ToLogString(Hashtable hashtable)
	{
		if (hashtable == null)
		{
			return "<>";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("< ");
		foreach (DictionaryEntry item in hashtable)
		{
			stringBuilder.Append("(");
			stringBuilder.Append(item.Key.ToString());
			stringBuilder.Append(",");
			stringBuilder.Append(item.Value.ToString());
			stringBuilder.Append(") ");
		}
		stringBuilder.Append(">");
		return stringBuilder.ToString();
	}

	public static void ListShuffle<T>(List<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			T value = list[i];
			int index = UnityEngine.Random.Range(i, list.Count);
			list[i] = list[index];
			list[index] = value;
		}
	}
}
