using Photon.Pun;
using UnityEngine;

namespace CharmGoldFish
{
	/// <summary>
	/// Creates a copy of a valuable next to it, on the host (only the host may create world objects). The prefabs live in the
	/// game's Resources size folders; monster orbs and the extraction surplus bag live elsewhere and are never copied.
	/// </summary>
	internal static class ValuableCopier
	{
		private static readonly string[] SizeFolders =
		{
			"01 Tiny", "02 Small", "03 Medium", "04 Big", "05 Wide", "06 Tall", "07 Very Tall",
		};

		private const float Gap = 0.15f;
		private const float ProtectSeconds = 2f;

		/// <summary>
		/// Spawns a copy of <paramref name="original"/> worth <paramref name="value"/>; null when its prefab is not one of the
		/// size-folder valuables. <paramref name="position"/> is the middle of the copy, for effects.
		/// </summary>
		internal static ValuableObject? TryCopy(ValuableObject original, int value, out Vector3 position)
		{
			position = original.transform.position;
			string prefabName = original.gameObject.name.Replace("(Clone)", string.Empty).Trim();
			string? path = null;
			GameObject? prefab = null;
			foreach (string folder in SizeFolders)
			{
				string candidate = "Valuables/" + folder + "/" + prefabName;
				prefab = Resources.Load<GameObject>(candidate);
				if (prefab != null)
				{
					path = candidate;
					break;
				}
			}

			if (path == null || prefab == null)
			{
				return null;
			}

			Bounds bounds = BoundsOf(original);
			position = FreeSpot(bounds);
			Vector3 spawnAt = original.transform.position + (position - bounds.center);
			Quaternion rotation = original.transform.rotation;

			// The original must not lose value if the new body touches it; a new valuable is protected by the game itself.
			original.physGrabObject.OverrideIndestructible(ProtectSeconds);

			GameObject spawned = SemiFunc.IsMultiplayer()
				? PhotonNetwork.InstantiateRoomObject(path, spawnAt, rotation, 0)
				: Object.Instantiate(prefab, spawnAt, rotation);
			ValuableObject? copy = spawned != null ? spawned.GetComponent<ValuableObject>() : null;
			if (copy == null)
			{
				return null;
			}

			// Read when the new valuable sets its value a moment later; the host then sends it to everybody.
			copy.dollarValueOverride = value;
			return copy;
		}

		private static Bounds BoundsOf(ValuableObject valuable)
		{
			Bounds bounds = new Bounds(valuable.transform.position, Vector3.zero);
			bool any = false;
			foreach (Collider collider in valuable.GetComponentsInChildren<Collider>())
			{
				if (collider.isTrigger || !collider.enabled)
				{
					continue;
				}

				if (any)
				{
					bounds.Encapsulate(collider.bounds);
				}
				else
				{
					bounds = collider.bounds;
					any = true;
				}
			}

			return bounds;
		}

		/// <summary>The middle of a free place for a body of the same size: above the original, else beside it, else above anyway.</summary>
		private static Vector3 FreeSpot(Bounds bounds)
		{
			Vector3 size = bounds.size;
			Vector3 above = bounds.center + Vector3.up * (size.y + Gap);
			Vector3[] spots =
			{
				above,
				bounds.center + Vector3.right * (size.x + Gap),
				bounds.center + Vector3.left * (size.x + Gap),
				bounds.center + Vector3.forward * (size.z + Gap),
				bounds.center + Vector3.back * (size.z + Gap),
			};

			int walls = LayerMask.GetMask("Default");
			int bodies = LayerMask.GetMask("Default", "PhysGrabObject", "PhysGrabObjectHinge", "StaticGrabObject");
			foreach (Vector3 spot in spots)
			{
				// Not behind a wall and not inside something.
				if (!Physics.Linecast(bounds.center, spot, walls, QueryTriggerInteraction.Ignore)
					&& !Physics.CheckBox(spot, bounds.extents * 0.9f, Quaternion.identity, bodies, QueryTriggerInteraction.Ignore))
				{
					return spot;
				}
			}

			return above;
		}
	}
}
