using System;
using UnityEngine;

public class SHSInvitationWindow : GUINotificationWindow
{
	internal class StateBaseState : IDisposable, IShsState
	{
		protected float startTime;

		private bool inVertMotion;

		private bool firstUpdate;

		internal SHSInvitationViewWindow owner;

		public StateBaseState(SHSInvitationWindow owner)
		{
			this.owner = (SHSInvitationViewWindow)owner;
		}

		public virtual void Enter(Type previousState)
		{
			startTime = Time.time;
		}

		public virtual void Update()
		{
			if (!firstUpdate)
			{
				owner.SetCustomSize(InitialScale);
				firstUpdate = true;
			}
			if (inVertMotion)
			{
				UpdateSlide();
			}
			else if (owner.currentIndex != owner.index)
			{
				inVertMotion = true;
			}
		}

		public void UpdateSlide()
		{
			Vector2 offset = owner.Offset;
			float num = offset.y;
			float num2 = VerticalInviteTop + VerticalInviteSpacing * owner.index;
			float num3 = Math.Abs(owner.currentIndex - owner.index) * VerticalInviteSpacing;
			float num4 = Math.Max(10f, VerticalTravelSpeed * Math.Abs(num2 - num) / num3);
			try
			{
				num += num4 * Time.deltaTime * (float)Math.Sign(num2 - num);
			}
			catch (Exception ex)
			{
				CspUtils.DebugLog("NaN ERROR:" + num2 + ":" + num + ": " + ex.Message);
			}
			if (Math.Abs(num - num2) < 3f)
			{
				num = num2;
				owner.currentIndex = owner.index;
				inVertMotion = false;
			}
			SHSInvitationViewWindow sHSInvitationViewWindow = owner;
			Vector2 offset2 = owner.Offset;
			sHSInvitationViewWindow.Offset = new Vector2(offset2.x, num);
		}

		public virtual void Leave(Type nextState)
		{
		}

		public virtual void Dispose()
		{
			owner = null;
		}
	}

	internal class DisplayBaseState : IDisposable, IShsState
	{
		protected float startTime;

		internal SHSInvitationWindow owner;

		public DisplayBaseState(SHSInvitationWindow owner)
		{
			this.owner = owner;
		}

		public virtual void Enter(Type previousState)
		{
			startTime = Time.time;
		}

		public virtual void Update()
		{
		}

		public virtual void Leave(Type nextState)
		{
		}

		public virtual void Dispose()
		{
			owner = null;
		}
	}

	internal class ExtendingState : DisplayBaseState
	{
		private float extendTime = 0.5f;

		private float inertOffset;

		public ExtendingState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			owner.DisplayFsm.GotoState<ExtendedState>();
		}

		public override void Update()
		{
			base.Update();
			float num = inertOffset - inertOffset * (Time.time - startTime) / extendTime;
			SHSInvitationWindow owner = base.owner;
			Vector2 offset = base.owner.Offset;
			owner.Offset = new Vector2(num, offset.y);
			if (num <= 0f)
			{
				base.owner.DisplayFsm.GotoState<ExtendedState>();
			}
		}
	}

	internal class ExtendedState : DisplayBaseState
	{
		public ExtendedState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
		}

		public override void Update()
		{
			base.Update();
		}
	}

	internal class RetractingState : DisplayBaseState
	{
		private float retractTime = 0.5f;

		private float inertOffset;

		public RetractingState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			Vector2 size = owner.Size;
			float x = size.x;
			Vector2 size2 = owner.Size;
			inertOffset = x - size2.x;
		}

		public override void Update()
		{
			base.Update();
			float num = inertOffset * (Time.time - startTime) / retractTime;
			SHSInvitationWindow owner = base.owner;
			Vector2 offset = base.owner.Offset;
			owner.Offset = new Vector2(num, offset.y);
			if (num >= inertOffset)
			{
				base.owner.DisplayFsm.GotoState<RetractedState>();
			}
		}
	}

	internal class RetractedState : DisplayBaseState
	{
		private bool offsetSet;

		public RetractedState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			offsetSet = false;
		}

		public override void Update()
		{
			base.Update();
			if (!offsetSet)
			{
				SHSInvitationWindow owner = base.owner;
				Vector2 size = base.owner.Size;
				float x = size.x;
				Vector2 size2 = base.owner.Size;
				float x2 = x - size2.x;
				Vector2 offset = base.owner.Offset;
				owner.Offset = new Vector2(x2, offset.y);
				offsetSet = true;
			}
		}
	}

	internal class FadingInState : StateBaseState
	{
		private float fadeTime = 0.5f;

		public FadingInState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Update()
		{
			base.Update();
			if (Time.time - startTime > fadeTime)
			{
				owner.Alpha = 1f;
				owner.StateFsm.GotoState<ActiveState>();
			}
			else
			{
				owner.Alpha = (Time.time - startTime) / fadeTime;
			}
		}
	}

	internal class HiddenState : StateBaseState
	{
		public HiddenState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Update()
		{
			base.Update();
			if (owner.IsVisible)
			{
				owner.StateFsm.GotoState<FadingInState>();
			}
		}
	}

	internal class ActiveState : StateBaseState
	{
		public ActiveState(SHSInvitationWindow owner)
			: base(owner)
		{
		}
	}

	internal class ClosingState : StateBaseState
	{
		private float fadeTime = 0.25f;

		public ClosingState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			if (owner.cancelFlag && owner.cancelReason != InvitationCancelReason.PlayerCanceled)
			{
				fadeTime = 1.5f;
			}
		}

		public override void Update()
		{
			base.Update();
			if (Time.time - startTime > fadeTime)
			{
				owner.Alpha = 0f;
				owner.StateFsm.GotoState<ClosedState>();
			}
			else
			{
				owner.Alpha = 1f - (Time.time - startTime) / fadeTime;
			}
		}
	}

	internal class ClosedState : StateBaseState
	{
		public ClosedState(SHSInvitationWindow owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			owner.Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
			owner.Hide();
			if (((SHSInvitationWindow)owner).OnClosedCallback != null)
			{
				((SHSInvitationWindow)owner).OnClosedCallback();
			}
		}
	}

	public delegate void OnClosedCallbackDelegate();

	public static readonly int ViewableInviteCount = 3;

	public static readonly int VerticalInviteSpacing = 50;

	public static readonly int VerticalInviteTop = 150;

	public static readonly float VerticalTravelSpeed = 500f;

	public static readonly float InvitationClientTimeout = 60f;

	public static readonly float InitialScale = 1f;

	public static readonly float FocusedScale = 1f;

	protected bool cancelFlag;

	protected InvitationCancelReason cancelReason;

	protected float invitationServerTime;

	protected ShsFSM displayFsm;

	protected ShsFSM stateFsm;

	protected bool timedOut;

	private float customScale;

	private int index;

	private int currentIndex = 3;

	private bool playedSFX;

	public ShsFSM StateFsm
	{
		get
		{
			return stateFsm;
		}
	}

	public ShsFSM DisplayFsm
	{
		get
		{
			return displayFsm;
		}
	}

	public float CustomScale
	{
		get
		{
			return customScale;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public override bool IsVisible
	{
		get
		{
			return base.IsVisible;
		}
		set
		{
			base.IsVisible = value;
		}
	}

	public event OnClosedCallbackDelegate OnClosedCallback;

	public SHSInvitationWindow()
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		Offset = new Vector2(0f, ViewableInviteCount * VerticalInviteSpacing + VerticalInviteTop);
		currentIndex = ViewableInviteCount;
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.KeepAlive;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		Alpha = 0f;
		MouseOver += SHSInvitationWindow_MouseOver;
		MouseOut += SHSInvitationWindow_MouseOut;
	}

	public virtual void SetCustomSize(float scale)
	{
		customScale = scale;
	}

	private void SHSInvitationWindow_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		GUIManager.Instance.NotificationManager.SendNotificationToPriorPosition(this);
		if (this is SHSInvitationViewWindow)
		{
			((SHSInvitationViewWindow)this).SetCustomSize(InitialScale);
		}
	}

	private void SHSInvitationWindow_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		GUIManager.Instance.NotificationManager.SendNotificationToFront(this);
		if (this is SHSInvitationViewWindow)
		{
			((SHSInvitationViewWindow)this).SetCustomSize(FocusedScale);
		}
	}

	public override void Update()
	{
		base.Update();
		if (!timedOut && Time.time - timeStarted > InvitationClientTimeout)
		{
			timedOut = true;
			stateFsm.GotoState<ClosingState>();
		}
		IsVisible = (index < ViewableInviteCount);
		if (IsVisible)
		{
			displayFsm.Update();
			stateFsm.Update();
		}
	}

	protected void SetInvitationInitialState()
	{
		if (displayFsm == null)
		{
			displayFsm = new ShsFSM();
			displayFsm.AddState(new RetractedState(this));
			displayFsm.AddState(new RetractingState(this));
			displayFsm.AddState(new ExtendingState(this));
			displayFsm.AddState(new ExtendedState(this));
		}
		if (stateFsm == null)
		{
			stateFsm = new ShsFSM();
			stateFsm.AddState(new HiddenState(this));
			stateFsm.AddState(new FadingInState(this));
			stateFsm.AddState(new ActiveState(this));
			stateFsm.AddState(new ClosingState(this));
			stateFsm.AddState(new ClosedState(this));
		}
		displayFsm.GotoState<ExtendingState>();
		stateFsm.GotoState<HiddenState>();
	}

	public void PerformCloseProcess()
	{
		stateFsm.GotoState<ClosingState>();
	}

	protected virtual void HandleInvitationResponse(bool accept)
	{
		throw new Exception("Subclass must implement response handling of invitations.");
	}

	public virtual void OnCancelled(InvitationCancelReason reason)
	{
		cancelFlag = true;
		cancelReason = reason;
	}

	public override void OnShow()
	{
		if (!playedSFX)
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("popup_invitation"));
			playedSFX = true;
		}
	}
}
