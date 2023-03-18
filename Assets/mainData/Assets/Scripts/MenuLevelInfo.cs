using System;
using System.Runtime.CompilerServices;

public class MenuLevelInfo : IComparable<MenuLevelInfo>
{
	protected float menuWidth;

	[CompilerGenerated]
	private bool _003CConfigured_003Ek__BackingField;

	[CompilerGenerated]
	private int _003COrdinal_003Ek__BackingField;

	[CompilerGenerated]
	private MenuChatItemWindow _003CCurrentMenuChatItemWindow_003Ek__BackingField;

	public bool Configured
	{
		[CompilerGenerated]
		get
		{
			return _003CConfigured_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CConfigured_003Ek__BackingField = value;
		}
	}

	public int Ordinal
	{
		[CompilerGenerated]
		get
		{
			return _003COrdinal_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003COrdinal_003Ek__BackingField = value;
		}
	}

	public float MenuWidth
	{
		get
		{
			return menuWidth;
		}
		set
		{
			menuWidth = value;
			Configured = true;
		}
	}

	public MenuChatItemWindow CurrentMenuChatItemWindow
	{
		[CompilerGenerated]
		get
		{
			return _003CCurrentMenuChatItemWindow_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CCurrentMenuChatItemWindow_003Ek__BackingField = value;
		}
	}

	public MenuLevelInfo(int ordinal)
	{
		Ordinal = ordinal;
	}

	public int CompareTo(MenuLevelInfo other)
	{
		return Ordinal.CompareTo(other.Ordinal);
	}

	public static bool operator <(MenuLevelInfo me, MenuLevelInfo you)
	{
		return me.Ordinal < you.Ordinal;
	}

	public static bool operator >(MenuLevelInfo me, MenuLevelInfo you)
	{
		return me.Ordinal > you.Ordinal;
	}
}
