using System;
using System.Collections.Generic;

namespace CharmGoldFish.Domain
{
	/// <summary>Game-free rules of Lucky Catch, so they can be unit tested.</summary>
	internal static class LuckyCatchMath
	{
		/// <summary>True when a roll in [0, 1) wins a chance given in percent. 0 never wins, 100 always does.</summary>
		internal static bool Wins(float chancePercent, double roll)
		{
			return roll * 100d < chancePercent;
		}

		/// <summary>
		/// The whole-dollar value of a copy of a valuable worth <paramref name="currentValue"/>; 0 means no copy can be made
		/// (the game reads a value override of 0 as "roll a random value").
		/// </summary>
		internal static int CopyValue(float currentValue)
		{
			if (float.IsNaN(currentValue) || currentValue < 1f)
			{
				return 0;
			}

			return currentValue >= int.MaxValue ? int.MaxValue : (int)Math.Floor(currentValue);
		}

		/// <summary>
		/// The wear, in percent of the fish's original value, that takes <paramref name="chargePercent"/> percent of the copy's
		/// value away from the fish. Capped at 100: a copy worth as much as the fish or more takes all of it.
		/// </summary>
		internal static float WearForCopy(int copyValue, float fishOriginalValue, float chargePercent)
		{
			if (copyValue <= 0 || fishOriginalValue <= 0f || chargePercent <= 0f)
			{
				return 0f;
			}

			float percent = copyValue * chargePercent / fishOriginalValue;
			return Math.Min(percent, 100f);
		}
	}

	/// <summary>The valuables of this level that already had their one chance (including the copies), by an id unique in the level.</summary>
	internal sealed class RolledValuables
	{
		private readonly HashSet<int> ids = new HashSet<int>();

		internal int Count => ids.Count;

		/// <summary>True the first time an id is claimed; false afterwards.</summary>
		internal bool TryClaim(int id)
		{
			return ids.Add(id);
		}

		internal void Clear()
		{
			ids.Clear();
		}
	}
}
