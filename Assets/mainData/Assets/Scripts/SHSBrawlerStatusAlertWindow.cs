using System;
using System.Collections.Generic;
using UnityEngine;

internal class SHSBrawlerStatusAlertWindow : GUISimpleControlWindow
{
	private class SHSBrawlerStatusAlert : GUISimpleControlWindow
	{
		public enum AlertStatus
		{
			None,
			Center,
			Move,
			Stack
		}

		public delegate void OnFinishMove();

		public AlertStatus status;

		public OnFinishMove onFinishMove;

		private GUIImage _alertBackground;

		private string _alertBackgroundTexture;

		private int _alertId;

		private bool _heroUp;

		private Transform _heroTransform;

		private Transform _heroHeadTransform;

		private List<SHSBrawlerStatusAlert> _arrowList;

		private SHSBrawlerStatusAlert _target;

		private SHSBrawlerStatusAlertAnimator _animator;

		public string AlertBackgroundTexture
		{
			get
			{
				return _alertBackgroundTexture;
			}
		}

		public bool IsHeroUp
		{
			get
			{
				return _heroUp;
			}
		}

		public Vector2 Destination
		{
			get
			{
				return (_animator == null) ? Vector2.zero : _animator.End;
			}
		}

		public bool IsMoving
		{
			get
			{
				return _animator != null && _animator.IsAnimating();
			}
		}

		public int AlertId
		{
			get
			{
				return _alertId;
			}
			set
			{
				_alertId = value;
			}
		}

		public SHSBrawlerStatusAlert Target
		{
			get
			{
				return _target;
			}
		}

		public SHSBrawlerStatusAlert(string alertBackgroundTexture, bool heroUp)
		{
			_alertBackgroundTexture = alertBackgroundTexture;
			_heroUp = heroUp;
			_animator = new SHSBrawlerStatusAlertAnimator(this);
		}

		public override bool InitializeResources(bool reload)
		{
			if (!base.InitializeResources(reload))
			{
				return false;
			}
			_alertBackground = new GUIImage();
			_alertBackground.SetPositionAndSize(QuickSizingHint.ParentSize);
			SetStatusAlertBackground(_alertBackgroundTexture, _heroUp);
			Add(_alertBackground);
			return true;
		}

		public override void Update()
		{
			base.Update();
			if (_heroTransform != null)
			{
				UpdatePositionToHero();
			}
			if (_animator == null)
			{
				return;
			}
			if (_animator.IsAnimating())
			{
				if (_animator.DoneAnimation())
				{
					FinishMove();
				}
				else if (_target != null)
				{
					_animator.End = _target.Offset;
				}
			}
			_animator.Update();
		}

		public void SetStatusAlertBackground(string alertBackgroundTexture, bool heroUp)
		{
			_alertBackgroundTexture = alertBackgroundTexture;
			_alertBackground.TextureSource = _alertBackgroundTexture;
			_heroUp = heroUp;
			if (_heroUp)
			{
				SetSize(AlertHeroUpMessageWindowSize);
			}
			else
			{
				SetSize(AlertMessageWindowSize);
			}
		}

		public void TargetHero(Transform hero, Transform heroHead)
		{
			_heroTransform = hero;
			_heroHeadTransform = heroHead;
		}

		public void TargetAlert(SHSBrawlerStatusAlert target)
		{
			if (_target != null)
			{
				if (_target._arrowList != null)
				{
					_target._arrowList.Remove(this);
				}
				_target = null;
			}
			_target = target;
			if (_target != null)
			{
				if (_target._arrowList == null)
				{
					_target._arrowList = new List<SHSBrawlerStatusAlert>();
				}
				if (!_target._arrowList.Contains(this))
				{
					_target._arrowList.Add(this);
				}
			}
		}

		public void DrawFire(SHSBrawlerStatusAlert fromTarget)
		{
			if (fromTarget != null && fromTarget._arrowList != null && fromTarget._arrowList.Count != 0)
			{
				List<SHSBrawlerStatusAlert> list = new List<SHSBrawlerStatusAlert>(fromTarget._arrowList);
				foreach (SHSBrawlerStatusAlert item in list)
				{
					if (item != null)
					{
						item.TargetAlert(this);
					}
				}
			}
		}

		public bool IsArrow()
		{
			return _target != null;
		}

		public bool IsTarget()
		{
			return _arrowList != null && _arrowList.Count > 0;
		}

		public void FlyToTarget()
		{
			if (_target != null)
			{
				StartMove(_target.Offset, 0f, MoveAlertPathTime, true);
			}
		}

		public void FlyToStack(Vector2 destination)
		{
			if (IsHeroUp)
			{
				destination.x = 1f - destination.x;
			}
			StartMove(destination, 0f, MoveAlertPathTime, true);
		}

		public void StartMove(Vector2 destination, float delay, float time, bool linear)
		{
			if (_animator != null)
			{
				_animator.StartAnimation(delay, time, destination, linear);
			}
		}

		public void StopMove()
		{
			if (_animator != null)
			{
				_animator.StopAnimation();
			}
		}

		public void FinishMove()
		{
			if (_animator != null)
			{
				Offset = _animator.End;
			}
			StopMove();
			if (onFinishMove != null)
			{
				onFinishMove();
				onFinishMove = null;
			}
		}

		public void StartBlink()
		{
			if (_animator != null)
			{
				_animator.StartBlink(0.12f);
			}
		}

		public void StopBlink()
		{
			if (_animator != null)
			{
				_animator.StopBlink();
			}
		}

		public void ClearAlert()
		{
			TargetHero(null, null);
			TargetAlert(null);
			StopMove();
			StopBlink();
		}

		private void UpdatePositionToHero()
		{
			if (_heroTransform != null)
			{
				Vector3 position = _heroTransform.position;
				if (_heroHeadTransform != null)
				{
					position = _heroHeadTransform.position;
				}
				Vector3 vector = Camera.main.WorldToScreenPoint(position);
				float x = vector.x / Parent.ScreenRect.width;
				float y = 1f - (vector.y + AlertMessageHeadOffset) / Parent.ScreenRect.height;
				SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.BottomMiddle, OffsetType.Percentage, new Vector2(x, y));
			}
		}
	}

	private class SHSBrawlerStatusAlertAnimator
	{
		public static readonly float BlinkPeriod = 1f;

		private SHSBrawlerStatusAlert _animateAlert;

		private float _animateTime;

		private float _animateStartTime;

		private Vector2 _animateStart;

		private Vector2 _animateEnd;

		private bool _animateLinear;

		private float _blinkPeriodStartTime;

		private float _blinkStartTime;

		private float _blinkTime;

		public Vector2 Start
		{
			get
			{
				return _animateStart;
			}
		}

		public Vector2 End
		{
			get
			{
				return _animateEnd;
			}
			set
			{
				_animateEnd = value;
			}
		}

		public float TotalTime
		{
			get
			{
				return _animateTime;
			}
		}

		public float ElapsedTime
		{
			get
			{
				return Time.time - _animateStartTime;
			}
		}

		public SHSBrawlerStatusAlertAnimator(SHSBrawlerStatusAlert animateAlert)
		{
			_animateAlert = animateAlert;
		}

		public void Update()
		{
			UpdateAnimation();
			UpdateBlink();
		}

		public bool StartAnimation(float animateDelay, float animateTime, Vector2 animateEnd, bool animateLinear)
		{
			if (_animateAlert == null)
			{
				return false;
			}
			_animateTime = animateTime;
			_animateStartTime = Time.time + animateDelay;
			_animateStart = _animateAlert.Offset;
			_animateEnd = animateEnd;
			_animateLinear = animateLinear;
			return true;
		}

		public void StopAnimation()
		{
			_animateStartTime = 0f;
			_animateTime = 0f;
		}

		public void StartBlink(float blinkTime)
		{
			_blinkPeriodStartTime = Time.time;
			_blinkTime = blinkTime;
			Blink();
		}

		public void StopBlink()
		{
			_blinkPeriodStartTime = 0f;
			_blinkStartTime = 0f;
			if (_animateAlert != null)
			{
				_animateAlert.IsVisible = true;
			}
		}

		public void Blink()
		{
			_blinkStartTime = Time.time;
			if (_animateAlert != null)
			{
				_animateAlert.IsVisible = !_animateAlert.IsVisible;
			}
		}

		public bool DoneAnimation()
		{
			return Time.time - _animateStartTime > _animateTime;
		}

		public bool IsAnimating()
		{
			return _animateStartTime > 0f;
		}

		public bool IsBlinking()
		{
			return _blinkStartTime > 0f;
		}

		public bool DoneBlinkPeriod()
		{
			return Time.time - _blinkPeriodStartTime >= BlinkPeriod;
		}

		public bool DoneBlink()
		{
			return Time.time - _blinkStartTime >= _blinkTime;
		}

		private void UpdateAnimation()
		{
			if (IsAnimating() && _animateAlert != null && !DoneAnimation() && !(Time.time < _animateStartTime))
			{
				Vector2 offset;
				if (_animateLinear)
				{
					offset = _animateStart + (_animateEnd - _animateStart) * (ElapsedTime / TotalTime);
				}
				else
				{
					float d = Mathf.Pow(ElapsedTime / TotalTime, 3f);
					offset = _animateStart + (_animateEnd - _animateStart) * d;
				}
				float min = Mathf.Min(_animateStart.x, _animateEnd.x);
				float max = Mathf.Max(_animateStart.x, _animateEnd.x);
				float min2 = Mathf.Min(_animateStart.y, _animateEnd.y);
				float max2 = Mathf.Max(_animateStart.y, _animateEnd.y);
				offset.x = Mathf.Clamp(offset.x, min, max);
				offset.y = Mathf.Clamp(offset.y, min2, max2);
				_animateAlert.Offset = offset;
			}
		}

		private void UpdateBlink()
		{
			if (_animateAlert != null && IsBlinking())
			{
				if (DoneBlinkPeriod())
				{
					StartBlink(_blinkTime / 2f);
				}
				if (DoneBlink())
				{
					Blink();
				}
			}
		}
	}

	public static readonly Vector2 AlertMessageWindowSize = new Vector2(238f, 118f);

	public static readonly Vector2 AlertHeroUpMessageWindowSize = new Vector2(204f, 142f);

	public static readonly float CenterAlertWaitTime = 0.05f;

	public static readonly float MoveAlertPathTime = 0.5f;

	public static readonly float StackAlertMoveDelay = 0.1f;

	public static readonly float FadeHeroAlertFadeTime = 0.28f;

	public static readonly float MoveAlertFinalScale = 0.5f;

	public static readonly float AlertMessageHeadOffset = 25f;

	private List<SHSBrawlerStatusAlert> _alertCenterQueue;

	private List<SHSBrawlerStatusAlert> _alertMoveQueue;

	private List<SHSBrawlerStatusAlert> _alertStackList;

	private List<SHSBrawlerStatusAlert> _alertStorage;

	private List<int> _alertToRemoveList;

	private bool _poweredUp;

	public override void Update()
	{
		base.Update();
		if (_alertToRemoveList != null)
		{
			List<int> list = null;
			foreach (int alertToRemove in _alertToRemoveList)
			{
				SHSBrawlerStatusAlert statusAlert = GetStatusAlert(alertToRemove);
				if (statusAlert == null || statusAlert.status == SHSBrawlerStatusAlert.AlertStatus.Stack)
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(alertToRemove);
					RemoveStatusAlert(statusAlert);
				}
			}
			if (list != null)
			{
				foreach (int item in list)
				{
					_alertToRemoveList.Remove(item);
				}
			}
		}
	}

	public void PushStatusAlert(int alertId, string alertTexture)
	{
		SHSBrawlerStatusAlert statusAlert = GetStatusAlert(alertTexture, false);
		if (statusAlert == null)
		{
			return;
		}
		statusAlert.AlertId = alertId;
		QueueCenterStatusAlert(statusAlert);
		SHSBrawlerStatusAlert statusAlertTarget = GetStatusAlertTarget("brawler_bundle|" + alertTexture);
		if (statusAlertTarget != null && statusAlertTarget != statusAlert)
		{
			statusAlert.TargetAlert(statusAlertTarget);
			if (InToRemoveList(statusAlertTarget))
			{
				_alertToRemoveList.Remove(statusAlertTarget.AlertId);
			}
			statusAlertTarget.StopBlink();
		}
	}

	public void RemoveStatusAlert(int alertId)
	{
		SHSBrawlerStatusAlert statusAlert = GetStatusAlert(alertId);
		if (statusAlert != null && !statusAlert.IsTarget())
		{
			if (_alertToRemoveList == null)
			{
				_alertToRemoveList = new List<int>();
			}
			_alertToRemoveList.Add(alertId);
		}
	}

	public void CountdownStatusAlert(int alertId)
	{
		SHSBrawlerStatusAlert statusAlert = GetStatusAlert(alertId);
		if (statusAlert != null && !statusAlert.IsTarget())
		{
			statusAlert.StartBlink();
		}
	}

	public void ChangePowerState(bool powerUp)
	{
		if (powerUp && !_poweredUp)
		{
			QueueCenterStatusAlert(GetStatusAlert("L_brawler_HUD_hero_up_text", true));
		}
		_poweredUp = powerUp;
	}

	public void SetPowerState(bool powerUp)
	{
		_poweredUp = powerUp;
	}

	public void SuppressPowerState(bool suppress)
	{
		SuppressHeroUpAlert(_alertCenterQueue, suppress);
		SuppressHeroUpAlert(_alertMoveQueue, suppress);
	}

	public void ShowCenterStatusAlert()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		if (_alertCenterQueue != null && _alertCenterQueue.Count != 0)
		{
			SHSBrawlerStatusAlert sHSBrawlerStatusAlert = _alertCenterQueue[0];
			if (sHSBrawlerStatusAlert != null)
			{
				Add(sHSBrawlerStatusAlert);
				AnimClip animClip = SHSAnimations.Generic.AnimationBounceTransitionIn(sHSBrawlerStatusAlert.Size, 1f, sHSBrawlerStatusAlert);
				animClip.OnFinished += (Action)(object)new Action(PopCenterStatusAlert);
				base.AnimationPieceManager.Add(animClip);
			}
		}
	}

	public void PopCenterStatusAlert()
	{
		QueueMoveStatusAlert(DequeueCenterStatusAlert());
	}

	public void PopMoveStatusAlert()
	{
		if (_alertMoveQueue == null || _alertMoveQueue.Count <= 0)
		{
			return;
		}
		SHSBrawlerStatusAlert sHSBrawlerStatusAlert = _alertMoveQueue[0];
		if (sHSBrawlerStatusAlert != null)
		{
			if (sHSBrawlerStatusAlert.IsHeroUp)
			{
				StoreStatusAlert(DequeueMoveStatusAlert());
			}
			else if (!sHSBrawlerStatusAlert.IsMoving)
			{
				PopMoveAlertOntoStack();
			}
			else
			{
				sHSBrawlerStatusAlert.onFinishMove = (SHSBrawlerStatusAlert.OnFinishMove)Delegate.Combine(sHSBrawlerStatusAlert.onFinishMove, new SHSBrawlerStatusAlert.OnFinishMove(PopMoveAlertOntoStack));
			}
		}
	}

	public void ClearStatusAlerts()
	{
		ClearStatusAlertList(_alertCenterQueue);
		ClearStatusAlertList(_alertMoveQueue);
		ClearStatusAlertList(_alertStackList);
		if (_alertToRemoveList != null)
		{
			_alertToRemoveList.Clear();
		}
	}

	private SHSBrawlerStatusAlert GetStatusAlert(string alertTexture, bool heroUp)
	{
		alertTexture = "brawler_bundle|" + alertTexture;
		SHSBrawlerStatusAlert sHSBrawlerStatusAlert = null;
		if (_alertStorage != null && _alertStorage.Count > 0)
		{
			sHSBrawlerStatusAlert = _alertStorage[0];
			sHSBrawlerStatusAlert.SetStatusAlertBackground(alertTexture, heroUp);
			_alertStorage.Remove(sHSBrawlerStatusAlert);
		}
		else
		{
			sHSBrawlerStatusAlert = new SHSBrawlerStatusAlert(alertTexture, heroUp);
		}
		return sHSBrawlerStatusAlert;
	}

	private SHSBrawlerStatusAlert GetStatusAlert(int alertId)
	{
		SHSBrawlerStatusAlert sHSBrawlerStatusAlert = null;
		sHSBrawlerStatusAlert = GetStatusAlert(alertId, _alertStackList);
		if (sHSBrawlerStatusAlert != null)
		{
			return sHSBrawlerStatusAlert;
		}
		sHSBrawlerStatusAlert = GetStatusAlert(alertId, _alertMoveQueue);
		if (sHSBrawlerStatusAlert != null)
		{
			return sHSBrawlerStatusAlert;
		}
		return GetStatusAlert(alertId, _alertCenterQueue);
	}

	private SHSBrawlerStatusAlert GetStatusAlertTarget(string alertTexture)
	{
		SHSBrawlerStatusAlert sHSBrawlerStatusAlert = null;
		sHSBrawlerStatusAlert = GetStatusAlertTarget(alertTexture, _alertStackList);
		if (sHSBrawlerStatusAlert != null)
		{
			return sHSBrawlerStatusAlert;
		}
		sHSBrawlerStatusAlert = GetStatusAlertTarget(alertTexture, _alertMoveQueue);
		if (sHSBrawlerStatusAlert != null)
		{
			return sHSBrawlerStatusAlert;
		}
		return GetStatusAlertTarget(alertTexture, _alertCenterQueue);
	}

	private SHSBrawlerStatusAlert GetStatusAlert(int alertId, List<SHSBrawlerStatusAlert> alertList)
	{
		if (alertList != null)
		{
			foreach (SHSBrawlerStatusAlert alert in alertList)
			{
				if (alert != null && alert.AlertId == alertId)
				{
					return alert;
				}
			}
		}
		return null;
	}

	private SHSBrawlerStatusAlert GetStatusAlertTarget(string alertTexture, List<SHSBrawlerStatusAlert> alertList)
	{
		if (alertList != null)
		{
			foreach (SHSBrawlerStatusAlert alert in alertList)
			{
				if (alert != null && alert.AlertBackgroundTexture == alertTexture)
				{
					return alert;
				}
			}
		}
		return null;
	}

	private void StoreStatusAlert(SHSBrawlerStatusAlert alert)
	{
		if (alert != null)
		{
			alert.ClearAlert();
			alert.AlertId = -1;
			alert.status = SHSBrawlerStatusAlert.AlertStatus.None;
			if (_alertStorage == null)
			{
				_alertStorage = new List<SHSBrawlerStatusAlert>();
			}
			_alertStorage.Add(alert);
			Remove(alert);
		}
	}

	private void QueueCenterStatusAlert(SHSBrawlerStatusAlert alert)
	{
		if (alert != null)
		{
			if (_alertCenterQueue == null)
			{
				_alertCenterQueue = new List<SHSBrawlerStatusAlert>();
			}
			if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
			{
				Transform heroHead = Utils.FindNodeInChildren(GameController.GetController().LocalPlayer.transform, "Head", true);
				alert.TargetHero(GameController.GetController().LocalPlayer.transform, heroHead);
			}
			else
			{
				alert.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.BottomMiddle, OffsetType.Percentage, new Vector2(0.5f, 0.5f));
			}
			alert.status = SHSBrawlerStatusAlert.AlertStatus.Center;
			_alertCenterQueue.Add(alert);
			if (_alertCenterQueue.Count == 1)
			{
				ShowCenterStatusAlert();
			}
		}
	}

	private void QueueMoveStatusAlert(SHSBrawlerStatusAlert alert)
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		if (alert != null)
		{
			if (_alertMoveQueue == null)
			{
				_alertMoveQueue = new List<SHSBrawlerStatusAlert>();
			}
			if (alert.IsArrow())
			{
				alert.FlyToTarget();
			}
			else
			{
				alert.FlyToStack(GetStackTopOffset());
			}
			Vector2 size = alert.Size;
			float x = size.x;
			Vector2 alertMessageWindowSize = AlertMessageWindowSize;
			AnimClip pieceOne = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(x, alertMessageWindowSize.x * MoveAlertFinalScale, MoveAlertPathTime), alert);
			Vector2 size2 = alert.Size;
			float y = size2.y;
			Vector2 alertMessageWindowSize2 = AlertMessageWindowSize;
			AnimClip animClip = pieceOne ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(y, alertMessageWindowSize2.y * MoveAlertFinalScale, MoveAlertPathTime), alert);
			if (alert.IsHeroUp)
			{
				animClip ^= SHSAnimations.Generic.AnimationFadeTransitionOut(0f, 1f, FadeHeroAlertFadeTime, alert);
			}
			animClip.OnFinished += (Action)(object)new Action(PopMoveStatusAlert);
			base.AnimationPieceManager.Add(animClip);
			if (alert.IsArrow())
			{
				animClip = SHSAnimations.Generic.AnimationFadeTransitionOut(0f, 1f, MoveAlertPathTime, alert.Target);
				base.AnimationPieceManager.Add(animClip);
			}
			alert.TargetHero(null, null);
			alert.status = SHSBrawlerStatusAlert.AlertStatus.Move;
			_alertMoveQueue.Add(alert);
			if (!alert.IsHeroUp)
			{
				UpdateStatusAlertStack();
			}
		}
	}

	private void PopMoveAlertOntoStack()
	{
		AddStackStatusAlert(DequeueMoveStatusAlert());
	}

	private void AddStackStatusAlert(SHSBrawlerStatusAlert alert)
	{
		if (alert == null)
		{
			return;
		}
		if (_alertStackList == null)
		{
			_alertStackList = new List<SHSBrawlerStatusAlert>();
		}
		int index = 0;
		if (alert.IsArrow())
		{
			SHSBrawlerStatusAlert target = alert.Target;
			int num = _alertStackList.IndexOf(target);
			if (num >= 0 && num < _alertStackList.Count)
			{
				index = num;
			}
		}
		alert.status = SHSBrawlerStatusAlert.AlertStatus.Stack;
		_alertStackList.Insert(index, alert);
		if (alert.IsArrow())
		{
			SHSBrawlerStatusAlert target2 = alert.Target;
			alert.TargetAlert(null);
			alert.DrawFire(target2);
			if (alert.IsTarget() && InToRemoveList(alert))
			{
				_alertToRemoveList.Remove(alert.AlertId);
			}
			RemoveStatusAlert(target2);
		}
	}

	private SHSBrawlerStatusAlert DequeueCenterStatusAlert()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		SHSBrawlerStatusAlert result = DequeueStatusAlert(_alertCenterQueue);
		if (_alertCenterQueue.Count > 0)
		{
			AnimClip animClip = SHSAnimations.Generic.Wait(CenterAlertWaitTime);
			animClip.OnFinished += (Action)(object)new Action(ShowCenterStatusAlert);
			base.AnimationPieceManager.Add(animClip);
		}
		return result;
	}

	private SHSBrawlerStatusAlert DequeueMoveStatusAlert()
	{
		return DequeueStatusAlert(_alertMoveQueue);
	}

	private SHSBrawlerStatusAlert DequeueStatusAlert(List<SHSBrawlerStatusAlert> alertList)
	{
		if (alertList == null || alertList.Count == 0)
		{
			return null;
		}
		SHSBrawlerStatusAlert result = alertList[0];
		alertList.RemoveAt(0);
		return result;
	}

	private void RemoveStatusAlert(SHSBrawlerStatusAlert alert)
	{
		if (alert != null)
		{
			List<SHSBrawlerStatusAlert> list = null;
			switch (alert.status)
			{
			case SHSBrawlerStatusAlert.AlertStatus.Center:
				list = _alertCenterQueue;
				break;
			case SHSBrawlerStatusAlert.AlertStatus.Move:
				list = _alertMoveQueue;
				break;
			case SHSBrawlerStatusAlert.AlertStatus.Stack:
				list = _alertStackList;
				break;
			}
			if (list != null)
			{
				list.Remove(alert);
			}
			if (alert.IsMoving)
			{
				alert.StopMove();
			}
			alert.StopBlink();
			StoreStatusAlert(alert);
			UpdateStatusAlertStack();
		}
	}

	private void SuppressHeroUpAlert(List<SHSBrawlerStatusAlert> alertList, bool suppress)
	{
		if (alertList != null)
		{
			foreach (SHSBrawlerStatusAlert alert in alertList)
			{
				if (alert != null && alert.IsHeroUp)
				{
					alert.IsVisible = !suppress;
				}
			}
		}
	}

	private Vector2 GetStackTopOffset()
	{
		float num = 10f;
		float num2 = 130f;
		Vector2 alertMessageWindowSize = AlertMessageWindowSize;
		float num3 = alertMessageWindowSize.x / 2f * MoveAlertFinalScale + num;
		Vector2 rectSize = base.RectSize;
		float x = 1f - num3 / rectSize.x;
		Vector2 rectSize2 = base.RectSize;
		return new Vector2(x, 1f - num2 / rectSize2.y);
	}

	private void UpdateStatusAlertStack()
	{
		if (_alertStackList != null && _alertStackList.Count > 0)
		{
			Vector2 stackTopOffset = GetStackTopOffset();
			int num = 0;
			if (_alertMoveQueue != null && _alertMoveQueue.Count > 0)
			{
				foreach (SHSBrawlerStatusAlert item in _alertMoveQueue)
				{
					if (item != null && !item.IsHeroUp && !item.IsArrow())
					{
						num++;
					}
				}
				float y = stackTopOffset.y;
				float num2 = num;
				Vector2 size = _alertStackList[0].Size;
				float y2 = size.y;
				Vector2 rectSize = base.RectSize;
				stackTopOffset.y = y - num2 * (y2 / rectSize.y);
			}
			float num3 = 0f;
			foreach (SHSBrawlerStatusAlert alertStack in _alertStackList)
			{
				if (alertStack != null)
				{
					if (alertStack.IsMoving)
					{
						Vector2 offset = alertStack.Offset;
						if (Mathf.Abs(offset.y - stackTopOffset.y) <= float.Epsilon)
						{
							alertStack.StopMove();
						}
						else
						{
							Vector2 destination = alertStack.Destination;
							if (Mathf.Abs(destination.y - stackTopOffset.y) > float.Epsilon)
							{
								alertStack.StopMove();
								alertStack.StartMove(stackTopOffset, num3, MoveAlertPathTime, false);
								num3 += StackAlertMoveDelay;
							}
						}
					}
					else
					{
						Vector2 offset2 = alertStack.Offset;
						if (Mathf.Abs(offset2.y - stackTopOffset.y) > float.Epsilon)
						{
							alertStack.StartMove(stackTopOffset, num3, MoveAlertPathTime, false);
							num3 += StackAlertMoveDelay;
						}
					}
					float y3 = stackTopOffset.y;
					Vector2 size2 = alertStack.Size;
					float y4 = size2.y;
					Vector2 rectSize2 = base.RectSize;
					stackTopOffset.y = y3 - y4 / rectSize2.y;
				}
			}
		}
	}

	private bool InToRemoveList(SHSBrawlerStatusAlert alert)
	{
		if (alert == null || _alertToRemoveList == null)
		{
			return false;
		}
		return _alertToRemoveList.Contains(alert.AlertId);
	}

	private void ClearStatusAlertList(List<SHSBrawlerStatusAlert> alertList)
	{
		for (SHSBrawlerStatusAlert sHSBrawlerStatusAlert = DequeueStatusAlert(alertList); sHSBrawlerStatusAlert != null; sHSBrawlerStatusAlert = DequeueStatusAlert(alertList))
		{
			StoreStatusAlert(sHSBrawlerStatusAlert);
		}
	}
}
