using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

namespace ShsAudio
{
	public class Definitions
	{
		private class LoadRequest
		{
			public TaggedAudioReferencesLoadedDelegate callback;

			public TransactionMonitor transactionMonitor;

			public object extraData;
		}

		public delegate void BundlePrefabLoaded(GameObject audioPrefab, object extraData);

		public delegate void TaggedAudioReferencesLoadedDelegate(object extraData);

		public const string AUDIO_BUNDLE_PATH = "Audio/";

		public const string TAGGED_AUDIO_REFERENCES_PATH = "Audio/tagged_audio_references";

		public const string TAGGED_AUDIO_REFERENCES_STEP_NAME = "audio_tagged_references";

		[CompilerGenerated]
		private static Dictionary<string, TaggedAudioBundleEntry> _003CTaggedAudioReferences_003Ek__BackingField;

		public static Dictionary<string, TaggedAudioBundleEntry> TaggedAudioReferences
		{
			[CompilerGenerated]
			get
			{
				return _003CTaggedAudioReferences_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CTaggedAudioReferences_003Ek__BackingField = value;
			}
		}

		public static void LoadTaggedAudioReferences(TaggedAudioReferencesLoadedDelegate onLoaded, TransactionMonitor transactionMonitor, object extraData)
		{
			if (transactionMonitor != null)
			{
				transactionMonitor.AddStep("audio_tagged_references");
			}
			LoadRequest loadRequest = new LoadRequest();
			loadRequest.callback = onLoaded;
			loadRequest.transactionMonitor = transactionMonitor;
			loadRequest.extraData = extraData;
			AppShell.Instance.DataManager.LoadGameData("Audio/tagged_audio_references", OnTaggedAudioReferencesLoaded, loadRequest);
		}

		private static void OnTaggedAudioReferencesLoaded(GameDataLoadResponse response, object extraData)
		{
			LoadRequest loadRequest = extraData as LoadRequest;
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				if (loadRequest.transactionMonitor != null)
				{
					loadRequest.transactionMonitor.FailStep("audio_tagged_references", "Unable to load tagged audio references");
				}
				return;
			}
			TaggedAudioReferences = new Dictionary<string, TaggedAudioBundleEntry>();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TaggedAudioBundleEntry));
			foreach (DataWarehouse item in response.Data.GetIterator("//audio-reference"))
			{
				using (StringReader textReader = new StringReader(item.Navigator.OuterXml))
				{
					TaggedAudioBundleEntry taggedAudioBundleEntry = xmlSerializer.Deserialize(textReader) as TaggedAudioBundleEntry;
					TaggedAudioReferences[taggedAudioBundleEntry.Key] = taggedAudioBundleEntry;
				}
			}
			if (loadRequest.callback != null)
			{
				loadRequest.callback(loadRequest.extraData);
			}
			if (loadRequest.transactionMonitor != null)
			{
				loadRequest.transactionMonitor.CompleteStep("audio_tagged_references");
			}
		}
	}
}
