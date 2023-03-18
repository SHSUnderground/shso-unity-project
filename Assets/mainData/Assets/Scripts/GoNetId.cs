using System;
using System.IO;

[Serializable]
public sealed class GoNetId
{
	public static int STATIC_ID_START = -100;

	public static int PLAYER_ID_FLAG = -10;

	public static int SCENE_ID_START = -20;

	public int parentId = -1;

	public int childId = -1;

	public int ParentId
	{
		get
		{
			return parentId;
		}
	}

	public int ChildId
	{
		get
		{
			return childId;
		}
	}

	public static GoNetId Invalid
	{
		get
		{
			return new GoNetId(-1, -1);
		}
	}

	public GoNetId(int parent, int child)
	{
		parentId = parent;
		childId = child;
	}

	public GoNetId(GoNetId goNetId)
	{
		parentId = goNetId.parentId;
		childId = goNetId.childId;
	}

	public static GoNetId FromBase64(string str)
	{
		byte[] buffer = Convert.FromBase64String(str);
		MemoryStream input = new MemoryStream(buffer);
		ShsSerializer.ShsReader shsReader = new ShsSerializer.ShsReader(input);
		return shsReader.ReadGoNetId();
	}

	public string ToBase64()
	{
		MemoryStream memoryStream = new MemoryStream(8);
		ShsSerializer.ShsWriter shsWriter = new ShsSerializer.ShsWriter(memoryStream);
		shsWriter.Write(this);
		return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
	}

	public bool IsValid()
	{
		return childId != -1 || parentId != -1;
	}

	public bool IsStaticId()
	{
		return ChildId <= STATIC_ID_START && ParentId <= -1 && ParentId != PLAYER_ID_FLAG;
	}

	public bool IsDynamicId()
	{
		return ChildId >= 0 && ParentId >= 0;
	}

	public bool IsPlayerId()
	{
		return ParentId == PLAYER_ID_FLAG;
	}

	public bool IsLocalPlayer()
	{
		return ParentId == PLAYER_ID_FLAG && ChildId == AppShell.Instance.ServerConnection.GetGameUserId();
	}

	private static ulong MakeId(int parent, int child)
	{
		ulong num = (uint)parent;
		num <<= 32;
		return num | (uint)child;
	}

	public override string ToString()
	{
		return "<" + ParentId + "," + ChildId + ">";
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (GetType() != obj.GetType())
		{
			return false;
		}
		GoNetId goNetId = (GoNetId)obj;
		if (parentId != goNetId.parentId)
		{
			return false;
		}
		if (childId != goNetId.childId)
		{
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return parentId ^ childId;
	}

	public static bool operator ==(GoNetId a, GoNetId b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		return a.parentId == b.parentId && a.childId == b.childId;
	}

	public static bool operator !=(GoNetId a, GoNetId b)
	{
		if (a == null || b == null)
		{
			return true;
		}
		return a.parentId != b.parentId || a.childId != b.childId;
	}

	public static implicit operator ulong(GoNetId a)
	{
		return MakeId(a.parentId, a.childId);
	}
}
