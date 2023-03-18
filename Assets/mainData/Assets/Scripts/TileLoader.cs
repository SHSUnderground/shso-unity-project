using System.Collections.Generic;
using UnityEngine;

public class TileLoader
{
	public delegate void TilesLoadedDelegate(bool success);

	protected TransactionMonitor loadTransaction;

	protected TilesLoadedDelegate onDoneCallback;

	protected TileLayoutData layout;

	protected Dictionary<string, AssetBundle> bundles;

	public IEnumerable<TileData> Tiles
	{
		get
		{
			return layout.tiles;
		}
	}

	public TileLoader()
	{
		Reset();
	}

	public void Load(string xmlFile, TilesLoadedDelegate onDoneCallback)
	{
		loadTransaction = TransactionMonitor.CreateTransactionMonitor("TileLoader_loadTransaction", OnReadyForSpawn, 300f, null);
		loadTransaction.AddStep("xml");
		this.onDoneCallback = onDoneCallback;
		AppShell.Instance.DataManager.LoadGameData(xmlFile, LayoutLoadedCallback, new TileLayoutData(), loadTransaction);
	}

	public void Abort()
	{
		Reset();
	}

	protected void Reset()
	{
		loadTransaction = null;
		onDoneCallback = null;
		layout = null;
		bundles = null;
	}

	protected void OnDone(bool success)
	{
		if (onDoneCallback != null)
		{
			onDoneCallback(success);
			onDoneCallback = null;
		}
		Reset();
	}

	protected void LayoutLoadedCallback(GameDataLoadResponse response, object extraData)
	{
		if (loadTransaction == extraData)
		{
			layout = (response.DataDefinition as TileLayoutData);
			if ((response.Error != null && response.Error != string.Empty) || layout == null)
			{
				OnDone(false);
				return;
			}
			bundles = new Dictionary<string, AssetBundle>();
			foreach (TileData tile in layout.tiles)
			{
				if (!bundles.ContainsKey(tile.bundle))
				{
					bundles.Add(tile.bundle, null);
					loadTransaction.AddStep(tile.bundle);
					AppShell.Instance.BundleLoader.FetchAssetBundle("Tiles/" + tile.bundle, OnTileLoaded, loadTransaction);
					loadTransaction.AddStepBundle(tile.bundle, "Tiles/" + tile.bundle);
				}
			}
			loadTransaction.CompleteStep("xml");
		}
	}

	protected void OnTileLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (loadTransaction != extraData)
		{
			return;
		}
		if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
		{
			OnDone(false);
			return;
		}
		string text = response.Path;
		int num = text.LastIndexOf('/');
		if (num != -1)
		{
			text = text.Substring(num + 1);
		}
		if (bundles.ContainsKey(text))
		{
			bundles[text] = response.Bundle;
		}
		loadTransaction.CompleteStep(text);
	}

	protected void OnReadyForSpawn(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		loadTransaction = null;
		int num = 0;
		foreach (TileData tile in layout.tiles)
		{
			Object original = bundles[tile.bundle].Load(tile.bundle);
			tile.tileObj = (Object.Instantiate(original, tile.position, tile.rotation) as GameObject);
			Snap[] components = Utils.GetComponents<Snap>(tile.tileObj, Utils.SearchChildren);
			Snap[] array = components;
			foreach (Snap obj in array)
			{
				Object.Destroy(obj);
			}
			SnapParent[] components2 = Utils.GetComponents<SnapParent>(tile.tileObj, Utils.SearchChildren);
			SnapParent[] array2 = components2;
			foreach (SnapParent obj2 in array2)
			{
				Object.Destroy(obj2);
			}
			if (tile.id == null || tile.id == string.Empty)
			{
				int num2 = ++num;
				tile.id = "unknown_" + num2.ToString();
			}
			TileInfo component = Utils.GetComponent<TileInfo>(tile.tileObj);
			component.TileName = tile.id;
		}
		OnDone(true);
	}
}
