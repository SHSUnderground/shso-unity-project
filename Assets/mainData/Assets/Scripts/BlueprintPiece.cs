public class BlueprintPiece
{
	public int id;

	public int blueprintID;

	public int ownableTypeID;

	public int quantity;

	public BlueprintPiece(BlueprintPieceJsonData data)
	{
		id = data.id;
		blueprintID = data.sid;
		ownableTypeID = data.o;
		quantity = data.q;
	}
}
