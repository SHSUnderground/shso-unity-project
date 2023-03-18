using System.Text;

public class LevelUpRewardItemsManifest : StaticDataDefinitionDictionary<LevelUpRewardItems>
{
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string key in base.Keys)
		{
			stringBuilder.AppendLine(this[key].ToString());
		}
		return stringBuilder.ToString();
	}
}
