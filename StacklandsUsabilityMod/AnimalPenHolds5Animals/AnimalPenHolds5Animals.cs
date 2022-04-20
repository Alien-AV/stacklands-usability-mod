using HarmonyLib;
using System;
using UnityEngine;

namespace StacklandsUsabilityMod.AnimalPenHolds5Animals
{
    [HarmonyPatch(typeof(Animal))]
    [HarmonyPatch("get_InAnimalPen")]
    class get_InAnimalPen
	{
		static bool Prefix(Animal __instance, ref bool __result)
		{
			__result = __instance.MyGameCard.Parent != null && __instance.MyGameCard.GetRootCard().CardData is AnimalPen;
			return false;
		}
	}

    [HarmonyPatch(typeof(Animal))]
    [HarmonyPatch("CanHaveCard")]
    class CanHaveCard
	{
		static bool Prefix(Animal __instance, ref bool __result, CardData otherCard)
		{
			var newCondition = __instance.InAnimalPen && otherCard is Animal && __instance.MyGameCard.GetAllCardsInStack().Count +	otherCard.MyGameCard.GetAllCardsInStack().Count <= 6;
			var oldCondition = !__instance.InAnimalPen && !(otherCard is Animal) && Reverse_Mob_Can_Have_Card.CanHaveCard(__instance, otherCard);

			__result = newCondition || oldCondition;
			return false;
		}
	}

	[HarmonyPatch]
	class Reverse_Mob_Can_Have_Card
	{
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(Mob), "CanHaveCard")]
		public static bool CanHaveCard(object instance, CardData otherCard)
		{
			throw new NotImplementedException("It's a stub");
		}
	}
}
