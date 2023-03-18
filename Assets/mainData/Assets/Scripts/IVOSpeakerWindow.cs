public interface IVOSpeakerWindow
{
	void SetVO(ResolvedVOAction vo);

	void SetCharacter(string characterID);

	void SetText(string textID);

	void AnimateIn();

	void AnimateOut();
}
