using System;
using System.Collections.Generic;
using CharmCore.Api;
using CharmGoldFish.Domain;
using Photon.Pun;
using Pockets.Api;
using UnityEngine;

namespace CharmGoldFish
{
	/// <summary>
	/// Lucky Catch: a valuable the carrier is the first to discover has a chance to be duplicated, and the Gold Fish pays the
	/// copy's value. The carrier's machine reports the discovery; the host rolls, creates the copy and wears the fish.
	/// </summary>
	internal sealed class GoldFishCharm : Charm
	{
		internal const string PrefabName = "Valuable Museum GoldFish";

		private const string DiscoveredAction = "discovered";

		private readonly RolledValuables rolled = new RolledValuables();
		private readonly System.Random random = new System.Random();

		// Totals since the game started, so the debug log shows how often the roll really wins.
		private int rolls;
		private int wins;

		internal GoldFishCharm()
			: base("GoldFish", "Gold Fish - Lucky Catch", defaultWearPercent: 100f, defaultCooldownSeconds: null, PrefabName)
		{
			WearDescription = "How much of a copy's value the Gold Fish pays for making it, in percent (0 to 100). 100 = the fish loses exactly the value that was created; it breaks when it cannot pay.";
			DuplicateChancePercent = AddSetting("DuplicateChancePercent", 20f, 0f, 100f, "Chance, in percent (0 to 100), that a valuable you are the first to discover is duplicated. 0 = never, 100 = always.");
		}

		/// <summary>Chance of a copy for each first discovery (0 to 100).</summary>
		internal CharmSetting DuplicateChancePercent { get; }

		/// <summary>Called on the machine of the local player right after they discovered <paramref name="valuable"/> for the first time.</summary>
		internal void ReportDiscovery(ValuableObject valuable)
		{
			if (!IsActive || valuable == null || !SemiFunc.RunIsLevel())
			{
				return;
			}

			// On the host the request is handled on the spot and may carry the object; a client sends the network id.
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				SendToHost(DiscoveredAction, valuable);
				return;
			}

			PhotonView view = valuable.GetComponent<PhotonView>();
			if (view != null)
			{
				SendToHost(DiscoveredAction, view.ViewID);
			}
		}

		/// <summary>Forgets which valuables of the level already had their chance.</summary>
		protected override void OnLevelEnded()
		{
			rolled.Clear();
		}

		protected override void OnHostRequest(PlayerAvatar sender, string action, object[] args)
		{
			if (action != DiscoveredAction || args.Length < 1 || !SemiFunc.RunIsLevel() || !SemiFunc.IsMasterClientOrSingleplayer())
			{
				return;
			}

			ValuableObject? original = Resolve(args[0]);
			if (original == null || original.physGrabObject == null || original.physGrabObject.dead || PocketsApi.IsPocketed(original))
			{
				return;
			}

			// One chance per valuable and level, whoever asks and however often.
			if (!rolled.TryClaim(original.GetInstanceID()))
			{
				if (DebugMode)
				{
					LogDebug($"Lucky Catch: {original.name} already had its chance; no roll");
				}

				return;
			}

			float chance = DuplicateChancePercent.Value;
			double roll = random.NextDouble();
			bool won = LuckyCatchMath.Wins(chance, roll);
			rolls++;
			wins += won ? 1 : 0;
			if (DebugMode)
			{
				LogDebug($"Lucky Catch roll #{rolls}: {original.name} (value {original.dollarValueCurrent:0}) rolled {roll * 100d:0.00}, needs below {chance:0.##} -> {(won ? "COPY" : "no copy")} (copies won since the game started: {wins} of {rolls})");
			}

			if (!won)
			{
				return;
			}

			IReadOnlyList<ValuableObject> fishes = CharmsApi.CarriedBy(sender, this);
			int value = LuckyCatchMath.CopyValue(original.dollarValueCurrent);
			if (fishes.Count == 0 || value == 0)
			{
				LogDebug($"Lucky Catch: nothing to copy (Gold Fish carried: {fishes.Count}, copy value {value})");
				return;
			}

			ValuableObject fish = fishes[0];
			ValuableObject? copy = ValuableCopier.TryCopy(original, value, out Vector3 position);
			if (copy == null)
			{
				LogDebug($"Lucky Catch: {original.name} cannot be copied; the Gold Fish pays nothing");
				return;
			}

			// The copy never gets a chance of its own.
			rolled.TryClaim(copy.GetInstanceID());
			CopyEffect.Show(position);

			float wear = LuckyCatchMath.WearForCopy(value, fish.dollarValueOriginal, WearPercent);
			LogDebug($"Lucky Catch: copied {original.name} (value {value}) at {position}; the Gold Fish (value {fish.dollarValueCurrent:0} of {fish.dollarValueOriginal:0}) wears {wear:0.##}%");
			WearValuable(fish, wear);
		}

		private static ValuableObject? Resolve(object argument)
		{
			if (argument is ValuableObject valuable)
			{
				return valuable;
			}

			if (argument is int viewId)
			{
				PhotonView view = PhotonView.Find(viewId);
				return view != null ? view.GetComponent<ValuableObject>() : null;
			}

			return null;
		}
	}
}
