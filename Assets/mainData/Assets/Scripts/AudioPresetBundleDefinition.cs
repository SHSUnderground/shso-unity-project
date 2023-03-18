using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;

public class AudioPresetBundleDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public const string AUDIO_PRESET_VOLUME_PATH = "Audio/audio_preset_volume";

	public const string AUDIO_PRESET_PITCH_PATH = "Audio/audio_preset_pitch";

	public const string AUDIO_PRESET_3D_PATH = "Audio/audio_preset_3d";

	public const string AUDIO_PRESET_FADE_PATH = "Audio/audio_preset_fade";

	public const string AUDIO_PRESET_VOICE_PATH = "Audio/audio_preset_voice";

	public const string AUDIO_PRESET_BUNDLE_PATH = "Audio/audio_preset_bundle";

	protected const string LOADING_STEP_VOLUME = "volume";

	protected const string LOADING_STEP_PITCH = "pitch";

	protected const string LOADING_STEP_3D = "3d";

	protected const string LOADING_STEP_FADE = "fade";

	protected const string LOADING_STEP_VOICE = "voice";

	public bool Initialized;

	public Dictionary<string, AudioPresetBundle> PresetBundles;

	public AudioPresetVolumeDefinition VolumeDefinitions;

	public AudioPresetPitchDefinition PitchDefinitions;

	public AudioPreset3DDefinition ThreeDDefinitions;

	public AudioPresetFadeDefinition FadeDefinitions;

	public AudioPresetVoiceDefinition VoiceDefinitions;

	protected TransactionMonitor loadingTransaction;

	public AudioPresetBundleDefinition()
	{
		Initialized = false;
		PresetBundles = new Dictionary<string, AudioPresetBundle>(50);
		loadingTransaction = TransactionMonitor.CreateTransactionMonitor("AudioPresetBundleDefinition_loadingTransaction", OnLoadingComplete, 90f, null);
		loadingTransaction.AddStep("volume");
		loadingTransaction.AddStep("pitch");
		loadingTransaction.AddStep("3d");
		loadingTransaction.AddStep("fade");
		loadingTransaction.AddStep("voice");
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_volume", OnPresetDefinitionsLoaded, new AudioPresetVolumeDefinition(), "volume");
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_pitch", OnPresetDefinitionsLoaded, new AudioPresetPitchDefinition(), "pitch");
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_3d", OnPresetDefinitionsLoaded, new AudioPreset3DDefinition(), "3d");
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_fade", OnPresetDefinitionsLoaded, new AudioPresetFadeDefinition(), "fade");
		AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_voice", OnPresetDefinitionsLoaded, new AudioPresetVoiceDefinition(), "voice");
	}

	protected void OnLoadingComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			AppShell.Instance.DataManager.LoadGameData("Audio/audio_preset_bundle", OnPresetBundlesLoaded);
		}
		else
		{
			Initialized = false;
			CspUtils.DebugLog("Failed to initialize the audio preset definitions! message=<" + error + ">.");
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.AudioDefinitionsLoadFail, error);
		}
		loadingTransaction = null;
	}

	protected void OnPresetDefinitionsLoaded(GameDataLoadResponse response, object extraData)
	{
		string text = extraData as string;
		if (response.Error == null || response.Error == string.Empty)
		{
			switch (text)
			{
			case "volume":
				VolumeDefinitions = (response.DataDefinition as AudioPresetVolumeDefinition);
				break;
			case "pitch":
				PitchDefinitions = (response.DataDefinition as AudioPresetPitchDefinition);
				break;
			case "3d":
				ThreeDDefinitions = (response.DataDefinition as AudioPreset3DDefinition);
				break;
			case "fade":
				FadeDefinitions = (response.DataDefinition as AudioPresetFadeDefinition);
				break;
			case "voice":
				VoiceDefinitions = (response.DataDefinition as AudioPresetVoiceDefinition);
				break;
			default:
				CspUtils.DebugLog("Unknown audio preset definition type <" + text + ">.");
				return;
			}
			loadingTransaction.CompleteStep(text);
		}
		else
		{
			CspUtils.DebugLog("Failed while trying to load audio " + text + " preset definitions. Error=<" + response.Error + ">.");
			loadingTransaction.FailStep(text, response.Error);
		}
	}

	protected void OnPresetBundlesLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error == null || response.Error == string.Empty)
		{
			InitializeFromData(response.Data);
			Initialized = true;
		}
		else
		{
			CspUtils.DebugLog("Failed while trying to load audio preset bundle definitions. Error=<" + response.Error + ">.");
			Initialized = false;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(AudioPresetBundle));
		AudioPresetBundle.SetLoadingPresetDefinitions(this);
		foreach (XPathNavigator value in data.GetValues("//bundle"))
		{
			AudioPresetBundle audioPresetBundle = xmlSerializer.Deserialize(new StringReader(value.OuterXml)) as AudioPresetBundle;
			PresetBundles.Add(audioPresetBundle.BundleName, audioPresetBundle);
		}
	}
}
