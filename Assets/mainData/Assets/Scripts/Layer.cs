public class Layer
{
	public class Mask
	{
		public const int Default = 1;

		public const int TransparentFX = 2;

		public const int IgnoreRaycast = 4;

		public const int Water = 16;

		public const int TransparentFXDOF = 524288;

		public const int Walkable = 256;

		public const int Clickable = 512;

		public const int CharacterClickable = 4096;

		public const int CharacterUnclickable = 2097152;

		public const int Throwable = 8192;

		public const int ClickBox = 16384;

		public const int ClickableSolid = 1048576;

		public const int Character = 2101248;

		public const int AllClickable = 1077760;

		public const int Fadeable = 1024;

		public const int WalkableNoFade = 2048;

		public const int AllWalkable = 100608;

		public const int AllFadeable = 328960;

		public const int Diorama = 8388608;

		public const int FadedObject = 268435456;

		public const int TemporaryRender = 536870912;

		public const int DynamicShadowMesh = 1073741824;

		public const int DynamicShadowTarget = int.MinValue;

		public const int DynamicShadow = -1073741824;

		public const int Floor = 32768;

		public const int FloorFadeable = 65536;

		public const int AllFloors = 98304;

		public const int Wall = 131072;

		public const int WallFadeable = 262144;

		public const int WallInvisible = 4194304;

		public const int AllWalls = 4587520;

		public const int Everything = 1073741823;

		public const int Transparent = 268959762;

		public const int TransparentDOF = 524304;

		public const int SolidAO = 804765677;

		public const int Solid = 804756969;

		public const int BrawlerIntangible = -271086103;

		public const int BrawlerTargetting = -275280407;

		public const int NonCharacters = -271077399;
	}

	public const int Default = 0;

	public const int TransparentFX = 1;

	public const int IgnoreRaycast = 2;

	public const int Water = 4;

	public const int Walkable = 8;

	public const int Clickable = 9;

	public const int Fadeable = 10;

	public const int WalkableNoFade = 11;

	public const int Character = 12;

	public const int Throwable = 13;

	public const int ClickBox = 14;

	public const int Floor = 15;

	public const int FloorFadeable = 16;

	public const int Wall = 17;

	public const int WallFadeable = 18;

	public const int TransparentFXDOF = 19;

	public const int ClickableSolid = 20;

	public const int CharacterUnclickable = 21;

	public const int WallInvisible = 22;

	public const int Diorama = 23;

	public const int FadedObject = 28;

	public const int TemporaryRender = 29;

	public const int DynamicShadowMesh = 30;

	public const int DynamicShadowTarget = 31;

	public static int LayerFromName(string name)
	{
		switch (name.ToLower())
		{
		case "walkable":
			return 16;
		case "walkablenofade":
			return 15;
		case "fadeable":
			return 18;
		case "transparentfx":
			return 1;
		case "transparentfxdof":
			return 19;
		case "ignoreraycast":
		case "ignore raycast":
			return 2;
		case "water":
			return 4;
		case "clickable":
			return 9;
		case "clickablesolid":
			return 20;
		case "throwable":
			return 13;
		case "clickBox":
			return 14;
		case "temporaryrnder":
			return 29;
		case "dynamicshadowmesh":
			return 30;
		case "dynamicshadowtarget":
			return 31;
		case "floor":
			return 15;
		case "floorfadeable":
		case "floor fadeable":
			return 16;
		case "wall":
			return 17;
		case "wallfadeable":
		case "wall fadeable":
			return 18;
		case "wallinvisible":
		case "wall invisible":
			return 22;
		case "diorama":
			return 23;
		case "dockpoint":
		case "snapanchor":
		case "default":
			return 0;
		default:
			CspUtils.DebugLog("Unkown layer name <" + name + "> using default");
			return 0;
		}
	}
}
