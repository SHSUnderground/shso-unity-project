using System.Collections;
using UnityEngine;

public abstract class LodBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Mode
	{
		Distance,
		Size,
		FPS
	}

	public Mode mode;

	public float pollSeconds = 0.25f;

	public float pollVariance = 0.1f;

	public float[] distances;

	protected int lod = -1;

	protected int forceLod = -1;

	protected float distToCam = -1f;

	public float Lod
	{
		get
		{
			return lod;
		}
	}

	public abstract void SetLod(int lod);

	public abstract bool IsVisible();

	public abstract Bounds GetBounds();

	public virtual void SetVisible(bool visible)
	{
		SetLod(lod);
	}

	public virtual void Start()
	{
	}

	public virtual void OnEnable()
	{
		switch (mode)
		{
		case Mode.Distance:
			StartCoroutine(PollCamaeraDistance());
			break;
		case Mode.Size:
			StartCoroutine(PollScreenSize());
			break;
		case Mode.FPS:
			StartCoroutine(PollFPS());
			break;
		}
	}

	public virtual void OnDisable()
	{
		StopAllCoroutines();
	}

	public virtual void ForceSetLod(int newLod)
	{
		forceLod = newLod;
	}

	protected IEnumerator PollCamaeraDistance()
	{
		yield return 0;
		float variance = Random.Range(0f - pollVariance, pollVariance);
		while (true)
		{
			distToCam = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
			int newLod = distances.Length;
			for (int i = 0; i < newLod; i++)
			{
				float d = distances[i];
				if (distToCam <= d * d)
				{
					newLod = i;
					break;
				}
			}
			SetLodInternal(newLod);
			yield return new WaitForSeconds(pollSeconds + variance);
		}
	}

	protected IEnumerator PollFPS()
	{
		yield return 0;
		float variance = Random.Range(0f - pollVariance, pollVariance);
		while (true)
		{
			int newLod = 0;
			if (AppShell.Instance != null)
			{
				float fps = AppShell.Instance.FPS;
				for (int i = 0; i < distances.Length; i++)
				{
					if (fps >= distances[i])
					{
						newLod = i;
						break;
					}
				}
			}
			SetLodInternal(newLod);
			yield return new WaitForSeconds(pollSeconds + variance);
		}
	}

	protected IEnumerator PollScreenSize()
	{
		yield return 0;
		float variance = Random.Range(0f - pollVariance, pollVariance);
		while (true)
		{
			if (IsVisible())
			{
				Bounds b = GetBounds();
				Camera main = Camera.main;
				Vector3 min = b.min;
				float x = min.x;
				Vector3 min2 = b.min;
				float y = min2.y;
				Vector3 min3 = b.min;
				Bounds boundsScreen = new Bounds(main.WorldToScreenPoint(new Vector3(x, y, min3.z)), Vector3.zero);
				Camera main2 = Camera.main;
				Vector3 min4 = b.min;
				float x2 = min4.x;
				Vector3 min5 = b.min;
				float y2 = min5.y;
				Vector3 max = b.max;
				boundsScreen.Encapsulate(main2.WorldToScreenPoint(new Vector3(x2, y2, max.z)));
				Camera main3 = Camera.main;
				Vector3 min6 = b.min;
				float x3 = min6.x;
				Vector3 max2 = b.max;
				float y3 = max2.y;
				Vector3 min7 = b.min;
				boundsScreen.Encapsulate(main3.WorldToScreenPoint(new Vector3(x3, y3, min7.z)));
				Camera main4 = Camera.main;
				Vector3 min8 = b.min;
				float x4 = min8.x;
				Vector3 max3 = b.max;
				float y4 = max3.y;
				Vector3 max4 = b.max;
				boundsScreen.Encapsulate(main4.WorldToScreenPoint(new Vector3(x4, y4, max4.z)));
				Camera main5 = Camera.main;
				Vector3 max5 = b.max;
				float x5 = max5.x;
				Vector3 min9 = b.min;
				float y5 = min9.y;
				Vector3 min10 = b.min;
				boundsScreen.Encapsulate(main5.WorldToScreenPoint(new Vector3(x5, y5, min10.z)));
				Camera main6 = Camera.main;
				Vector3 max6 = b.max;
				float x6 = max6.x;
				Vector3 min11 = b.min;
				float y6 = min11.y;
				Vector3 max7 = b.max;
				boundsScreen.Encapsulate(main6.WorldToScreenPoint(new Vector3(x6, y6, max7.z)));
				Camera main7 = Camera.main;
				Vector3 max8 = b.max;
				float x7 = max8.x;
				Vector3 max9 = b.max;
				float y7 = max9.y;
				Vector3 min12 = b.min;
				boundsScreen.Encapsulate(main7.WorldToScreenPoint(new Vector3(x7, y7, min12.z)));
				Camera main8 = Camera.main;
				Vector3 max10 = b.max;
				float x8 = max10.x;
				Vector3 max11 = b.max;
				float y8 = max11.y;
				Vector3 max12 = b.max;
				boundsScreen.Encapsulate(main8.WorldToScreenPoint(new Vector3(x8, y8, max12.z)));
				Vector3 size = boundsScreen.size;
				float x9 = size.x;
				Vector3 size2 = boundsScreen.size;
				float pct = x9 * size2.y / (Camera.main.pixelWidth * Camera.main.pixelHeight);
				int newLod = 0;
				for (int i = 0; i < distances.Length; i++)
				{
					if (pct >= distances[i])
					{
						newLod = i;
						break;
					}
				}
				SetLodInternal(newLod);
			}
			yield return new WaitForSeconds(pollSeconds + variance);
		}
	}

	private void SetLodInternal(int newLod)
	{
		if (forceLod >= 0)
		{
			newLod = forceLod;
		}
		if (newLod < GraphicsOptions.MaxLod)
		{
			newLod = GraphicsOptions.MaxLod;
		}
		if (lod != newLod)
		{
			lod = newLod;
			SetLod(newLod);
		}
	}
}
