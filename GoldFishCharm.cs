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
			// Extra host-synced numbers for this power go here, for example:
			// Amount = AddSetting("Amount", 30f, 1f, 100f, "Description for the config file.");
		}
	}
}
