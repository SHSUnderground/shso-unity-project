using System.Collections.Generic;

public class AnimClipManager
{
	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
    public string Name;

	private List<AnimClip> animationPieces = new List<AnimClip>();

	private List<AnimClip> animationPiecesToRemove = new List<AnimClip>();

	private List<AnimClip> animationPiecesToAdd = new List<AnimClip>();

	private static bool globalSkipAnimations;

	private bool skipAnimations;

	private bool updateInProgress;

	private bool queuedClearAll;

	private bool queuedForceCompleteAllAndClear;

	private List<AnimClip> AnimationListAdd
	{
		get
		{
			if (updateInProgress)
			{
				return animationPiecesToAdd;
			}
			return animationPieces;
		}
	}

	private List<AnimClip> AnimationListRemove
	{
		get
		{
			if (updateInProgress)
			{
				return animationPiecesToRemove;
			}
			return animationPieces;
		}
	}

	public bool GlobalSkipAnimations
	{
		get
		{
			return globalSkipAnimations;
		}
		set
		{
			globalSkipAnimations = value;
		}
	}

	public bool SkipAnimations
	{
		get
		{
			return skipAnimations;
		}
		set
		{
			skipAnimations = value;
		}
	}

	public void Update(float deltaTime)
	{
		if (animationPieces.Count > 0 && SHSDebugAnimClipManagerInfoWindow.RecordingAnim)
		{
			SHSDebugAnimClipManagerInfoWindow.Publish(true, Name, string.Empty);
		}
		updateInProgress = true;
		if (!skipAnimations && !globalSkipAnimations)
		{
			foreach (AnimClip animationPiece in animationPieces)
			{
				animationPiece.Update(deltaTime);
				if (animationPiece.Done)
				{
					animationPiecesToRemove.Add(animationPiece);
				}
			}
		}
		else
		{
			foreach (AnimClip animationPiece2 in animationPieces)
			{
				animationPiece2.ForceComplete();
				animationPiecesToRemove.Add(animationPiece2);
			}
		}
		animationPieces.AddRange(animationPiecesToAdd);
		foreach (AnimClip item in animationPiecesToRemove)
		{
			animationPieces.Remove(item);
		}
		animationPiecesToRemove.Clear();
		animationPiecesToAdd.Clear();
		updateInProgress = false;
		if (queuedClearAll)
		{
			queuedClearAll = false;
			ClearAll();
		}
		if (queuedForceCompleteAllAndClear)
		{
			queuedForceCompleteAllAndClear = false;
			ForceCompleteAllAndClear();
		}
	}

	public void Add(AnimClip toAdd)
	{
		AnimationListAdd.Add(toAdd);
	}

	public void Remove(AnimClip toRemove)
	{
		AnimationListRemove.Remove(toRemove);
	}

	public void AddAll(List<AnimClip> toAdd)
	{
		AnimationListAdd.AddRange(toAdd);
	}

	public void RemoveAll(List<AnimClip> toRemove)
	{
		foreach (AnimClip item in toRemove)
		{
			AnimationListRemove.Remove(item);
		}
	}

	public void RemoveIfUnfinished(AnimClip toRemove)
	{
		if (toRemove != null && !toRemove.Done)
		{
			AnimationListRemove.Remove(toRemove);
		}
	}

	public void SwapOut(ref AnimClip OldPiece, AnimClip NewPiece)
	{
		RemoveIfUnfinished(OldPiece);
		OldPiece = NewPiece;
		Add(OldPiece);
	}

	public void RemoveAllIfUnfinished(List<AnimClip> toRemove)
	{
		foreach (AnimClip item in toRemove)
		{
			RemoveIfUnfinished(item);
		}
	}

	public void ClearAll()
	{
		if (updateInProgress)
		{
			queuedClearAll = true;
			return;
		}
		animationPieces.Clear();
		animationPiecesToRemove.Clear();
		animationPiecesToAdd.Clear();
	}

	public void ForceCompleteAllAndClear()
	{
		if (updateInProgress)
		{
			queuedForceCompleteAllAndClear = true;
			return;
		}
		updateInProgress = true;
		foreach (AnimClip animationPiece in animationPieces)
		{
			animationPiece.ForceComplete();
		}
		updateInProgress = false;
		ClearAll();
	}

	public bool ContainsAnimations()
	{
		return animationPieces.Count != 0;
	}

	public int RunningAnimations()
	{
		return animationPieces.Count;
	}

	public bool Contains(AnimClip piece)
	{
		if (updateInProgress)
		{
			return animationPieces.Contains(piece) || animationPiecesToAdd.Contains(piece);
		}
		return animationPieces.Contains(piece);
	}
}
