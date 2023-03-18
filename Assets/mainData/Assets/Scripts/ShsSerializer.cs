using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShsSerializer
{
	public class ShsWriter : BinaryWriter
	{
		public ShsWriter(Stream output)
			: base(output)
		{
		}

		public virtual void Write(Type type)
		{
			Write(NetType.GetNetType(type));
		}

		public virtual void Write(Guid v)
		{
			byte[] array = v.ToByteArray();
			for (int i = 0; i < 16; i++)
			{
				Write(array[i]);
			}
		}

		public virtual void Write(Vector2 v)
		{
			Write(v.x);
			Write(v.y);
		}

		public virtual void Write(Vector3 v)
		{
			Write(v.x);
			Write(v.y);
			Write(v.z);
		}

		public virtual void WriteXZ(Vector3 v)
		{
			Write(v.x);
			Write(v.z);
		}

		public virtual void Write(Quaternion v)
		{
			Write(v.x);
			Write(v.y);
			Write(v.z);
			Write(v.w);
		}

		public virtual void Write(GoNetId id)
		{
			Write(id.ParentId);
			Write(id.ChildId);
		}

		public virtual void Write(GameObject go)
		{
			if (go == null)
			{
				Write(new GoNetId(-1, -1));
				return;
			}
			NetworkComponent networkComponent = go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent == null)
			{
				throw new Exception("Trying to marshall a non-networked game object: " + go.name);
			}
			if (!networkComponent.goNetId.IsValid())
			{
				throw new Exception("Trying to marshall a game object without a goNetId: " + go.name);
			}
			Write(networkComponent.goNetId);
		}

		public virtual void Write(NetworkMessage msg)
		{
			Write(msg.GetType());
			msg.SerializeToBinary(this);
		}

		public virtual void Write(List<NetworkMessage> msgList)
		{
			Write(msgList.Count);
			foreach (NetworkMessage msg in msgList)
			{
				Write(msg);
			}
		}

		public virtual void Write(NetAction act)
		{
			Write(act.GetType());
			act.SerializeToBinary(this);
		}

		public virtual void WriteBasicType(object obj)
		{
			byte netType = NetType.GetNetType(obj.GetType());
			if (netType >= 8)
			{
				throw new Exception("Unknown basic type");
			}
			Write(netType);
			switch (netType)
			{
			case 0:
				Write((int)obj);
				break;
			case 1:
				Write((long)obj);
				break;
			case 2:
				Write((float)obj);
				break;
			case 3:
				Write((string)obj);
				break;
			case 4:
				Write((Vector3)obj);
				break;
			case 5:
				Write((Quaternion)obj);
				break;
			case 6:
				Write((GoNetId)obj);
				break;
			case 7:
				Write((GameObject)obj);
				break;
			default:
				CspUtils.DebugLog("Unhandled type <" + netType + "> in WriteBasicType");
				break;
			}
		}

		public virtual void Write(Hashtable table)
		{
			Write(table.Count);
			foreach (DictionaryEntry item in table)
			{
				byte netType = NetType.GetNetType(item.Value.GetType());
				if (netType >= 8)
				{
					CspUtils.DebugLog("ShsWriter: Unknown type in hashtable, Key = " + item.Key.ToString());
				}
				else
				{
					Write((string)item.Key);
					WriteBasicType(item.Value);
				}
			}
		}
	}

	public class ShsReader : BinaryReader
	{
		private const int MAX_AGGREGATE_MSGS = 100;

		public ShsReader(Stream input)
			: base(input)
		{
		}

		public virtual float ReadFloat()
		{
			return ReadSingle();
		}

		public virtual long ReadLong()
		{
			return ReadInt64();
		}

		public virtual Guid ReadGuid()
		{
			byte[] array = new byte[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = ReadByte();
			}
			return new Guid(array);
		}

		public virtual object ReadType()
		{
			byte idx = ReadByte();
			return NetType.ObjectFactory(idx);
		}

		public virtual Vector3 ReadVector2()
		{
			Vector2 v = default(Vector2);
			v.x = ReadFloat();
			v.y = ReadFloat();
			return v;
		}

		public virtual Vector3 ReadVector3()
		{
			Vector3 result = default(Vector3);
			result.x = ReadFloat();
			result.y = ReadFloat();
			result.z = ReadFloat();
			return result;
		}

		public virtual Vector3 ReadVectorXZ()
		{
			Vector3 result = default(Vector3);
			result.x = ReadFloat();
			result.y = 0f;
			result.z = ReadFloat();
			return result;
		}

		public virtual Quaternion ReadQuaternion()
		{
			Quaternion result = default(Quaternion);
			result.x = ReadFloat();
			result.y = ReadFloat();
			result.z = ReadFloat();
			result.w = ReadFloat();
			return result;
		}

		public virtual GoNetId ReadGoNetId()
		{
			int parent = ReadInt32();
			int child = ReadInt32();
			return new GoNetId(parent, child);
		}

		public virtual GameObject ReadGameObject()
		{
			GoNetId goNetId = ReadGoNetId();
			if (!goNetId.IsValid())
			{
				return null;
			}
			return AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(goNetId);
		}

		public virtual NetworkMessage ReadNetworkMessage()
		{
			NetworkMessage networkMessage = ReadType() as NetworkMessage;
			networkMessage.DeserializeFromBinary(this);
			return networkMessage;
		}

		public virtual List<NetworkMessage> ReadNetworkMessageList()
		{
			int num = ReadInt32();
			if (num > 100)
			{
				CspUtils.DebugLog("Too many NetworkMessages received. Discarding " + (num - 100));
				num = 100;
			}
			List<NetworkMessage> list = new List<NetworkMessage>(num);
			for (int i = 0; i < num; i++)
			{
				NetworkMessage item = ReadNetworkMessage();
				list.Add(item);
			}
			return list;
		}

		public virtual NetAction ReadNetAction()
		{
			NetAction netAction = ReadType() as NetAction;
			netAction.DeserializeFromBinary(this);
			return netAction;
		}

		public virtual object ReadBasicType()
		{
			byte b = ReadByte();
			object result = null;
			switch (b)
			{
			case 0:
				result = ReadInt32();
				break;
			case 1:
				result = ReadLong();
				break;
			case 2:
				result = ReadFloat();
				break;
			case 3:
				result = ReadString();
				break;
			case 4:
				result = ReadVector3();
				break;
			case 5:
				result = ReadQuaternion();
				break;
			case 6:
				result = ReadGoNetId();
				break;
			case 7:
				result = ReadGameObject();
				break;
			default:
				CspUtils.DebugLog("Unhandled type <" + b + "> in ReadBasicType");
				break;
			}
			return result;
		}

		public virtual Hashtable ReadHashtable()
		{
			Hashtable hashtable = new Hashtable();
			int num = ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = ReadString();
				object obj2 = hashtable[key] = ReadBasicType();
			}
			return hashtable;
		}
	}
}
