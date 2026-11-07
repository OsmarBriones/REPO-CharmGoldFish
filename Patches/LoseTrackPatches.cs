using CharmGoldFish.Domain;
using HarmonyLib;
using UnityEngine;

namespace CharmGoldFish.Patches
{
	/// <summary>
	/// Short Memory hooks. Each postfix runs after the monster state's own Update, only on the master, and only when the monster
	/// is still in that state and hunting a player who carries the Gold Fish. It subtracts a further (multiplier - 1) x frame time
	/// from the countdown that ends the hunt, clamped at 0, so the game's own "time is up" check in the next Update performs the
	/// normal transition.
	/// </summary>
	internal static class LoseTrackShared
	{
		/// <summary>True when the countdown of <paramref name="enemy"/> in <paramref name="state"/> should be sped up; gives the multiplier and the hunted carrier.</summary>
		internal static bool TryBegin(Enemy enemy, EnemyState state, out PlayerAvatar target, out float multiplier)
		{
			target = null!;
			multiplier = 1f;
			ShortMemory? memory = ShortMemory.Instance;
			if (memory == null || enemy == null || !enemy.MasterClient || enemy.CurrentState != state || !SemiFunc.IsMasterClientOrSingleplayer())
			{
				return false;
			}

			PlayerAvatar? hunted = enemy.TargetPlayerAvatar;
			if (hunted == null || !memory.IsCarrier(hunted))
			{
				return false;
			}

			target = hunted;
			multiplier = CharmGoldFishPlugin.Charm.LoseTrackMultiplier.Value;
			return multiplier > 1f;
		}

		/// <summary>Speeds up one countdown and reports the chase for wear when it really changed.</summary>
		internal static float Speed(float timer, float multiplier, PlayerAvatar target)
		{
			float deltaTime = Time.deltaTime;
			if (!LoseTrackMath.Applies(timer, deltaTime, multiplier))
			{
				return timer;
			}

			ShortMemory.Instance?.NoteChased(target);
			return LoseTrackMath.ExtraDecrease(timer, deltaTime, multiplier);
		}
	}

	/// <summary>Chase: both the sight countdown and the "give up" countdown run faster.</summary>
	[HarmonyPatch(typeof(EnemyStateChase), "Update")]
	internal static class EnemyStateChase_Update_Patch
	{
		private static void Postfix(EnemyStateChase __instance, Enemy ___Enemy, ref float ___StateTimer)
		{
			if (!LoseTrackShared.TryBegin(___Enemy, EnemyState.Chase, out PlayerAvatar target, out float multiplier))
			{
				return;
			}

			___StateTimer = LoseTrackShared.Speed(___StateTimer, multiplier, target);

			// The sight countdown only runs down while the monster cannot see the target; do not touch it while the player is in view.
			bool inView = ___Enemy.HasVision
				&& ___Enemy.Vision.VisionTriggered.TryGetValue(target.photonView.ViewID, out bool seen)
				&& seen;
			if (!inView)
			{
				__instance.VisionTimer = LoseTrackShared.Speed(__instance.VisionTimer, multiplier, target);
			}
		}
	}

	/// <summary>Slow chase (the monster follows the last known trail): the countdown to ChaseEnd runs faster.</summary>
	[HarmonyPatch(typeof(EnemyStateChaseSlow), "Update")]
	internal static class EnemyStateChaseSlow_Update_Patch
	{
		private static void Postfix(Enemy ___Enemy, ref float ___StateTimer)
		{
			if (LoseTrackShared.TryBegin(___Enemy, EnemyState.ChaseSlow, out PlayerAvatar target, out float multiplier))
			{
				___StateTimer = LoseTrackShared.Speed(___StateTimer, multiplier, target);
			}
		}
	}

	/// <summary>Look under (the monster searches where the player hid): the searching countdown runs faster once the waiting is done.</summary>
	[HarmonyPatch(typeof(EnemyStateLookUnder), "Update")]
	internal static class EnemyStateLookUnder_Update_Patch
	{
		private static void Postfix(EnemyStateLookUnder __instance, Enemy ___Enemy, ref float ___LookTimer)
		{
			if (__instance.WaitDone && LoseTrackShared.TryBegin(___Enemy, EnemyState.LookUnder, out PlayerAvatar target, out float multiplier))
			{
				___LookTimer = LoseTrackShared.Speed(___LookTimer, multiplier, target);
			}
		}
	}
}
