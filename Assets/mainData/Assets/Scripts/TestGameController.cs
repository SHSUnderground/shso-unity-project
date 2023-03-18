public class TestGameController : GameController
{
	public override void Start()
	{
		base.Start();
		TileLoader tileLoader = new TileLoader();
		tileLoader.Load("Layouts/Tile Test_layout", OnTilesLoaded);
		CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
	}

	protected void OnTilesLoaded(bool succes)
	{
		CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + succes);
	}
}
