public class PokeAndJiggleAdapter : InteractiveObjectAnimation
{
	public void Triggered()
	{
		PokeAndJiggle component = Utils.GetComponent<PokeAndJiggle>(base.gameObject, Utils.SearchParents);
		if (component != null)
		{
			component.DropLoot(true);
		}
	}
}
