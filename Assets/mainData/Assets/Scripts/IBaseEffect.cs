using System;
using UnityEngine;

public interface IBaseEffect
{
	bool HasAsset
	{
		get;
	}

	Type AssetType
	{
		get;
	}

	object Asset
	{
		get;
		set;
	}

	string GetName();

	float GetTimeOffset();

	float GetLifetime(GameObject obj);

	float GetAssetLifetime(GameObject obj);

	void SetTimeOffset(float time_offset);

	void SetLifetime(float duration);

	bool GetAllowsLifetime();
}
