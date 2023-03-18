using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSequence : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void OnSequenceEvent(EffectSequence seq, EventEffect effect);

	public delegate void OnSequenceDone(EffectSequence seq);

	public delegate UnityEngine.Object OnInstantiateParticlePrefab(UnityEngine.Object particlePrefab);

	public float TotalLifetime = -1f;

	public bool AutoStart;

	public bool AttachToParent = true;

	public bool Looping;

	public bool FadeParticlesOnEnd;

	public bool FadeSoundsOnEnd;

	public bool FadeToIdleOnEnd;

	[HideInInspector]
	public bool AnimationInterrupted;

	public ParticleEffect[] Particles;

	public SoundEffect[] Sounds;

	public AnimationEffect[] Animations;

	public EventEffect[] Events;

	public GeneralEffect[] Generals;

	[HideInInspector]
	public OnInstantiateParticlePrefab onParticleInstantiate;

	protected bool bInitialized;

	protected GameObject owningObject;

	protected Animation animationComponent;

	protected OnSequenceDone onSequenceDone;

	protected OnSequenceEvent onSequenceEvent;

	protected List<ParticleObject> createdParticles;

	protected List<SoundEffect> playingSFX;

	protected bool particlesDisabled;

	protected bool sfxDisabled;

	protected bool terminated;

	protected float activeTime;

	protected float effectLifetime = -1f;

	protected bool effectStarted;

	protected bool paused;

	protected bool unloading;

	protected EffectScope originatorScope;

	protected bool localInstigator;

	protected bool scaleEffectsToOwner = true;

	protected Transform owningObjectTransform;

	private float timeScale = 1f;

	private float storedTimeOffset;

	private FacialAnimation _cachedFace;

	public bool Initialized
	{
		get
		{
			return bInitialized;
		}
	}

	public GameObject Owner
	{
		get
		{
			return owningObject;
		}
	}

	public bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
			if (!effectStarted)
			{
				return;
			}
			if (animationComponent != null)
			{
				float speed = 1f;
				if (value)
				{
					speed = 0f;
				}
				Animation[] components = Utils.GetComponents<Animation>(animationComponent.gameObject, Utils.SearchChildren);
				if (components != null)
				{
					Animation[] array = components;
					foreach (Animation animation in array)
					{
						foreach (AnimationState item in animation)
						{
							item.speed = speed;
						}
					}
				}
			}
			foreach (ParticleObject createdParticle in createdParticles)
			{
				if (createdParticle != null && !(createdParticle.Instance == null))
				{
					ParticleEmitter[] components2 = Utils.GetComponents<ParticleEmitter>(createdParticle.Instance, Utils.SearchChildren);
					ParticleEmitter[] array2 = components2;
					foreach (ParticleEmitter particleEmitter in array2)
					{
						particleEmitter.emit = !value;
						if (value)
						{
							particleEmitter.ClearParticles();
						}
					}
				}
			}
			ShsAudioSource[] components3 = Utils.GetComponents<ShsAudioSource>(base.gameObject, Utils.SearchChildren);
			if (components3 == null)
			{
				return;
			}
			ShsAudioSource[] array3 = components3;
			foreach (ShsAudioSource shsAudioSource in array3)
			{
				if (value)
				{
					shsAudioSource.Pause();
				}
				else
				{
					shsAudioSource.Play();
				}
			}
		}
	}

	public float Lifetime
	{
		get
		{
			return effectLifetime;
		}
		set
		{
			effectLifetime = value;
		}
	}

	public bool ScaleEffectsToOwner
	{
		get
		{
			return scaleEffectsToOwner;
		}
		set
		{
			scaleEffectsToOwner = value;
		}
	}

	public float TimeScale
	{
		get
		{
			return timeScale;
		}
		set
		{
			timeScale = value;
		}
	}

	public float TimeOffset
	{
		get
		{
			if (effectStarted)
			{
				return activeTime;
			}
			return storedTimeOffset;
		}
		set
		{
			JumpTo(value);
		}
	}

	public FacialAnimation Face
	{
		get
		{
			if (_cachedFace == null && owningObject != null)
			{
				_cachedFace = owningObject.GetComponent<FacialAnimation>();
			}
			return _cachedFace;
		}
	}

	public bool PersistFace
	{
		get
		{
			if (Face != null)
			{
				return Face.PersistOnAnimEnd;
			}
			return false;
		}
		set
		{
			if (Face != null)
			{
				Face.PersistOnAnimEnd = value;
			}
		}
	}

	public IEnumerable<IBaseEffect> Effects
	{
		get
		{
			if (Particles != null)
			{
				ParticleEffect[] particles = Particles;
				for (int i = 0; i < particles.Length; i++)
				{
					yield return particles[i];
				}
			}
			if (Generals != null)
			{
				GeneralEffect[] generals = Generals;
				for (int j = 0; j < generals.Length; j++)
				{
					yield return generals[j];
				}
			}
			if (Sounds != null)
			{
				SoundEffect[] sounds = Sounds;
				for (int k = 0; k < sounds.Length; k++)
				{
					yield return sounds[k];
				}
			}
			if (Animations != null)
			{
				AnimationEffect[] animations = Animations;
				for (int l = 0; l < animations.Length; l++)
				{
					yield return animations[l];
				}
			}
			if (Events != null)
			{
				EventEffect[] events = Events;
				for (int m = 0; m < events.Length; m++)
				{
					yield return events[m];
				}
			}
		}
	}

	public int NumberEffects
	{
		get
		{
			return Particles.Length + Generals.Length + Sounds.Length + Animations.Length + Events.Length;
		}
	}

	private void Start()
	{
		if (!AutoStart || (owningObject != null && !owningObject.active))
		{
			return;
		}
		if (!bInitialized)
		{
			GameObject parent = null;
			if (base.transform.root != null && base.transform.root != base.transform)
			{
				parent = base.transform.root.gameObject;
			}
			Initialize(parent, null, null);
		}
		StartSequence();
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		DestroyCreatedAttachedParticles();
		particlesDisabled = true;
		if (playingSFX != null)
		{
			playingSFX.RemoveAll(delegate(SoundEffect sound)
			{
				return sound.soundInstance == null;
			});
			foreach (SoundEffect item in playingSFX)
			{
				item.soundInstance.Pause();
			}
			sfxDisabled = true;
		}
	}

	private void OnEnable()
	{
		if (particlesDisabled)
		{
			if (!bInitialized)
			{
				Initialize(owningObject, onSequenceDone, onSequenceEvent);
			}
			RestartParticles();
			particlesDisabled = false;
		}
		if (sfxDisabled)
		{
			if (!bInitialized)
			{
				Initialize(owningObject, onSequenceDone, onSequenceEvent);
			}
			foreach (SoundEffect item in playingSFX)
			{
				StartCoroutine(ContinueSound(item));
			}
			sfxDisabled = false;
		}
	}

	public IBaseEffect AddEffect(Type effectType, int insertIndex)
	{
		object[] array = null;
		object[] array2 = null;
		if (effectType == typeof(ParticleEffect))
		{
			array = Particles;
			array2 = new ParticleEffect[Particles.Length + 1];
		}
		else if (effectType == typeof(SoundEffect))
		{
			array = Sounds;
			array2 = new SoundEffect[Sounds.Length + 1];
		}
		else if (effectType == typeof(AnimationEffect))
		{
			array = Animations;
			array2 = new AnimationEffect[Animations.Length + 1];
		}
		else if (effectType == typeof(EventEffect))
		{
			array = Events;
			array2 = new EventEffect[Events.Length + 1];
		}
		else if (effectType == typeof(GeneralEffect))
		{
			array = Generals;
			array2 = new GeneralEffect[Generals.Length + 1];
		}
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i];
		}
		IBaseEffect baseEffect = null;
		if (insertIndex == -1)
		{
			insertIndex = array2.Length - 1;
		}
		if (effectType == typeof(ParticleEffect))
		{
			baseEffect = (IBaseEffect)(array2[insertIndex] = new ParticleEffect());
			Particles = (array2 as ParticleEffect[]);
		}
		else if (effectType == typeof(SoundEffect))
		{
			baseEffect = (IBaseEffect)(array2[insertIndex] = new SoundEffect());
			Sounds = (array2 as SoundEffect[]);
		}
		else if (effectType == typeof(AnimationEffect))
		{
			baseEffect = (IBaseEffect)(array2[insertIndex] = new AnimationEffect());
			Animations = (array2 as AnimationEffect[]);
		}
		else if (effectType == typeof(EventEffect))
		{
			baseEffect = (IBaseEffect)(array2[insertIndex] = new EventEffect());
			Events = (array2 as EventEffect[]);
		}
		else if (effectType == typeof(GeneralEffect))
		{
			baseEffect = (IBaseEffect)(array2[insertIndex] = new GeneralEffect());
			Generals = (array2 as GeneralEffect[]);
		}
		baseEffect.SetTimeOffset(0f);
		if (baseEffect.GetAllowsLifetime())
		{
			baseEffect.SetLifetime(2f);
		}
		return baseEffect;
	}

	public void RemoveEffect(IBaseEffect baseEffect)
	{
		object[] array = null;
		if (baseEffect is ParticleEffect)
		{
			array = Particles;
		}
		else if (baseEffect is SoundEffect)
		{
			array = Sounds;
		}
		else if (baseEffect is AnimationEffect)
		{
			array = Animations;
		}
		else if (baseEffect is EventEffect)
		{
			array = Events;
		}
		else if (baseEffect is GeneralEffect)
		{
			array = Generals;
		}
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == baseEffect)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		if (baseEffect is ParticleEffect)
		{
			ParticleEffect[] array2 = new ParticleEffect[Particles.Length - 1];
			int num2 = 0;
			for (int j = 0; j < Particles.Length; j++)
			{
				if (Particles[j] != baseEffect)
				{
					array2[num2++] = Particles[j];
				}
			}
			Particles = array2;
		}
		else if (baseEffect is SoundEffect)
		{
			SoundEffect[] array3 = new SoundEffect[Sounds.Length - 1];
			int num3 = 0;
			for (int k = 0; k < Sounds.Length; k++)
			{
				if (Sounds[k] != baseEffect)
				{
					array3[num3++] = Sounds[k];
				}
			}
			Sounds = array3;
		}
		else if (baseEffect is AnimationEffect)
		{
			AnimationEffect[] array4 = new AnimationEffect[Animations.Length - 1];
			int num4 = 0;
			for (int l = 0; l < Animations.Length; l++)
			{
				if (Animations[l] != baseEffect)
				{
					array4[num4++] = Animations[l];
				}
			}
			Animations = array4;
		}
		else if (baseEffect is EventEffect)
		{
			EventEffect[] array5 = new EventEffect[Events.Length - 1];
			int num5 = 0;
			for (int m = 0; m < Events.Length; m++)
			{
				if (Events[m] != baseEffect)
				{
					array5[num5++] = Events[m];
				}
			}
			Events = array5;
		}
		else
		{
			if (!(baseEffect is GeneralEffect))
			{
				return;
			}
			GeneralEffect[] array6 = new GeneralEffect[Generals.Length - 1];
			int num6 = 0;
			for (int n = 0; n < Generals.Length; n++)
			{
				if (Generals[n] != baseEffect)
				{
					array6[num6++] = Generals[n];
				}
			}
			Generals = array6;
		}
	}

	public void SetParent(GameObject obj)
	{
		SetParent(obj, false);
	}

	public void SetParent(GameObject obj, bool forceSet)
	{
		owningObject = obj;
		animationComponent = (owningObject.GetComponentInChildren(typeof(Animation)) as Animation);
		if (!(base.transform.parent == null) && !forceSet)
		{
			return;
		}
		if (AttachToParent)
		{
			if (obj != null)
			{
				Utils.AttachGameObject(obj, base.gameObject);
			}
			else
			{
				base.transform.parent = null;
			}
		}
		else
		{
			base.transform.position += obj.transform.position;
			base.transform.rotation *= obj.transform.rotation;
		}
	}

	public void Initialize(GameObject parent, OnSequenceDone onDone, OnSequenceEvent onEvent)
	{
		if (bInitialized)
		{
			CspUtils.DebugLog("EffectSequence " + base.gameObject.name + " was initialized twice.  Is Auto Start set when its shouldn't be?");
			return;
		}
		bInitialized = true;
		onParticleInstantiate = UnityEngine.Object.Instantiate;
		onSequenceDone = onDone;
		onSequenceEvent = onEvent;
		if (parent != null)
		{
			SetParent(parent);
		}
		else if (base.transform.root != null)
		{
			owningObject = base.transform.root.gameObject;
		}
		createdParticles = new List<ParticleObject>();
		playingSFX = new List<SoundEffect>();
	}

	public void StartSequence()
	{
		activeTime = storedTimeOffset;
		storedTimeOffset = 0f;
		if (ScaleEffectsToOwner)
		{
			InitializeOwnerTransform();
		}
		bool looping;
		float lifetime;
		StartParticles(out looping, out lifetime);
		GeneralEffect[] generals = Generals;
		foreach (GeneralEffect generalEffect in generals)
		{
			generalEffect.started = false;
			if (originatorScope == EffectScope.Both || generalEffect.scope == EffectScope.Both || originatorScope == generalEffect.scope)
			{
				float lifetime2 = generalEffect.GetLifetime(null);
				if (lifetime2 <= 0f && generalEffect.IsLooping())
				{
					looping = true;
				}
				else
				{
					lifetime = Mathf.Max(lifetime, lifetime2 + generalEffect.TimeOffset);
				}
				if (generalEffect.TimeOffset <= activeTime)
				{
					StartCoroutine(StartGeneral(generalEffect));
				}
			}
		}
		SoundEffect[] sounds = Sounds;
		foreach (SoundEffect soundEffect in sounds)
		{
			soundEffect.started = false;
			if (originatorScope != 0 && soundEffect.scope != 0 && originatorScope != soundEffect.scope)
			{
				continue;
			}
			float lifetime3 = soundEffect.GetLifetime(null);
			if (lifetime3 == 0f)
			{
				if (soundEffect.SoundPrefab == null)
				{
					CspUtils.DebugLog("Sound sequence in " + base.gameObject.name + " has no prefab");
				}
				else
				{
					CspUtils.DebugLog("SoundSequence found bad sound lifetime: " + soundEffect.SoundPrefab.name);
				}
				continue;
			}
			if (lifetime3 < 0f)
			{
				looping = true;
			}
			else
			{
				lifetime = Mathf.Max(lifetime, lifetime3 + soundEffect.TimeOffset);
			}
			if (soundEffect.TimeOffset <= activeTime)
			{
				StartCoroutine(StartSound(soundEffect));
			}
		}
		if (animationComponent != null)
		{
			float num = 0f;
			AnimationEffect[] animations = Animations;
			foreach (AnimationEffect animationEffect in animations)
			{
				animationEffect.started = false;
				if (originatorScope != 0 && animationEffect.scope != 0 && originatorScope != animationEffect.scope)
				{
					continue;
				}
				float lifetime4 = animationEffect.GetLifetime(animationComponent.gameObject);
				if (lifetime4 > num)
				{
					num = lifetime4;
				}
				if (lifetime4 == 0f)
				{
					string gameObjectFullName = Utils.GetGameObjectFullName(animationComponent.gameObject);
					CspUtils.DebugLog("EffectSequence playing on <" + gameObjectFullName + "> found bad animation <" + animationEffect.Animation + ">");
					continue;
				}
				if (lifetime4 < 0f)
				{
					looping = true;
				}
				else
				{
					lifetime = Mathf.Max(lifetime, lifetime4 + animationEffect.TimeOffset);
				}
				if (animationEffect.TimeOffset <= activeTime)
				{
					StartCoroutine(StartAnimation(animationEffect));
				}
			}
			if (num > 0f && FadeToIdleOnEnd)
			{
				if (TotalLifetime > 0f)
				{
					StartCoroutine(PlayIdle(TotalLifetime));
				}
				else
				{
					StartCoroutine(PlayIdle(num));
				}
			}
		}
		EventEffect[] events = Events;
		foreach (EventEffect eventEffect in events)
		{
			eventEffect.started = false;
			if (originatorScope == EffectScope.Both || eventEffect.scope == EffectScope.Both || originatorScope == eventEffect.scope)
			{
				lifetime = Mathf.Max(lifetime, eventEffect.TimeOffset);
				if (eventEffect.TimeOffset <= activeTime)
				{
					StartCoroutine(StartEvent(eventEffect));
				}
			}
		}
		if (TotalLifetime > 0f)
		{
			lifetime = TotalLifetime;
		}
		else if (looping)
		{
			lifetime = -1f;
		}
		if (lifetime >= 0f)
		{
			Cleanup(lifetime);
		}
		effectStarted = true;
	}

	public void StopSequence(bool killParticles)
	{
		terminated = true;
		if (killParticles)
		{
			DestroyCreatedParticles();
		}
	}

	public void StopSequenceImmediately()
	{
		if (base.gameObject.name == "sandman_playable")
		{
			CspUtils.DebugLogWarning("STOPPING SANDMAN");
		}
		terminated = true;
		DestroyCreatedParticles();
		onParticleInstantiate = null;
		onSequenceDone = null;
		onSequenceEvent = null;
	}

	private void StartParticles(out bool looping, out float lifetime)
	{
		looping = false;
		lifetime = 0f;
		ParticleEffect[] particles = Particles;
		foreach (ParticleEffect particleEffect in particles)
		{
			particleEffect.started = false;
			float lifetime2 = particleEffect.GetLifetime(null);
			if (particleEffect.ParticlePrefab == null)
			{
				CspUtils.DebugLog("EffectSequence found bad particle prefab: " + owningObject.name);
				continue;
			}
			if (lifetime2 <= 0f)
			{
				looping = true;
			}
			else if (!particleEffect.DetachedParticlePermanent)
			{
				lifetime = Mathf.Max(lifetime, lifetime2 + particleEffect.TimeOffset);
			}
			else
			{
				lifetime = Mathf.Max(lifetime, particleEffect.TimeOffset);
			}
			if (particleEffect.TimeOffset <= activeTime)
			{
				StartCoroutine(StartParticle(particleEffect));
			}
		}
	}

	private void RestartParticles()
	{
		ParticleEffect[] particles = Particles;
		foreach (ParticleEffect particleEffect in particles)
		{
			if (particleEffect.minimumQuality <= GraphicsOptions.ModelQuality && (originatorScope == EffectScope.Both || particleEffect.scope == EffectScope.Both || originatorScope == particleEffect.scope))
			{
				float lifetime = particleEffect.GetLifetime(null);
				if (lifetime < 0f)
				{
					StartCoroutine(StartParticle(particleEffect));
				}
			}
		}
	}

	public void Cancel()
	{
		StopAllCoroutines();
		if (onSequenceDone != null)
		{
			onSequenceDone(this);
		}
		onSequenceEvent = null;
		onSequenceDone = null;
		if (FadeParticlesOnEnd)
		{
			StartParticleFadeOnEnd();
		}
		if (FadeSoundsOnEnd)
		{
			StartSoundFadeOnEnd();
		}
		base.gameObject.SendMessage("OnEffectSequenceCanceled", this, SendMessageOptions.DontRequireReceiver);
		PersistFace = false;
		if (FadeToIdleOnEnd && !AnimationInterrupted)
		{
			animationComponent.CrossFade("movement_idle");
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public float GetEventOffset(string eventName)
	{
		EventEffect[] events = Events;
		foreach (EventEffect eventEffect in events)
		{
			if (eventEffect.GetName() == eventName)
			{
				return eventEffect.GetTimeOffset();
			}
		}
		return -1f;
	}

	public static GameObject FindEffect(string effectName, GameObject parent)
	{
		Transform transform = Utils.FindNodeInChildren(parent.transform, effectName + "(Clone)");
		return (!(transform != null)) ? null : transform.gameObject;
	}

	public static void TransferEffect(string effectName, GameObject orgParent, GameObject newParent)
	{
		if (orgParent == null || newParent == null)
		{
			return;
		}
		GameObject gameObject = FindEffect(effectName, orgParent);
		if (!(gameObject != null))
		{
			return;
		}
		Utils.DetachGameObject(gameObject);
		Utils.AttachGameObject(newParent, gameObject);
		EffectSequence component = gameObject.GetComponent<EffectSequence>();
		if (component != null)
		{
			component.SetParent(newParent);
			if (component.createdParticles != null)
			{
				foreach (ParticleObject createdParticle in component.createdParticles)
				{
					if (createdParticle != null && !(createdParticle.Instance == null) && createdParticle.IsAttached)
					{
						GameObject instance = createdParticle.Instance;
						Transform parent = instance.transform.parent;
						if (parent == null)
						{
							CspUtils.DebugLog("original parent node not found for <" + instance.gameObject.name + ">");
						}
						else
						{
							Transform transform = Utils.FindNodeInChildren(newParent.transform, parent.gameObject.name);
							if (transform == null)
							{
								CspUtils.DebugLog("new parent <" + newParent.gameObject.name + "> does not have node <" + parent.gameObject.name + "> for particle <" + instance.gameObject.name + ">");
							}
							else
							{
								Utils.DetachGameObject(instance);
								Utils.AttachGameObject(transform.gameObject, instance);
							}
						}
					}
				}
			}
		}
	}

	protected void Cleanup(float lifetime)
	{
		ParticleEffect[] particles = Particles;
		foreach (ParticleEffect particleEffect in particles)
		{
			if (particleEffect.ParticlePrefab == null || !particleEffect.FadeOut || particleEffect.DetachedParticlePermanent)
			{
				continue;
			}
			ParticleEmitter particleEmitter = particleEffect.ParticlePrefab.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;
			if (particleEmitter != null)
			{
				float num = particleEffect.TimeOffset + particleEffect.Lifetime + particleEmitter.maxEnergy;
				if (num > lifetime)
				{
					lifetime = num;
				}
			}
			Component[] componentsInChildren = particleEffect.ParticlePrefab.GetComponentsInChildren(typeof(ParticleEmitter));
			Component[] array = componentsInChildren;
			for (int j = 0; j < array.Length; j++)
			{
				ParticleEmitter particleEmitter2 = (ParticleEmitter)array[j];
				float num2 = particleEffect.TimeOffset + particleEffect.Lifetime + particleEmitter2.maxEnergy;
				if (num2 > lifetime)
				{
					lifetime = num2;
				}
			}
		}
		effectLifetime = lifetime;
	}

	protected IEnumerator StartParticle(ParticleEffect particle)
	{
		if (particle.ParticlePrefab == null || particle.minimumQuality > GraphicsOptions.ModelQuality || (originatorScope != 0 && particle.scope != 0 && originatorScope != particle.scope) || (CardGameController.Instance != null && CardGameController.EffectBlacklist.ContainsKey(particle.ParticlePrefab.name)))
		{
			yield break;
		}
		particle.started = true;
		if (!terminated && particle.ParticlePrefab != null)
		{
			Transform attachNode = base.transform;
			bool isAttached = false;
			if (particle.AttachNodeName != string.Empty)
			{
				if (owningObject == null)
				{
					CspUtils.DebugLog("Un-parented EffectSequence <" + base.gameObject.name + "> cannot have particles that attach to a node <" + particle.AttachNodeName + ">!");
					yield break;
				}
				string nodeName = particle.AttachNodeName;
				attachNode = ResolveNode(nodeName);
				if (attachNode == null)
				{
					CspUtils.DebugLog("Unable to find attach node <" + nodeName + "> for particle at time offset <" + particle.TimeOffset + "> in EffectSequence <" + base.gameObject.name + ">.  Particle not run!");
					CspUtils.DebugLog("parent name is <" + owningObject.name + ">");
					yield break;
				}
				isAttached = true;
			}
			GameObject particleObj = onParticleInstantiate(particle.ParticlePrefab) as GameObject;
			Utils.AttachGameObject(attachNode.gameObject, particleObj);
			bool wasAttached = isAttached;
			if (particle.DetachedParticle)
			{
				particleObj.transform.parent = null;
				isAttached = true;
			}
			if (!wasAttached && ScaleEffectsToOwner && particle.ScaleToOwner)
			{
				ScaleEffectToOwner(particleObj);
			}
			if (!particle.DetachedParticlePermanent)
			{
				AddCreatedParticle(particleObj, particle, isAttached);
			}
			else if (particle.Lifetime > 0f)
			{
				Utils.GetComponentAlways<TimedSelfDestruct>(particleObj).lifetime = particle.Lifetime;
				if (particle.FadeOut && particle.FadeTime > 0f)
				{
					Utils.GetComponentAlways<EmissionLifetime>(particleObj).lifetime = Mathf.Max(particle.Lifetime - particle.FadeTime, 0f);
				}
			}
			if (particle.Lifetime > 0f)
			{
				if (particle.FadeOut)
				{
					float fadeTime = particle.FadeTime;
					Component[] emitters = particleObj.GetComponentsInChildren(typeof(ParticleEmitter));
					float fadeoutTime = particle.Lifetime - fadeTime;
					if (fadeoutTime < 0f)
					{
						fadeoutTime = 0f;
					}
					yield return new WaitForSeconds(fadeoutTime);
					if (base.particleEmitter != null)
					{
						base.particleEmitter.emit = false;
					}
					Component[] array = emitters;
					for (int i = 0; i < array.Length; i++)
					{
						ParticleEmitter emitter = (ParticleEmitter)array[i];
						if (emitter != null)
						{
							emitter.emit = false;
						}
					}
					yield return new WaitForSeconds(fadeTime);
				}
				else
				{
					yield return new WaitForSeconds(particle.Lifetime);
				}
				if (particleObj != null)
				{
					Utils.DelayedDestroyNetworkedChildren(particleObj);
					UnityEngine.Object.Destroy(particleObj);
					RemoveCreatedParticle(particleObj);
				}
			}
		}
		yield return 0;
	}

	protected IEnumerator StartGeneral(GeneralEffect general)
	{
		general.started = true;
		if (general.Prefab == null)
		{
			yield break;
		}
		IGeneralEffect prefabGeneral = general.GetFirstIGeneralEffect(general.Prefab);
		if (general.minimumQuality > GraphicsOptions.ModelQuality)
		{
			yield break;
		}
		float chanceToPlay = general.chanceToPlay;
		if (chanceToPlay < 0f && prefabGeneral != null)
		{
			chanceToPlay = prefabGeneral.ChanceToPlay;
		}
		if (UnityEngine.Random.value > chanceToPlay)
		{
			yield break;
		}
		if (!terminated && general.Prefab != null)
		{
			Transform attachNode = base.transform;
			bool isAttached = false;
			if (general.AttachNodeName != string.Empty)
			{
				if (owningObject == null)
				{
					CspUtils.DebugLog("Un-parented EffectSequence <" + base.gameObject.name + "> cannot have particles that attach to a node <" + general.AttachNodeName + ">!");
					yield break;
				}
				string nodeName = general.AttachNodeName;
				attachNode = ResolveNode(nodeName);
				if (attachNode == null)
				{
					CspUtils.DebugLog("Unable to find attach node <" + nodeName + "> for particle at time offset <" + general.TimeOffset + "> in EffectSequence <" + base.gameObject.name + ">.  Particle not run!");
					CspUtils.DebugLog("parent name is <" + owningObject.name + ">");
					yield break;
				}
				isAttached = true;
			}
			GameObject generalObj = UnityEngine.Object.Instantiate(general.Prefab) as GameObject;
			Vector3 localPosition = generalObj.transform.localPosition;
			Vector3 localScale = generalObj.transform.localScale;
			Quaternion localRotation = generalObj.transform.localRotation;
			generalObj.transform.parent = attachNode;
			generalObj.transform.localPosition = localPosition;
			generalObj.transform.localScale = localScale;
			generalObj.transform.localRotation = localRotation;
			if (!isAttached && ScaleEffectsToOwner && general.ScaleToOwner)
			{
				ScaleEffectToOwner(generalObj);
			}
			AddCreatedParticle(generalObj, general, isAttached);
			generalObj.SendMessage("OnCreatedAsGeneralEffect", this, SendMessageOptions.DontRequireReceiver);
			if (general.Lifetime > 0f)
			{
				yield return new WaitForSeconds(general.Lifetime);
				Utils.DelayedDestroyNetworkedChildren(generalObj);
				UnityEngine.Object.Destroy(generalObj);
				RemoveCreatedParticle(generalObj);
			}
			else
			{
				IGeneralEffect generalComponent = general.GetFirstIGeneralEffect(generalObj);
				if (generalComponent != null)
				{
					while (generalComponent != null && !generalComponent.IsFinished())
					{
						yield return 0;
					}
					Utils.DelayedDestroyNetworkedChildren(generalObj);
					UnityEngine.Object.Destroy(generalObj);
					RemoveCreatedParticle(generalObj);
				}
			}
		}
		yield return 0;
	}

	protected IEnumerator StartSound(SoundEffect sound)
	{
		if (sound.SoundPrefab == null)
		{
			yield break;
		}
		sound.started = true;
		if ((sound.chanceToPlay >= 0f && UnityEngine.Random.Range(0f, 1f) > sound.chanceToPlay) || terminated)
		{
			yield break;
		}
		GameObject soundObj;
		if (!sound.keepLocalTransform)
		{
			soundObj = (UnityEngine.Object.Instantiate(sound.SoundPrefab, Vector3.zero, Quaternion.identity) as GameObject);
			soundObj.transform.parent = base.transform;
			soundObj.transform.localPosition = Vector3.zero;
		}
		else
		{
			soundObj = (UnityEngine.Object.Instantiate(sound.SoundPrefab) as GameObject);
			Utils.AttachGameObject(base.gameObject, soundObj);
		}
		ShsAudioSource shsAudioSource = Utils.GetComponent<ShsAudioSource>(soundObj);
		if (shsAudioSource != null)
		{
			sound.soundInstance = shsAudioSource;
			playingSFX.Add(sound);
			shsAudioSource.TimeOffset = activeTime - sound.TimeOffset;
			if (originatorScope == EffectScope.Local || localInstigator)
			{
				shsAudioSource.PlayedFromLocalHero = true;
			}
			else if (CardGameController.Instance != null && Utils.GetComponent<CharacterGlobals>(base.gameObject, Utils.SearchParents) != null)
			{
				shsAudioSource.PlayedFromLocalHero = true;
			}
			if (!shsAudioSource.PlayOnAwake)
			{
				shsAudioSource.Play();
			}
			shsAudioSource.DestroyWhenFinished();
			float lifetime = sound.GetLifetime(soundObj);
			if (lifetime == -99f)
			{
				yield break;
			}
			if (lifetime <= 0f)
			{
				shsAudioSource.Loop = true;
			}
			else
			{
				if (!(lifetime > 0f))
				{
					yield break;
				}
				if (sound.duckMusic)
				{
					AppShell.Instance.EventMgr.Fire(null, new MusicDuckMessage(lifetime));
				}
				if (sound.FadeTime > 0f && lifetime > sound.FadeTime)
				{
					yield return new WaitForSeconds(sound.Lifetime - sound.FadeTime);
					float originalVolume = shsAudioSource.Volume;
					float remaining = sound.FadeTime;
					while (remaining > 0f)
					{
						remaining -= Time.deltaTime;
						shsAudioSource.Volume = Mathf.SmoothStep(0f, originalVolume, remaining / sound.FadeTime);
						yield return 0;
					}
				}
				else
				{
					yield return new WaitForSeconds(lifetime);
				}
				if (shsAudioSource != null)
				{
					shsAudioSource.Stop();
				}
			}
		}
		else
		{
			CspUtils.DebugLog("No ShsAudioSource found on <" + ((!(sound.SoundPrefab == null)) ? sound.SoundPrefab.name : "null") + "> in sequence <" + base.gameObject.name + ">");
		}
	}

	protected IEnumerator ContinueSound(SoundEffect sfx)
	{
		yield return 0;
		sfx.soundInstance.Play();
		if (!(sfx.GetLifetime(sfx.soundInstance.gameObject) < 0f))
		{
			yield return new WaitForSeconds(sfx.Lifetime - (sfx.TimeOffset - activeTime));
			UnityEngine.Object.Destroy(sfx.soundInstance.gameObject);
		}
	}

	protected IEnumerator PlayIdle(float waitForSeconds)
	{
		yield return new WaitForSeconds(waitForSeconds);
		if (FadeToIdleOnEnd)
		{
			animationComponent.CrossFade("movement_idle");
		}
	}

	protected IEnumerator StartAnimation(AnimationEffect anim)
	{
		anim.started = true;
		if (animationComponent != null && base.gameObject != null)
		{
			if (!terminated)
			{
				AnimationState animState = animationComponent[anim.Animation];
				if (animState != null)
				{
					animState.wrapMode = anim.Mode;
					animationComponent.Rewind(anim.Animation);
					animationComponent.CrossFade(anim.Animation, anim.BlendTime);
					PersistFace = anim.persistFace;
				}
				else
				{
					CspUtils.DebugLog(base.gameObject.name + " does not have animation " + anim.Animation);
				}
			}
		}
		else
		{
			CspUtils.DebugLog("EffectSequence does not have a valid animation target");
		}
		yield return 0;
	}

	protected IEnumerator StartEvent(EventEffect evt)
	{
		evt.started = true;
		if (!terminated && onSequenceEvent != null)
		{
			onSequenceEvent(this, evt);
		}
		yield return 0;
	}

	public void AssignCreator(CharacterGlobals charGlobals)
	{
		if (charGlobals == null)
		{
			CspUtils.DebugLog("Null charGlobals passed in to AssignCreator for " + base.gameObject.name);
		}
		else if (!(charGlobals.spawnData == null))
		{
			if (charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
			{
				originatorScope = EffectScope.Local;
			}
			else
			{
				originatorScope = EffectScope.Remote;
			}
		}
	}

	public void AssignInstigator(CharacterGlobals charGlobals)
	{
		if (charGlobals == null)
		{
			CspUtils.DebugLog("Null charGlobals passed in to AssignInstigator for " + base.gameObject.name);
		}
		else if (charGlobals.spawnData == null)
		{
			if (CardGameController.Instance != null)
			{
				localInstigator = true;
			}
		}
		else
		{
			localInstigator = (charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer);
		}
	}

	public void Update()
	{
		if (terminated)
		{
			Cancel();
		}
		else
		{
			if (!effectStarted)
			{
				return;
			}
			if (!paused)
			{
				activeTime += Time.deltaTime * TimeScale;
			}
			JumpTo(activeTime);
			if (effectLifetime >= 0f && activeTime >= effectLifetime)
			{
				if (Looping)
				{
					StopAllCoroutines();
					StartSequence();
				}
				else
				{
					Cancel();
				}
			}
		}
	}

	public void JumpTo(float timeOffset)
	{
		if (!effectStarted)
		{
			storedTimeOffset = timeOffset;
			return;
		}
		activeTime = timeOffset;
		GeneralEffect[] generals = Generals;
		foreach (GeneralEffect generalEffect in generals)
		{
			if (!generalEffect.started && timeOffset >= generalEffect.TimeOffset)
			{
				StartCoroutine(StartGeneral(generalEffect));
			}
		}
		SoundEffect[] sounds = Sounds;
		foreach (SoundEffect soundEffect in sounds)
		{
			if (!soundEffect.started && timeOffset >= soundEffect.TimeOffset)
			{
				StartCoroutine(StartSound(soundEffect));
			}
		}
		if (animationComponent != null)
		{
			AnimationEffect[] animations = Animations;
			foreach (AnimationEffect animationEffect in animations)
			{
				if (!animationEffect.started && timeOffset >= animationEffect.TimeOffset)
				{
					StartCoroutine(StartAnimation(animationEffect));
				}
			}
		}
		EventEffect[] events = Events;
		foreach (EventEffect eventEffect in events)
		{
			if (!eventEffect.started && timeOffset >= eventEffect.TimeOffset)
			{
				StartCoroutine(StartEvent(eventEffect));
			}
		}
		ParticleEffect[] particles = Particles;
		foreach (ParticleEffect particleEffect in particles)
		{
			if (!particleEffect.started && timeOffset >= particleEffect.TimeOffset)
			{
				StartCoroutine(StartParticle(particleEffect));
			}
		}
	}

	public static EffectSequence PlayOneShot(EffectSequence sequence, GameObject parent)
	{
		if (sequence != null)
		{
			return PlayOneShot(sequence.gameObject, parent);
		}
		return null;
	}

	public static EffectSequence PlayOneShot(GameObject sequencePrefab, GameObject parent)
	{
		return PlayOneShot(sequencePrefab, parent, DestroySequenceGO);
	}

	public static EffectSequence PlayOneShot(GameObject sequencePrefab, GameObject parent, OnSequenceDone onDone)
	{
		if (sequencePrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(sequencePrefab) as GameObject;
			EffectSequence componentInChildren = gameObject.GetComponentInChildren<EffectSequence>();
			if (componentInChildren != null)
			{
				componentInChildren.Initialize(parent, onDone, null);
				if (!componentInChildren.AutoStart)
				{
					componentInChildren.StartSequence();
				}
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			return componentInChildren;
		}
		return null;
	}

	private static void DestroySequenceGO(EffectSequence sequence)
	{
		UnityEngine.Object.Destroy(sequence.gameObject);
	}

	protected void ScaleEffectToOwner(GameObject effectObj)
	{
		if (!(effectObj == null) && !(owningObjectTransform == null))
		{
			GameObject gameObject = new GameObject(effectObj.name + "_scaled");
			if (!(gameObject == null))
			{
				gameObject.AddComponent<DestroyOnEmpty>();
				gameObject.transform.localScale = owningObjectTransform.localScale;
				Utils.AttachGameObject(effectObj.transform.parent, gameObject.transform);
				Vector3 localScale = effectObj.transform.localScale;
				effectObj.transform.parent = gameObject.transform;
				effectObj.transform.localScale = localScale;
			}
		}
	}

	protected void InitializeOwnerTransform()
	{
		Transform transform = (!(owningObject != null)) ? base.transform.parent : owningObject.transform;
		if (transform != null)
		{
			owningObjectTransform = Utils.FindNodeInChildren(transform, "export_node");
			if (owningObjectTransform == null && transform.GetComponent<CharacterGlobals>() != null)
			{
				CspUtils.DebugLog("Unable to find export transform in owning object or parent for effect sequence <" + base.gameObject.name + ">");
			}
		}
	}

	protected void AddCreatedParticle(GameObject particle, IBaseEffect creator, bool attached)
	{
		if (createdParticles != null)
		{
			createdParticles.Add(new ParticleObject(particle, creator, attached));
		}
	}

	protected void RemoveCreatedParticle(GameObject particle)
	{
		if (createdParticles != null)
		{
			ParticleObject particleObject = FindCreatedParticle(particle);
			if (particleObject != null)
			{
				createdParticles.Remove(particleObject);
			}
		}
	}

	protected ParticleObject FindCreatedParticle(GameObject particle)
	{
		if (createdParticles == null)
		{
			return null;
		}
		return createdParticles.Find(delegate(ParticleObject particleObject)
		{
			return particleObject != null && particleObject.Instance == particle;
		});
	}

	protected void DestroyCreatedParticles()
	{
		if (createdParticles != null)
		{
			foreach (ParticleObject createdParticle in createdParticles)
			{
				if (createdParticle != null)
				{
					createdParticle.Destroy();
				}
			}
			createdParticles.Clear();
		}
	}

	protected void DestroyCreatedAttachedParticles()
	{
		if (createdParticles != null)
		{
			List<ParticleObject> list = new List<ParticleObject>();
			foreach (ParticleObject createdParticle in createdParticles)
			{
				if (createdParticle != null && createdParticle.IsAttached)
				{
					if (!unloading)
					{
						createdParticle.Destroy();
					}
					list.Add(createdParticle);
				}
			}
			foreach (ParticleObject item in list)
			{
				createdParticles.Remove(item);
			}
		}
	}

	protected void StartParticleFadeOnEnd()
	{
		if (createdParticles != null)
		{
			List<ParticleObject> list = new List<ParticleObject>();
			foreach (ParticleObject createdParticle in createdParticles)
			{
				if (createdParticle != null && !(createdParticle.Instance == null) && createdParticle.CanFade && !(createdParticle.FadeTime <= 0f))
				{
					if (!createdParticle.IsAttached)
					{
						createdParticle.Instance.transform.parent = base.transform.parent;
					}
					TimedSelfDestruct timedSelfDestruct = createdParticle.Instance.AddComponent<TimedSelfDestruct>();
					if (timedSelfDestruct != null)
					{
						timedSelfDestruct.lifetime = createdParticle.FadeTime;
						list.Add(createdParticle);
					}
					ParticleEmitter[] componentsInChildren = createdParticle.Instance.GetComponentsInChildren<ParticleEmitter>();
					ParticleEmitter[] array = componentsInChildren;
					foreach (ParticleEmitter particleEmitter in array)
					{
						if (particleEmitter != null)
						{
							particleEmitter.emit = false;
						}
					}
				}
			}
			foreach (ParticleObject item in list)
			{
				createdParticles.Remove(item);
			}
		}
	}

	protected void StartSoundFadeOnEnd()
	{
		GameObject gameObject = new GameObject("FadingSoundContainer_" + base.gameObject.name);
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		gameObject.transform.parent = base.transform.parent;
		List<SoundEffect> list = new List<SoundEffect>();
		float num = -1f;
		foreach (SoundEffect item in playingSFX)
		{
			if (item != null && item.soundInstance != null)
			{
				if (item.FadeTime > 0f)
				{
					FadeOutSound.StartFade(item.soundInstance, item.FadeTime, true);
					item.soundInstance.transform.parent = gameObject.transform;
					num = Mathf.Max(num, item.FadeTime);
				}
				else
				{
					list.Add(item);
				}
			}
		}
		if (num > 0f)
		{
			gameObject.AddComponent<TimedSelfDestruct>().lifetime = num;
		}
		else
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		playingSFX = list;
	}

	protected Transform ResolveNode(string nodeName)
	{
		if (nodeName.StartsWith("#"))
		{
			nodeName = nodeName.ToLower();
			if (nodeName == "#pickup_node")
			{
				CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(owningObject, Utils.SearchParents);
				if (component == null)
				{
					CspUtils.DebugLog("Tried to use " + nodeName + " on an object that isn't a character.");
					return null;
				}
				return Utils.FindNodeInChildren(owningObject.transform, component.characterController.pickupBone, true);
			}
			if (nodeName == "#ribcage")
			{
				Transform transform = Utils.FindNodeInChildren(owningObject.transform, "ribcage", true);
				if (transform == null)
				{
					transform = Utils.FindNodeInChildren(owningObject.transform, "pelvis", true);
				}
				return transform;
			}
			return null;
		}
		return Utils.FindNodeInChildren(owningObject.transform, nodeName, true);
	}
}
