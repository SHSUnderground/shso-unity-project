public class MedallionData
{
	public int medallionID = -1;

	public string name = string.Empty;

	public string medallionTextureSource = string.Empty;

	public MedallionData(int medallionID, string name, string medallionTextureSource)
	{
		this.medallionID = medallionID;
		this.name = name;
		this.medallionTextureSource = medallionTextureSource;
	}
}
