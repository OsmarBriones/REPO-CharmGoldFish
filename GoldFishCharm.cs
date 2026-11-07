using CharmCore.Api;

namespace CharmGoldFish
{
	/// <summary>Short Memory: monsters lose track of you twice as fast.</summary>
	internal sealed class GoldFishCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum GoldFish";

		internal GoldFishCharm()
			: base("GoldFish", "Gold Fish - Short Memory", defaultWearPercent: 2f, defaultCooldownSeconds: null, PrefabName)
		{
			// WearPercent is per second of being chased while the power works (it only changes the config description).
			WearsPerSecond = true;
			LoseTrackMultiplier = AddSetting("LoseTrackMultiplier", 2f, 1f, 5f, "How many times faster monsters that are chasing the carrier lose track of them (2 = twice as fast, 1 = no effect, 5 = five times as fast).");
		}

		/// <summary>How many times faster the chase timers of monsters hunting the carrier run out (1 to 5).</summary>
		internal CharmSetting LoseTrackMultiplier { get; }
	}
}
