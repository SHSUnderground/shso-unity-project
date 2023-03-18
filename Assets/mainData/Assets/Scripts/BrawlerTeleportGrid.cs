using System.Collections.Generic;
using UnityEngine;

public class BrawlerTeleportGrid : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const int GRID_SEGMENTS = 7;

	private const int TOTAL_SEGMENTS = 49;

	public bool circle;

	protected int halfUnits = 3;

	protected int circleRange = 12;

	protected Vector3 gridStart = Vector3.zero;

	protected Vector3 gridCol = Vector3.zero;

	protected Vector3 gridRow = Vector3.zero;

	protected Vector3 drawStart = Vector3.zero;

	protected Vector3 drawUnit = new Vector3(0.75f, 0.75f, 0.75f);

	protected int[] dangerGrid;

	public void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		dangerGrid = new int[49];
		ClearGrid();
	}

	public void ClearGrid()
	{
		for (int i = 0; i < 49; i++)
		{
			dangerGrid[i] = 0;
		}
	}

	protected void CalculateDimensions()
	{
		gridStart = base.transform.position;
		
		float x = gridStart.x;
		Vector3 localScale = base.transform.localScale;
		gridStart.x = x - localScale.x * 0.5f;
		
		float z = gridStart.z;
		Vector3 localScale2 = base.transform.localScale;
		gridStart.z = z - localScale2.z * 0.5f;
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		gridStart = Quaternion.AngleAxis(eulerAngles.y, Vector3.up) * (gridStart - base.transform.position) + base.transform.position;
		
		Vector3 localScale3 = base.transform.localScale;
		gridCol.x = localScale3.x / 7f;
		gridCol.y = 0f;
		gridCol.z = 0f;
		Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
		gridCol = Quaternion.AngleAxis(eulerAngles2.y, Vector3.up) * gridCol;
		gridRow.x = 0f;
		gridRow.y = 0f;
		
		Vector3 localScale4 = base.transform.localScale;
		gridRow.z = localScale4.z / 7f;
		Vector3 eulerAngles3 = base.transform.rotation.eulerAngles;
		gridRow = Quaternion.AngleAxis(eulerAngles3.y, Vector3.up) * gridRow;
		drawStart = gridStart + (gridRow + gridCol) * 0.5f;
		drawUnit = (gridRow + gridCol) * 0.75f;
		drawUnit.y = 0.25f;
	}

	public Vector3 GetSafeLocation(CombatController.Faction myFaction)
	{
		List<GameObject> list = new List<GameObject>();
		if (myFaction == CombatController.Faction.Enemy)
		{
			List<PlayerCombatController> playerList = PlayerCombatController.PlayerList;
			if (playerList != null)
			{
				foreach (PlayerCombatController item in playerList)
				{
					list.Add(item.gameObject);
				}
			}
		}
		if (myFaction == CombatController.Faction.Player)
		{
			AICombatController[] array = Utils.FindObjectsOfType<AICombatController>();
			if (array != null)
			{
				AICombatController[] array2 = array;
				foreach (AICombatController aICombatController in array2)
				{
					list.Add(aICombatController.gameObject);
				}
			}
		}
		return FillDangerGrid(list);
	}

	public Vector3 FillDangerGrid(List<GameObject> dangerousObjects)
	{
		CalculateDimensions();
		ClearGrid();
		if (circle)
		{
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					int num = i - halfUnits;
					int num2 = j - halfUnits;
					int num3 = num * num + num2 * num2;
					if (num3 > circleRange)
					{
						dangerGrid[i + j * 7] = 20;
					}
				}
			}
		}
		Vector2 vector = new Vector2(0f, 0f);
		float num4 = 0f;
		foreach (GameObject dangerousObject in dangerousObjects)
		{
			Vector3 vector2 = dangerousObject.transform.position - gridStart;
			vector2.y = 0f;
			float magnitude = Vector3.Project(vector2, gridRow.normalized).magnitude;
			Vector3 localScale = base.transform.localScale;
			float x = magnitude / localScale.z;
			float magnitude2 = Vector3.Project(vector2, gridCol.normalized).magnitude;
			Vector3 localScale2 = base.transform.localScale;
			Vector2 vector3 = new Vector2(x, magnitude2 / localScale2.x);
			if (!(vector3.x < 0f) && !((vector3.x >= 1f) | (vector3.y < 0f)) && !(vector3.y >= 1f))
			{
				vector += vector3;
				num4 += 1f;
				Vector2 vector4 = vector3;
				int num5 = (int)(vector4.x * 7f);
				int num6 = (int)(vector4.y * 7f);
				int num7 = num5 + num6 * 7;
				dangerGrid[num7] += 3;
				if (num5 - 1 >= 0)
				{
					dangerGrid[num7 - 1] += 2;
				}
				if (num5 + 1 < 7)
				{
					dangerGrid[num7 + 1] += 2;
				}
				if (num6 - 1 >= 0)
				{
					dangerGrid[num7 - 7] += 2;
				}
				if (num6 + 1 < 7)
				{
					dangerGrid[num7 + 7] += 2;
				}
				if (num5 - 1 >= 0 && num6 - 1 >= 0)
				{
					dangerGrid[num7 - 8]++;
				}
				if (num5 - 1 >= 0 && num6 + 1 < 7)
				{
					dangerGrid[num7 + 7 - 1]++;
				}
				if (num5 + 1 < 7 && num6 - 1 >= 0)
				{
					dangerGrid[num7 + 1 - 7]++;
				}
				if (num5 + 1 < 7 && num6 + 1 < 7)
				{
					dangerGrid[num7 + 1 + 7]++;
				}
			}
		}
		if (num4 > 0f)
		{
			vector.x /= num4;
			vector.y /= num4;
			int num8 = (int)(vector.x * 7f);
			int num9 = (int)(vector.y * 7f);
			int num10 = 0;
			int num11 = 0;
			int num12 = 100;
			int num13 = 49;
			for (int k = 0; k < 7; k++)
			{
				for (int l = 0; l < 7; l++)
				{
					bool flag = false;
					int num14 = dangerGrid[k + l * 7];
					if (num14 == num12)
					{
						int num15 = k - num8;
						int num16 = l - num9;
						int num17 = num15 * num15 + num16 * num16;
						if (num17 > num13)
						{
							flag = true;
						}
					}
					else if (num14 < num12)
					{
						flag = true;
					}
					if (flag)
					{
						int num18 = k - num8;
						int num19 = l - num9;
						num13 = num18 * num18 + num19 * num19;
						num12 = num14;
						num10 = k;
						num11 = l;
					}
				}
			}
			dangerGrid[num10 + num11 * 7] = -1;
			Vector3 a = gridStart + (gridCol + gridRow) * 0.5f;
			a += gridRow * num10;
			return a + gridCol * num11;
		}
		dangerGrid[halfUnits + halfUnits * 7] = -1;
		return base.transform.position;
	}

	private void OnDrawGizmosSelected()
	{
		if (dangerGrid == null || dangerGrid.Length != 49)
		{
			Initialize();
		}
		GetSafeLocation(CombatController.Faction.Enemy);
		Vector3 a = gridStart;
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				if (dangerGrid[i + j * 7] == -1)
				{
					Gizmos.color = Color.green;
				}
				else
				{
					float num = Mathf.Min((float)dangerGrid[i + j * 7] / 3f, 1f);
					Gizmos.color = new Color(num * num, 0f, (1f - num) * (1f - num));
				}
				Gizmos.DrawSphere(a + (gridRow + gridCol) * 0.5f, 0.2f);
				a += gridCol;
			}
			a += gridRow;
			a -= gridCol * 7f;
		}
	}
}
