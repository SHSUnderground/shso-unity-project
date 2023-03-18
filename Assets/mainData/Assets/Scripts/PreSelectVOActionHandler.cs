using System.Collections;
using UnityEngine;
using VO;

public class PreSelectVOActionHandler : IResolvedVOActionHandler
{
	private const float speakerScale = 1.05f;

	private IResolvedVOActionHandler baseHandler;

	private OnVOActionFinishedDelegate onFinished;

	private SHSCharacterSelect.CharacterItem characterItem;

	private GameObject headBouncer;

	private Vector2 originalOffset;

	private Vector2 originalSize;

	public PreSelectVOActionHandler(SHSCharacterSelect.CharacterItem characterItem)
	{
		this.characterItem = characterItem;
	}

	public void HandleResolvedVOAction(ResolvedVOAction vo, OnVOActionFinishedDelegate onFinished)
	{
		HighlightCharacter();
		baseHandler = VOManager.Instance.GetLocaleSpecificVOActionHandler();
		this.onFinished = onFinished;
		baseHandler.HandleResolvedVOAction(vo, OnBaseHandlerFinished);
	}

	public void CancelVOAction(ResolvedVOAction vo)
	{
		if (baseHandler != null)
		{
			baseHandler.CancelVOAction(vo);
		}
	}

	private void OnBaseHandlerFinished()
	{
		UnhighlightCharacter();
		if (onFinished != null)
		{
			onFinished();
		}
	}

	private void HighlightCharacter()
	{
		characterItem.currentState = SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Highlighted;
		characterItem.RequestRefresh = true;
		originalSize = characterItem.heroHead.Size;
		originalOffset = characterItem.heroHead.Offset;
		Vector2 vector = originalSize * 1.05f;
		characterItem.heroHead.Size = vector;
		characterItem.heroHead.Offset = originalOffset - (vector - originalSize) * 0.5f;
		headBouncer = new GameObject(characterItem.Name + " bouncer");
		headBouncer.AddComponent<CoroutineContainer>().StartCoroutine(BounceHead());
	}

	private void UnhighlightCharacter()
	{
		headBouncer.GetComponent<CoroutineContainer>().StopAllCoroutines();
		Object.Destroy(headBouncer);
		characterItem.heroHead.ForcedHover = false;
		characterItem.heroHead.Size = originalSize;
		characterItem.heroHead.Offset = originalOffset;
		characterItem.currentState = SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active;
		characterItem.RequestRefresh = true;
	}

	private IEnumerator BounceHead()
	{
		float timeoutTime = Time.time + 10f;
		while (Time.time < timeoutTime)
		{
			characterItem.heroHead.ForcedHover = true;
			yield return new WaitForSeconds(0.25f);
			characterItem.heroHead.ForcedHover = false;
			yield return new WaitForSeconds(0.25f);
		}
		Object.Destroy(headBouncer);
	}
}
