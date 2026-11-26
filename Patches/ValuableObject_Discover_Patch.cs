using HarmonyLib;

namespace CharmGoldFish.Patches
{
	/// <summary>
	/// The game calls <c>Discover</c> only on the machine of the player who is looking, and it returns true only for the first
	/// discovery of a valuable, so this is exactly "you found it, and nobody had before".
	/// </summary>
	[HarmonyPatch(typeof(ValuableObject), nameof(ValuableObject.Discover))]
	internal static class ValuableObject_Discover_Patch
	{
		private static void Postfix(ValuableObject __instance, ValuableDiscoverGraphic.State _state, bool __result)
		{
			if (__result && _state == ValuableDiscoverGraphic.State.Discover)
			{
				CharmGoldFishPlugin.Charm.ReportDiscovery(__instance);
			}
		}
	}
}
