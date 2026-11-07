using System;

namespace CharmGoldFish.Domain
{
	/// <summary>Game-free rules of Short Memory: how a monster's countdown is sped up and how per-second wear is gathered. Free of Unity and game types so it is unit tested.</summary>
	internal static class LoseTrackMath
	{
		/// <summary>
		/// The value of a countdown after the extra decrease. The game has already subtracted <paramref name="deltaTime"/> once;
		/// this subtracts a further (<paramref name="multiplier"/> - 1) x <paramref name="deltaTime"/>, so the countdown runs
		/// <paramref name="multiplier"/> times as fast. The result never drops below 0, and a countdown that is already at or below 0,
		/// a multiplier of 1 or less, or a non-positive time step leave the value unchanged.
		/// </summary>
		internal static float ExtraDecrease(float timer, float deltaTime, float multiplier)
		{
			if (timer <= 0f || deltaTime <= 0f || multiplier <= 1f)
			{
				return timer;
			}

			float result = timer - ((multiplier - 1f) * deltaTime);
			return result < 0f ? 0f : result;
		}

		/// <summary>True when <see cref="ExtraDecrease"/> would change the countdown (the power is really working on it).</summary>
		internal static bool Applies(float timer, float deltaTime, float multiplier)
		{
			return timer > 0f && deltaTime > 0f && multiplier > 1f;
		}

		/// <summary>
		/// Adds <paramref name="percentPerSecond"/> x <paramref name="deltaTime"/> to the pending wear and returns the amount to wear now:
		/// the whole pending amount once it reaches <paramref name="threshold"/> (and the pending amount restarts from 0), otherwise 0.
		/// </summary>
		internal static float TakeWear(ref float pending, float percentPerSecond, float deltaTime, float threshold = 1f)
		{
			if (percentPerSecond <= 0f || deltaTime <= 0f)
			{
				return 0f;
			}

			pending += percentPerSecond * deltaTime;
			if (pending < Math.Max(threshold, 0.0001f))
			{
				return 0f;
			}

			float step = pending;
			pending = 0f;
			return step;
		}
	}
}
