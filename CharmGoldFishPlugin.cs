using BepInEx;
using CharmCore.Api;
using HarmonyLib;
using UnityEngine;

namespace CharmGoldFish
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.osmar.Pockets", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.osmar.CharmCore", BepInDependency.DependencyFlags.HardDependency)]
	public class CharmGoldFishPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "com.osmar.CharmGoldFish";
		public const string PluginName = "CharmGoldFish";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;

		/// <summary>The registered charm, for the Harmony patches.</summary>
		internal static GoldFishCharm Charm { get; private set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, LoseTrackMultiplier and the Debug section (DebugMode) in this mod's config file.
			GoldFishCharm charm = new GoldFishCharm();
			Charm = charm;
			CharmsApi.Register(charm, Config);
			this.gameObject.AddComponent<ShortMemory>().Charm = charm;
			new Harmony(PluginGuid).PatchAll();

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}
