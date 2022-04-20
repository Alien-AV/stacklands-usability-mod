using HarmonyLib;
using UnityEngine;

namespace StacklandsUsabilityMod
{
	[HarmonyPatch(typeof(WorldManager))]
	[HarmonyPatch("DetermineTargetWorldSize")]
	class ZoomOutFarther
	{
		static bool Prefix(Market __instance, ref float __result)
		{
			var worldManager = Traverse.Create(__instance);
			__result = Mathf.Clamp((float)worldManager.Method("CardCapIncrease").GetValue<int>() * 0.03f, 0.15f, 5f);
			return false;
		}
	}
}
