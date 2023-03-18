using System.Collections;
using UnityEngine;
using VO;

public class RelationshipsVOHandler : IResolvedVOActionHandler
{
	private const float ANIMATE_TIMEOUT = 20f;

	private const float ANIMATE_CYCLE_TIME = 0.25f;

	private IResolvedVOActionHandler baseHandler;

	private OnVOActionFinishedDelegate onFinished;

	private bool animate;

	public void HandleResolvedVOAction(ResolvedVOAction vo, OnVOActionFinishedDelegate onFinished)
	{
		StartAnimating(vo);
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
		StopAnimating();
		if (onFinished != null)
		{
			onFinished();
		}
	}

	private void StartAnimating(ResolvedVOAction vo)
	{
		animate = true;
		CreateFaceSwitcher(vo.Emitter);
	}

	private void StopAnimating()
	{
		animate = false;
	}

	private void CreateFaceSwitcher(GameObject emitter)
	{
		GameObject gameObject = new GameObject("Relationships VO: Face Switcher");
		gameObject.AddComponent<CoroutineContainer>().StartCoroutine(CoSwitchFace(emitter, gameObject));
	}

	private IEnumerator CoSwitchFace(GameObject emitter, GameObject faceSwitcher)
	{
		int happiness = 1;
		float animateTimeout = Time.time + 20f;
		while (animate && Time.time < animateTimeout)
		{
			if (BrawlerStatManager.Active)
			{
				happiness = (happiness - 2) * -1 + 1;
				BrawlerStatManager.instance.ReportEmotionalEvent(emitter, happiness);
			}
			yield return new WaitForSeconds(0.25f);
		}
		if (BrawlerStatManager.Active)
		{
			BrawlerStatManager.instance.ReportEmotionalEvent(emitter, 1);
		}
		Object.Destroy(faceSwitcher);
	}
}
