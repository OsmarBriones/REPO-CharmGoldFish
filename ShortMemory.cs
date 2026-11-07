using System;
using System.Collections.Generic;
using CharmCore.Api;
using CharmGoldFish.Domain;
using UnityEngine;

namespace CharmGoldFish
{
	/// <summary>
	/// Short Memory: monsters lose track of you faster. Runs on the host only (monster AI runs there). The Harmony patches in
	/// <c>Patches/</c> speed up the chase countdowns; this component keeps the list of carriers and turns "a monster is chasing a
	/// carrier" into per-second wear, once per carrier however many monsters hunt them.
	/// </summary>
	internal sealed class ShortMemory : MonoBehaviour
	{
		private const float RefreshSeconds = 0.2f;

		private readonly Dictionary<PlayerAvatar, ValuableObject> carriers = new Dictionary<PlayerAvatar, ValuableObject>();
		private readonly Dictionary<PlayerAvatar, float> pendingWear = new Dictionary<PlayerAvatar, float>();
		private readonly HashSet<PlayerAvatar> chased = new HashSet<PlayerAvatar>();
		private readonly List<PlayerAvatar> scratch = new List<PlayerAvatar>();
		private readonly Action<PlayerAvatar, ValuableObject> collectCarrier;
		private float nextRefresh;

		public ShortMemory()
		{
			// One delegate for the whole life of the component, so the 5 Hz refresh does not allocate a closure each time.
			collectCarrier = (player, valuable) => carriers[player] = valuable;
		}

		internal static ShortMemory? Instance { get; private set; }

		internal GoldFishCharm Charm { get; set; } = null!;

		/// <summary>True while at least one carrier is known; the monster patches exit at once when it is false.</summary>
		internal bool HasCarriers => carriers.Count > 0;

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		/// <summary>True on the host (or alone) when <paramref name="player"/> carries the Gold Fish and the power is on.</summary>
		internal bool IsCarrier(PlayerAvatar? player)
		{
			if (player == null || !SemiFunc.IsMasterClientOrSingleplayer() || Charm == null || !Charm.IsEnabled)
			{
				return false;
			}

			return carriers.TryGetValue(player, out ValuableObject valuable) && valuable != null;
		}

		/// <summary>Called by the patches each frame a monster's countdown was really sped up for <paramref name="player"/>.</summary>
		internal void NoteChased(PlayerAvatar player)
		{
			chased.Add(player);
		}

		private void Update()
		{
			if (Charm == null || !SemiFunc.IsMasterClientOrSingleplayer())
			{
				ClearAll();
				return;
			}

			if (Time.time >= nextRefresh)
			{
				nextRefresh = Time.time + RefreshSeconds;
				Refresh();
			}

			if (chased.Count == 0)
			{
				return;
			}

			float deltaTime = Time.deltaTime;
			foreach (PlayerAvatar player in chased)
			{
				if (!carriers.TryGetValue(player, out ValuableObject valuable) || valuable == null)
				{
					continue;
				}

				pendingWear.TryGetValue(player, out float pending);
				float step = LoseTrackMath.TakeWear(ref pending, Charm.WearPercent, deltaTime);
				pendingWear[player] = pending;
				if (step > 0f)
				{
					if (Charm.DebugMode)
					{
						Charm.LogDebug($"Short Memory: monsters are chasing {player.name}; wearing {valuable.name} by {step:0.##}%");
					}

					Charm.WearValuable(valuable, step, quiet: true);
				}
			}

			chased.Clear();
		}

		private void Refresh()
		{
			carriers.Clear();
			CharmsApi.ForEachCarrier(Charm, collectCarrier);

			if (pendingWear.Count == 0)
			{
				return;
			}

			scratch.Clear();
			foreach (PlayerAvatar player in pendingWear.Keys)
			{
				if (player == null || !carriers.ContainsKey(player))
				{
					scratch.Add(player!);
				}
			}

			foreach (PlayerAvatar player in scratch)
			{
				pendingWear.Remove(player);
			}

			scratch.Clear();
		}

		/// <summary>Forgets everything about the level: carriers, pending wear, chased players. Called when the level ends and while not the host.</summary>
		internal void ClearAll()
		{
			if (carriers.Count > 0 || pendingWear.Count > 0 || chased.Count > 0 || scratch.Count > 0)
			{
				carriers.Clear();
				pendingWear.Clear();
				chased.Clear();
				scratch.Clear();
			}

			nextRefresh = 0f;
		}
	}
}
