using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace CharmGoldFish
{
	/// <summary>
	/// The flash and sound where a copy appears, shown on every machine so players understand what happened. The host shows
	/// its own and tells the others with a Photon event.
	/// </summary>
	internal sealed class CopyEffect : IOnEventCallback
	{
		/// <summary>Photon event code (CharmCore uses 183 and 184; the game itself uses 123 and 124).</summary>
		internal const byte EventCode = 186;

		private const float Scale = 2f;

		internal static void Show(Vector3 position)
		{
			Play(position);
			if (SemiFunc.IsMultiplayer() && PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.RaiseEvent(EventCode, position, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
			}
		}

		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == EventCode && photonEvent.CustomData is Vector3 position)
			{
				Play(position);
			}
		}

		private static void Play(Vector3 position)
		{
			AssetManager assets = AssetManager.instance;
			if (assets == null)
			{
				return;
			}

			Object.Instantiate(assets.prefabTeleportEffect, position, Quaternion.identity).transform.localScale = Vector3.one * Scale;
			assets.soundUnequip.Play(position);
		}
	}
}
