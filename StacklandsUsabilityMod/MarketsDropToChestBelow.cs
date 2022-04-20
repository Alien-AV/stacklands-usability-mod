using HarmonyLib;
using UnityEngine;

namespace StacklandsUsabilityMod
{
	[HarmonyPatch(typeof(Market))]
	[HarmonyPatch("SellWithMarket")]
	class MarketsDropToChestBelow
	{
		static bool Prefix(Market __instance)
		{
			SellWithMarket(__instance);
			return false;
		}
		public static void SellWithMarket(Market __instance)
		{
			GameCard child = __instance.MyGameCard.Child;
			if (child == null)
			{
				return;
			}
			GameCard gameCard = null;
			if (child.Child != null && WorldManager.instance.CardCanBeSold(child.Child, true))
			{
				gameCard = child.Child;
			}
			child.RemoveFromStack();
			if (gameCard != null)
			{
				__instance.MyGameCard.Child = gameCard;
				gameCard.Parent = __instance.MyGameCard;
			}
			AchievementManager.instance.SpecialActionComplete("sell_at_market", __instance);
			WorldManager.instance.SellCard(__instance.transform.position + new Vector3(0f, 0f, -1f), child, 2f, true);
		}

	}
}
